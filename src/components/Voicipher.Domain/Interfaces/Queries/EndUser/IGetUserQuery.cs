using System;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Queries.EndUser
{
    public interface IGetUserQuery : IQuery<Guid, QueryResult<User>>
    {
    }
}
