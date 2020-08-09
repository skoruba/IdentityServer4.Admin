using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using SendGrid;
using Skoruba.IdentityServer4.Shared.Configuration.Email;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Shared.Email
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly ILogger<SendGridEmailSender> _logger;
        private readonly SendGridConfiguration _configuration;
        private readonly ISendGridClient _client;

        public SendGridEmailSender(ILogger<SendGridEmailSender> logger, SendGridConfiguration configuration, ISendGridClient client)
        {
            _logger = logger;
            _configuration = configuration;
            _client = client;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(
                 new SendGrid.Helpers.Mail.EmailAddress(_configuration.SourceEmail, _configuration.SourceName),
                 new SendGrid.Helpers.Mail.EmailAddress(email),
                  subject,
                  null,
                  htmlMessage
             );

            // More information about click tracking: https://sendgrid.com/docs/ui/account-and-settings/tracking/
            message.SetClickTracking(_configuration.EnableClickTracking, _configuration.EnableClickTracking);

            var response = await _client.SendEmailAsync(message);

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                case System.Net.HttpStatusCode.Created:
                case System.Net.HttpStatusCode.Accepted:
                    _logger.LogInformation($"Email: {email}, subject: {subject}, message: {htmlMessage} successfully sent");
                    break;
                default:
                {
                    var errorMessage = response.Body.ReadAsStringAsync();
                    _logger.LogError($"Response with code {response.StatusCode} and body {errorMessage} after sending email: {email}, subject: {subject}");
                    break;
                }
            }
        }
    }
}
