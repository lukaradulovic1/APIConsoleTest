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
    public class Program : WebAPICommunicator
    {
        // Main app calls the webAPIclient which calls POST and GET methods
        // webAPICommunicator gathers all data needed for the client
        static void Main(string[] args)
        {
            WebAPIClient webAPIClient = new WebAPIClient();
            WebAPICommunicator webAPICommunicator = new WebAPICommunicator();
            webAPICommunicator.ExecuteAPICommunication();
        }
    }


}
