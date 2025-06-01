namespace Kaizen.Server.Application.Dtos.Benefits
{
    public class EmployeeBenefitListDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Value { get; set; }
        public int MinMonths { get; set; }
    }
}
