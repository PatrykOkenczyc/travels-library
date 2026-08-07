using System.Net;
using System.Net.Mail;

namespace TravelsLibrary.Api.Services
{
	public class EmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendPasswordResetEmailAsync(string toEmail, string resetCode)
		{
			var smtpSettings = _configuration.GetSection("Smtp");

			var host = smtpSettings["Host"];
			var port = int.Parse(smtpSettings["Port"]!);
			var username = smtpSettings["Username"];
			var password = smtpSettings["Password"];
			var fromEmail = smtpSettings["FromEmail"];
			var fromName = smtpSettings["FromName"];

			using var client = new SmtpClient(host, port)
			{
				Credentials = new NetworkCredential(username, password),
				EnableSsl = true
			};

			var message = new MailMessage
			{
				From = new MailAddress(fromEmail!, fromName),
				Subject = "Your password reset code",
				Body = $"Your password reset code is: {resetCode}\n\nThis code expires in 15 minutes.",
				IsBodyHtml = false
			};
			message.To.Add(toEmail);

			await client.SendMailAsync(message);
		}
	}
}