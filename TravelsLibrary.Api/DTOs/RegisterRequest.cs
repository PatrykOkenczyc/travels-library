using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TravelsLibrary.Api.DTOs
{
	public class RegisterRequest : IValidatableObject
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		public string Password { get; set; } = string.Empty;

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (string.IsNullOrEmpty(Password))
			{
				yield break; // [Required] już to obsłuży osobno
			}

			if (Password.Length < 8)
			{
				yield return new ValidationResult(
					"Password must be at least 8 characters long.",
					new[] { nameof(Password) });
			}

			if (!Regex.IsMatch(Password, @"[A-Z]"))
			{
				yield return new ValidationResult(
					"Password must contain at least one uppercase letter.",
					new[] { nameof(Password) });
			}

			if (!Regex.IsMatch(Password, @"\d"))
			{
				yield return new ValidationResult(
					"Password must contain at least one digit.",
					new[] { nameof(Password) });
			}

			if (!Regex.IsMatch(Password, @"[!@#$%^&*(),.?""{}|<>]"))
			{
				yield return new ValidationResult(
					"Password must contain at least one special character.",
					new[] { nameof(Password) });
			}
		}
	}
}