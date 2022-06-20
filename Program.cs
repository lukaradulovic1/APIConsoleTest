using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace APICallerAppConsole
{
    public class Program
    {
        /// <summary>
        /// App for calling api in APP Center
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ExecuteAPICommunication();
        }

        private static void ExecuteAPICommunication()
        {
            WebAPIClient webAPIClient;
            List<Root> jsonBranchModelList;
            List<string> branchNamesList;
            string branchNameLastBuild, buildStatus;
            AcquireAPICommunicationParameters(out webAPIClient, out jsonBranchModelList, out branchNamesList, out branchNameLastBuild, out buildStatus);


            // Deserailize and get latest build info after posting
            var stringToDeserializeUpdated = string.Empty;
            List<Root> jsonBranchModelListUpdated = new List<Root>();

            foreach (var branch in branchNamesList)
            {
                GetCurrentBranchBuildInfo(jsonBranchModelList, out branchNameLastBuild, out buildStatus, branch);

                CreateBuildPOSTMethod(branch);

                Console.WriteLine("Wait 2 minutes for build response.");
                Thread.Sleep(300000);

                PrintUpdatedBranchBuildInfo(webAPIClient, out branchNameLastBuild, out buildStatus, out stringToDeserializeUpdated, out jsonBranchModelListUpdated, branch);

            }
        }

        private static void CreateBuildPOSTMethod(string branch)
        {
            // Create build
            Console.WriteLine($"Calling Post method that creates a new build on {branch} branch");
            WebAPIClient.PostCreateBuild(branch);
        }

        private static void PrintUpdatedBranchBuildInfo(WebAPIClient webAPIClient, out string branchNameLastBuild, out string buildStatus, out string stringToDeserializeUpdated, out List<Root> jsonBranchModelListUpdated, string branch)
        {
            // Update build info from branch

            stringToDeserializeUpdated = webAPIClient.GetBranchesList();

            jsonBranchModelListUpdated = JsonConvert.DeserializeObject<List<Root>>(stringToDeserializeUpdated);

            branchNameLastBuild = jsonBranchModelListUpdated.Where(x => x.Branch.Name.Contains(branch)).Select(x => x.LastBuild.BuildNumber).FirstOrDefault();

            buildStatus = jsonBranchModelListUpdated.Where(x => x.Branch.Name.Contains(branch)).Select(x => x.LastBuild.Status).FirstOrDefault();

            if (buildStatus.Contains("Completed"))
            {
                Console.WriteLine($"Build ID: {branchNameLastBuild} for Branch: {branch} status: {buildStatus} \nBuild log link: {WebAPIClient.GetBuildLogsLink(branchNameLastBuild)}\n");
            }
            else
            {
                Thread.Sleep(125000);
                Console.WriteLine($"Build ID: {branchNameLastBuild} for Branch: {branch} status: {buildStatus} \nBuild log link: {WebAPIClient.GetBuildLogsLink(branchNameLastBuild)}\n");
            }
        }

        private static void GetCurrentBranchBuildInfo(List<Root> jsonBranchModelList, out string branchNameLastBuild, out string buildStatus, string branch)
        {
            branchNameLastBuild = jsonBranchModelList.Where(x => x.Branch.Name.Contains(branch)).Select(x => x.LastBuild.BuildNumber).FirstOrDefault();
            buildStatus = jsonBranchModelList.Where(x => x.Branch.Name.Contains(branch)).Select(x => x.LastBuild.Status).FirstOrDefault();
        }

        private static void AcquireAPICommunicationParameters(out WebAPIClient webAPIClient, out List<Root> jsonBranchModelList, out List<string> branchNamesList, out string branchNameLastBuild, out string buildStatus)
        {
            // Deserailize first JSON list of branches, get json branch models and its values
            webAPIClient = new WebAPIClient();
            var stringToDeserialize = webAPIClient.GetBranchesList();
            jsonBranchModelList = JsonConvert.DeserializeObject<List<Root>>(stringToDeserialize);
            branchNamesList = jsonBranchModelList.Select(x => x.Branch.Name).ToList();
            branchNameLastBuild = string.Empty;
            buildStatus = string.Empty;
        }
    }
    public class WebAPIClient
    {
        public static void PostCreateBuild(string branchName)
        {
            // Post method
            string postBuildUrl = String.Format($"https://api.appcenter.ms/v0.1/apps/lukaradulovic1/Xamarin-antistress-app/branches/{branchName}/builds");
            WebRequest postBuildWebRequest = (HttpWebRequest)WebRequest.Create(postBuildUrl);
            string userTokenPost = "2b2daebe91b83bc7844e7f20b9efb37d5d106d49";
            postBuildWebRequest.ContentType = "application/json";



            postBuildWebRequest.Headers.Add("X-API-Token", userTokenPost);
            postBuildWebRequest.Headers.Add("owner_name", "lukaradulovic1");
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
            Console.WriteLine("Create build initiated.");
        }

        public string GetBranchesList()
        {
            // Get list of branches method
            string userUrlPage = String.Format("https://api.appcenter.ms/v0.1/apps/lukaradulovic1/Xamarin-antistress-app/branches");
            WebRequest requestObject = WebRequest.Create(userUrlPage);
            HttpWebResponse responseObjectGet = null;
            string userTokenGet = "2b2daebe91b83bc7844e7f20b9efb37d5d106d49";

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(userTokenGet);

            requestObject.Headers.Add("X-API-Token", userTokenGet);
            requestObject.Headers.Add("owner_name", "lukaradulovic1");
            requestObject.Headers.Add("app_name", "Xamarin-antistress-app");
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
        public static string GetBuildLogsLink(string buildID)
        {
            return string.Format($"https://api.appcenter.ms/v0.1/apps/lukaradulovic1/Xamarin-antistress-app/builds/{buildID}/logs?build_id={buildID}&owner_name=lukaradulovic1&app_name=Xamarin-antistress-app");
        }
    }

}
