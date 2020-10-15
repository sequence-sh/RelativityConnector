using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace Reductech.Connectors.Relativity
{
    public static class DocumentQueryHelper
    {
        public static async Task<Result<DocumentQueryResult>>  GetDocumentQueryResultAsync(IRelativitySettings relativitySettings, int workspaceArtifactID)
        {
            var url = Url.Combine(relativitySettings.Url, $"Relativity.REST/Workspace/{workspaceArtifactID}/Document");

            DocumentQueryResult response;

            try
            {
                response = await
                    url.WithBasicAuth(relativitySettings.RelativityUsername, relativitySettings.RelativityPassword)
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
            [JsonProperty("ArtifactID")]
            public int ArtifactID { get; set; }

            [JsonProperty("Guids")]
            public object Guids { get; set; }

            [JsonProperty("ArtifactTypeID")]
            public object ArtifactTypeID { get; set; }

            [JsonProperty("ArtifactTypeName")]
            public object ArtifactTypeName { get; set; }

            [JsonProperty("ArtifactTypeGuids")]
            public object ArtifactTypeGuids { get; set; }

            [JsonProperty("__Location")]
            public object Location { get; set; }
        }

        public class Field
        {
            /// <inheritdoc />
            public override string ToString() => Name;


            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("FieldType")]
            public string FieldType { get; set; }

            [JsonProperty("FieldGuids")]
            public object FieldGuids { get; set; }

            [JsonProperty("FieldArtifactID")]
            public int FieldArtifactID { get; set; }
        }

        public class DocumentResult
        {
            /// <inheritdoc />
            public override string ToString() => Location ?? (ArtifactID.ToString());

            [JsonProperty("Artifact ID")]
            public int ArtifactID { get; set; }

            [JsonProperty("Guids")]
            public object Guids { get; set; }

            [JsonProperty("Artifact Type ID")]
            public int ArtifactTypeID { get; set; }

            [JsonProperty("ArtifactTypeName")]
            public string ArtifactTypeName { get; set; }

            [JsonProperty("ArtifactTypeGuids")]
            public List<string> ArtifactTypeGuids { get; set; }

            [JsonProperty("ParentArtifact")]
            public ParentArtifact ParentArtifact { get; set; }

            [JsonProperty("__Location")]
            public string Location { get; set; }

            [JsonProperty("RelativityTextIdentifier")]
            public string RelativityTextIdentifier { get; set; }

            [JsonProperty("__Fields")]
            public List<Field> Fields { get; set; }
        }

        public class DocumentQueryResult
        {
            [JsonProperty("TotalResultCount")]
            public int TotalResultCount { get; set; }

            [JsonProperty("ResultCount")]
            public int ResultCount { get; set; }

            [JsonProperty("Results")]
            public List<DocumentResult> Results { get; set; }

            [JsonProperty("NextPage")]
            public object NextPage { get; set; }

            [JsonProperty("PreviousPage")]
            public object PreviousPage { get; set; }

            [JsonProperty("QueryToken")]
            public object QueryToken { get; set; }
        }
    }


}
