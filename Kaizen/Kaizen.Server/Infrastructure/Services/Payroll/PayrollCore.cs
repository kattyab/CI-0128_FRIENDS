using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.Payroll;

namespace Kaizen.Server.Infrastructure.Services.Payroll
{
    public class PayrollCore : IPayrollCore
    {
        private const decimal LaborChargeRate = 0.2667m;

        public decimal GetLaborChargeRate() => LaborChargeRate;

        public bool IsPayrollValid(PayrollSummary payrollSummary)
        {
            return payrollSummary.NetSalary >= 0;
        }

        public PayrollResultSumary CreatePayrollResult(PayrollRequest request, List<PayrollSummary> payrollResults)
        {
            var failedPayrolls = payrollResults.Where(p => !IsPayrollValid(p)).ToList();

            var result = new PayrollResultSumary
            {
                CompanyId = request.CompanyId,
                IsSuccess = !failedPayrolls.Any(),
                FailedPayrolls = failedPayrolls.Select(p => p.EmployeeId).ToList(),
                FailedNetSalaries = failedPayrolls.Select(p => p.NetSalary).ToList()
            };

            if (failedPayrolls.Any())
            {
                return result;
            }

            PopulateSuccessfulResult(result, request, payrollResults);
            return result;
        }

        private void PopulateSuccessfulResult(PayrollResultSumary result, PayrollRequest request, List<PayrollSummary> payrollResults)
        {
            result.Gross = payrollResults.Sum(p => p.GrossSalary);
            result.Net = payrollResults.Sum(p => p.NetSalary);
            result.Deductions = CalculateTotalDeductions(payrollResults);
            result.SocialCharges = result.Gross * LaborChargeRate;
            result.Manager = request.Email;
            result.Period = $"{request.Start:yyyy-MM} → {request.End:yyyy-MM}";
            result.Type = request.Type;
            result.TotalPaid = result.Gross + result.SocialCharges;
        }

        private static decimal CalculateTotalDeductions(List<PayrollSummary> payrollResults)
        {
            return payrollResults.Sum(p =>
                p.ApiDeductions.Values.Sum() +
                p.BenefitDeductions.Sum(b => b.DeductionValue) +
                p.CCSSDeduction +
                p.IncomeTax);
        }
    }
}
