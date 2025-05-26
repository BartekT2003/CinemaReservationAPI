using System.IO; // Dodaj tę linię
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CinemaReservationAPI.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CinemaDbContext>
    {
        public CinemaDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<CinemaDbContext>();
            var connectionString = configuration.GetConnectionString("CinemaDb");
            builder.UseSqlServer(connectionString);

            return new CinemaDbContext(builder.Options);
        }
    }
}