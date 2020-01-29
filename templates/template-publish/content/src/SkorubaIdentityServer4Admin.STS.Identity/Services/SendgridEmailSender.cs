using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using SendGrid;
using SkorubaIdentityServer4Admin.STS.Identity.Configuration;
using System.Threading.Tasks;

namespace SkorubaIdentityServer4Admin.STS.Identity.Services
{
    public class SendgridEmailSender : IEmailSender
    {
        private ISendGridClient _client;
        private readonly SendgridConfiguration _configuration;
        private readonly ILogger<SendgridEmailSender> _logger;

        public SendgridEmailSender(ILogger<SendgridEmailSender> logger, ISendGridClient client, SendgridConfiguration configuration)
        {
            _logger = logger;
            _client = client;
            _configuration = configuration;
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
            } else
            {
                var errorMessage = response.Body.ReadAsStringAsync();
                _logger.LogError($"Response with code {response.StatusCode} and body {errorMessage} after sending email: {email}, subject: {subject}");
            }
        }
    }
}






