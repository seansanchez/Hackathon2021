using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H21.Wellness.Api.Request;
using H21.Wellness.Api.Response;
using H21.Wellness.Extensions;
using H21.Wellness.Models.Extensions;
using H21.Wellness.Persistence;
using H21.Wellness.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
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
        private readonly IScavengerHuntService _scavengerHuntService;
        private readonly IScoringService _scoringService;
        private readonly ILogger<ScavengerHuntController> _logger;

        public ScavengerHuntController(
            IImageValidatorService imageValidatorService,
            IScavengerHuntRepository scavengerHuntRepository,
            IScavengerHuntService scavengerHuntService,
            IScoringService scoringService,
            ILogger<ScavengerHuntController> logger)
        {
            imageValidatorService.ThrowIfNull(nameof(logger));
            scavengerHuntRepository.ThrowIfNull(nameof(scavengerHuntRepository));
            scavengerHuntService.ThrowIfNull(nameof(scavengerHuntService));
            scoringService.ThrowIfNull(nameof(scoringService));
            logger.ThrowIfNull(nameof(logger));
            
            this._imageValidatorService = imageValidatorService;
            this._scavengerHuntRepository = scavengerHuntRepository;
            this._scavengerHuntService = scavengerHuntService;
            this._scoringService = scoringService;
            this._logger = logger;
        }

        [HttpGet("game/random")]
        [ProducesResponseType(typeof(GetRandomScavengerHuntResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRandomScavengerHuntAsync(CancellationToken cancellationToken)
        {
            IActionResult result;

            var scavengerHunt = await _scavengerHuntService
                .GetRandomScavengerHuntAsync(cancellationToken)
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
                result = this.NotFound();
            }

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
                result = this.NotFound();
            }

            return result;
        }

        [HttpPost]
        [ProducesResponseType(typeof(PostScavengerHuntResponse), StatusCodes.Status201Created)]
        [ActionName(nameof(PostScavengerHuntAsync))]
        public async Task<IActionResult> PostScavengerHuntAsync([FromBody] PostScavengerHuntRequest request)
        {
            request.ThrowIfNull(nameof(request));

            var id = await _scavengerHuntService
                .CreateScavengerHuntAsync(request.Name, request.Description, request.TimeLimitInMinutes, request.ItemIds)
                .ConfigureAwait(false);


            var response = new PostScavengerHuntResponse
            {
                Id = id
            };

            return this.CreatedAtAction(nameof(PostScavengerHuntAsync), response);
        }

        [HttpPost("score")]
        [ProducesResponseType(typeof(PostScavengerHuntScoreResponse), StatusCodes.Status201Created)]
        [ActionName(nameof(PostScavengerHuntScoreAsync))]
        public async Task<IActionResult> PostScavengerHuntScoreAsync(
            [FromBody] PostScavengerHuntScoreRequest request,
            CancellationToken cancellationToken)
        {
            var score = await this._scoringService.GetScore(
                request.Id,
                request.CompleteCount,
                request.CompletedTimeInSeconds,
                cancellationToken);

            var response = new PostScavengerHuntScoreResponse
            {
                Score = score
            };

            var result = this.CreatedAtAction(nameof(PostScavengerHuntScoreAsync), response);

            return result;
        }

        [HttpGet("item")]
        [ProducesResponseType(typeof(GetScavengerHuntItemsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetScavengerHuntItemsAsync(CancellationToken cancellationToken)
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
        public async Task<IActionResult> GetScavengerHuntItemAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
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

        [HttpPost("detect")]
        [ProducesResponseType(typeof(List<DetectedTagResponse>), StatusCodes.Status200OK)]
        [ActionName(nameof(PostDetectImageRequest))]
        public async Task<IActionResult> PostDetectImageAsync(
            [FromBody] PostDetectImageRequest request)
        {
            request.ThrowIfNull(nameof(request));
            request.ImageDataUri.ThrowIfNull($"{nameof(request)}.{nameof(request.ImageDataUri)}");

            var tags = await this._imageValidatorService.GetTagsFromImageUri(request.ImageDataUri).ConfigureAwait(false);

            var response = tags.Tags.Select(t => new DetectedTagResponse()
            {
                Name = t.Name,
                Confidence = t.Confidence,
                Hint = t.Hint
            }).ToList();

            var result = Ok(response);

            return result;
        }

        [HttpPost("validate")]
        [ProducesResponseType(typeof(PostValidateItemResponse), StatusCodes.Status201Created)]
        [ActionName(nameof(PostValidateImageAsync))]
        public async Task<IActionResult> PostValidateImageAsync(
            [FromBody] PostValidateImageRequest request,
            CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));
            request.Id.ThrowIfNull($"{nameof(request)}.{nameof(request.Id)}");
            request.ImageDataUri.ThrowIfNull($"{nameof(request)}.{nameof(request.ImageDataUri)}");

            var isValid = await this._imageValidatorService.IsValid(request.Id, request.ImageDataUri, cancellationToken).ConfigureAwait(false);

            var response = new PostValidateItemResponse
            {
                IsMatch = isValid
            };

            var result = this.CreatedAtAction(nameof(PostValidateImageAsync), response);

            return result;
        }
    }
}