using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Commands
{
    public class ContactFormCommand : Command<ContactFormInputModel, CommandResult<OkOutputModel>>, IContactFormCommand
    {
        private readonly IContactFormRepository _contactFormRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ContactFormCommand(
            IContactFormRepository contactFormRepository,
            IMapper mapper,
            ILogger logger)
        {
            _contactFormRepository = contactFormRepository;
            _mapper = mapper;
            _logger = logger.ForContext<ContactFormCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(ContactFormInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!parameter.Validate().IsValid)
            {
                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var contactForm = _mapper.Map<ContactForm>(parameter);
            if (!contactForm.Validate().IsValid)
            {
                _logger.Error("Invalid entity data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            await _contactFormRepository.AddAsync(contactForm);
            await _contactFormRepository.SaveAsync(cancellationToken);

            _logger.Information($"Contact '{contactForm.Id}' form was created.");

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
