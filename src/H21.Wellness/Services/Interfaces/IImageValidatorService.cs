
using System;
using System.Threading;
using System.Threading.Tasks;

namespace H21.Wellness.Services.Interfaces
{
    public interface IImageValidatorService
    {
        /// <summary>
        ///     Returns if the provided image is valid for the imageId
        /// </summary>
        /// <param name="imageId">The image id to validate against</param>
        /// <param name="imageDataUri">The image data URI to validate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> IsValid(Guid imageId, string imageDataUri, CancellationToken cancellationToken);
    }
}
