using Application.Helpers;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Infrastructure.Services
{
       public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> logger;
        private readonly UrlConfig urlConfig;

        public EmailService(
            ILogger<EmailService> logger,
            IOptions<UrlConfig> urlConfig)
        {
            this.logger = logger;
            this.urlConfig = urlConfig.Value;
        }

        public async Task SendPasswordReset(string email, string token)
        {
            
        }
        

        internal async Task SendEmail(List<string> recipientEmail, string subject, string templatePath, Dictionary<string, string> replacements, string activity)
        {
            // string body = await TemplateGenerator.RenderTemplateAsync(templatePath, replacements);

            string url = $"{urlConfig.VulnWatchBaseUrl}{urlConfig.EmailEndpoint}";

            var client = new RestClient(url);
            var request = new RestRequest
            {
                Method = Method.Post
            };

            request.AddHeader("Subscription-Key", urlConfig.SubscriptionKey);

            var recipients = string.Join(";", recipientEmail);

            var requestBody = new
            {
                // appId = urlConfig.AppId,
                // appReference = Guid.NewGuid().ToString(),
                // senderName = urlConfig.EmailSettings.SenderName,
                // senderEmail = urlConfig.EmailSettings.SenderEmail,
                // recipientEmails = recipients,
                // cc = "",
                // bcc = "",
                // subject,
                // body,
                // priority = true,
                // attachments = new List<object> { },
            };

            request.AddJsonBody(requestBody);

            try
            {
                var response = await client.ExecuteAsync(request);
                logger.LogInformation("{Activity} sent to {Recipients}", activity, recipients);
                logger.LogInformation(
                    "EmailNotificationService: Request/Response ==> {RequestResponse}",
                    JsonConvert.SerializeObject(new { Endpoint = url, Request = requestBody, Response = response.Content ?? response.ErrorMessage })
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EmailNotificationService: Email Unable to send to {Recipients}", recipients);
            }
        }
    }
}