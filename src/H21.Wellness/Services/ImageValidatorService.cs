using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using H21.Wellness.Clients;
using H21.Wellness.Extensions;
using H21.Wellness.Services.Interfaces;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Logging;

namespace H21.Wellness.Services
{
    public class ImageValidatorService : IImageValidatorService
    {
        private readonly IComputerVisionClientFactory _computerVisionClientFactory;
        private readonly ILogger<ImageValidatorService> _logger;

        public ImageValidatorService(
            IComputerVisionClientFactory computerVisionClientFactory,
            ILogger<ImageValidatorService> logger)
        {
            computerVisionClientFactory.ThrowIfNull(nameof(computerVisionClientFactory));
            logger.ThrowIfNull(nameof(logger));

            this._computerVisionClientFactory = computerVisionClientFactory;
            this._logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> IsValid(Guid imageId, string imageDataUri)
        {
            var tagResult = await this.GetTagsFromImageUri(imageDataUri).ConfigureAwait(false);
            var highConfidenceTags = tagResult.Tags.Where(tag => tag.Confidence >= Constants.ComputerVisionConstants.MinConfidence);

            return true;
        }

        private async Task<TagResult> GetTagsFromImageUri(string imageDataUri)
        {
            byte[] binData;
            try
            {
                var base64Data = Regex.Match(imageDataUri, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                binData = Convert.FromBase64String(base64Data);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"imageDataUri not valid. Failed to parse to bin array. Inner error: {e.Message}");
            }

            TagResult imageTags;
            try
            {
                await using (var stream = new MemoryStream(binData))
                {
                    imageTags = await this._computerVisionClientFactory.GenerateTagsForStreamAsync(stream)
                        .ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                throw new ComputerVisionErrorResponseException($"Error processing image through Computer Vision. Inner error: {e.Message}");
            }

            return imageTags;
        }
    }
}
