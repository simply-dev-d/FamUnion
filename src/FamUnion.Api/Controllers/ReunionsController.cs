﻿using System;
using System.Threading.Tasks;
using FamUnion.Core.Interface;
using FamUnion.Core.Model;
using FamUnion.Core.Request;
using FamUnion.Core.Utility;
using FamUnion.Core.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FamUnion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReunionsController : ControllerBase
    {
        private readonly IReunionService _reunionService;
        private readonly ILogger<ReunionsController> _logger;

        public ReunionsController(IReunionService reunionService, ILogger<ReunionsController> logger)
        {
            _reunionService = Validator.ThrowIfNull(reunionService, nameof(reunionService));
            _logger = Validator.ThrowIfNull(logger, nameof(logger));
        }

        [HttpGet()]
        public async Task<IActionResult> GetReunions()
        {
            _logger.LogInformation("ReunionsController.GetReunions()");
            try
            {
                var result = await _reunionService.GetReunionsAsync()
                    .ConfigureAwait(continueOnCapturedContext: false);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReunion(Guid id)
        {
            _logger.LogInformation($"ReunionsController.GetReunion|{id}");
            try
            {
                var result = await _reunionService.GetReunionAsync(id)
                    .ConfigureAwait(continueOnCapturedContext: false);

                if (result is null)
                    return NotFound($"Id: {id} was not found");

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("new")]
        public async Task<IActionResult> NewReunion([FromBody] NewReunionRequest request)
        {
            _logger.LogInformation($"{GetType()}.NewReunion|{JsonConvert.SerializeObject(request)}");
            Reunion reunion = null;
            try
            {
                reunion = NewReunionRequestMapper.Map(request);
                var result = await _reunionService.SaveReunionAsync(reunion)
                    .ConfigureAwait(continueOnCapturedContext: false);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                if(!reunion.IsValid())
                {
                    return BadRequest(request);
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> SaveReunion([FromBody] Reunion reunion)
        {
            _logger.LogInformation($"ReunionsController.SaveReunion|{JsonConvert.SerializeObject(reunion)}");
            try
            {
                var result = await _reunionService.SaveReunionAsync(reunion)
                    .ConfigureAwait(continueOnCapturedContext: false);
                return new OkObjectResult(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                if (!reunion.IsValid())
                {
                    return BadRequest(reunion);
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{reunionId}")]
        public async Task<IActionResult> DeleteReunion(Guid reunionId)
        {
            _logger.LogInformation($"ReunionsController.DeleteReunion|{reunionId}");
            try
            {
                await _reunionService.DeleteReunionAsync(reunionId)
                    .ConfigureAwait(continueOnCapturedContext: false);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}