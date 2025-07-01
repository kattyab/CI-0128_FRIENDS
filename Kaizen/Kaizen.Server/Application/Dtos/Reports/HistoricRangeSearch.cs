namespace Kaizen.Server.Application.Dtos.Reports
{
    public class HistoricRangeSearch
    {
        public Guid EmployeeId { get; set; } = default!;
        public DateTime Start { get; set; } = default!;
        public DateTime End { get; set; } = default!;
    }
}
