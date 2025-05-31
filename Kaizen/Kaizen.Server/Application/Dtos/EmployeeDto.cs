namespace Kaizen.Server.Application.Dtos;

public class EmployeeDto
{
    public Guid EmpID { get; set; } = default!;
    public Guid PersonPK { get; set; } = default!;
    public Guid WorksFor { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string LastName { get; set; }  = default!;
    public string Id { get; set; } = default!;
    public string JobPosition { get; set; } = default!;
    public string ContractType { get; set; } = default!;
    public DateTime StartDate { get; set; } = default!;
    public decimal BruteSalary { get; set; } = default!;
}
