using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/users")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Lazy<IUserRepository> _userRepository;

        public UserController(Lazy<IUserRepository> userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var users = await _userRepository.Value.GetAllAsync(cancellationToken);
            var outputModels = users.Select(x => new
            {
                Id = x.Id,
                Email = x.Email,
                GivenName = x.GivenName,
                FamilyName = x.FamilyName,
                DateRegisteredUtc = x.DateRegisteredUtc
            });

            return Ok(outputModels);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser(Guid userId, string email)
        {
            throw new NotImplementedException();
        }
    }
}
