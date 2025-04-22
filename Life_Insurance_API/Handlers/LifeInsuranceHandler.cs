using LifeInsuranceApi.Models;

namespace LifeInsuranceApi.Handlers
{
    /// <summary>
    /// Handles life insurance calculations based on user-provided data.
    /// </summary>
    public class LifeInsuranceHandler
    {
        /// <summary>
        /// Calculates the monthly cost of life insurance based on the request details.
        /// </summary>
        /// <param name="request">The life insurance request containing the date of birth and sex of the applicant.</param>
        /// <returns>A <see cref="LifeInsuranceResponse"/> containing the calculated monthly cost.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="request"/> is null.</exception>
        public LifeInsuranceResponse Calculate(LifeInsuranceRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            int age = DateTime.Today.Year - request.DateOfBirth.Year;
            if (request.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            decimal cost = 0;
            string sex = request.Sex.ToLower();

            if (sex == "male" || sex == "hombre")
            {
                if (age < 30)
                    cost = 15000;
                else if (age <= 50)
                    cost = 25000;
                else
                    cost = 35000;
            }
            else if (sex == "female" || sex == "mujer")
            {
                if (age < 30)
                    cost = 20000;
                else if (age <= 50)
                    cost = 30000;
                else
                    cost = 45000;
            }

            return new LifeInsuranceResponse
            {
                MonthlyCost = cost
            };
        }
    }
}
