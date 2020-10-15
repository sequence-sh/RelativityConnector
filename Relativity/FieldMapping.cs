using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace Reductech.Connectors.Relativity
{
    public static class FieldMapping
    {
        public static async Task<MappableSourceField[]> GetMappableFields(IRelativitySettings relativitySettings, bool catalogFieldsOnly, int workspaceArtifactID)
        {
            var relativitySuffix =
                "Relativity.Rest/api/Relativity.Services.FieldMapping.IFieldMappingModule/FieldMappingService/GetInvariantFieldsAsync";

            var url = Url.Combine(relativitySettings.Url, relativitySuffix);

            var request = new MappableSourceFieldRequest
            {
                CatalogFieldsOnly = catalogFieldsOnly,
                WorkspaceArtifactID = workspaceArtifactID
            };

            var result = await

                url
                    .WithHeader("X-CSRF-Header", "-")
                    .WithBasicAuth(relativitySettings.RelativityUsername, relativitySettings.RelativityPassword)
                    .PostJsonAsync(request)
                    .ReceiveJson<MappableSourceField[]>();




            return result;
        }

        public class MappableSourceFieldRequest
        {
            [JsonProperty("workspaceArtifactID")]
            public int WorkspaceArtifactID { get; set; }

            [JsonProperty("catalogFieldsOnly")]
            public bool CatalogFieldsOnly { get; set; }
        }


        public class MappableSourceField
        {
            /// <inheritdoc />
            public override string ToString() => (FriendlyName, Category, SourceName, DataType).ToString();

            [JsonProperty("Category")]
            public string Category { get; set; }

            [JsonProperty("SourceName")]
            public string SourceName { get; set; }

            [JsonProperty("FriendlyName")]
            public string FriendlyName { get; set; }

            [JsonProperty("Description")]
            public string Description { get; set; }

            [JsonProperty("DataType")]
            public string DataType { get; set; }

            [JsonProperty("MappedFields")]
            public List<string> MappedFields { get; set; }
        }

    }
}
