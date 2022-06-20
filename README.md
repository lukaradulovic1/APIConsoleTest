# APIConsoleTest

## Description

Small project for calling MS APP CENTER API to GET or POST build data for custom Xamarin app using custom authentication token for user and app token aswell. 
App automatically calls GET methods, gathers branch info and for each branch calls a build, returning branch name, build id, build status and build log link to download the file.

## Getting Started

### Dependencies

Windows 10//1
Visual Studio 2019/2022
.NET Core 3.1 Link: https://dotnet.microsoft.com/en-us/download/dotnet/3.1

### Installing

Download files and open APICallerAppConsole.exe in localfolder\APICallerAppConsole\bin\Debug\netcoreapp3.1

### Executing program

Open APICallerAppConsole.exe or build solution in VS 2019/2022
Requires 2-3 minutes for first completed build and retries in 15s intervals if the build is not complete. 

Acknowledgments
Djordje Cvijic
Bojan Gigovski
Djordje Petrovic
