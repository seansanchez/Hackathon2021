using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using H21.Wellness.Api.Request;
using H21.Wellness.Api.Response;
using H21.Wellness.Clients;
using H21.Wellness.Extensions;
using H21.Wellness.Models;
using H21.Wellness.Models.Extensions;
using H21.Wellness.Persistence;
using H21.Wellness.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace H21.Wellness.Api.Controllers
{
    [ApiController]
    [Route("api/scavenger-hunt")]
    [Produces("application/json")]
    public class ScavengerHuntController : ControllerBase
    {
        private readonly IImageValidatorService _imageValidatorService;
        private readonly IScavengerHuntRepository _scavengerHuntRepository;
        private readonly ILogger<ScavengerHuntController> _logger;

        public ScavengerHuntController(
            IImageValidatorService imageValidatorService,
            IScavengerHuntRepository scavengerHuntRepository,
            ILogger<ScavengerHuntController> logger)
        {
            imageValidatorService.ThrowIfNull(nameof(logger));
            scavengerHuntRepository.ThrowIfNull(nameof(scavengerHuntRepository));
            logger.ThrowIfNull(nameof(logger));

            this._imageValidatorService = imageValidatorService;
            this._scavengerHuntRepository = scavengerHuntRepository;
            this._logger = logger;
        }

        // sean
        [HttpGet("game/random")]
        [ProducesResponseType(typeof(GetRandomScavengerHuntResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRandomScavengerHuntAsync(CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();

            var allItems = 
                await this._scavengerHuntRepository.GetScavengerHuntItemsAsync(cancellationToken).ConfigureAwait(false);

            var response = new GetRandomScavengerHuntResponse
            {
                Id = id,
                Name = "Test Scavenger Hunt",
                Description = "This is a stub scavenger hunt.",
                Items = allItems.OrderBy(x => Guid.NewGuid()).Take(10).Select(item =>
                {
                    return new ScavengerHuntItemModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description
                    };
                })
            };

            var result = this.Ok(response);

            return result;
        }

        // sean
        [HttpGet("game/{id}")]
        [ProducesResponseType(typeof(GetScavengerHuntResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> GetScavengerHuntAsync([FromRoute] Guid id)
        {
            var response = new GetScavengerHuntResponse
            {
                Id = id,
                Name = "Test Scavenger Hunt",
                Description = "This is a stub scavenger hunt.",
                Items = new List<ScavengerHuntItemModel>()
                {
                    new ScavengerHuntItemModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Test Item 1",
                        Description = "Description 1"
                    }
                }
            };

            var result = this.Ok(response);

            return Task.FromResult<IActionResult>(result);
        }

        // sean
        [HttpPost]
        [ProducesResponseType(typeof(PostScavengerHuntResponse), StatusCodes.Status201Created)]
        [ActionName(nameof(PostScavengerHuntAsync))]
        public Task<IActionResult> PostScavengerHuntAsync([FromBody] PostScavengerHuntRequest request)
        {
            request.ThrowIfNull(nameof(request));

            var response = new PostScavengerHuntResponse
            {
                Id = Guid.NewGuid()
            };

            var result = this.CreatedAtAction(nameof(PostScavengerHuntAsync), response);

            return Task.FromResult<IActionResult>(result);
        }

        // un
        [HttpPost("score")]
        [ProducesResponseType(typeof(PostScavengerHuntScoreResponse), StatusCodes.Status201Created)]
        [ActionName(nameof(PostScavengerHuntScoreAsync))]
        public Task<IActionResult> PostScavengerHuntScoreAsync([FromBody] PostScavengerHuntScoreRequest request)
        {
            var response = new PostScavengerHuntScoreResponse
            {
                Score = 99.99
            };

            var result = this.CreatedAtAction(nameof(PostScavengerHuntScoreAsync), response);

            return Task.FromResult<IActionResult>(result);
        }


        [HttpGet("item")]
        [ProducesResponseType(typeof(GetScavengerHuntItemsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetImagesAsync(CancellationToken cancellationToken)
        {
            var items =
                await _scavengerHuntRepository
                    .GetScavengerHuntItemsAsync(cancellationToken)
                    .ConfigureAwait(false);

            var response = new GetScavengerHuntItemsResponse
            {
                Items = items.ToModels()
            };

            var result = this.Ok(response);

            return result;
        }

   
        [HttpGet("item/{id}")]
        [ProducesResponseType(typeof(GetScavengerHuntItemResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetImageAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var item =
                     await _scavengerHuntRepository.GetScavengerHuntItemAsync(id, cancellationToken)
                     .ConfigureAwait(false);

            var response = new GetScavengerHuntItemResponse
            {
                Item = item.ToModel()
            };

            var result = this.Ok(response);

            return result;
        }

        // steven
        [HttpPost("validate")]
        [ProducesResponseType(typeof(PostValidateImageResponse), StatusCodes.Status201Created)]
        [ActionName(nameof(PostValidateImageAsync))]
        public async Task<IActionResult> PostValidateImageAsync(
            [FromBody] PostValidateImageRequest request,
            CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));
            request.Id.ThrowIfNull($"{nameof(request)}.{nameof(request.Id)}");
            request.ImageDataUri.ThrowIfNull($"{nameof(request)}.{nameof(request.ImageDataUri)}");

            var isValid = await this._imageValidatorService.IsValid(request.Id, request.ImageDataUri, cancellationToken).ConfigureAwait(false);

            var response = new PostValidateImageResponse
            {
                IsMatch = isValid
            };

            var result = this.CreatedAtAction(nameof(PostValidateImageAsync), response);

            return result;
        }
    }
}