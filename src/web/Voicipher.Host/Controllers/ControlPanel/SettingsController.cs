using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.Enums;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/settings")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        [HttpGet("storage-setting")]
        public async Task<IActionResult> GetStorageSetting()
        {
            throw new NotImplementedException();
        }

        [HttpGet("database-backup")]
        public async Task<IActionResult> GetDatabaseBackupSetting()
        {
            throw new NotImplementedException();
        }

        [HttpGet("notifications-setting")]
        public async Task<IActionResult> GetNotificationsSetting()
        {
            throw new NotImplementedException();
        }

        [HttpGet("chunks-storage-setting")]
        public async Task<IActionResult> GetChunksStorageSetting()
        {
            throw new NotImplementedException();
        }

        [HttpPut("change-storage")]
        public async Task<IActionResult> ChangeStorage(StorageSetting storageSetting)
        {
            throw new NotImplementedException();
        }

        [HttpPut("change-database-backup")]
        public async Task<IActionResult> ChangeDatabaseBackupSettings(bool isEnabled)
        {
            throw new NotImplementedException();
        }

        [HttpPut("change-notifications-setting")]
        public async Task<IActionResult> ChangeNotificationsSettings(bool isEnabled)
        {
            throw new NotImplementedException();
        }

        [HttpPut("change-chunks-storage")]
        public async Task<IActionResult> ChangeChunksStorage(StorageSetting storageSetting)
        {
            throw new NotImplementedException();
        }

        [HttpPut("clean-up")]
        public IActionResult CleanUp([FromBody] object cleanUpSettingsModel)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("clean-chunks")]
        public async Task<IActionResult> CleanOutdatedChunks()
        {
            throw new NotImplementedException();
        }

        [HttpPut("subscription-recalculation")]
        public async Task<IActionResult> SubscriptionRecalculation()
        {
            throw new NotImplementedException();
        }
    }
}
