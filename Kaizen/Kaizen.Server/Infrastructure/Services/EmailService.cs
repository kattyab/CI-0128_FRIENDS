using Kaizen.Server.Application.Configuraton;
using Kaizen.Server.Application.Emails;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Kaizen.Server.Application.Interfaces.Services;

namespace Kaizen.Server.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger logger;
    private readonly EmailConfiguration emailConfiguration;

    public EmailService(ILogger<EmailService> logger,
        IOptions<EmailConfiguration> emailConfiguration)
    {
        this.logger = logger;
        this.emailConfiguration = emailConfiguration.Value;
    }

    public async Task SendEmail(Email message)
    {
        try
        {
            var emailMessage = this.CreateEmailMessage(message);
            await this.Send(emailMessage);
        }
        catch
        {
            throw new Exception("Could not send email.");
        }
    }

    private MimeMessage CreateEmailMessage(Email message)
    {
        BodyBuilder bodyBuilder = new()
        {
            TextBody = message.GetText(),
        };

        MimeMessage emailMessage = new();
        emailMessage.From.Add(new MailboxAddress(this.emailConfiguration.Name, this.emailConfiguration.Address));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;

        if (message.Attachments.Count > 0)
        {
            foreach (EmailAttachment attachment in message.Attachments)
            {
                bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content, attachment.ContentType);
            }
        }

        emailMessage.Body = bodyBuilder.ToMessageBody();

        return emailMessage;
    }
    private async Task Send(MimeMessage mailMessage)
    {
        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(this.emailConfiguration.Hostname, this.emailConfiguration.Port, SecureSocketOptions.Auto);
            await client.AuthenticateAsync(this.emailConfiguration.Username, this.emailConfiguration.Password);
            await client.SendAsync(mailMessage);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Could not send email");
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true);
            client.Dispose();
        }
    }
}
