using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace Reductech.Connectors.Relativity
{
    public static class RestRetrieve
    {

        public static async Task<Result<ResultClasses.RequestResult>> GetResultAsync(IRelativitySettings settings, int workspaceId)
        {
            var urlSuffix = $"/Relativity.REST/api/Relativity.Objects/workspace/{workspaceId}/object/read";

            var url = Url.Combine(settings.Url, urlSuffix);

            var requestQuery = new RequestClasses.RequestQuery()
            {
                Request = new RequestClasses.Request()
                {
                    Object = new RequestClasses.Object()
                    {
                        ArtifactID = 1040848
                    },
                    Fields = new List<RequestClasses.Field>()
                    {
                        new RequestClasses.Field()
                        {
                            Name = "Control Number"
                        }
                    }
                }
            };

            ResultClasses.RequestResult requestResult;


            try
            {
                requestResult = await url.WithBasicAuth(settings.RelativityUsername, settings.RelativityPassword)
                    .WithHeader("X-CSRF-Header", "-")
                    .PostJsonAsync(requestQuery)
                .ReceiveJson<ResultClasses.RequestResult>();
            }
            catch (FlurlHttpException e)
            {
                return Result.Failure<ResultClasses.RequestResult>(e.Message);
            }


            return requestResult;
        }


        public static class RequestClasses
        {
            public class RequestQuery
            {
                [JsonProperty("Request")]
                public Request Request { get; set; }


            }

            public class Object
            {
                [JsonProperty("ArtifactID")]
                public int ArtifactID { get; set; }
            }

            public class Field
            {
                [JsonProperty("Name")]
                public string Name { get; set; }
            }

            public class Request
            {
                [JsonProperty("Object")]
                public Object Object { get; set; }

                [JsonProperty("Fields")]
                public List<Field> Fields { get; set; }
            }
        }

        public static class ResultClasses
        {

            public class RequestResult
            {
                [JsonProperty("Message")]
                public string Message { get; set; }

                [JsonProperty("Object")]
                public Object Object { get; set; }

                [JsonProperty("ObjectType")]
                public ObjectType ObjectType { get; set; }
            }
            public class ParentObject
            {
                [JsonProperty("ArtifactID")]
                public int ArtifactID { get; set; }
            }

            public class Field
            {
                /// <inheritdoc />
                public override string ToString() => Name;


                [JsonProperty("ArtifactID")]
                public int ArtifactID { get; set; }

                [JsonProperty("FieldCategory")]
                public string FieldCategory { get; set; }

                [JsonProperty("FieldType")]
                public string FieldType { get; set; }

                [JsonProperty("Guids")]
                public List<string> Guids { get; set; }

                [JsonProperty("Name")]
                public string Name { get; set; }

                [JsonProperty("ViewFieldID")]
                public int ViewFieldID { get; set; }
            }

            public class FieldValue
            {
                [JsonProperty("Field")]
                public Field Field { get; set; }

                [JsonProperty("Value")]
                public object Value { get; set; }
            }

            public class Object
            {
                [JsonProperty("ParentObject")]
                public ParentObject ParentObject { get; set; }

                [JsonProperty("FieldValues")]
                public List<FieldValue> FieldValues { get; set; }

                [JsonProperty("ArtifactID")]
                public int ArtifactID { get; set; }

                [JsonProperty("Guids")]
                public List<object> Guids { get; set; }
            }

            public class ObjectType
            {
                /// <inheritdoc />
                public override string ToString() => Name;

                [JsonProperty("ArtifactID")]
                public int ArtifactID { get; set; }

                [JsonProperty("Name")]
                public string Name { get; set; }

                [JsonProperty("Guids")]
                public List<string> Guids { get; set; }

                [JsonProperty("ArtifactTypeID")]
                public int ArtifactTypeID { get; set; }
            }


        }




    }
}
