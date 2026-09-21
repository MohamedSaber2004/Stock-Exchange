using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;
using System.Text;

namespace Stock_Exchange.Infrastructure.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IWebHostEnvironment? _environment;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            IWebHostEnvironment? environment = null)
        {
            _emailSettings = emailSettings.Value;
            _environment = environment;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            bool isHtml = false,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException(nameof(toEmail));

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.Name, _emailSettings.Email));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart(isHtml ? TextFormat.Html : TextFormat.Plain)
            {
                Text = body
            };

            using var client = new SmtpClient();

            var socketOptions = _emailSettings.Port switch
            {
                465 => SecureSocketOptions.SslOnConnect,
                587 => SecureSocketOptions.StartTls,
                _ => SecureSocketOptions.Auto
            };

            await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, socketOptions, cancellationToken);

            if (!string.IsNullOrWhiteSpace(_emailSettings.Username) && !string.IsNullOrWhiteSpace(_emailSettings.Password))
            {
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }

        public async Task SendTemplateEmailAsync(
            string toEmail,
            string subject,
            string templateName,
            IDictionary<string, string> placeholders,
            bool isHtml = false,
            CancellationToken cancellationToken = default)
        {
            var templatePath = ResolveTemplatePath(templateName);
            if (templatePath == null || !File.Exists(templatePath))
                throw new FileNotFoundException(templateName);

            var content = await File.ReadAllTextAsync(templatePath, Encoding.UTF8, cancellationToken);

            if (placeholders != null)
            {
                foreach (var kvp in placeholders)
                {
                    content = content.Replace($"{{{kvp.Key}}}", kvp.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
                }
            }

            await SendEmailAsync(toEmail, subject, content, isHtml, cancellationToken);
        }

        private string? ResolveTemplatePath(string templateName)
        {
            var normalizedName = templateName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
                ? templateName
                : $"{templateName}.txt";

            var candidatePaths = new List<string>
            {
                Path.Combine(AppContext.BaseDirectory, "Templates", "Email", normalizedName),
                Path.Combine(AppContext.BaseDirectory, "Templates", normalizedName),
                Path.Combine(AppContext.BaseDirectory, normalizedName)
            };

            if (_environment != null)
            {
                candidatePaths.Add(Path.Combine(_environment.ContentRootPath, "Templates", "Email", normalizedName));
                candidatePaths.Add(Path.Combine(_environment.ContentRootPath, "Templates", normalizedName));
            }

            var currentDirectory = Directory.GetCurrentDirectory();
            candidatePaths.Add(Path.Combine(currentDirectory, "Templates", "Email", normalizedName));
            candidatePaths.Add(Path.Combine(currentDirectory, "..", "Stock-Exchange.Infrastructure", "Templates", "Email", normalizedName));

            foreach (var path in candidatePaths)
            {
                if (File.Exists(path))
                    return Path.GetFullPath(path);
            }

            if (!normalizedName.Contains('_'))
            {
                var enFallback = Path.GetFileNameWithoutExtension(normalizedName) + "_en.txt";
                return ResolveTemplatePath(enFallback);
            }

            if (!normalizedName.EndsWith("_en.txt", StringComparison.OrdinalIgnoreCase))
            {
                var baseName = normalizedName[..normalizedName.LastIndexOf('_')];
                return ResolveTemplatePath($"{baseName}_en.txt");
            }

            return null;
        }
    }
}
