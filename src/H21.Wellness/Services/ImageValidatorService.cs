using System;
using System.Threading.Tasks;
using H21.Wellness.Clients;
using H21.Wellness.Extensions;
using H21.Wellness.Services.Interfaces;
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
            var tags = this._computerVisionClientFactory.GenerateTagsForUrlAsync(imageDataUri).ConfigureAwait(false);
            return true;
        }
    }
}
