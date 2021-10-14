using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using H21.Wellness.Api.Request;
using H21.Wellness.Api.Response;
using H21.Wellness.Extensions;
using H21.Wellness.Models;
using H21.Wellness.Models.Extensions;
using H21.Wellness.Persistence;
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
        private readonly IScavengerHuntRepository _scavengerHuntRepository;
        private readonly IScavengerHuntService _scavengerHuntService;
        private readonly ILogger<ScavengerHuntController> _logger;

        public ScavengerHuntController(
            IScavengerHuntRepository scavengerHuntRepository,
            IScavengerHuntService scavengerHuntService,
            ILogger<ScavengerHuntController> logger)
        {
            scavengerHuntRepository.ThrowIfNull(nameof(scavengerHuntRepository));
            logger.ThrowIfNull(nameof(logger));

            _scavengerHuntRepository = scavengerHuntRepository;
            _scavengerHuntService = scavengerHuntService;
            _logger = logger;
        }

        // sean
        [HttpGet("game/random")]
        [ProducesResponseType(typeof(GetRandomScavengerHuntResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> GetRandomScavengerHuntAsync(
            CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();

            var response = new GetRandomScavengerHuntResponse
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
            var response = new PostScavengerHuntScoreResponse
            {
                Score = 99.99
            };

            var result = this.CreatedAtAction(nameof(PostScavengerHuntScoreAsync), response);

            return Task.FromResult<IActionResult>(result);
        }

        // Anjana
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

        // Anjana
        [HttpGet("item/{id}")]
        [ProducesResponseType(typeof(GetScavengerHuntItemResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetScavengerHuntItemAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // steven
        [HttpPost("validate")]
        [ProducesResponseType(typeof(PostValidateItemResponse), StatusCodes.Status201Created)]
        [ActionName(nameof(PostValidateItemAsync))]
        public Task<IActionResult> PostValidateItemAsync(
            [FromBody] PostValidateImageRequest request,
            CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            var response = new PostScavengerHuntResponse
            {
                Id = Guid.NewGuid()
            };

            var result = this.CreatedAtAction(nameof(PostValidateItemAsync), response);

            return Task.FromResult<IActionResult>(result);
        }
    }
}