using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelsLibrary.Api.Data;
using TravelsLibrary.Api.Models;

namespace TravelsLibrary.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TripsController : ControllerBase
	{
		private readonly TravelsLibraryDbContext _context;

		public TripsController(TravelsLibraryDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<List<Trip>>> GetTrips()
		{
			return await _context.Trips.ToListAsync();
		}

		[HttpPost]
		public async Task<ActionResult<Trip>> CreateTrip(Trip trip)
		{
			_context.Trips.Add(trip);
			await _context.SaveChangesAsync();
			return Ok(trip);
		}
	}
}