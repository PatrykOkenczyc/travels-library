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

		public DbSet<Trip> Trips { get; set; }
	}
}
