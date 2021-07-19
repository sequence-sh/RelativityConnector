using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace Reductech.EDR.Connectors.Relativity
{
    public static class DocumentQueryHelper
    {
        public static async Task<Result<DocumentQueryResult>> GetDocumentQueryResultAsync(
            RelativitySettings relativitySettings, int workspaceArtifactID, IFlurlClient flurlClient)
        {
            var url = Url.Combine(relativitySettings.Url, $"Relativity.REST/Workspace/{workspaceArtifactID}/Document");

            DocumentQueryResult response;

            try
            {
                response = await
                    url.WithBasicAuth(relativitySettings.RelativityUsername, relativitySettings.RelativityPassword)
                        .WithClient(flurlClient)
                        .GetJsonAsync<DocumentQueryResult>();
            }
            catch (FlurlHttpException e)
            {
                return Result.Failure<DocumentQueryResult>(e.Message);
            }

            if (response.Results.Count < 1)
                return Result.Failure<DocumentQueryResult>("The Document collection is empty.");

            return response;
        }


        public class ParentArtifact
        {
            [JsonProperty("ArtifactID")] public int ArtifactID { get; set; }

            [JsonProperty("Guids")] public object Guids { get; set; } = null!;

            [JsonProperty("ArtifactTypeID")] public object ArtifactTypeID { get; set; } = null!;

            [JsonProperty("ArtifactTypeName")] public object ArtifactTypeName { get; set; } = null!;

            [JsonProperty("ArtifactTypeGuids")] public object ArtifactTypeGuids { get; set; } = null!;

            [JsonProperty("__Location")] public object Location { get; set; } = null!;
        }

        public class Field
        {
            /// <inheritdoc />
            public override string ToString() => Name;


            [JsonProperty("Name")] public string Name { get; set; } = null!;

            [JsonProperty("FieldType")] public string FieldType { get; set; } = null!;

            [JsonProperty("FieldGuids")] public object FieldGuids { get; set; } = null!;

            [JsonProperty("FieldArtifactID")] public int FieldArtifactID { get; set; }
        }

        public class DocumentResult
        {
            /// <inheritdoc />
            public override string ToString() => Location ?? (ArtifactID.ToString());

            [JsonProperty("Artifact ID")] public int ArtifactID { get; set; }

            [JsonProperty("Guids")] public object Guids { get; set; } = null!;

            [JsonProperty("Artifact Type ID")] public int ArtifactTypeID { get; set; }

            [JsonProperty("ArtifactTypeName")] public string ArtifactTypeName { get; set; } = null!;

            [JsonProperty("ArtifactTypeGuids")] public List<string> ArtifactTypeGuids { get; set; } = null!;

            [JsonProperty("ParentArtifact")] public ParentArtifact ParentArtifact { get; set; } = null!;

            [JsonProperty("__Location")] public string Location { get; set; } = null!;

            [JsonProperty("RelativityTextIdentifier")]
            public string RelativityTextIdentifier { get; set; }

            [JsonProperty("__Fields")] public List<Field> Fields { get; set; } = null!;
        }

        public class DocumentQueryResult
        {
            [JsonProperty("TotalResultCount")] public int TotalResultCount { get; set; }

            [JsonProperty("ResultCount")] public int ResultCount { get; set; }

            [JsonProperty("Results")] public List<DocumentResult> Results { get; set; } = null!;

            [JsonProperty("NextPage")] public object NextPage { get; set; } = null!;

            [JsonProperty("PreviousPage")] public object PreviousPage { get; set; } = null!;

            [JsonProperty("QueryToken")] public object QueryToken { get; set; } = null!;
        }
    }
}