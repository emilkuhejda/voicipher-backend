﻿using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Job;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Business.Commands.Job
{
    public class CreateBackgroundJobCommand : Command<CreateBackgroundJobPayload, CommandResult<BackgroundJob>>, ICreateBackgroundJobCommand
    {
        private readonly IBackgroundJobRepository _backgroundJobRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CreateBackgroundJobCommand(
            IBackgroundJobRepository backgroundJobRepository,
            IMapper mapper,
            ILogger logger)
        {
            _backgroundJobRepository = backgroundJobRepository;
            _mapper = mapper;
            _logger = logger.ForContext<CreateBackgroundJobCommand>();
        }

        protected override async Task<CommandResult<BackgroundJob>> Execute(CreateBackgroundJobPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var backgroundJob = _mapper.Map<BackgroundJob>(parameter);
            await _backgroundJobRepository.AddAsync(backgroundJob);
            await _backgroundJobRepository.SaveAsync(cancellationToken);

            _logger.Information($"Background job {backgroundJob.Id} was created");

            return new CommandResult<BackgroundJob>(backgroundJob);
        }
    }
}
