namespace Kaizen.Server.Application.Dtos.Reports
{
    public class HistoricRangeInitialDto
    {
        public string CompanyName { get; set; } = default!;
        public List<ReportEmployeeDto> Employees { get; set; } = default!;
    }
}
