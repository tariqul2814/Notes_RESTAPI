using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightRESTAPI.Model.Data
{
    public class SeedDB
    {
        private static readonly List<ApplicationUser> useres = new List<ApplicationUser>()
        {
            new ApplicationUser { Name = "Muyeen Hossain", DateOfBirth = Convert.ToDateTime(DateTime.ParseExact("07-11-1997", "dd-MM-yyyy", CultureInfo.InvariantCulture)), UserName = "muyeen.aiub@gmail.com", PhoneNumber = "01683105317", IsRemoved = false, Email = "muyeen.aiub@gmail.com" }
        };

        private static readonly string[] roles = new string[] { "User" };

        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole()
                    {
                        Name = role
                    });
                }
            }

            foreach (var user in useres)
            {
                if (await userManager.FindByNameAsync(user.UserName) == null)
                {
                    var result = await userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        await userManager.AddPasswordAsync(user, "Asd123@");
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }
            }

            await context.SaveChangesAsync();
        }

    }
}
