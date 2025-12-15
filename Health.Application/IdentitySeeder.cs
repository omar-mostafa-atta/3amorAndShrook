using Health.Application.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application
{

    public static class IdentitySeeder
    {
        public static async Task SeedRolesAsync(
            RoleManager<ApplicationRole> roleManager)
        {
            string[] roles = { "Patient", "Doctor", "Nurse" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new ApplicationRole { Name = role });
                }
            }
        }
    }


}
