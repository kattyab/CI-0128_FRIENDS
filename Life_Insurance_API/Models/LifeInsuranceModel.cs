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
        public required string Sex { get; set; }
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

    /// <summary>
    /// Represents the result of validating input parameters for a life insurance request.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Indicates whether the validation was successful.
        /// Defaults to <c>true</c>.
        /// </summary>
        public bool IsValid { get; set; } = true;

        /// <summary>
        /// The parsed date of birth if validation was successful.
        /// Otherwise, <c>null</c>.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// The error message describing why validation failed.
        /// Populated only if <see cref="IsValid"/> is <c>false</c>.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
