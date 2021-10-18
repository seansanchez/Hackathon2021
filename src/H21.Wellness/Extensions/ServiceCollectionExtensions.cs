using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace H21.Wellness.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddComputerVisionOptions(this IServiceCollection source, string section = null)
        {
            source.ThrowIfNull(nameof(source));

            section ??= nameof(ComputerVisionOptions);

            source
                .AddOptions<ComputerVisionOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(section).Bind(settings);
                })
                .ValidateDataAnnotations();
        }

        public static void AddAzureStorageOptions(this IServiceCollection source, string section = null)
        {
            source.ThrowIfNull(nameof(source));

            section ??= nameof(AzureStorageOptions);

            source
                .AddOptions<AzureStorageOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(section).Bind(settings);
                })
                .ValidateDataAnnotations();
        }
    }
}