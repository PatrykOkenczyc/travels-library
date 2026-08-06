using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TravelsLibrary.Api.Data;
using TravelsLibrary.Api.DTOs;
using TravelsLibrary.Api.Models;

namespace TravelsLibrary.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly TravelsLibraryDbContext _context;
		private readonly IConfiguration _configuration;

		public UsersController(TravelsLibraryDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		[HttpPost("register")]
		public async Task<ActionResult<Guid>> Register(RegisterRequest request)
		{
			
			if (await _context.Users.AnyAsync(u => u.Email == request.Email))
			{
				return BadRequest("Email already in use.");
			}

			var user = new User
			{
				Id = Guid.NewGuid(),
				Email = request.Email,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
				EmailConfirmed = true, 
				ResetAttempts = 0,
				CreatedAt = DateTime.UtcNow
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return Ok(user.Id);
		}

		[HttpPost("login")]
		public async Task<ActionResult<string>> Login(LoginRequest request)
		{

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
			if (user == null)
			{
				return BadRequest("Email does not exist.");
			}

			if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
			{
				return BadRequest("Password was incorrect.");
			}

			var token = GenerateJwtToken(user);
			return Ok(token);
		}

		[HttpGet("me")]
		[Authorize]
		public async Task<ActionResult<object>> Me()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var user = await _context.Users.FindAsync(Guid.Parse(userId));
			if (user == null)
			{
				return NotFound();
			}

			return Ok(new { user.Id, user.Email, user.CreatedAt });
		}


		private string GenerateJwtToken(User user)
		{
			var jwtSettings = _configuration.GetSection("Jwt");
			var secretKey = jwtSettings["SecretKey"];

			var claims = new[]
			{
		new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
		new Claim(JwtRegisteredClaimNames.Email, user.Email),
		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
	};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: jwtSettings["Issuer"],
				audience: jwtSettings["Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(1),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}


	}	
}
