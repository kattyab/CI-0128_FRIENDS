using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Repositories;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController(NotificationsRepository notificationsRepository, Login loginRepository) : ControllerBase
{
    private readonly NotificationsRepository _notificationsRepository = notificationsRepository;
    private readonly Login _loginRepository = loginRepository ;

    [HttpGet("")]
    public IActionResult Index()
    {
        if (this.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            return this.Unauthorized();
        }

        string? email = this.HttpContext.User.Identity?.Name;

        object? user = this._loginRepository.ObtainUserData(email!);

        if (user is null)
        {
            return this.NotFound();
        }

        Guid userPK = Guid.Parse((string)user.GetType().GetProperty("UserPK")!.GetValue(user)!);

        List<NotificationDto> notifications = this._notificationsRepository.GetNotifications(userPK);

        return this.Ok(notifications);
    }
}
