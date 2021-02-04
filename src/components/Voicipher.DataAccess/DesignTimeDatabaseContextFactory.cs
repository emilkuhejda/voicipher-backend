using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Voicipher.DataAccess
{
    public class DesignTimeDatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();
            var connectionString = configuration.GetSection("ApplicationSettings").GetSection("ConnectionString").Value;

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

            return new DatabaseContext(connectionString, optionsBuilder.Options);
        }
    }
}
