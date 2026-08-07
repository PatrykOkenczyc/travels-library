using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TravelsLibrary.Api.Data;
using TravelsLibrary.Api.DTOs;
using TravelsLibrary.Api.Models;
using TravelsLibrary.Api.Services;

namespace TravelsLibrary.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly TravelsLibraryDbContext _context;
		private readonly IConfiguration _configuration;
		private static string HashResetCode(string code)
		{
			var bytes = Encoding.UTF8.GetBytes(code);
			var hash = System.Security.Cryptography.SHA256.HashData(bytes);
			return Convert.ToBase64String(hash);
		}

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
		public async Task<ActionResult<string>> Login([FromServices] JwtService jwtService, LoginRequest request)
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

			var token = jwtService.GenerateToken(user);
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


		[HttpPost("forgot-password")]
		public async Task<ActionResult> ForgotPassword([FromServices] EmailService emailService, ForgotPasswordRequest request)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
			if (user == null)
			{
				return Ok("If this email exists, a reset code has been sent.");
			}

			var resetCode = Random.Shared.Next(100000, 999999).ToString();

			user.ResetCodeHash = HashResetCode(resetCode);
			user.ResetExpire = DateTime.UtcNow.AddMinutes(15);
			user.ResetAttempts = 0;

			await _context.SaveChangesAsync();
			await emailService.SendPasswordResetEmailAsync(user.Email, resetCode);

			return Ok("If this email exists, a reset code has been sent.");
		}

		[HttpPost("reset-password")]
		public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
			if (user == null || user.ResetCodeHash == null || user.ResetExpire == null)
			{
				return BadRequest("Invalid or expired reset code.");
			}

			if (user.ResetAttempts >= 5)
			{
				return BadRequest("Too many failed attempts. Please request a new code.");
			}

			if (user.ResetExpire < DateTime.UtcNow)
			{
				return BadRequest("Invalid or expired reset code.");
			}

			var providedCodeHash = HashResetCode(request.Code);
			if (providedCodeHash != user.ResetCodeHash)
			{
				user.ResetAttempts++;
				await _context.SaveChangesAsync();
				return BadRequest("Invalid or expired reset code.");
			}

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
			user.ResetCodeHash = null;
			user.ResetExpire = null;
			user.ResetAttempts = 0;

			await _context.SaveChangesAsync();

			return Ok("Password has been reset successfully.");
		}





	}	
}
