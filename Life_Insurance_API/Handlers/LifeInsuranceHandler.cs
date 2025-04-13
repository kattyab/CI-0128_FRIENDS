using LifeInsuranceApi.Models;

namespace LifeInsuranceApi.Handlers
{
    public class LifeInsuranceHandler
    {
        public LifeInsuranceResponse Calculate(LifeInsuranceRequest request)
        {
            int age = DateTime.Today.Year - request.DateOfBirth.Year;
            if (request.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            decimal cost = 0;
            string sex = request.Sex.ToLower();

            // cost calculation up to specs
            if (sex == "male")
            {
                if (age < 30)
                    cost = 15000;
                else if (age <= 50)
                    cost = 25000;
                else
                    cost = 35000;
            }
            else if (sex == "female")
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
