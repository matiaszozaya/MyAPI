using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyAPI.Entities;

namespace MyAPI.Data
{
	public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<ApplicationUser>(entity =>
			{
				entity.ToTable("ApplicationUsers");
				entity.Property(e => e.FirstName).HasMaxLength(100);
				entity.Property(e => e.MiddleName).HasMaxLength(100);
				entity.Property(e => e.LastName).HasMaxLength(100);
				entity.Property(e => e.DocumentId).HasMaxLength(10);
				entity.Property(e => e.Province).HasMaxLength(100);
				entity.Property(e => e.City).HasMaxLength(100);
				entity.Property(e => e.Address).HasMaxLength(100);
			});
		}
	}
}