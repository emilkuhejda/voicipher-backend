using Microsoft.EntityFrameworkCore;
using Voicipher.DataAccess.EntitiesConfiguration;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess
{
    public class DatabaseContext : DbContext
    {
        private readonly string _connectionString;

        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        public DatabaseContext(string connectionString, DbContextOptions options)
            : base(options)
        {
            _connectionString = connectionString;
        }

        public DbSet<User> Users { get; set; }

        public DbSet<UserSubscription> UserSubscriptions { get; set; }

        public DbSet<CurrentUserSubscription> CurrentUserSubscriptions { get; set; }

        public DbSet<UserDevice> UserDevices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString, providerOptions => providerOptions.CommandTimeout(60));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserSubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new CurrentUserSubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new UserDeviceConfiguration());
        }
    }
}
