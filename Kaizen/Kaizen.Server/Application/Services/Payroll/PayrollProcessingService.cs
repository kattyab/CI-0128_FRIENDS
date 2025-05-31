using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Application.Services.Payroll
{
        public interface IPayrollProcessingService
    {
        Task ProcessCompanyPayrollAsync(Guid companyId);
        Task<List<PayrollSummary>> CalculateCompanyPayrollAsync(Guid companyId);
    }

    public class PayrollProcessingService : IPayrollProcessingService
    {
        private readonly IConfiguration _configuration;
        private readonly PayrollCalculator _payrollCalculator;
        private readonly IApiDeductionServiceFactory _apiDeductionServiceFactory;
        private readonly IBenefitDeductionServiceFactory _benefitDeductionServiceFactory;

        public PayrollProcessingService(
            IConfiguration configuration,
            PayrollCalculator payrollCalculator,
            IApiDeductionServiceFactory apiDeductionServiceFactory,
            IBenefitDeductionServiceFactory benefitDeductionServiceFactory)
        {
            _configuration = configuration;
            _payrollCalculator = payrollCalculator;
            _apiDeductionServiceFactory = apiDeductionServiceFactory;
            _benefitDeductionServiceFactory = benefitDeductionServiceFactory;
        }

        public async Task ProcessCompanyPayrollAsync(Guid companyId)
        {
            var payrollResults = await CalculateCompanyPayrollAsync(companyId);

            foreach (var payrollSummary in payrollResults)
            {
                PrintPayrollSummary(payrollSummary);
            }
        }

        public async Task<List<PayrollSummary>> CalculateCompanyPayrollAsync(Guid companyId)
        {
            var employeeData = await GetEmployeeDataAsync(companyId);

            var apiDeductionService = _apiDeductionServiceFactory.Create(companyId);
            var benefitDeductionService = _benefitDeductionServiceFactory.Create(companyId);

            var payrollResults = new List<PayrollSummary>();

            foreach (var employee in employeeData)
            {
                var payrollSummary = await _payrollCalculator.CalculatePayrollAsync(
                    employee.EmpID,
                    employee.BruteSalary,
                    employee.ContractType,
                    employee.RegistersHours,
                    apiDeductionService,
                    benefitDeductionService);

                payrollResults.Add(payrollSummary);
            }

            return payrollResults;
        }

        private async Task<List<EmployeeDto>> GetEmployeeDataAsync(Guid companyId)
        {
            var employeeData = new List<EmployeeDto>();
            var connectionString = _configuration.GetConnectionString("KaizenDb");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var cmdText = @"
            SELECT E.EmpID, E.BruteSalary, E.StartDate, E.FireDate, E.ContractType, E.RegistersHours
            FROM dbo.Employees E
            INNER JOIN dbo.Companies C ON E.WorksFor = C.CompanyPK
            WHERE C.CompanyPK = @CompanyID";

            await using var command = new SqlCommand(cmdText, connection);
            command.Parameters.AddWithValue("@CompanyID", companyId.ToString());

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var empId = reader.GetGuid(0);
                var salary = reader.GetDecimal(1);
                var startDate = reader.GetDateTime(2);
                var fireDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);
                var contractType = reader.GetString(4);
                var registersHours = reader.GetBoolean(5);

                employeeData.Add(new EmployeeDto
                {
                    EmpID = empId,
                    BruteSalary = salary,
                    StartDate = startDate,
                    FireDate = fireDate,
                    ContractType = contractType,
                    RegistersHours = registersHours
                });
            }

            return employeeData;
        }

        private static void PrintPayrollSummary(PayrollSummary payrollSummary)
        {
            Console.WriteLine($"--- Payroll for Employee: {payrollSummary.EmployeeId} ---");
            Console.WriteLine($"Contract Type: {payrollSummary.ContractType}");
            Console.WriteLine($"Registers Hours: {(payrollSummary.RegistersHours ? "Yes" : "No")}");
            Console.WriteLine($"Gross Salary: {payrollSummary.GrossSalary:C}");
            Console.WriteLine($"Net Salary: {payrollSummary.NetSalary:C}");
            Console.WriteLine($"Total Deductions: {payrollSummary.TotalDeductions:C}");

            Console.WriteLine("API Deductions:");
            foreach (var kv in payrollSummary.ApiDeductions)
                Console.WriteLine($"  {kv.Key}: {kv.Value:C}");

            Console.WriteLine("Benefit Deductions:");
            foreach (var b in payrollSummary.BenefitDeductions)
                Console.WriteLine($"  {b.BenefitName}: {b.DeductionValue:C}");

            Console.WriteLine($"CCSS Deduction: {payrollSummary.CCSSDeduction:C}");
            if (payrollSummary.ContractType == "Servicios Profesionales" && payrollSummary.CCSSDeduction == 0)
                Console.WriteLine("  (Exempt - Servicios Profesionales)");

            Console.WriteLine($"Income Tax: {payrollSummary.IncomeTax:C}");
            if (payrollSummary.ContractType == "Servicios Profesionales" && payrollSummary.IncomeTax == 0)
                Console.WriteLine("  (Exempt - Servicios Profesionales)");

            Console.WriteLine("-------------------------------------");
        }
    }

    public class PayrollSummary
    {
        public Guid EmployeeId { get; set; }
        public string ContractType { get; set; } = string.Empty;
        public bool RegistersHours { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public decimal TotalDeductions { get; set; }
        public Dictionary<string, decimal> ApiDeductions { get; set; } = new();
        public List<BenefitDeductionResult> BenefitDeductions { get; set; } = new();
        public decimal CCSSDeduction { get; set; }
        public decimal IncomeTax { get; set; }
    }

    public class EmployeeData
    {
        public Guid EmpID { get; set; }
        public decimal BruteSalary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FireDate { get; set; }
        public string ContractType { get; set; } = string.Empty;
        public bool RegistersHours { get; set; }
    }
}
