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
                throw new ArgumentException("Recipient email address cannot be empty.", nameof(toEmail));

            var fromEmail = !string.IsNullOrWhiteSpace(_emailSettings.Email) ? _emailSettings.Email.Trim() : "mohamed7saber10tech@gmail.com";
            var fromName = string.IsNullOrWhiteSpace(_emailSettings.Name) ? "StockExchange@Team" : _emailSettings.Name;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart(isHtml ? TextFormat.Html : TextFormat.Plain)
            {
                Text = body
            };

            using var client = new SmtpClient();
            client.Timeout = 15000;
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            var host = string.IsNullOrWhiteSpace(_emailSettings.Host) ? "smtp.gmail.com" : _emailSettings.Host.Trim();
            var port = _emailSettings.Port > 0 ? _emailSettings.Port : 587;

            var socketOptions = port switch
            {
                465 => SecureSocketOptions.SslOnConnect,
                587 => SecureSocketOptions.StartTls,
                _ => SecureSocketOptions.Auto
            };

            try
            {
                await client.ConnectAsync(host, port, socketOptions, cancellationToken);
            }
            catch (Exception)
            {
                // Fallback to alternate port (587 <-> 465) if the primary port is blocked by hosting firewall
                var fallbackPort = port == 587 ? 465 : 587;
                var fallbackOptions = fallbackPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
                await client.ConnectAsync(host, fallbackPort, fallbackOptions, cancellationToken);
            }

            var username = !string.IsNullOrWhiteSpace(_emailSettings.Username) ? _emailSettings.Username.Trim() : fromEmail;
            var password = !string.IsNullOrWhiteSpace(_emailSettings.Password)
                ? _emailSettings.Password.Trim()
                : "crdmcmajlrbxgfru";

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                await client.AuthenticateAsync(username, password, cancellationToken);
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
            string content;
            var templatePath = ResolveTemplatePath(templateName);
            if (templatePath != null && File.Exists(templatePath))
            {
                content = await File.ReadAllTextAsync(templatePath, Encoding.UTF8, cancellationToken);
            }
            else
            {
                content = GetFallbackTemplate(templateName);
            }

            if (placeholders != null)
            {
                foreach (var kvp in placeholders)
                {
                    content = content.Replace($"{{{kvp.Key}}}", kvp.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
                }
            }

            await SendEmailAsync(toEmail, subject, content, isHtml, cancellationToken);
        }

        private static string GetFallbackTemplate(string templateName)
        {
            if (templateName.Contains("ar", StringComparison.OrdinalIgnoreCase))
            {
                return "مرحباً {Name}،\n\nرمز التحقق الخاص بك لإعادة تعيين كلمة المرور هو: {Code}\n\nهذا الرمز صالح لمدة {ExpiryMinutes} دقيقة.\nإذا لم تطلب إعادة تعيين كلمة المرور، يرجى تجاهل هذا البريد.\n\nمع تحيات فريق Stock Exchange";
            }

            return "Hello {Name},\n\nYour password reset verification code is: {Code}\n\nThis code is valid for {ExpiryMinutes} minutes.\nIf you did not request a password reset, please ignore this email.\n\nBest regards,\nStock Exchange Team";
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
