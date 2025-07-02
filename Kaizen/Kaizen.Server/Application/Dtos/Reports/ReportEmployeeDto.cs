namespace Kaizen.Server.Application.Dtos.Reports
{
    public class ReportEmployeeDto
    {
        public Guid Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string LastName { get; set; } = default!;
    }
}
