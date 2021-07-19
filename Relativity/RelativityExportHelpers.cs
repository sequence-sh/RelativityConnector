using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal.Errors;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity
{
    public static class RelativityExportHelpers
    {
        public static async Task<Result<Array<Entity>, IError>> ExportAsync(
            RelativitySettings relativitySettings,
            int workspaceId,
            ArtifactType artifactType,
            IReadOnlyList<string> fieldNames,
            string condition,
            int start,
            int batchSize,
            IFlurlClient flurlClient,
            ErrorLocation errorLocation,
            CancellationToken cancellationToken
        )
        {
            var setupExportResult = await SetupExportAsync(relativitySettings, workspaceId, artifactType,
                fieldNames, condition, start, flurlClient, cancellationToken);

            if (setupExportResult.IsFailure)
                return setupExportResult.MapError(x => x.WithLocation(errorLocation))
                    .ConvertFailure<Array<Entity>>();


            var allResults = GetResultElements(relativitySettings, setupExportResult.Value, workspaceId,
                batchSize, fieldNames, flurlClient, errorLocation, cancellationToken);

            var array = new LazyArray<Entity>(allResults);

            return array;
        }

        /// <summary>
        /// A token indicating a long string that should be downloaded separately
        /// </summary>
        public const string LongStringToken = "#KCURA99DF2F0FEB88420388879F1282A55760#";

        public const string NativeFileKey = "NativeFile";



        public static async IAsyncEnumerable<Entity> GetResultElements(
            RelativitySettings relativitySettings,
            ExportResult exportResult,
            int workspaceId,
            int batchSize,
            IReadOnlyList<string> fieldNames,
            IFlurlClient flurlClient,
            ErrorLocation errorLocation,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var fields = fieldNames.Select(x => new Field() { Name = x }).ToList();


            var urlSuffix =
                $"/Relativity.REST/api/Relativity.Objects/workspace/{workspaceId}/object/retrieveNextResultsBlockFromExport";

            var url = Url.Combine(relativitySettings.Url, urlSuffix);

            var request = new ExportBatchRequest()
            {
                RunID = exportResult.RunID,
                BatchSize = batchSize
            };

            var current = 0;

            while (current < exportResult.RecordCount)
            {
                var resultElements =
                    await url.WithBasicAuth(relativitySettings.RelativityUsername,
                            relativitySettings.RelativityPassword)
                        .WithHeader("X-CSRF-Header", "-")
                        .WithClient(flurlClient)
                        .PostJsonAsync(request, cancellationToken)
                        .ReceiveJson<IList<ExportResultElement>>();

                if (resultElements is not null)
                {
                    foreach (var resultElement in resultElements)
                    {
                        var properties = new List<EntityProperty>();

                        var pairs = fields.Zip(resultElement.Values);

                        var order = 0;
                        foreach (var (field, fieldValue) in pairs)
                        {
                            

                            if (fieldValue?.ToString() == LongStringToken)
                            {
                                var v = await GetLongText(relativitySettings, workspaceId, field.Name,
                                    resultElement.ArtifactID,
                                    flurlClient,
                                    cancellationToken);

                                if (v.IsFailure)
                                    throw v.Error;

                                properties.Add(new EntityProperty(field.Name,
                                    new EntityValue.String(v.Value), null, order
                                    
                                    ));
                            }
                            else
                            {
                                properties.Add(new EntityProperty(
                                    field.Name,
                                    EntityValue.CreateFromObject(fieldValue), 
                                    null, order
                                    ));
                            }


                            order++;
                        }

                        var downloadResult = await DocumentFileManager.DownloadFile(relativitySettings,
                            flurlClient,
                            errorLocation,
                            workspaceId,
                            resultElement.ArtifactID, cancellationToken);

                        if (downloadResult.IsFailure)
                        {
                            throw new ErrorException(downloadResult.Error);
                        }

                        properties.Add(new EntityProperty(NativeFileKey,
                            new EntityValue.String(downloadResult.Value),
                            null,order
                            ));

                        var entity = new Entity(properties);

                        yield return entity;
                    }
                }


                current += batchSize;
            }
        }


        public static async Task<Result<ExportResult, IErrorBuilder>> SetupExportAsync(
            RelativitySettings relativitySettings,
            int workspaceId,
            ArtifactType artifactType,
            IReadOnlyList<string> fieldNames,
            string condition,
            int start,
            IFlurlClient flurlClient,
            CancellationToken cancellationToken)
        {
            var urlSuffix = $"/Relativity.REST/api/Relativity.Objects/workspace/{workspaceId}/object/initializeexport";

            var url = Url.Combine(relativitySettings.Url, urlSuffix);


            var request = new ExportRequestRoot
            {
                Start = start,
                QueryRequest = new QueryRequest
                {
                    Condition = condition,
                    Fields = fieldNames.Select(i => new Field() { Name = i }).ToList(),
                    ObjectType = new ObjectType { ArtifactType = artifactType }
                }
            };


            ExportResult exportResult;

            try
            {
                exportResult = await url
                    .WithBasicAuth(relativitySettings.RelativityUsername, relativitySettings.RelativityPassword)
                    .WithHeader("X-CSRF-Header", "-")
                    .WithClient(flurlClient)
                    .PostJsonAsync(request, cancellationToken)
                    .ReceiveJson<ExportResult>();
            }
            catch (FlurlHttpException e)
            {
                return ErrorCode.Unknown.ToErrorBuilder(e);
            }


            return exportResult;
        }


        public static async Task<Result<string, FlurlHttpException>> GetLongText(
            RelativitySettings relativitySettings,
            int workspaceId,
            string fieldName,
            int artifactId,
            IFlurlClient flurlClient,
            CancellationToken cancellationToken)
        {
            var request = new LongTextRequest
            {
                LongTextField = new LongTextField
                {
                    Name = fieldName
                },
                ExportObject = new ExportObject
                {
                    ArtifactID = artifactId
                }
            };

            var url = Url.Combine(
                relativitySettings.Url,
                $"/Relativity.REST/api/Relativity.Objects/workspace/{workspaceId}/object/streamlongtext");


            string longText;

            try
            {
                longText = await url.WithClient(flurlClient)
                    .WithBasicAuth(relativitySettings.RelativityUsername, relativitySettings.RelativityPassword)
                    .WithHeader("X-CSRF-Header", "-")
                    .PostJsonAsync(request, cancellationToken).ReceiveString();
            }
            catch (FlurlHttpException e)
            {
                return Result.Failure<string, FlurlHttpException>(e);
            }

            return longText;
        }


        public class ExportBatchRequest
        {
            [JsonProperty("runID")] public string RunID { get; set; }

            [JsonProperty("batchSize")] public int BatchSize { get; set; }
        }


        public class ExportResultElement
        {
            [JsonProperty("ArtifactID")] public int ArtifactID { get; set; }

            [JsonProperty("Values")] public List<object> Values { get; set; }
        }


        public class ExportRequestRoot
        {
            [JsonProperty("queryRequest")] public QueryRequest QueryRequest { get; set; }

            [JsonProperty("start")] public int Start { get; set; }
        }

        public class QueryRequest
        {
            [JsonProperty("ObjectType")] public ObjectType ObjectType { get; set; }

            [JsonProperty("fields")] public List<Field> Fields { get; set; }

            [JsonProperty("condition")] public string Condition { get; set; }
        }


        public class ExportResult
        {
            [JsonProperty("RunID")] public string RunID { get; set; }

            [JsonProperty("RecordCount")] public int RecordCount { get; set; }
        }

        public class ObjectType
        {
            [JsonProperty("ArtifactTypeID")] public int ArtifactTypeID { get; set; }

            [JsonIgnore]
            public ArtifactType ArtifactType
            {
                get => (ArtifactType)ArtifactTypeID;
                set => ArtifactTypeID = (int)value;
            }
        }

        public class Field : IEquatable<Field>
        {
            /// <inheritdoc />
            public override string ToString() => Name;

            [JsonProperty("Name")] public string Name { get; set; }

            ///// <inheritdoc />
            //public override int GetHashCode() => ArtifactID;
            /// <inheritdoc />
            public bool Equals(Field? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                return Name == other.Name;
            }

            /// <inheritdoc />
            public override bool Equals(object? obj)
            {
                if (obj is null) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((Field)obj);
            }

            /// <inheritdoc />
            public override int GetHashCode() => Name.GetHashCode();
        }


        public class LongTextRequest
        {
            [JsonProperty("exportObject")] public ExportObject ExportObject { get; set; }

            [JsonProperty("longTextField")] public LongTextField LongTextField { get; set; }
        }

        public class ExportObject
        {
            /// <summary>
            /// Artifact Id of the object
            /// </summary>
            [JsonProperty("ArtifactID")]
            public int ArtifactID { get; set; }
        }

        public class LongTextField
        {
            [JsonProperty("Name")] public string Name { get; set; }

            //[JsonProperty("ArtifactID")]
            //public int ArtifactID { get; set; }
        }
    }


    public enum ArtifactType
    {
        //https://platform.relativity.com/RelativityOne/index.htm#../Subsystems/rsapiclasses/Content/html/T_kCura_Relativity_Client_ArtifactType.htm

        Batch = 27,
        BatchSet = 24,
        Case = 8,
        Client = 5,
        Code = 7,
        Document = 10,
        Error = 18,
        Field = 14,
        Folder = 9,
        Group = 3,
        Layout = 16,
        Matter = 6,
        MarkupSet = 22,
        Production = 17,
        ObjectType = 25,
        RelativityScript = 28,
        ResourcePool = 31,
        ResourceServer = 32,
        SearchIndex = 29,
        Search = 15,
        Tab = 23,
        User = 2,
        View = 4,
        SearchContainer = 26,
        InstanceSetting = 42,
        Credential = 43,
    }
}