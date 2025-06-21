using Kaizen.Server.Application.Dtos.Auth;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Kaizen.Server.Infrastructure.Repositories;

namespace Kaizen.Server.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Login _loginRepository;

    public AuthService(IHttpContextAccessor httpContextAccessor, Login loginRepository)
    {
        this._httpContextAccessor = httpContextAccessor;
        this._loginRepository = loginRepository;
    }

    public bool IsAuthenticated()
    {
        return this._httpContextAccessor?.HttpContext?.User.Identity?.IsAuthenticated == true;
    }

    public AuthUserDto GetAuthUser()
    {
        if (this.IsAuthenticated() == false)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        string email = this._httpContextAccessor?.HttpContext?.User.Identity?.Name!;
        return this._loginRepository.GetAuthUser(email);
    }

    public Guid GetAuthUserCompanyPK()
    {
        AuthUserDto authUser = this.GetAuthUser();
        return this._loginRepository.GetAuthUserCompanyPK(authUser);
    }

    public Guid GetAuthUserEmployeePK()
    {
        AuthUserDto authUser = this.GetAuthUser();
        return this._loginRepository.GetAuthUserEmployeePK(authUser);
    }

}
