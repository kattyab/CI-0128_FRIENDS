using Kaizen.Server.Application.Dtos.Auth;

namespace Kaizen.Server.Application.Interfaces.Services.Auth;

public interface IAuthService
{
    public bool IsAuthenticated();
    public AuthUserDto GetAuthUser();
}
