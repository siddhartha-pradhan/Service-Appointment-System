using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Models.Constants;
using ServiceAppointmentSystem.Repositories.Interfaces;

namespace ServiceAppointmentSystem.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Count() > 0)
                {
                    _dbContext.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }

            if (!_roleManager.RoleExistsAsync(Constants.User).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Constants.User)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Constants.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Constants.Employee)).GetAwaiter().GetResult();

                var user = new AppUser()
                {
                    Email = "admin@admin.com",
                    UserName = "admin@admin.com",
                    EmailConfirmed = true,
                    CityAddress = "45th State",
                    PhoneNumber = "9800000000",
                    FullName = "Admin Woaksey",
                    RegionName = "Stateless Region",
                };

                _userManager.CreateAsync(user, "Admin@123").GetAwaiter().GetResult();

                var result = _dbContext.AppUsers.FirstOrDefault(u => u.Email == "admin@dotnetmastery.com");

                _userManager.AddToRoleAsync(user, Constants.Admin).GetAwaiter().GetResult();

            }
        }
    }
}
