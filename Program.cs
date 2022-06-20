namespace APICallerAppConsole
{
    public class Program : WebAPICommunicator
    {
        // Main app calls the webAPIclient which calls POST and GET methods
        // webAPICommunicator gathers all data needed for the client
        static void Main()
        {
            new WebAPICommunicator().ExecuteAPICommunication();
        }
    }
}
