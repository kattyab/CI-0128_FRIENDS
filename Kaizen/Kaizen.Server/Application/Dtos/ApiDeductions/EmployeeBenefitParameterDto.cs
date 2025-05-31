namespace Kaizen.Server.Application.Dtos.ApiDeductions;

public class EmployeeBenefitParameterDto
{
    public Guid EmployeeId { get; set; }
    public int BenefitId { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}
