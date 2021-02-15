﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Business.Extensions;
using Voicipher.Domain.Interfaces.Queries.TranscribeItems;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/transcribe-items")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    [ApiController]
    public class TranscribeItemsController : ControllerBase
    {
        private readonly Lazy<IGetTranscribeItemSourceQuery> _getTranscribeItemSourceQuery;
        private readonly Lazy<ITranscribeItemRepository> _transcribeItemRepository;
        private readonly Lazy<IMapper> _mapper;

        public TranscribeItemsController(
            Lazy<IGetTranscribeItemSourceQuery> getTranscribeItemSourceQuery,
            Lazy<ITranscribeItemRepository> transcribeItemRepository,
            Lazy<IMapper> mapper)
        {
            _getTranscribeItemSourceQuery = getTranscribeItemSourceQuery;
            _transcribeItemRepository = transcribeItemRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TranscribeItemOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetTranscribeItemsAll")]
        public async Task<ActionResult> GetAll(DateTime updatedAfter, Guid applicationId, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetNameIdentifier();
            var transcribeItems = await _transcribeItemRepository.Value.GetAllAfterDateAsync(userId, updatedAfter, applicationId, cancellationToken);
            var outputModels = transcribeItems.Select(_mapper.Value.Map<TranscribeItemOutputModel>);

            return Ok(outputModels);
        }

        [HttpGet("{fileItemId}")]
        [ProducesResponseType(typeof(IEnumerable<TranscribeItemOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetTranscribeItems")]
        public async Task<ActionResult> GetAllByAudioFileId(Guid fileItemId, CancellationToken cancellationToken)
        {
            var transcribeItems = await _transcribeItemRepository.Value.GetAllByAudioFileIdAsync(fileItemId, cancellationToken);
            var outputModels = transcribeItems.Select(_mapper.Value.Map<TranscribeItemOutputModel>);

            return Ok(outputModels);
        }

        [HttpGet("audio/{transcribeItemId}")]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetTranscribeAudioSource")]
        public async Task<ActionResult> GetAudioSource(Guid transcribeItemId, CancellationToken cancellationToken)
        {
            var queryResult = await _getTranscribeItemSourceQuery.Value.ExecuteAsync(transcribeItemId, HttpContext.User, cancellationToken);
            if (!queryResult.IsSuccess)
                return NotFound(queryResult.Error.ErrorCode);

            return Ok(queryResult.Value);
        }

        [HttpGet("audio-stream/{transcribeItemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetTranscribeAudioSourceStream")]
        public async Task<ActionResult> GetAudioSourceStream(Guid transcribeItemId, CancellationToken cancellationToken)
        {
            var queryResult = await _getTranscribeItemSourceQuery.Value.ExecuteAsync(transcribeItemId, HttpContext.User, cancellationToken);
            if (!queryResult.IsSuccess)
                return NotFound(queryResult.Error.ErrorCode);

            return new FileContentResult(queryResult.Value, "audio/wav");
        }

        [HttpPut("update-transcript")]
        // [ProducesResponseType(typeof(OkDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "UpdateUserTranscript")]
        public ActionResult UpdateUserTranscript(object updateTranscribeItemModel)
        {
            throw new NotImplementedException();
        }
    }
}
