using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Queries.Audio;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/files")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    [ApiController]
    public class FileItemController : ControllerBase
    {
        private readonly Lazy<IGetAudioFilesQuery> _getAudioFilesQuery;
        private readonly Lazy<ICreateAudioFileCommand> _createAudioFileCommand;

        public FileItemController(
            Lazy<IGetAudioFilesQuery> getAudioFilesQuery,
            Lazy<ICreateAudioFileCommand> createAudioFileCommand)
        {
            _getAudioFilesQuery = getAudioFilesQuery;
            _createAudioFileCommand = createAudioFileCommand;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AudioFileOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetFileItems")]
        public async Task<ActionResult> Get(DateTime updatedAfter, Guid applicationId, CancellationToken cancellationToken)
        {
            var audioFilesPayload = new AudioFilesPayload(updatedAfter, applicationId);
            var queryResult = await _getAudioFilesQuery.Value.ExecuteAsync(audioFilesPayload, HttpContext.User, cancellationToken);
            if (!queryResult.IsSuccess)
                return BadRequest();

            return Ok(queryResult.Value);
        }

        [HttpGet("deleted")]
        // [ProducesResponseType(typeof(IEnumerable<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetDeletedFileItemIds")]
        public IActionResult GetDeletedFileItemIds(DateTime updatedAfter, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("temporary-deleted")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetTemporaryDeletedFileItems()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{fileItemId}")]
        // [ProducesResponseType(typeof(FileItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetFileItem")]
        public IActionResult Get(Guid fileItemId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(AudioFileOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "CreateFileItem")]
        public async Task<IActionResult> CreateFileItem(string name, string language, string fileName, DateTime dateCreated, Guid applicationId, CancellationToken cancellationToken)
        {
            var createAudioFilePayload = new CreateAudioFilePayload
            {
                Name = name,
                Language = language,
                FileName = fileName,
                DateCreated = dateCreated,
                ApplicationId = applicationId
            };

            var commandResult = await _createAudioFileCommand.Value.ExecuteAsync(createAudioFilePayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        // [ProducesResponseType(typeof(FileItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "UploadFileItem")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [RequestSizeLimit(int.MaxValue)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Upload(string name, string language, string fileName, DateTime dateCreated, Guid applicationId, IFormFile file)
        {
            throw new NotImplementedException();
        }

        [HttpPut("update")]
        // [ProducesResponseType(typeof(FileItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "UpdateFileItem")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Update([FromForm] object updateFileItemModel)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("delete")]
        // [ProducesResponseType(typeof(OkDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "DeleteFileItem")]
        public IActionResult Delete(Guid fileItemId, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("delete-all")]
        // [ProducesResponseType(typeof(OkDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "DeleteAllFileItems")]
        public IActionResult DeleteAll(object fileItems, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("permanent-delete-all")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult PermanentDeleteAll(object fileItemIds, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("restore-all")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult RestoreAll(object fileItemIds, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("transcribe")]
        // [ProducesResponseType(typeof(OkDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "TranscribeFileItem")]
        public IActionResult Transcribe(Guid fileItemId, string language, Guid applicationId)
        {
            throw new NotImplementedException();
        }
    }
}
