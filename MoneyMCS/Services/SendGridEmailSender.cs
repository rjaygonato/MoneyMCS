
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text.Encodings.Web;

namespace MoneyMCS.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        public SendGridEmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<SendGridEmailSender> logger)
        {
            _logger = logger;

            Options = optionsAccessor.Value;
        }

        private readonly ILogger _logger;
        public AuthMessageSenderOptions Options { get; }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(Options.SendGridKey))
            {
                throw new Exception("Null SendGridKey");
            }
            await Execute(Options.SendGridKey, subject, htmlMessage, email);
        }

        public async Task Execute(string apiKey, string subject, string message, string toEmail) 
        {
            var client = new SendGridClient(apiKey);
     
            var from = new EmailAddress("ziegfred@moneymcs.com");
            var to = new EmailAddress(toEmail);
            var msg =  MailHelper.CreateSingleEmail(from, to, subject, message, message);

            var response = await client.SendEmailAsync(msg);
            
            _logger.LogInformation(response.IsSuccessStatusCode
                               ? $"Email to {toEmail} queued successfully!"
                               : $"Failure Email to {toEmail}");
        }

    }
}
