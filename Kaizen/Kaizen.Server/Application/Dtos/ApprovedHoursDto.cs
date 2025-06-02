namespace Kaizen.Server.Application.Dtos
{
    public class ApprovedHoursDto
    {
        public Guid EmpID { get; set; }
        public Guid? SupID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal HoursWorked { get; set; }
        public string? Status { get; set; }              
        public bool IsSentForApproval { get; set; }      
    }

}
