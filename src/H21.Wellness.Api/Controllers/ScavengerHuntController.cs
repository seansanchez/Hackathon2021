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
using Newtonsoft.Json;

namespace H21.Wellness.Api.Controllers
{
    [ApiController]
    [Route("api/scavenger-hunt")]
    public class ScavengerHuntController : ControllerBase
    {
        private readonly ILogger<ScavengerHuntController> _logger;

        public ScavengerHuntController(ILogger<ScavengerHuntController> logger)
        {
            logger.ThrowIfNull(nameof(logger));

            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetScavengerHuntResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> GetScavengerHuntAsync([FromRoute] Guid id)
        {
            var response = new GetScavengerHuntResponse
            {
                ScavengerHunt = new ScavengerHuntModel
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
                }
            };

            var result = this.Ok(response);

            return Task.FromResult<IActionResult>(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PostScavengerHuntResponse), StatusCodes.Status201Created)]
        [ActionName(nameof(PostAsync))]
        public Task<IActionResult> PostAsync([FromBody] PostScavengerHuntRequest request)
        {
            request.ThrowIfNull(nameof(request));

            var response = new PostScavengerHuntResponse
            {
                Id = Guid.NewGuid()
            };

            var result = this.CreatedAtAction(nameof(PostAsync), response);

            return Task.FromResult<IActionResult>(result);
        }


        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public object GetCuratedList()
        {
            string curatedList = System.IO.File.ReadAllText(@"..\Response\curatedList.json");
            
            return JsonConvert.DeserializeObject(curatedList); ;
        }
    }
}