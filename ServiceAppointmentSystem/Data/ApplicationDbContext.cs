using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ServiceAppointmentSystem.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ServiceAppointmentSystem.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> AppUsers { get; set; }

    public DbSet<Item> Items { get; set; }  

	public DbSet<Professional> Professionals { get; set; }

	public DbSet<Service> Services { get; set; }    

    public DbSet<OrderHeader> Orders { get; set; }

    public DbSet<OrderDetail> OrderDetails { get; set; }    

    public DbSet<ShoppingCart> ShoppingCart { get; set;  }

    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<IdentityUser>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserToken<string>>().ToTable("Tokens");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("LoginAttempts");

        builder.Entity<Appointment>()
            .HasOne(x => x.AppUser)
            .WithMany(x => x.Appointments)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        base.OnModelCreating(builder);
    }
}
