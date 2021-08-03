using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal.Errors;
using Relativity.Kepler.Transport;
using Relativity.Services.DataContracts.DTOs.Results;
using Relativity.Services.Interfaces.Document;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity
{
    public static class RelativityExportHelpers
    {
        public static async Task<Result<Array<Entity>, IError>> ExportAsync(
            int workspaceId,
            ArtifactType artifactType,
            IReadOnlyList<string> fieldNames,
            string condition,
            int start,
            int batchSize,
            IDocumentFileManager documentFileManager,
            IObjectManager objectManager,
            ErrorLocation errorLocation,
            CancellationToken cancellationToken
        )
        {
            var setupExportResult = await SetupExportAsync(
                workspaceId, artifactType, fieldNames, condition, start,
                objectManager
            );

            if (setupExportResult.IsFailure)
                return setupExportResult.MapError(x => x.WithLocation(errorLocation))
                    .ConvertFailure<Array<Entity>>();


            var allResults = GetResultElements(setupExportResult.Value,
                workspaceId, batchSize, fieldNames, documentFileManager, objectManager, errorLocation, cancellationToken
            );

            var array = new LazyArray<Entity>(allResults);

            return array;
        }

        /// <summary>
        /// A token indicating a long string that should be downloaded separately
        /// </summary>
        public const string LongStringToken = "#KCURA99DF2F0FEB88420388879F1282A55760#";

        public const string NativeFileKey = "NativeFile";


        public static async IAsyncEnumerable<Entity> GetResultElements(
            ExportInitializationResults exportResult,
            int workspaceId,
            int batchSize,
            IReadOnlyList<string> fieldNames,
            IDocumentFileManager documentFileManager,
            IObjectManager objectManager,
            ErrorLocation errorLocation,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var fields = fieldNames.Select(x => new Field() { Name = x }).ToList();

            var current = 0;

            while (current < exportResult.RecordCount)
            {
                var resultElements =
                    await objectManager.RetrieveNextResultsBlockFromExportAsync(workspaceId, exportResult.RunID,
                        batchSize);

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
                                var v = await
                                    GetLongText(workspaceId, field.Name, resultElement.ArtifactID, objectManager);

                                if (v.IsFailure)
                                    throw new ErrorException(v.Error.WithLocation(errorLocation));

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

                        string data;

                        try
                        {
                            using IKeplerStream keplerStream =
                                await documentFileManager.DownloadNativeFileAsync(workspaceId,
                                    resultElement.ArtifactID);
                            await using Stream fileStream = await keplerStream.GetStreamAsync();
                            using var streamReader = new StreamReader(fileStream);
                            data = await streamReader.ReadToEndAsync();
                        }
                        catch (Exception e)
                        {
                            throw new ErrorException(ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(e.Message)
                                .WithLocationSingle(errorLocation));
                        }


                        properties.Add(new EntityProperty(NativeFileKey,
                            new EntityValue.String(data),
                            null, order
                        ));

                        var entity = new Entity(properties);

                        yield return entity;
                    }
                }


                current += batchSize;
            }
        }


        public static async Task<Result<ExportInitializationResults, IErrorBuilder>> SetupExportAsync(
            int workspaceId,
            ArtifactType artifactType,
            IReadOnlyList<string> fieldNames,
            string condition,
            int start,
            IObjectManager objectManager)
        {
            var request = new QueryRequest()
            {
                Condition = condition,
                Fields = fieldNames.Select(s => new FieldRef() { Name = s }).ToList(),
                ObjectType = new ObjectTypeRef() { ArtifactTypeID = (int)artifactType }
            };

            ExportInitializationResults exportResult;

            try
            {
                exportResult = await objectManager.InitializeExportAsync(
                    workspaceId, request, start
                );
            }
            catch (Exception e)
            {
                return ErrorCode.Unknown.ToErrorBuilder(e);
            }


            return exportResult;
        }


        public static async Task<Result<string, IErrorBuilder>> GetLongText(
            int workspaceId,
            string fieldName,
            int artifactId,
            IObjectManager objectManager)
        {
            string longText;

            try
            {
                using var keplerStream = await objectManager.StreamLongTextAsync(workspaceId,
                    new RelativityObjectRef() { ArtifactID = artifactId },
                    new FieldRef() { Name = fieldName });
                await using var stream = await keplerStream.GetStreamAsync();
                using var streamReader = new StreamReader(stream);
                longText = await streamReader.ReadToEndAsync();
            }
            catch (Exception e)
            {
                return ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(e.Message);
            }

            return longText;
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