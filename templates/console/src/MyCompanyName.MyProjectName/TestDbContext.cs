using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;

namespace MyCompanyName.MyProjectName
{
    [ConnectionStringName("Default")]
    public class TestDbContext : AbpDbContext<TestDbContext>
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureIdentity();
        }
    }
}
