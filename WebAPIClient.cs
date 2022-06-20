using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace APICallerAppConsole
{
    public class WebAPIClient
    {
        private const string userApiPage = "https://api.appcenter.ms/v0.1/apps/lukaradulovic2/Xamarin-antistress-app/branches";
        private const string createBuildEndPoint = "https://api.appcenter.ms/v0.1/apps/lukaradulovic2/Xamarin-antistress-app/branches/beta/builds";
        private const string apiUser = "lukaradulovic2";
        private const string appName = "Xamarin-antistress-app";

        public void CreateBuild(string branchName, string userToken, string commitId)
        {
            // Post method
            WebRequest postBuildWebRequest = (HttpWebRequest)WebRequest.Create(createBuildEndPoint);
            postBuildWebRequest.ContentType = "application/json";
            postBuildWebRequest.Headers.Add("X-API-Token", userToken);
            postBuildWebRequest.Headers.Add("owner_name", apiUser);
            postBuildWebRequest.Headers.Add("branch", branchName);
            postBuildWebRequest.Headers.Add("app_name", appName);
            postBuildWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(postBuildWebRequest.GetRequestStream()))
            {
                var buildSource = new BuildSource()
                {
                    sourceVersion = commitId,
                    debug = true
                };
                streamWriter.Write(JsonConvert.SerializeObject(buildSource));
            }

            postBuildWebRequest.GetResponse();
            Console.WriteLine("\nCreate build initiated.\n");
        }

        public List<BranchInfo> GetBranchesList(string userToken, string appName)
        {
            // Get list of branches method
            string userUrlPage = String.Format(userApiPage);
            WebRequest requestObject = WebRequest.Create(userUrlPage);

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(userToken);

            requestObject.Headers.Add("X-API-Token", userToken);
            requestObject.Headers.Add("owner_name", apiUser);
            requestObject.Headers.Add("app_name", appName);
            requestObject.Method = "GET";
            var responseObjectGet = (HttpWebResponse)requestObject.GetResponse();
            string branchListResult = null;
            using (Stream stream = responseObjectGet.GetResponseStream())
            {
                StreamReader streamReader = new StreamReader(stream);
                branchListResult = streamReader.ReadToEnd();
                streamReader.Close();
            }
            return JsonConvert.DeserializeObject<List<BranchInfo>>(branchListResult);
        }

        public string GenerateBuildLogLink(string buildID)
        {
            return $"https://api.appcenter.ms/v0.1/apps/lukaradulovic2/Xamarin-antistress-app/builds/{buildID}"+$"/logs?build_id={buildID}&owner_name=lukaradulovic2&app_name=Xamarin-antistress-app";
        }
    }
}
