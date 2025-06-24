namespace Kaizen.Server.Application.Dtos.Benefits;

public class BenefitDto
{
    public Guid ID { get; set; }
    public string Name { get; set; } = default!;
    public int MinWorkDurationMonths { get; set; }
    public bool IsFixed { get; set; }
    public decimal? FixedValue { get; set; }
    public bool IsPercentage { get; set; }
    public decimal? PercentageValue { get; set; }
    public bool IsFullTime { get; set; }
    public bool IsPartTime { get; set; }
    public bool IsByHours { get; set; }
    public bool IsByService { get; set; }

    public bool IsSubscribed { get; set; }
}
