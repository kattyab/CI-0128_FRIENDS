using Kaizen.Server.Application.Emails;

namespace Kaizen.Server.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendEmail(Email message);
}
