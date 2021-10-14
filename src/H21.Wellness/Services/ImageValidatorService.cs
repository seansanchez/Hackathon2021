using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using H21.Wellness.Clients;
using H21.Wellness.Extensions;
using H21.Wellness.Persistence;
using H21.Wellness.Services.Interfaces;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Logging;

namespace H21.Wellness.Services
{
    public class ImageValidatorService : IImageValidatorService
    {
        private readonly IComputerVisionClientFactory _computerVisionClientFactory;
        private readonly IScavengerHuntRepository _scavengerHuntRepository;
        private readonly ILogger<ImageValidatorService> _logger;

        public ImageValidatorService(
            IComputerVisionClientFactory computerVisionClientFactory,
            IScavengerHuntRepository scavengerHuntRepository,
            ILogger<ImageValidatorService> logger)
        {
            computerVisionClientFactory.ThrowIfNull(nameof(computerVisionClientFactory));
            scavengerHuntRepository.ThrowIfNull(nameof(scavengerHuntRepository));
            logger.ThrowIfNull(nameof(logger));

            this._computerVisionClientFactory = computerVisionClientFactory;
            this._scavengerHuntRepository = scavengerHuntRepository;
            this._logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> IsValid(Guid imageId, string imageDataUri, CancellationToken cancellationToken = default)
        {
            var image = await this._scavengerHuntRepository.GetScavengerHuntItemAsync(imageId, cancellationToken).ConfigureAwait(false);
            var possibleNames = 
                image.Synonyms?
                    .Select(name => name.ToLower().Replace(" ", "").Trim())
                    .ToList() 
                ?? new List<string>();
            possibleNames.Add(image.Name.ToLower().Replace(" ", "").Trim());

            var tagResult = await this.GetTagsFromImageUri(imageDataUri).ConfigureAwait(false);
            var highConfidenceTags = tagResult.Tags
                .Where(tag => tag.Confidence >= Constants.ComputerVisionConstants.MinConfidence)
                .Select(tag => tag.Name.ToLower().Replace(" ", "").Trim())
                .ToList();

            return highConfidenceTags
                .Any(tag => possibleNames.Any(name => name.Contains(tag) || tag.Contains(name)));
        }

        public async Task<TagResult> GetTagsFromImageUri(string imageDataUri)
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
