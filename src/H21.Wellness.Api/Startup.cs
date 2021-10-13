using H21.Wellness.Clients;
using H21.Wellness.Extensions;
using H21.Wellness.Persistence;
using H21.Wellness.Services;
using H21.Wellness.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace H21.Wellness.Api
{
    public class Startup
    {
        private const string CorsPolicyName = "CorsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddAzureStorageOptions();
            services.TryAddTransient<IScavengerHuntRepository, ScavengerHuntRepository>();
            services.AddSingleton<IAzureStorageClientFactory, AzureStorageClientFactory>();
            services.AddSingleton<IComputerVisionClientFactory, ComputerVisionClientFactory>();
            services.AddScoped<IImageValidatorService, ImageValidatorService>();

            services.AddComputerVisionOptions();

            services
                .AddHealthChecks()
                .AddCheck<AzureTableStorageHealthCheck>(nameof(AzureTableStorageHealthCheck));

            services.AddCors(options =>
            {
                options.AddPolicy(
                    CorsPolicyName,
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "H21.Wellness", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "H21.Wellness v1"));

            app.UseHttpsRedirection();

            app.UseCors(CorsPolicyName);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
