using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/contact-forms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class ContactFormController : ControllerBase
    {
        private readonly Lazy<IContactFormRepository> _contactFormRepository;

        public ContactFormController(Lazy<IContactFormRepository> contactFormRepository)
        {
            _contactFormRepository = contactFormRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var contactForms = await _contactFormRepository.Value.GetAllAsync(cancellationToken);
            var outputModels = contactForms.Select(MapContactForm).ToArray();

            return Ok(outputModels);
        }

        [HttpGet("{contactFormId}")]
        public async Task<IActionResult> Get(Guid contactFormId, CancellationToken cancellationToken)
        {
            var contactForm = await _contactFormRepository.Value.GetAsync(contactFormId, cancellationToken);
            var outputModel = MapContactForm(contactForm);

            return Ok(outputModel);
        }

        private object MapContactForm(ContactForm contactForm)
        {
            return new
            {
                Id = contactForm.Id,
                Name = contactForm.Name,
                Email = contactForm.Email,
                Message = contactForm.Message,
                DateCreatedUtc = contactForm.DateCreatedUtc
            };
        }
    }
}
