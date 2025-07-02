using System.Collections.Generic;
using System;

namespace Kaizen.Server.Application.Dtos;

public class EmployeeDashboardDto
{
    public Guid UserPK { get; set; }
    public Guid PersonPK { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string JobPosition { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public decimal BruteSalary { get; set; }
    public string ContractType { get; set; } = string.Empty;
    public Guid EmpID { get; set; }

    public List<PayrollInfoDto> RecentPayrolls { get; set; } = new();
    public List<OptionalDeductionDto> OptionalDeductions { get; set; } = new();
}

public class PayrollInfoDto
{
    public Guid PayrollID { get; set; }
    public Guid GeneralPayrollPK { get; set; }
    public DateTime ExecutedOn { get; set; }
    public decimal IncomeTax { get; set; }
    public decimal CCSS { get; set; }
    public decimal BrutePaid { get; set; }
    public decimal NetPaid { get; set; }
}

public class OptionalDeductionDto
{
    public string OptionalDeductionName { get; set; } = string.Empty;
    public decimal OptionalDeductionAmount { get; set; }
    public Guid OptionalDeductionPayrollId { get; set; }
}
