using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TravelsLibrary.Api.DTOs
{
	public class ResetPasswordRequest : IValidatableObject
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		public string Code { get; set; } = string.Empty;

		[Required]
		public string NewPassword { get; set; } = string.Empty;

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (string.IsNullOrEmpty(NewPassword))
			{
				yield break;
			}

			if (NewPassword.Length < 8)
			{
				yield return new ValidationResult(
					"Password must be at least 8 characters long.",
					new[] { nameof(NewPassword) });
			}

			if (!Regex.IsMatch(NewPassword, @"[A-Z]"))
			{
				yield return new ValidationResult(
					"Password must contain at least one uppercase letter.",
					new[] { nameof(NewPassword) });
			}

			if (!Regex.IsMatch(NewPassword, @"\d"))
			{
				yield return new ValidationResult(
					"Password must contain at least one digit.",
					new[] { nameof(NewPassword) });
			}

			if (!Regex.IsMatch(NewPassword, @"[!@#$%^&*(),.?""{}|<>]"))
			{
				yield return new ValidationResult(
					"Password must contain at least one special character.",
					new[] { nameof(NewPassword) });
			}
		}
	}
}