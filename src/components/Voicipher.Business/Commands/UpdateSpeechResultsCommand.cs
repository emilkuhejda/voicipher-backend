using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Commands
{
    public class UpdateSpeechResultsCommand : Command<SpeechResultInputModel[], CommandResult<TimeSpanWrapperOutputModel>>, IUpdateSpeechResultsCommand
    {
        private readonly ISpeechResultRepository _speechResultRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UpdateSpeechResultsCommand(
            ISpeechResultRepository speechResultRepository,
            IMapper mapper,
            ILogger logger)
        {
            _speechResultRepository = speechResultRepository;
            _mapper = mapper;
            _logger = logger.ForContext<UpdateSpeechResultsCommand>();
        }

        protected override async Task<CommandResult<TimeSpanWrapperOutputModel>> Execute(SpeechResultInputModel[] parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var speechResults = parameter.Select(_mapper.Map<SpeechResult>).ToArray();
            _speechResultRepository.UpdateAll(speechResults);

            var totalTime = TimeSpan.FromTicks(parameter.Sum(x => x.Ticks));

            return new CommandResult<TimeSpanWrapperOutputModel>(new TimeSpanWrapperOutputModel(0));
        }
    }
}
