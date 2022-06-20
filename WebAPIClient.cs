using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace APICallerAppConsole
{
    public class WebAPIClient
    {


        public void PostCreateBuild(string branchName, string userToken)
        {
            // Post method
            string postBuildUrl = String.Format($"https://api.appcenter.ms/v0.1/apps/lukaradulovic2/Xamarin-antistress-app/branches/{branchName}/builds");
            WebRequest postBuildWebRequest = (HttpWebRequest)WebRequest.Create(postBuildUrl);

            postBuildWebRequest.ContentType = "application/json";



            postBuildWebRequest.Headers.Add("X-API-Token", userToken);
            postBuildWebRequest.Headers.Add("owner_name", "lukaradulovic2");
            postBuildWebRequest.Headers.Add("branch", branchName);
            postBuildWebRequest.Headers.Add("app_name", "Xamarin-antistress-app");
            postBuildWebRequest.Method = "POST";


            using (var streamWriter = new StreamWriter(postBuildWebRequest.GetRequestStream()))
            {
                string json = "{\"sourceVersion\":\"613af73288c221cf2b69d2d9c567ef8079300aa5\"," +
                              "\"debug\": true}";

                streamWriter.Write(json);
            }
            HttpWebResponse responseObjectPost = null;
            responseObjectPost = (HttpWebResponse)postBuildWebRequest.GetResponse();
            Console.WriteLine("\nCreate build initiated.\n");
        }

        public string GetBranchesList(string userToken, string appName)
        {
            // Get list of branches method
            string userUrlPage = String.Format("https://api.appcenter.ms/v0.1/apps/lukaradulovic2/Xamarin-antistress-app/branches");
            WebRequest requestObject = WebRequest.Create(userUrlPage);
            HttpWebResponse responseObjectGet = null;

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(userToken);

            requestObject.Headers.Add("X-API-Token", userToken);
            requestObject.Headers.Add("owner_name", "lukaradulovic2");
            requestObject.Headers.Add("app_name", appName);
            requestObject.Method = "GET";
            responseObjectGet = (HttpWebResponse)requestObject.GetResponse();
            string branchListResult = null;
            using (Stream stream = responseObjectGet.GetResponseStream())
            {
                StreamReader streamReader = new StreamReader(stream);
                branchListResult = streamReader.ReadToEnd();
                streamReader.Close();
            }
            return branchListResult;
        }
        public string GetBuildLogsLink(string buildID)
        {
            return string.Format($"https://api.appcenter.ms/v0.1/apps/lukaradulovic2/Xamarin-antistress-app/builds/{buildID}/logs?build_id={buildID}&owner_name=lukaradulovic2&app_name=Xamarin-antistress-app");
        }
    }
}
