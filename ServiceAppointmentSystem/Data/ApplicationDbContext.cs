using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ServiceAppointmentSystem.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ServiceAppointmentSystem.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<AppUser>(b =>
        {
            b.ToTable("Users");

            b.Property(u => u.ProfileImage);
            b.Property(u => u.CertificationURL);
            b.Property(u => u.FullName).HasMaxLength(256);
            b.Property(u => u.CityAddress).HasMaxLength(256);
            b.Property(u => u.RegionName).HasMaxLength(256);
        });

        base.OnModelCreating(builder);

        builder.Entity<IdentityUser>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserToken<string>>().ToTable("Tokens");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("LoginAttempts");
    }
}
