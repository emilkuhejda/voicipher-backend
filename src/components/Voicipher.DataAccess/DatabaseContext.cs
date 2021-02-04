using System;
using Microsoft.EntityFrameworkCore;
using Serilog;

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString, providerOptions => providerOptions.CommandTimeout(60));
            }
        }

        public void InitializeDatabase(ILogger logger)
        {
            try
            {
                Database.Migrate();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Initializing of the database failed");
            }
        }
    }
}
