namespace Kaizen.Server.Application.Dtos
{
    public class ApprovedHoursDto
    {
        public Guid ApprovalID { get; set; }
        public Guid EmpID { get; set; }
        public Guid? SupID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal HoursWorked { get; set; }
        public string? Status { get; set; }              
        public bool IsSentForApproval { get; set; }

        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public DateTime EmployeeStartDate { get; set; }
        public string ContractType { get; set; } = string.Empty;

        public string? PayrollType { get; set; }
    }

}
