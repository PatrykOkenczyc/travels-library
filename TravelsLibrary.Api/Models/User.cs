namespace TravelsLibrary.Api.Models
{
	public class User
	{
		public Guid Id { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public bool EmailConfirmed { get; set; }
		public string? ResetCodeHash { get; set; }
		public DateTime? ResetExpire { get; set; }
		public int ResetAttempts { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
