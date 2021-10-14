using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using H21.Wellness.Api.Request;
using H21.Wellness.Api.Response;
using H21.Wellness.Extensions;
using H21.Wellness.Models;
using H21.Wellness.Models.Extensions;
using H21.Wellness.Persistence;
using H21.Wellness.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            scoringService.ThrowIfNull(nameof(scoringService));
            logger.ThrowIfNull(nameof(logger));

            _scavengerHuntRepository = scavengerHuntRepository;
            _logger = logger;
        }

        // sean
        [HttpGet("game/random")]
        [ProducesResponseType(typeof(GetRandomScavengerHuntResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> GetRandomScavengerHuntAsync()
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

        [HttpGet("game/{id}")]
        [ProducesResponseType(typeof(GetScavengerHuntResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetScavengerHuntAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            IActionResult result;

            var scavengerHunt = await _scavengerHuntService
                .GetScavengerHuntAsync(id, cancellationToken)
                .ConfigureAwait(false);

            if (scavengerHunt != null)
            {
                var response = new GetScavengerHuntResponse
                {
                    Id = scavengerHunt.Id,
                    Name = scavengerHunt.Name,
                    Description = scavengerHunt.Description,
                    TimeLimitInMinutes = scavengerHunt.TimeLimitInMinutes,
                    Items = scavengerHunt.Items
                };

                result = this.Ok(response);
            }
            else
            {
                result = NotFound();
            }

            return result;
        }

        [HttpPost]
        [ProducesResponseType(typeof(PostScavengerHuntResponse), StatusCodes.Status201Created)]
        [ActionName(nameof(PostScavengerHuntAsync))]
        public async Task<IActionResult> PostScavengerHuntAsync(
            [FromBody] PostScavengerHuntRequest request,
            CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            var id = await _scavengerHuntService
                .CreateScavengerHuntAsync(request.Name, request.Description, request.TimeLimitInMinutes, request.ItemIds, cancellationToken)
                .ConfigureAwait(false);

            var response = new PostScavengerHuntResponse
            {
                Id = id
            };

            return this.CreatedAtAction(nameof(PostScavengerHuntAsync), response);
        }

        // un
        [HttpPost("score")]
        [ProducesResponseType(typeof(PostScavengerHuntScoreResponse), StatusCodes.Status201Created)]
        [ActionName(nameof(PostScavengerHuntScoreAsync))]
        public Task<IActionResult> PostScavengerHuntScoreAsync(
            [FromBody] PostScavengerHuntScoreRequest request,
            CancellationToken cancellationToken)
        {
            var score = this._scoringService.GetScore(
                request.Id, 
                request.CompleteCount,
                request.CompletedTimeInSeconds);

            var response = new PostScavengerHuntScoreResponse
            {
                Score = score
            };

            var result = this.CreatedAtAction(nameof(PostScavengerHuntScoreAsync), response);

            return Task.FromResult<IActionResult>(result);
        }


        [HttpGet("item")]
        [ProducesResponseType(typeof(GetScavengerHuntItemsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetScavengerHuntItemsAsync(
            CancellationToken cancellationToken)
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
        public async Task<IActionResult> GetImageAsync([FromRoute] Guid id)
        {
            throw new NotImplementedException();
        }

        // steven
        [HttpPost("validate")]
        [ProducesResponseType(typeof(PostValidateImageResponse), StatusCodes.Status201Created)]
        [ActionName(nameof(PostValidateImageAsync))]
        public Task<IActionResult> PostValidateImageAsync([FromBody] PostValidateImageRequest request)
        {
            request.ThrowIfNull(nameof(request));
            request.Id.ThrowIfNull($"{nameof(request)}.{nameof(request.Id)}");
            request.ImageDataUri.ThrowIfNull($"{nameof(request)}.{nameof(request.ImageDataUri)}");

            var isValid = await this._imageValidatorService.IsValid(request.Id, request.ImageDataUri, cancellationToken).ConfigureAwait(false);

            var response = new PostValidateImageResponse
            {
                IsMatch = isValid
            };

            var result = this.CreatedAtAction(nameof(PostValidateItemAsync), response);

            return result;
        }
    }
}