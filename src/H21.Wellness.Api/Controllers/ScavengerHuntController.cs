using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using H21.Wellness.Api.Request;
using H21.Wellness.Api.Response;
using H21.Wellness.Clients;
using H21.Wellness.Extensions;
using H21.Wellness.Models;
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
        private readonly ILogger<ScavengerHuntController> _logger;

        public ScavengerHuntController(
            IImageValidatorService imageValidatorService,
            ILogger<ScavengerHuntController> logger)
        {
            imageValidatorService.ThrowIfNull(nameof(logger));
            logger.ThrowIfNull(nameof(logger));

            this._imageValidatorService = imageValidatorService;
            this._logger = logger;
        }

        // sean
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

        // Anjana
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

        // Anjana
        [HttpGet("image/{id}")]
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

        // steven
        [HttpPost("validate")]
        [ProducesResponseType(typeof(PostValidateImageResponse), StatusCodes.Status201Created)]
        [ActionName(nameof(PostValidateImageAsync))]
        public async Task<IActionResult> PostValidateImageAsync([FromBody] PostValidateImageRequest request)
        {
            request.ThrowIfNull(nameof(request));

            var isValid = await this._imageValidatorService.IsValid(request.Id, request.ImageDateUri).ConfigureAwait(false);

            var response = new PostValidateImageResponse
            {
                IsMatch = isValid
            };

            var result = this.CreatedAtAction(nameof(PostValidateImageAsync), response);

            return result;
        }
    }
}