using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using CSharpFunctionalExtensions;
using Newtonsoft.Json.Linq;

namespace Reductech.Connectors.Relativity
{
    public static class Rest
    {
        public static Result<string> Main(RelativitySettings settings)
        {
            //Initialize the HttpClient.
            var client = new HttpClient {BaseAddress = new Uri("http://localhost/")};

            //Add the Accept header.

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            //Set the required headers for Relativity.REST.
            client.DefaultRequestHeaders.Add("X-CSRF-Header", "-");
            client.DefaultRequestHeaders.Add("Authorization", settings.AuthParameters);

            //Read the Workspace collection.
            var workspaceCollectionString = GetWorkspaceCollectionAsString(client, settings);

            if (workspaceCollectionString.IsFailure)
                return workspaceCollectionString;


            //Get the Artifact ID for the first Workspace in the collection.
            var workspaceArtifactID = ParseWorkspaceArtifactIDFromWorkspaceCollectionString(workspaceCollectionString.Value);

            if (workspaceArtifactID.IsFailure)
                return workspaceArtifactID.ConvertFailure<string>();

            //Get the __Location (URL) of the first Document in the Workspace.
            var firstDocumentUrl = GetFirstDocumentUrlForWorkspace(client, settings, workspaceArtifactID.Value);

            if (firstDocumentUrl.IsFailure)
                return firstDocumentUrl;

            return firstDocumentUrl;
        }


        private static Result<string> GetWorkspaceCollectionAsString(HttpClient client, RelativitySettings relativitySettings)
        {
            var url = Path.Combine(relativitySettings.Url, "Relativity.Rest/Relativity/Workspace");
            var response = client.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
                return Result.Failure<string>($"The collection GET for Workspace failed. {response.ReasonPhrase}");

            var r = response.Content.ReadAsStringAsync().Result;
            return r;

        }

        private static Result<int> ParseWorkspaceArtifactIDFromWorkspaceCollectionString(string workspaceCollection)
        {
            if (string.IsNullOrEmpty(workspaceCollection))
                return Result.Failure<int>("Could not parse Workspace collection string because it was null or empty.");

            //Turn the workspace collection string into a JObject and obtain the Results JArray.

            var jWorkspaceResult = JObject.Parse(workspaceCollection);

            if (!(jWorkspaceResult["Results"] is JArray workSpaces))
                return Result.Failure<int>("The Workspace collection is missing or null.");

            if(!workSpaces.Any())
                return Result.Failure<int>("The Workspace collection is empty.");


            var firstWorkspace = workSpaces.First();
            var workspaceArtifactId = (int?)firstWorkspace["Artifact ID"];

            if (workspaceArtifactId.HasValue)
                return workspaceArtifactId.Value;

            return Result.Failure<int>("Could not find an 'Artifact ID' on the first Workspace.");

        }


        private static Result<string> GetFirstDocumentUrlForWorkspace(HttpClient client, RelativitySettings relativitySettings, int workspaceArtifactID)
        {
            var url = Path.Combine(relativitySettings.Url, $"Relativity.REST/Workspace/{workspaceArtifactID}/Document");

            var response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
                return Result.Failure<string>($"The collection GET for Documents failed. {response.ReasonPhrase}");
            //Read the Documents collection, and parse out the Results JArray.

            var documents = response.Content.ReadAsStringAsync().Result;
            var jDocumentResult = JObject.Parse(documents);
            var jDocuments = jDocumentResult["Results"] as JArray;

            //If the array of Documents contained data, obtain the  __Location of the first Document.

            if (jDocuments != null && jDocuments.Any())
                return GetUrlFromDocument(jDocuments.First());


            return Result.Failure<string>(jDocuments == null ? "The Document collection is missing or null." : "The Document collection is empty.");

        }


        private static Result<string> GetUrlFromDocument(JToken documentJToken)
        {
            switch (documentJToken)
            {
                case null:
                    return Result.Failure<string>("The Document JToken is null.");
                case JObject jObject:
                {
                    var url = (string)jObject["__Location"];
                    if (string.IsNullOrWhiteSpace(url))
                        return Result.Failure<string>("The Document's __Location is missing or null.");
                    return url;
                }
                default:
                    return Result.Failure<string>($"The Document JTokenType is a '{documentJToken.Type}' but should be a JObject.");
            }
        }

    }
}
