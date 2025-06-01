using System.Data;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Application.Services.Payroll
{
    public interface IPayrollProcessingService
    {
        Task<string> ProcessCompanyPayrollAsync(Guid companyId);
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

        public async Task<string> ProcessCompanyPayrollAsync(Guid companyId)
        {
            var payrollResults = await CalculateCompanyPayrollAsync(companyId);

            var failedPayrolls = payrollResults.Where(payrollSummary => payrollSummary.NetSalary < 0).ToList();


            /*if (failedPayrolls.Any())
            {
                var failedIds = string.Join(", ", failedPayrolls.Select(p => p.EmployeeId));
                return $"Failed, {failedIds}";
            }*/

            await SavePayrollAsync(companyId, payrollResults);

            foreach (var payrollSummary in payrollResults)
            {
                PrintPayrollSummary(payrollSummary);
            }

            return "Success";
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
                var EmployeeId = reader.GetGuid(0);
                var salary = reader.GetDecimal(1);
                var startDate = reader.GetDateTime(2);
                var fireDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);
                var contractType = reader.GetString(4);
                var registersHours = reader.GetBoolean(5);

                employeeData.Add(new EmployeeDto
                {
                    EmpID = EmployeeId,
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

        private async Task SavePayrollAsync(Guid companyId, List<PayrollSummary> summaries)
        {
            var connectionString = _configuration.GetConnectionString("KaizenDb");
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            var generalPayrollId = Guid.NewGuid();
            var generalData = BuildGeneralPayrollData(companyId, generalPayrollId, summaries);
            var payrollsTable = BuildPayrollsTable(generalPayrollId, summaries);
            var deductionsTable = BuildOptionalDeductionsTable(summaries);

            await using var cmd = new SqlCommand("SaveFullPayroll", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@GeneralPayrollsID", generalPayrollId);
            cmd.Parameters.AddWithValue("@PaidBy", companyId);
            cmd.Parameters.AddWithValue("@TotalDeductionsBenefits", generalData.TotalDeductionsBenefits);
            cmd.Parameters.AddWithValue("@TotalObligatoryDeductions", generalData.TotalObligatoryDeductions);
            cmd.Parameters.AddWithValue("@TotalLaborCharges", generalData.TotalLaborCharges);
            cmd.Parameters.AddWithValue("@TotalMoneyPaid", generalData.TotalMoneyPaid);
            cmd.Parameters.AddWithValue("@StartDate", generalData.StartDate);

            var payrollsParam = cmd.Parameters.AddWithValue("@Payrolls", payrollsTable);
            payrollsParam.SqlDbType = SqlDbType.Structured;
            payrollsParam.TypeName = "dbo.PayrollsType";

            var deductionsParam = cmd.Parameters.AddWithValue("@OptionalDeductions", deductionsTable);
            deductionsParam.SqlDbType = SqlDbType.Structured;
            deductionsParam.TypeName = "dbo.OptionalDeductionsType";

            await cmd.ExecuteNonQueryAsync();
        }

        private static GeneralPayrollData BuildGeneralPayrollData(Guid companyId, Guid generalPayrollId, List<PayrollSummary> summaries)
        {
            var totalOptionalDeductions = summaries.Sum(s =>
                s.ApiDeductions.Values.Sum() + s.BenefitDeductions.Sum(b => b.DeductionValue));

            var totalObligatoryDeductions = summaries.Sum(s =>
                s.CCSSDeduction + s.IncomeTax);

            var totalLaborCharges = summaries.Sum(s => s.GrossSalary) * 0.2667m;
            var totalMoneyPaid = totalLaborCharges + summaries.Sum(s => s.GrossSalary);

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
            var startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            return new GeneralPayrollData
            {
                TotalDeductionsBenefits = totalOptionalDeductions,
                TotalObligatoryDeductions = totalObligatoryDeductions,
                TotalLaborCharges = totalLaborCharges,
                TotalMoneyPaid = totalMoneyPaid,
                StartDate = startDate
            };
        }

        /// <summary>
        /// LOOK AT THIS FUNCTION PLEASE DONT FORGET TO CHANGE THIS
        /// </summary>
        /// <param name="generalPayrollId"></param>
        /// <param name="summaries"></param>
        /// <returns></returns>
        private static DataTable BuildPayrollsTable(Guid generalPayrollId, List<PayrollSummary> summaries)
        {
            var table = new DataTable();
            table.Columns.Add("PayrollID", typeof(Guid));
            table.Columns.Add("PaidTo", typeof(Guid));
            table.Columns.Add("ExecutedBy", typeof(Guid));
            table.Columns.Add("IsClosed", typeof(bool));
            table.Columns.Add("IncomeTax", typeof(decimal));
            table.Columns.Add("CCSS", typeof(decimal));
            table.Columns.Add("ApprovalID", typeof(Guid));
            table.Columns.Add("GeneralPayrollPk", typeof(Guid));
            table.Columns.Add("BrutePaid", typeof(decimal));
            table.Columns.Add("NetPaid", typeof(decimal));

            // LOOK HERE!
            var hardcodedExecutorPersonPk = new Guid("23681BFF-82CB-4663-BA5E-16E6A5EA599D");

            foreach (var employeePayroll in summaries)
            {
                employeePayroll.PayrollId = Guid.NewGuid();
                table.Rows.Add(
                    employeePayroll.PayrollId,
                    employeePayroll.EmployeeId,
                    hardcodedExecutorPersonPk, // HARDCODED HERE!!
                    false, // IsClosed Nani??
                    employeePayroll.IncomeTax,
                    employeePayroll.CCSSDeduction,
                    DBNull.Value,
                    generalPayrollId,
                    employeePayroll.GrossSalary,
                    employeePayroll.NetSalary);
            }

            return table;
        }

        private static DataTable BuildOptionalDeductionsTable(List<PayrollSummary> summaries)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(Guid));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Amount", typeof(decimal));
            table.Columns.Add("PayrollId", typeof(Guid));

            foreach (var payrollSummary in summaries)
            {
                foreach (var apiDeduction in payrollSummary.ApiDeductions)
                    table.Rows.Add(Guid.NewGuid(), apiDeduction.Key, apiDeduction.Value, payrollSummary.PayrollId);

                foreach (var benefitDeduction in payrollSummary.BenefitDeductions)
                    table.Rows.Add(Guid.NewGuid(), benefitDeduction.BenefitName, benefitDeduction.DeductionValue, payrollSummary.PayrollId);
            }

            return table;
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
        public Guid PayrollId { get; set; }
    }

    /*public class EmployeeData
    {
        public Guid EmpID { get; set; }
        public decimal BruteSalary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FireDate { get; set; }
        public string ContractType { get; set; } = string.Empty;
        public bool RegistersHours { get; set; }
    }*/

    public class GeneralPayrollData
    {
        public decimal TotalDeductionsBenefits { get; set; }
        public decimal TotalObligatoryDeductions { get; set; }
        public decimal TotalLaborCharges { get; set; }
        public decimal TotalMoneyPaid { get; set; }
        public DateTime StartDate { get; set; }
    }
}
