using Kaizen.Server.Application.Configuration;
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
        emailMessage.Subject = message.GetSubject();

        Multipart multipart = new("mixed")
        {
            new TextPart("plain") { Text = message.GetText() }
        };

        if (message.Attachments.Count > 0)
        {
            foreach (EmailAttachment attachment in message.Attachments)
            {
                multipart.Add(new MimePart
                {
                    Content = new MimeContent(new MemoryStream(attachment.Content)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = attachment.FileName,
                });
            }
        }

        emailMessage.Body = multipart;

        return emailMessage;
    }
    private async Task Send(MimeMessage mailMessage)
    {
        using var client = new SmtpClient();

        try
        {
            logger.LogInformation("Connecting to SMTP {Host}:{Port} with SSL: {SslOption}",
                emailConfiguration.Hostname,
                emailConfiguration.Port,
                emailConfiguration.Port == 465 ? "True (SSL)" : "False (STARTTLS)");

            SecureSocketOptions socketOption = emailConfiguration.Port == 465
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;

            await client.ConnectAsync(emailConfiguration.Hostname, emailConfiguration.Port, socketOption);

            logger.LogInformation("Authenticating as {Username}", emailConfiguration.Username);

            await client.AuthenticateAsync(emailConfiguration.Username, emailConfiguration.Password);

            logger.LogInformation("Sending email to {Recipients}", string.Join(", ", mailMessage.To));

            await client.SendAsync(mailMessage);

            logger.LogInformation("Email sent successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not send email");
            throw;
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true);
            }
        }
    }

}
