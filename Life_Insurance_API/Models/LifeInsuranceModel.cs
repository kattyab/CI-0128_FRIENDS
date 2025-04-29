namespace LifeInsuranceApi.Models
{
    public class LifeInsuranceRequest
    {
        public DateTime DateOfBirth { get; set; }

        public required string Sex { get; set; }
    }

    public class LifeInsuranceResponse
    {
        public decimal MonthlyCost { get; set; }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; } = true;

        public DateTime? DateOfBirth { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
