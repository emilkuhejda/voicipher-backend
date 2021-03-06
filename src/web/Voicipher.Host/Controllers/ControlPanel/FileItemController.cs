using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/files")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class FileItemController : ControllerBase
    {
        private readonly Lazy<IAudioFileRepository> _audioFileRepository;

        public FileItemController(Lazy<IAudioFileRepository> audioFileRepository)
        {
            _audioFileRepository = audioFileRepository;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAll(Guid userId, CancellationToken cancellationToken)
        {
            var audioFiles = await _audioFileRepository.Value.GetAllCreatedAsync(userId, cancellationToken);
            var outputModels = audioFiles.Select(MapAudioFile).ToArray();

            return Ok(outputModels);
        }

        [HttpGet("detail/{fileItemId}")]
        public async Task<IActionResult> Get(Guid fileItemId, CancellationToken cancellationToken)
        {
            var audioFile = await _audioFileRepository.Value.GetAsync(fileItemId, cancellationToken);
            var outputModel = MapAudioFile(audioFile);

            return Ok(outputModel);
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore(Guid userId, Guid fileItemId, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("update-recognition-state")]
        public async Task<IActionResult> UpdateRecognitionState(object updateModel)
        {
            throw new NotImplementedException();
        }

        private object MapAudioFile(AudioFile audioFile)
        {
            return new
            {
                Id = audioFile.Id,
                UserId = audioFile.UserId,
                ApplicationId = audioFile.ApplicationId,
                Name = audioFile.Name,
                FileName = audioFile.FileName,
                Language = audioFile.Language,
                RecognitionState = audioFile.RecognitionState,
                OriginalSourceFileName = audioFile.OriginalSourceFileName,
                SourceFileName = audioFile.SourceFileName,
                Storage = StorageSetting.Azure,
                UploadStatus = audioFile.UploadStatus,
                TotalTime = audioFile.TotalTime,
                TranscribedTime = audioFile.TranscribedTime,
                DateCreated = audioFile.DateCreated,
                DateProcessedUtc = audioFile.DateProcessedUtc,
                DateUpdatedUtc = audioFile.DateUpdatedUtc,
                IsDeleted = audioFile.IsDeleted,
                WasCleaned = audioFile.WasCleaned
            };
        }
    }
}
