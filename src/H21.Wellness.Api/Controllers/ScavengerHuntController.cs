using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using H21.Wellness.Api.Request;
using H21.Wellness.Api.Response;
using H21.Wellness.Extensions;
using H21.Wellness.Models;
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
        private readonly ILogger<ScavengerHuntController> _logger;

        public ScavengerHuntController(ILogger<ScavengerHuntController> logger)
        {
            logger.ThrowIfNull(nameof(logger));

            _logger = logger;
        }

        [HttpGet("game/random")]
        [ProducesResponseType(typeof(GetRandomScavengerHuntResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> GetRandomScavengerHuntAsync()
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

        [HttpGet("image")]
        [ProducesResponseType(typeof(GetImagesResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> GetImagesAsync()
        {
            var response = new GetImagesResponse
            {
                Images = new List<ScavengerHuntItemModel>()
                {
                    new ScavengerHuntItemModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Image",
                        Description = "Descr"
                    }
                }
            };

            var result = this.Ok(response);

            return Task.FromResult<IActionResult>(result);
        }

        [HttpGet("images/{id}")]
        [ProducesResponseType(typeof(GetImageResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> GetImageAsync([FromRoute] Guid id)
        {
            var response = new GetImageResponse
            {
                Id = Guid.NewGuid(),
                Name = "Image",
                Description = "Descr"
            };

            var result = this.Ok(response);

            return Task.FromResult<IActionResult>(result);
        }

        [HttpPost("validate")]
        [ProducesResponseType(typeof(PostValidateImageResponse), StatusCodes.Status201Created)]
        [ActionName(nameof(PostValidateImageAsync))]
        public Task<IActionResult> PostValidateImageAsync([FromBody] PostValidateImageRequest request)
        {
            request.ThrowIfNull(nameof(request));

            var response = new PostScavengerHuntResponse
            {
                Id = Guid.NewGuid()
            };

            var result = this.CreatedAtAction(nameof(PostValidateImageAsync), response);

            return Task.FromResult<IActionResult>(result);
        }
    }
}