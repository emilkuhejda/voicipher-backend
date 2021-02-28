using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Repositories
{
    public class UserDeviceRepository : Repository<UserDevice>, IUserDeviceRepository
    {
        public UserDeviceRepository(DatabaseContext context)
            : base(context)
        {
        }

        public async Task AddOrUpdateAsync(UserDevice userDevice, CancellationToken cancellationToken)
        {
            var entity = await Context
                .UserDevices
                .SingleOrDefaultAsync(x => x.UserId == userDevice.UserId && x.InstallationId == userDevice.InstallationId, cancellationToken);

            if (entity == null)
            {
                await Context.UserDevices.AddAsync(userDevice, cancellationToken);
            }
            else
            {
                entity.UpdateFrom(userDevice);
            }
        }

        public Task<UserDevice> GetByInstallationIdAsync(Guid userId, Guid installationId, CancellationToken cancellationToken)
        {
            return Context.UserDevices.SingleOrDefaultAsync(x => x.UserId == userId && x.InstallationId == installationId, cancellationToken);
        }

        public Task<Guid[]> GetPlatformSpecificInstallationIdsAsync(RuntimePlatform runtimePlatform, Language language, Guid? userId, CancellationToken cancellationToken)
        {
            var query = Context.UserDevices
                .AsNoTracking()
                .Where(x => x.RuntimePlatform == runtimePlatform && x.Language == language);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId);

            return query.Select(x => x.InstallationId).ToArrayAsync(cancellationToken);
        }
    }
}
