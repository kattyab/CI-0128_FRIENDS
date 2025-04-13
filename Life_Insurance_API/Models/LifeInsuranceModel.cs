namespace LifeInsuranceApi.Models
{
    public class LifeInsuranceRequest
    {
        public DateTime DateOfBirth { get; set; }
        public string Sex { get; set; } // "male" or "female"
    }

    public class LifeInsuranceResponse
    {
        public decimal MonthlyCost { get; set; }
    }
}
