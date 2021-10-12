using System;
using System.IO;
using System.Runtime.InteropServices;
using Azure.Core;
using Azure.Identity;
using H21.Wellness;
using H21.Wellness.Extensions;
using Microsoft.Extensions.Configuration;

namespace Fenix.Extensions
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
        public static void ConfigureFenix(
            this IConfigurationBuilder source,
            string applicationRootPath = null)
        {
            source.ThrowIfNull(nameof(source));

            var environment = Environment.GetEnvironmentVariable(Constants.EnvironmentSettings.FenixEnvironment);

            source.AddEnvironmentVariables();
            source.AddAzureKeyVaultConfigurationProvider();
            source.AddJsonConfigurationProvider(environment, applicationRootPath);
        }

        private static void AddJsonConfigurationProvider(
            this IConfigurationBuilder source,
            string environment,
            string applicationRootPath = null)
        {
            source.ThrowIfNull(nameof(source));
            environment.ThrowIfNullOrWhitespace(nameof(environment));

            var appSettingsFileName = "appsettings.json";
            var appSettingsWithEnvironmentFileName = $"appsettings.{environment}.json";

            source
                .AddJsonFile(
                    path: applicationRootPath == null ? appSettingsFileName : Path.Combine(applicationRootPath, appSettingsFileName),
                    optional: true,
                    reloadOnChange: false)
                .AddJsonFile(
                    path: applicationRootPath == null ? appSettingsWithEnvironmentFileName : Path.Combine(applicationRootPath, appSettingsWithEnvironmentFileName),
                    optional: true,
                    reloadOnChange: false);
        }

        private static void AddAzureKeyVaultConfigurationProvider(
            this IConfigurationBuilder source)
        {
            source.ThrowIfNull(nameof(source));

            var keyVaultUri = Environment.GetEnvironmentVariable(Constants.EnvironmentSettings.KeyVaultUri);

            if (!string.IsNullOrWhiteSpace(keyVaultUri))
            {
                source.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
            }
        }
    }
}