namespace Kaizen.Server.Models
{
    public class BenefitCreationDto
    {
        public string Name { get; set; }
        public int MinWorkDurationMonths { get; set; }
        public string AdminEmail { get; set; }
        public string AdminRole { get; set; }
        public bool IsFullTime { get; set; }
        public bool IsPartTime { get; set; }
        public bool IsByHours { get; set; }
        public bool IsByService { get; set; }
        public bool IsFixed { get; set; }
        public decimal? FixedValue { get; set; }
        public bool IsPercentage { get; set; }
        public decimal? PercentageValue { get; set; }
        public bool IsAPI { get; set; }
        public string? ApiPath { get; set; }
        public int? NumParameters { get; set; }
    }
}
