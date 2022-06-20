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
            // Deserailize first JSON list of branches, get json branch model
            var stringToDeserialize = new Program().GetBranchesList();
            var jsonBranchModelList = JsonConvert.DeserializeObject<List<Root>>(stringToDeserialize);
            var branchNamesList = jsonBranchModelList.Select(x => x.Branch.Name).ToList();
            var branchNameLastBuild = string.Empty;
            var buildStatus = string.Empty;

            var buildLogFromBetaBranch = string.Empty;

            // Deserailize and get latest build info after posting
            var stringToDeserializeUpdated = string.Empty;
            List<Root> jsonBranchModelListUpdated = new List<Root>();

            foreach (var branch in branchNamesList)
            {
                branchNameLastBuild = jsonBranchModelList.Where(x => x.Branch.Name.Contains(branch)).Select(x => x.LastBuild.BuildNumber).FirstOrDefault();
                buildStatus = jsonBranchModelList.Where(x => x.Branch.Name.Contains(branch)).Select(x => x.LastBuild.Status).FirstOrDefault();

                // Create build
                Console.WriteLine($"Calling Post method that creates a new build on {branch} branch");
                PostCreateBuild(branch);

                Console.WriteLine("Wait 2 minutes for build response.");
                Thread.Sleep(180000);

                // Update build info from branch

                stringToDeserializeUpdated = new Program().GetBranchesList();

                jsonBranchModelListUpdated = JsonConvert.DeserializeObject<List<Root>>(stringToDeserializeUpdated);

                branchNameLastBuild = jsonBranchModelListUpdated.Where(x => x.Branch.Name.Contains(branch)).Select(x => x.LastBuild.BuildNumber).FirstOrDefault();

                buildStatus = jsonBranchModelListUpdated.Where(x => x.Branch.Name.Contains(branch)).Select(x => x.LastBuild.Status).FirstOrDefault();
                if (buildStatus.Contains("Completed"))
                {
                    Console.WriteLine($"Build ID: {branchNameLastBuild} for Branch: {branch} status: {buildStatus} \nBuild log link: {GetBuildLogsLink(branchNameLastBuild)}\n");
                }
                else
                {
                    Thread.Sleep(125000);
                    Console.WriteLine($"Build ID: {branchNameLastBuild} for Branch: {branch} status: {buildStatus} \nBuild log link: {GetBuildLogsLink(branchNameLastBuild)}\n");
                }
                
            }
        }


        private static void PostCreateBuild(string branchName)
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

        private string GetBranchesList()
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
        private static string GetBuildLogsLink(string buildID)
        {
            return string.Format($"https://api.appcenter.ms/v0.1/apps/lukaradulovic1/Xamarin-antistress-app/builds/{buildID}/logs?build_id=17&owner_name=lukaradulovic1&app_name=Xamarin-antistress-app");
        }

    }

}
