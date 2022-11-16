using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EasyAbp.FileManagement.EntityFrameworkCore
{
    public class UnifiedDbContextFactory : IDesignTimeDbContextFactory<UnifiedDbContext>
    {
        public UnifiedDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<UnifiedDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"));

            return new UnifiedDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.secrets.json", optional: true);

            return builder.Build();
        }
    }
}