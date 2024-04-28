using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCutter.DataAccess.Data;
using TreeCutter.Models;
using TreeCutter.Utility;

namespace TreeCutter.DataAccess.DbInitializer
{
    public class DbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public void Initialize()
        {
            //migration if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception) { }

            //creates role if they not created, then create admin user as well
            if (!_roleManager.RoleExistsAsync(ApplicationRoles.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Role_Employee)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "MasterAdmin",
                    Email = "admin@treecutter-mail.com",
                    PhoneNumber = "111222333",
                    StreetAddress = "test 123 Ave",
                    State = "IL",
                    PostalCode = "23-422",
                    City = "Chicago"
                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser? user = _db.Users.FirstOrDefault(u => u.Email == "admin@treecutter-mail.com");
                if (user == null) return;
                _userManager.AddToRoleAsync(user, ApplicationRoles.Role_Admin).GetAwaiter().GetResult();
            }
        }
    }
}
