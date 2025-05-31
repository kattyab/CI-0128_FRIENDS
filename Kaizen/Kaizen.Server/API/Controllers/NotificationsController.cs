using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Dtos.Auth;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Kaizen.Server.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController(IAuthService authService, NotificationsRepository notificationsRepository) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly NotificationsRepository _notificationsRepository = notificationsRepository;

    [HttpGet("")]
    public IActionResult Index()
    {
        if (this._authService.IsAuthenticated() == false)
        {
            return this.Unauthorized();
        }

        try
        {
            AuthUserDto authUser = this._authService.GetAuthUser();

            List<NotificationDto> notifications = this._notificationsRepository.GetNotifications(authUser.UserPK);

            return this.Ok(notifications);
        }
        catch (Exception)
        {
            return this.StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
