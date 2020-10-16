using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MyCompanyName.MyProjectName
{
    public class MyProjectNameMigrationsDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"));

            return new TestDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
