namespace Kaizen.Server.Application.Dtos.Auth;

public class UserInfoDto
{
    public Guid UserPK { get; set; }
    public Guid EmpID { get; set; }
    public bool? RegistersHours { get; set; }
    public string? PayrollType { get; set; }
}