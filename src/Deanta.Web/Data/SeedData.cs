using Deanta.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Deanta.Web.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(
            IServiceProvider services)
        {
            var roleManager = services
                .GetRequiredService<RoleManager<IdentityRole>>();
            await EnsureRolesAsync(roleManager);

            var userManager = services
                 .GetRequiredService<UserManager<IdentityUser>>();
            await AddAdminRights(userManager);
        }

        private static async Task AddAdminRights(UserManager<IdentityUser> userManager)
        {
            var adminExists = userManager.Users
                .Any(x => x.UserName == "nev@nev.ie");

            if (!adminExists)
            {
                var admin = new IdentityUser
                {
                    Email = "nev@nev.ie",
                    UserName = "test"
                };
                await userManager.CreateAsync(admin, "pwd123");
                await userManager.AddToRoleAsync(
                    admin, Constants.AdministratorRole);
            }
        }

        private static async Task EnsureRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {
            var adminRoleAlreadyExists = await roleManager
                .RoleExistsAsync(Constants.AdministratorRole);

            var userRoleAlreadyExists = await roleManager
                .RoleExistsAsync(Constants.UserRole);

            if (!adminRoleAlreadyExists)
                await roleManager.CreateAsync(
                    new IdentityRole(Constants.AdministratorRole));

            if (!userRoleAlreadyExists)
                await roleManager.CreateAsync(
                    new IdentityRole(Constants.UserRole));
        }
    }
}
