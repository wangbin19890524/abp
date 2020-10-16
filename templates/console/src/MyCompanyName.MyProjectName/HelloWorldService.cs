using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace MyCompanyName.MyProjectName
{
    public class HelloWorldService : ITransientDependency
    {
        public async Task SayHello(IAbpApplicationWithExternalServiceProvider application)
        {
            var unitOfWorkManager = application.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
            var userRepository = application.ServiceProvider.GetRequiredService<IIdentityUserRepository>();
            var roleRepository = application.ServiceProvider.GetRequiredService<IIdentityRoleRepository>();
            var guidGenerator = application.ServiceProvider.GetRequiredService<IGuidGenerator>();

            var stopwatch = Stopwatch.StartNew();

            await application.ServiceProvider.GetRequiredService<TestDbContext>().Database.ExecuteSqlRawAsync("DELETE FROM AbpUsers");
            //Console.WriteLine("DELETE TEST DATA {0}" ,stopwatch.ElapsedMilliseconds);

            using (var uow = unitOfWorkManager.Begin())
            {
                var roleIdList = new List<Guid>();
                for (var i = 0; i < 10; i++)
                {
                    var role = new IdentityRole(Guid.NewGuid(), $"role_{i}");
                    await roleRepository.InsertAsync(role);
                    roleIdList.Add(role.Id);
                }
                await uow.SaveChangesAsync();

                for (var i = 0; i < 50; i++)
                {
                    var user = new IdentityUser(Guid.NewGuid(), $"test_{i}", $"test_{i}");
                    foreach (var guid in roleIdList)
                    {
                        user.AddRole(guid);
                    }

                    for (var ii = 0; ii < 10; ii++)
                    {
                        user.SetToken($"LoginProvider{ii}", $"LoginName{ii}", $"LoginValue{ii}");
                        user.AddClaim(guidGenerator, new Claim($"LoginType{ii}", $"LoginValue{ii}"));
                        user.AddLogin(new UserLoginInfo($"LoginProvider{ii}", $"LoginName{ii}", $"LoginValue{ii}"));
                    }

                    await userRepository.InsertAsync(user);
                }

                await uow.CompleteAsync();
            }

            stopwatch.Stop();
            Console.WriteLine("INIT DATA:\t {0}", stopwatch.ElapsedMilliseconds);

            // Same with AsSplitQuery
            // stopwatch.Restart();
            // using (var uow = unitOfWorkManager.Begin())
            // {
            //     await userRepository.GetListAsync();
            //     await uow.CompleteAsync();
            // }
            // stopwatch.Stop();
            // Console.WriteLine("GetList {0}", stopwatch.ElapsedMilliseconds);

            using (var scope = application.ServiceProvider.CreateScope())
            {
                stopwatch.Restart();
                await scope.ServiceProvider.GetRequiredService<TestDbContext>().Set<IdentityUser>().As<IQueryable<IdentityUser>>()
                    .Include(x => x.Roles)
                    .Include(x => x.Logins)
                    .Include(x => x.Claims)
                    .Include(x => x.Tokens)
                    .Include(x => x.OrganizationUnits)
                    .AsSplitQuery()
                    .ToListAsync();
                Console.WriteLine("AsSplitQuery:\t {0}", stopwatch.ElapsedMilliseconds);
            }

            using (var scope = application.ServiceProvider.CreateScope())
            {
                stopwatch.Restart();
                await scope.ServiceProvider.GetRequiredService<TestDbContext>().Set<IdentityUser>().As<IQueryable<IdentityUser>>()
                    .Include(x => x.Roles)
                    .Include(x => x.Logins)
                    .Include(x => x.Claims)
                    .Include(x => x.Tokens)
                    .Include(x => x.OrganizationUnits)
                    .AsSingleQuery()
                    .ToListAsync();
                Console.WriteLine("AsSingleQuery:\t {0}", stopwatch.ElapsedMilliseconds);
            }

            await application.ServiceProvider.GetRequiredService<TestDbContext>().Database.ExecuteSqlRawAsync("DELETE FROM AbpUsers");
            //Console.WriteLine("DELETE TEST DATA {0}" ,stopwatch.ElapsedMilliseconds);
        }
    }
}
