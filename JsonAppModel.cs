using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace APICallerAppConsole
{// Root myDeserializedClass = JsonSerializer.Deserialize<List<Root>>(myJsonResponse);
    public class Branch
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("commit")]
        public Commit Commit { get; set; }

        [JsonPropertyName("protected")]
        public bool Protected { get; set; }

        [JsonPropertyName("protection")]
        public Protection Protection { get; set; }

        [JsonPropertyName("protection_url")]
        public string ProtectionUrl { get; set; }
    }

    public class Commit
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class LastBuild
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("buildNumber")]
        public string BuildNumber { get; set; }

        [JsonPropertyName("queueTime")]
        public DateTime QueueTime { get; set; }

        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("finishTime")]
        public DateTime FinishTime { get; set; }

        [JsonPropertyName("lastChangedDate")]
        public DateTime LastChangedDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("sourceBranch")]
        public string SourceBranch { get; set; }

        [JsonPropertyName("sourceVersion")]
        public string SourceVersion { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        [JsonPropertyName("properties")]
        public Properties Properties { get; set; }
    }

    public class Properties
    {
    }

    public class Protection
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("required_status_checks")]
        public RequiredStatusChecks RequiredStatusChecks { get; set; }
    }

    public class RequiredStatusChecks
    {
        [JsonPropertyName("enforcement_level")]
        public string EnforcementLevel { get; set; }

        [JsonPropertyName("contexts")]
        public List<object> Contexts { get; set; }

        [JsonPropertyName("checks")]
        public List<object> Checks { get; set; }
    }

    public class Root
    {
        [JsonPropertyName("branch")]
        public Branch Branch { get; set; }

        [JsonPropertyName("configured")]
        public bool Configured { get; set; }

        [JsonPropertyName("lastBuild")]
        public LastBuild LastBuild { get; set; }

        [JsonPropertyName("trigger")]
        public string Trigger { get; set; }
    }
}


