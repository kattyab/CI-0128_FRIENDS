using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.Payroll;

namespace Kaizen.Server.Infrastructure.Helpers.Payroll
{
    public class PayrollOutputService : IPayrollOutputService
    {
        public void PrintPayrollSummary(PayrollSummary payrollSummary)
        {
#if DEBUG
            Console.WriteLine($"--- Payroll for Employee: {payrollSummary.EmployeeId} ---");
            Console.WriteLine($"Contract Type: {payrollSummary.ContractType}");
            Console.WriteLine($"Registers Hours: {(payrollSummary.RegistersHours ? "Yes" : "No")}");
            Console.WriteLine($"Gross Salary: {payrollSummary.GrossSalary:C}");
            Console.WriteLine($"Net Salary: {payrollSummary.NetSalary:C}");
            Console.WriteLine($"Total Deductions: {payrollSummary.TotalDeductions:C}");

            PrintApiDeductions(payrollSummary);
            PrintBenefitDeductions(payrollSummary);
            PrintObligatoryDeductions(payrollSummary);

            Console.WriteLine("-------------------------------------");
#else
#endif
        }

        public void PrintPayrollResults(List<PayrollSummary> payrollResults)
        {
            foreach (var payrollSummary in payrollResults)
            {
                PrintPayrollSummary(payrollSummary);
            }
        }

        private static void PrintApiDeductions(PayrollSummary payrollSummary)
        {
            Console.WriteLine("API Deductions:");
            foreach (var kv in payrollSummary.ApiDeductions)
                Console.WriteLine($"  {kv.Key}: {kv.Value:C}");
        }

        private static void PrintBenefitDeductions(PayrollSummary payrollSummary)
        {
            Console.WriteLine("Benefit Deductions:");
            foreach (var b in payrollSummary.BenefitDeductions)
                Console.WriteLine($"  {b.BenefitName}: {b.DeductionValue:C}");
        }

        private static void PrintObligatoryDeductions(PayrollSummary payrollSummary)
        {
            Console.WriteLine($"CCSS Deduction: {payrollSummary.CCSSDeduction:C}");
            if (payrollSummary.ContractType == "Servicios Profesionales" && payrollSummary.CCSSDeduction == 0)
                Console.WriteLine("  (Exempt - Servicios Profesionales)");

            Console.WriteLine($"Income Tax: {payrollSummary.IncomeTax:C}");
            if (payrollSummary.ContractType == "Servicios Profesionales" && payrollSummary.IncomeTax == 0)
                Console.WriteLine("  (Exempt - Servicios Profesionales)");
        }
    }
}
