using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace APICallerAppConsole
{
    public class WebAPICommunicator
    {
        public const string userToken = "d51f9803a5415da3ebb1c3e23694566a89066d62";
        public const string appName = "Xamarin-antistress-app";

        // Gather all neccessary variables needed for the API calls
        private void AcquireAPICommunicationParameters(out WebAPIClient webAPIClient, out List<Root> jsonBranchModelList, out List<string> branchNamesList, out string branchNameLastBuild, out string buildStatus)
        {
            // Deserailize first JSON list of branches, get json branch models and its values
            webAPIClient = new WebAPIClient();
            var stringToDeserialize = webAPIClient.GetBranchesList(userToken, appName);
            jsonBranchModelList = JsonConvert.DeserializeObject<List<Root>>(stringToDeserialize);
            branchNamesList = jsonBranchModelList.Select(x => x.Branch.Name).ToList();
            branchNameLastBuild = string.Empty;
            buildStatus = string.Empty;

            Console.WriteLine("Available branches are: ");
            foreach (var branch in branchNamesList)
            {
                Console.WriteLine(branch);
            }
        }

        // Execute communication with Web API and call POST method that creates a build, waits 2 minutes for completion and depending on output waits or prints out the results
        public void ExecuteAPICommunication()
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

                // Create build
                Console.WriteLine($"\nCalling Post method that creates a new build on {branch} branch");
                webAPIClient.PostCreateBuild(branch, userToken);

                Console.WriteLine("Waiting for build response.");
                Thread.Sleep(125000);

                PrintUpdatedBranchBuildInfo(webAPIClient, out branchNameLastBuild, out buildStatus, out stringToDeserializeUpdated, out jsonBranchModelListUpdated, branch);

            }
        }

        // Print out branch build values and log the creation in case the build is not complete in expected time interval
        private void PrintUpdatedBranchBuildInfo(WebAPIClient webAPIClient, out string branchNameLastBuild, out string buildStatus, out string stringToDeserializeUpdated, out List<Root> jsonBranchModelListUpdated, string branch)
        {
            // Update build info from branch

            Console.WriteLine("Getting branch values from JSON response\n");
            stringToDeserializeUpdated = webAPIClient.GetBranchesList(userToken, appName);
            Console.WriteLine("Updating branch data after POST method\n");
            jsonBranchModelListUpdated = JsonConvert.DeserializeObject<List<Root>>(stringToDeserializeUpdated);
            Console.WriteLine("Getting last build from branch\n");
            branchNameLastBuild = jsonBranchModelListUpdated.Where(x => x.Branch.Name.Contains(branch)).Select(x => x.LastBuild.BuildNumber).FirstOrDefault();
            Console.WriteLine("Getting build status of current build\n");
            buildStatus = jsonBranchModelListUpdated.Where(x => x.Branch.Name.Contains(branch)).Select(x => x.LastBuild.Status).FirstOrDefault();
            Console.WriteLine($"Current build status is {buildStatus}\n");

            bool isComplete = false;
            while (!isComplete)
            {
                Console.WriteLine("Retrying for new build status.\n");
                if (buildStatus.Contains("completed"))
                {
                    Console.WriteLine($"Branch: {branch} Build ID: {branchNameLastBuild} Build status: {buildStatus} \nBuild log link: {webAPIClient.GetBuildLogsLink(branchNameLastBuild)}\n");
                    isComplete = true;
                }
                Thread.Sleep(15000);
                stringToDeserializeUpdated = webAPIClient.GetBranchesList(userToken, appName);
                jsonBranchModelListUpdated = JsonConvert.DeserializeObject<List<Root>>(stringToDeserializeUpdated);
                buildStatus = jsonBranchModelListUpdated.Where(x => x.Branch.Name.Contains(branch)).Select(x => x.LastBuild.Status).FirstOrDefault();
            }
        }
    }
}