using System.ComponentModel.DataAnnotations;

namespace TravelsLibrary.Api.DTOs
{
	public class ForgotPasswordRequest
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;
	}
}