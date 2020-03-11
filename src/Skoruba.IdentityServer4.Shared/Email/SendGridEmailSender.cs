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
            var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(
                 new SendGrid.Helpers.Mail.EmailAddress(_configuration.SourceEmail, _configuration.SourceName),
                 new SendGrid.Helpers.Mail.EmailAddress(email),
                  subject,
                  null,
                  htmlMessage
             );

            var response = await _client.SendEmailAsync(msg);

            if (response.StatusCode == System.Net.HttpStatusCode.OK
                || response.StatusCode == System.Net.HttpStatusCode.Created
                || response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogInformation($"Email: {email}, subject: {subject}, message: {htmlMessage} successfully sent");
            }
            else
            {
                var errorMessage = response.Body.ReadAsStringAsync();
                _logger.LogError($"Response with code {response.StatusCode} and body {errorMessage} after sending email: {email}, subject: {subject}");
            }
        }
    }
}
