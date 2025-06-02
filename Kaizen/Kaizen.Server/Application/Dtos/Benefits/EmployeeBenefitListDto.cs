namespace Kaizen.Server.Application.Dtos.Benefits
{
    public class EmployeeBenefitListDto
    {
        public Guid? BenefitId { get; set; }
        public int? APIId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Value { get; set; }
        public int MinMonths { get; set; }
    }
}
