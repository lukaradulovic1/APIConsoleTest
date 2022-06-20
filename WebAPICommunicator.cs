using System;
using System.Threading;

namespace APICallerAppConsole
{
    public class WebAPICommunicator
    {
        public const string userToken = "d51f9803a5415da3ebb1c3e23694566a89066d62";
        public const string appName = "Xamarin-antistress-app";
        private const int sleepSeconds = 125000;
        private const int retrySeconds = 15000;

        /// <summary>
        /// Print out all branch names
        /// </summary>
        /// <param name="webAPIClient"></param>
        private void PrintBranches(WebAPIClient webAPIClient)
        {
            var branchInfoList = webAPIClient.GetBranchesList(userToken, appName);

            Console.WriteLine("Available branches are: ");
            foreach (var branch in branchInfoList)
            {
                Console.WriteLine(branch.Branch.Name);
            }
        }

        /// <summary>
        /// Execute communication with Web API and call POST method that creates a build, waits 2 minutes for completion and depending on output waits or prints out the results
        /// </summary>
        public void ExecuteAPICommunication()
        {
            var webAPIClient = new WebAPIClient();
            PrintBranches(webAPIClient);
            var branchInfoList = webAPIClient.GetBranchesList(userToken, appName);

            // Deserailize and get latest build info after posting
            foreach (var branch in branchInfoList)
            {
                // Create build
                Console.WriteLine($"\nCalling Post method that creates a new build on {branch.Branch.Name} branch");
                webAPIClient.CreateBuild(branch.Branch.Name, userToken, branch.Branch.Commit.Sha);

                Console.WriteLine("Waiting for build response.");
                Thread.Sleep(sleepSeconds);

                PrintUpdatedBranchBuildInfo(webAPIClient, branch);
            }
        }

        /// <summary>
        /// Print out branch build values and log the creation in case the build is not complete in expected time interval
        /// </summary>
        /// <param name="webAPIClient"></param>
        /// <param name="branch"></param>
        private void PrintUpdatedBranchBuildInfo(WebAPIClient webAPIClient, BranchInfo branch)
        {
            // Update build info from branch
            bool isCompleted = false;
            do
            {
                if (branch.LastBuild.Status.Contains("completed"))
                {
                    Console.WriteLine(
                        $"Branch: {branch.Branch.Name} " +
                        $"Build ID: {branch.LastBuild.Id} " +
                        $"Build status: {branch.LastBuild.Status} \n" +
                        $"Build log link: {webAPIClient.GenerateBuildLogLink(branch.LastBuild.BuildNumber)}\n");
                    isCompleted = true;
                }
                Thread.Sleep(retrySeconds);
            }
            while (!isCompleted);
        }
    }
}