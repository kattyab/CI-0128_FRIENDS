namespace LifeInsuranceApi.Models
{
    /// <summary>
    /// Represents a request for a life insurance quote calculation.
    /// </summary>
    public class LifeInsuranceRequest
    {
        /// <summary>
        /// The date of birth of the person requesting insurance.
        /// Used to calculate age-based premium factors.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// The biological sex of the person requesting insurance.
        /// Must be either "male" or "female".
        /// </summary>
        public string Sex { get; set; } 
    }

    /// <summary>
    /// Represents the response containing life insurance quote information.
    /// </summary>
    public class LifeInsuranceResponse
    {
        /// <summary>
        /// The calculated monthly premium cost for the life insurance policy.
        /// Value is in colones.
        /// </summary>
        public decimal MonthlyCost { get; set; }
    }
}