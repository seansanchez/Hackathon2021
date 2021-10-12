using H21.Wellness.Extensions;
using H21.Wellness.Persistence.Interfaces;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;

namespace H21.Wellness.Persistence
{
    public class ComputerVisionClientFactory : IComputerVisionClientFactory
    {
        private readonly IOptions<ComputerVisionOptions> _options;
        public ComputerVisionClientFactory(IOptions<ComputerVisionOptions> options)
        {
            options.ThrowIfNull(nameof(options));
            options.Value.ThrowIfNull($"{nameof(options)}.{nameof(options.Value)}");
            options.Value.ApiKey.ThrowIfNullOrWhitespace($"{nameof(options)}.{nameof(options.Value)}.{nameof(options.Value.ApiKey)}");
            options.Value.Endpoint.ThrowIfNullOrWhitespace($"{nameof(options)}.{nameof(options.Value)}.{nameof(options.Value.Endpoint)}");


            _options = options;
        }


        protected ApiKeyServiceClientCredentials Credentials
        {
            get
            {
                return new ApiKeyServiceClientCredentials(_options.Value.ApiKey);
            }
        }

        protected string Endpoint
        {
            get
            {
                return _options.Value.Endpoint;
            }
        }


        /// <summary>
        /// Sends a URL to Cognitive Services and generates tags for it.
        /// </summary>
        /// <param name="imageUrl">The URL of the image for which to generate tags.</param>
        /// <returns>Awaitable tagging result.</returns>
        public async Task<TagResult> GenerateTagsForUrlAsync(string imageUrl)
        {
            //
            // Create Cognitive Services Vision API Service client.
            //
            using (var client = new ComputerVisionClient(Credentials) { Endpoint = Endpoint })
            {
                
                TagResult analysisResult = await client.TagImageAsync(imageUrl, "EN");
                return analysisResult;
            }
        }


        /// <summary>
        /// Sends a URL to Cognitive Services and generates tags for it.
        /// </summary>
        /// <param name="imageStream">The stream of the image for which to generate tags.</param>
        /// <returns>Awaitable tagging result.</returns>
        public async Task<TagResult> GenerateTagsForStreamAsync(Stream imageStream)
        {
            //
            // Create Cognitive Services Vision API Service client.
            //
            using (var client = new ComputerVisionClient(Credentials) { Endpoint = Endpoint })
            {

                TagResult analysisResult = await client.TagImageInStreamAsync(imageStream);
                return analysisResult;
            }
        }
    }
}
