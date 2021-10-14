using System.Text.Json;

namespace H21.Wellness
{
    public static class Constants
    {
        public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static class EnvironmentSettings
        {
            public const string KeyVaultUri = "KeyVaultUri";

            public const string Environment = "ASPNETCORE_ENVIRONMENT";
        }

        public static class ComputerVisionConstants
        {
            public const double MinConfidence = 0.7;
        }
    }
}