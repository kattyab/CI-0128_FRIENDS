namespace Kaizen.Server.Application.Dtos.BenefitDeductions
{
    public class Benefit
    {
        public Guid BenefitID { get; set; }
        public string Name { get; set; } = "";
        public int MinWorkDurationMonths { get; set; }
        public bool IsFixed { get; set; }
        public decimal? FixedValue { get; set; }
        public bool IsPercetange { get; set; }
        public decimal? PercentageValue { get; set; }
        public bool IsFullTime { get; set; }
        public bool IsPartTime { get; set; }
        public bool IsByHours { get; set; }
        public bool IsByService { get; set; }
    }
}
