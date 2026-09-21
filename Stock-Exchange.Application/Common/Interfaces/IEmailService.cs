namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default);
        Task SendTemplateEmailAsync(string toEmail, string subject, string templateName, IDictionary<string, string> placeholders, bool isHtml = false, CancellationToken cancellationToken = default);
    }
}
