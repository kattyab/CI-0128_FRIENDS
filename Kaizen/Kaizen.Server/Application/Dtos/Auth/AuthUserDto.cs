namespace Kaizen.Server.Application.Dtos.Auth;

public class AuthUserDto
{
  public string Email { get; set; } = default!;
  public bool Active { get; set; }
  public string Role { get; set; } = default!;
  public Guid PersonPK { get; set; } = default!;
  public Guid UserPK { get; set; } = default!;
}
