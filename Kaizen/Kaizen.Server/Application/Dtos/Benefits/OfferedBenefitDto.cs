namespace Kaizen.Server.Application.Dtos.Benefits
{
    public class OfferedBenefitDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int MinMonths { get; set; }

        public bool IsAvailable { get; set; }
        public string ReasonUnavailable { get; set; }
        public decimal Value { get; set; } 
    }
}