using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Queries.EndUser;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Queries.EndUser
{
    public class GetUserQuery : Query<Guid, QueryResult<User>>, IGetUserQuery
    {
        private readonly IUserRepository _userRepository;

        public GetUserQuery(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected override async Task<QueryResult<Domain.Models.User>> Execute(Guid parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(parameter, cancellationToken);
            return new QueryResult<Domain.Models.User>(user);
        }
    }
}
