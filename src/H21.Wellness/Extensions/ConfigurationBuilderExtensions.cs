using System;
using System.IO;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace H21.Wellness.Extensions
{
    /// <summary>
    ///     Provides extension methods for <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        ///     Adds the common <see cref="IConfigurationProvider"/> to the <see cref="IConfigurationBuilder"/>.
        /// </summary>
        /// <param name="source">The source <see cref="IConfigurationBuilder"/>.</param>
        /// <param name="applicationRootPath">The application root path.</param>
        public static void ConfigureWellness(
            this IConfigurationBuilder source,
            string applicationRootPath = null)
        {
            source.ThrowIfNull(nameof(source));

            source.AddEnvironmentVariables();
            source.AddAzureKeyVaultConfigurationProvider();
            source.AddJsonConfigurationProvider(applicationRootPath);
        }

        private static void AddJsonConfigurationProvider(
            this IConfigurationBuilder source,
            string applicationRootPath = null)
        {
            source.ThrowIfNull(nameof(source));

            var appSettingsFileName = "appsettings.json";

            source
                .AddJsonFile(
                    path: applicationRootPath == null ? appSettingsFileName : Path.Combine(applicationRootPath, appSettingsFileName),
                    optional: true,
                    reloadOnChange: false);

            var environment = Environment.GetEnvironmentVariable(Constants.EnvironmentSettings.Environment);

            if (!string.IsNullOrWhiteSpace(environment))
            {
                var appSettingsWithEnvironmentFileName = $"appsettings.{environment}.json";

                source.AddJsonFile(
                    path: applicationRootPath == null ? appSettingsWithEnvironmentFileName : Path.Combine(applicationRootPath, appSettingsWithEnvironmentFileName),
                    optional: true,
                    reloadOnChange: false);
            }
        }

        private static void AddAzureKeyVaultConfigurationProvider(
            this IConfigurationBuilder source)
        {
            source.ThrowIfNull(nameof(source));

            var keyVaultUri = Environment.GetEnvironmentVariable(Constants.EnvironmentSettings.KeyVaultUri);

            if (!string.IsNullOrWhiteSpace(keyVaultUri))
            {
                var environment = Environment.GetEnvironmentVariable(Constants.EnvironmentSettings.Environment);

                if (!string.IsNullOrWhiteSpace(environment) && environment.Equals("Development"))
                {
                    source.AddAzureKeyVault(new Uri(keyVaultUri), new AzureCliCredential());
                }
                else
                {
                    source.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
                }
            }
        }
    }
}