using System;
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
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.InputModels.Audio;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels;
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
        private readonly Lazy<ICreateAudioFileCommand> _createAudioFileCommand;
        private readonly Lazy<IUploadAudioFileCommand> _uploadAudioFileCommand;
        private readonly Lazy<IUpdateAudioFileCommand> _updateAudioFileCommand;
        private readonly Lazy<IDeleteAudioFileCommand> _deleteAudioFileCommand;
        private readonly Lazy<IDeleteAllAudioFileCommand> _deleteAllAudioFileCommand;
        private readonly Lazy<IPermanentDeleteAllCommand> _permanentDeleteAllCommand;
        private readonly Lazy<IRestoreAllCommand> _restoreAllCommand;
        private readonly Lazy<ITranscribeCommand> _transcribeCommand;
        private readonly Lazy<IAudioFileRepository> _audioFileRepository;
        private readonly Lazy<IMapper> _mapper;

        public FileItemController(
            Lazy<ICreateAudioFileCommand> createAudioFileCommand,
            Lazy<IUploadAudioFileCommand> uploadAudioFileCommand,
            Lazy<IUpdateAudioFileCommand> updateAudioFileCommand,
            Lazy<IDeleteAudioFileCommand> deleteAudioFileCommand,
            Lazy<IDeleteAllAudioFileCommand> deleteAllAudioFileCommand,
            Lazy<IPermanentDeleteAllCommand> permanentDeleteAllCommand,
            Lazy<IRestoreAllCommand> restoreAllCommand,
            Lazy<ITranscribeCommand> transcribeCommand,
            Lazy<IAudioFileRepository> audioFileRepository,
            Lazy<IMapper> mapper)
        {
            _createAudioFileCommand = createAudioFileCommand;
            _uploadAudioFileCommand = uploadAudioFileCommand;
            _updateAudioFileCommand = updateAudioFileCommand;
            _deleteAudioFileCommand = deleteAudioFileCommand;
            _deleteAllAudioFileCommand = deleteAllAudioFileCommand;
            _permanentDeleteAllCommand = permanentDeleteAllCommand;
            _restoreAllCommand = restoreAllCommand;
            _transcribeCommand = transcribeCommand;
            _audioFileRepository = audioFileRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FileItemOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetFileItems")]
        public async Task<IActionResult> Get(DateTime updatedAfter, Guid applicationId, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetNameIdentifier();
            var audioFiles = await _audioFileRepository.Value.GetAllAsync(userId, updatedAfter, applicationId, cancellationToken);

            var outputModels = audioFiles.Select(x => _mapper.Value.Map<FileItemOutputModel>(x)).ToArray();
            return Ok(outputModels);
        }

        [HttpGet("deleted")]
        [ProducesResponseType(typeof(IEnumerable<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetDeletedFileItemIds")]
        public async Task<IActionResult> GetDeletedFileItemIds(DateTime updatedAfter, Guid applicationId, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetNameIdentifier();
            var audioFileIds = await _audioFileRepository.Value.GetAllDeletedIdsAsync(userId, updatedAfter, applicationId, cancellationToken);

            return Ok(audioFileIds);
        }

        [HttpGet("temporary-deleted")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetTemporaryDeletedFileItems(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetNameIdentifier();
            var audioFiles = await _audioFileRepository.Value.GetTemporaryDeletedAudioFilesAsync(userId, cancellationToken);

            var outputModels = audioFiles.Select(x => _mapper.Value.Map<FileItemOutputModel>(x));
            return Ok(outputModels);
        }

        [HttpGet("{fileItemId}")]
        [ProducesResponseType(typeof(FileItemOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetFileItem")]
        public async Task<IActionResult> Get(Guid fileItemId, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetNameIdentifier();
            var audioFile = await _audioFileRepository.Value.GetAsync(userId, fileItemId, cancellationToken);

            var outputModel = _mapper.Value.Map<FileItemOutputModel>(audioFile);
            return Ok(outputModel);
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(FileItemOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "CreateFileItem")]
        public async Task<IActionResult> CreateFileItem(string name, string language, string fileName, bool isPhoneCall, uint startTimeSeconds, uint endTimeSeconds, DateTime dateCreated, Guid applicationId, CancellationToken cancellationToken)
        {
            var createAudioFilePayload = new CreateAudioFilePayload
            {
                Name = name,
                Language = language,
                FileName = fileName,
                IsPhoneCall = isPhoneCall,
                TranscriptionStartTime = TimeSpan.FromSeconds(startTimeSeconds),
                TranscriptionEndTime = TimeSpan.FromSeconds(endTimeSeconds),
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
        [ProducesResponseType(typeof(FileItemOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "UploadFileItem")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [RequestSizeLimit(int.MaxValue)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Upload(string name, string language, string fileName, bool isPhoneCall, uint startTimeSeconds, uint endTimeSeconds, DateTime dateCreated, Guid applicationId, IFormFile file, CancellationToken cancellationToken)
        {
            var uploadAudioFilePayload = new UploadAudioFilePayload
            {
                Name = name,
                Language = language,
                FileName = fileName,
                IsPhoneCall = isPhoneCall,
                TranscriptionStartTime = TimeSpan.FromSeconds(startTimeSeconds),
                TranscriptionEndTime = TimeSpan.FromSeconds(endTimeSeconds),
                DateCreated = dateCreated,
                ApplicationId = applicationId,
                File = file
            };

            var commandResult = await _uploadAudioFileCommand.Value.ExecuteAsync(uploadAudioFilePayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpPut("update")]
        [ProducesResponseType(typeof(FileItemOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "UpdateFileItem")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [RequestSizeLimit(int.MaxValue)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Update([FromForm] UpdateAudioFileInputModel updateAudioFileInputModel, CancellationToken cancellationToken)
        {
            var commandResult = await _updateAudioFileCommand.Value.ExecuteAsync(updateAudioFileInputModel, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(OkOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "DeleteFileItem")]
        public async Task<IActionResult> Delete(Guid fileItemId, Guid applicationId, CancellationToken cancellationToken)
        {
            var deleteAudioFilePayload = new DeleteAudioFilePayload(fileItemId, applicationId);
            var commandResult = await _deleteAudioFileCommand.Value.ExecuteAsync(deleteAudioFilePayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpDelete("delete-all")]
        [ProducesResponseType(typeof(OkOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "DeleteAllFileItems")]
        public async Task<IActionResult> DeleteAll(IEnumerable<DeletedAudioFileInputModel> audioFileInputModels, Guid applicationId, CancellationToken cancellationToken)
        {
            var deleteAllAudioFilePayload = new DeleteAllAudioFilePayload(audioFileInputModels, applicationId);
            var commandResult = await _deleteAllAudioFileCommand.Value.ExecuteAsync(deleteAllAudioFilePayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpPut("permanent-delete-all")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> PermanentDeleteAll(IEnumerable<Guid> audioFilesIds, Guid applicationId, CancellationToken cancellationToken)
        {
            var permanentDeleteAllPayload = new PermanentDeleteAllPayload(audioFilesIds, HttpContext.User.GetNameIdentifier(), applicationId);
            var commandResult = await _permanentDeleteAllCommand.Value.ExecuteAsync(permanentDeleteAllPayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpPut("restore-all")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> RestoreAll(IEnumerable<Guid> audioFilesIds, Guid applicationId, CancellationToken cancellationToken)
        {
            var restoreAllPayload = new RestoreAllPayload(audioFilesIds, applicationId);
            var commandResult = await _restoreAllCommand.Value.ExecuteAsync(restoreAllPayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpPut("transcribe")]
        [ProducesResponseType(typeof(OkOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "TranscribeFileItem")]
        public async Task<IActionResult> Transcribe(Guid fileItemId, string language, bool isPhoneCall, uint startTimeSeconds, uint endTimeSeconds, Guid applicationId, CancellationToken cancellationToken)
        {
            return BadRequest();

            var transcribePayload = new TranscribePayload(fileItemId, language, isPhoneCall, startTimeSeconds, endTimeSeconds, applicationId);
            var commandResult = await _transcribeCommand.Value.ExecuteAsync(transcribePayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }
    }
}
