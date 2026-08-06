using Microsoft.EntityFrameworkCore;
using TravelsLibrary.Api.Models;

namespace TravelsLibrary.Api.Data
{
	public class TravelsLibraryDbContext : DbContext
	{
		public TravelsLibraryDbContext(DbContextOptions<TravelsLibraryDbContext> options)
		: base(options) 
		{ 
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();
		}

		public DbSet<Trip> Trips { get; set; }
		public DbSet<User> Users { get; set; }
	}
}
