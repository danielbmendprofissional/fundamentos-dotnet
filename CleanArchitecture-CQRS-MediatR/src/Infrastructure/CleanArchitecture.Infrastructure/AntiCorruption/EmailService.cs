using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Infrastructure.AntiCorruption.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;

namespace CleanArchitecture.Infrastructure.AntiCorruption
{
    public class EmailService : IEmailService
    {
        protected readonly EmailConfigurationOptions _emailOptions;

        public EmailService(IServiceProvider serviceProvider) : base()
        {
            _emailOptions = serviceProvider.GetRequiredService<IOptions<EmailConfigurationOptions>>().Value
                ?? throw new ArgumentNullException(nameof(_emailOptions));
        }

        public async Task EnviarEmail(string destinatario, string assunto, string html, CancellationToken cancellationToken)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailOptions.SenderName, _emailOptions.SenderEmail));
            email.To.Add(new MailboxAddress(destinatario, destinatario));
            email.Subject = assunto;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = html };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_emailOptions.SmtpServer, _emailOptions.Port, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
                await smtp.AuthenticateAsync(_emailOptions.UserName, _emailOptions.Password, cancellationToken);
                await smtp.SendAsync(email, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            finally
            {
                await smtp.DisconnectAsync(true, cancellationToken);
            }
        }
    }
}