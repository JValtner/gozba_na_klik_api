using System.Net;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Settings;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;


namespace Gozba_na_klik.Services.EmailServices
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpSettings _settings;
        private readonly UserManager<User> _userManager;

        public SmtpEmailService(IOptions<SmtpSettings> settings, UserManager<User> userManager)
        {
            _settings = settings.Value;
            _userManager = userManager;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Gozba na klik", _settings.From));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;

            var htmlPart = new TextPart("html")
            {
                Text = body
            };

            message.Body = htmlPart;

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }



        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string fileName)
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Gozba na klik", _settings.From));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            builder.Attachments.Add(fileName, attachment);

            message.Body = builder.ToMessageBody();

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        
    }
}
