using System.Data;
using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Application.Services.Payroll
{
    public interface IPayrollProcessingService
    {
        Task<PayrollResultSumary> ProcessCompanyPayrollAsync(PayrollRequest payrollInformation);
        Task<List<PayrollSummary>> CalculateCompanyPayrollAsync(PayrollRequest payrollInformation);
    }

    public class PayrollProcessingService : IPayrollProcessingService
    {
        private const decimal LaborChargeRate = 0.2667m;
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

        public async Task<PayrollResultSumary> ProcessCompanyPayrollAsync(PayrollRequest payrollInformation)
        {
            var payrollResults = await CalculateCompanyPayrollAsync(payrollInformation);
            var failedPayrolls = payrollResults.Where(payrollSummary => payrollSummary.NetSalary < 0).ToList();
            
            var result = new PayrollResultSumary
            {
                CompanyId = payrollInformation.CompanyId,
                IsSuccess = !failedPayrolls.Any(),
                FailedPayrolls = failedPayrolls.Select(p => p.EmployeeId).ToList(),
                FailedNetSalaries = failedPayrolls.Select(p => p.NetSalary).ToList()
            };

           if (failedPayrolls.Any())
            {
                return result;
            }

            result.Gross = payrollResults.Sum(p => p.GrossSalary);
            result.Net = payrollResults.Sum(p => p.NetSalary);
            result.Deductions = payrollResults.Sum(p =>
               p.ApiDeductions.Values.Sum() +
               p.BenefitDeductions.Sum(b => b.DeductionValue) +
               p.CCSSDeduction +
               p.IncomeTax);
            result.SocialCharges = result.Gross * LaborChargeRate;
            result.Manager = payrollInformation.Email;
            result.Period = $"{payrollInformation.Start:yyyy-MM} → {payrollInformation.End:yyyy-MM}";
            result.Type = payrollInformation.Type;
            result.TotalPaid = result.Gross + result.SocialCharges;

            await SavePayrollAsync(payrollInformation.CompanyId, payrollResults, payrollInformation.Email);

            foreach (var payrollSummary in payrollResults)
            {
                PrintPayrollSummary(payrollSummary);
            }

            return result;
        }


        public async Task<List<PayrollSummary>> CalculateCompanyPayrollAsync(PayrollRequest payrollInformation)
        {
            var employeeData = await GetEmployeeDataAsync(payrollInformation.CompanyId);
            var apiDeductionService = _apiDeductionServiceFactory.Create(payrollInformation.CompanyId);
            var benefitDeductionService = _benefitDeductionServiceFactory.Create(payrollInformation.CompanyId);

            var payrollResults = new List<PayrollSummary>();

            foreach (var employee in employeeData)
            {
                var payrollSummary = await _payrollCalculator.CalculatePayrollAsync(
                    employee,
                    apiDeductionService,
                    benefitDeductionService,
                    payrollInformation);

                payrollResults.Add(payrollSummary);
            }
            return payrollResults;
        }

        private async Task<List<EmployeePayroll>> GetEmployeeDataAsync(Guid companyId)
        {
            var employeeData = new List<EmployeePayroll>();
            var connectionString = _configuration.GetConnectionString("KaizenDb");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var cmdText = @"
                SELECT 
                    E.EmpID, E.BruteSalary, E.StartDate, E.FireDate, 
                    E.ContractType, E.RegistersHours,
                    dbo.GetPayrollTypeDescription(C.PayrollType) AS PayrollTypeDescription
                FROM dbo.Employees E
                INNER JOIN dbo.Companies C ON E.WorksFor = C.CompanyPK
                WHERE 
                    C.CompanyPK = @CompanyID
                AND (E.IsDeleted = 0 OR E.IsDeleted IS NULL);";

            await using var command = new SqlCommand(cmdText, connection);
            command.Parameters.AddWithValue("@CompanyID", companyId.ToString());

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var employeeId = reader.GetGuid(0);
                var salary = reader.GetDecimal(1);
                var startDate = reader.GetDateTime(2);
                var fireDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);
                var contractType = reader.GetString(4);
                var registersHours = reader.GetBoolean(5);
                var PayrollTypeDescription = reader.IsDBNull(6) ? null : reader.GetString(6);

                employeeData.Add(new EmployeePayroll
                {
                    EmpID = employeeId,
                    BruteSalary = salary,
                    StartDate = startDate,
                    FireDate = fireDate,
                    ContractType = contractType,
                    RegistersHours = registersHours,
                    PayrollTypeDescription = PayrollTypeDescription
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

        private async Task SavePayrollAsync(Guid companyId, List<PayrollSummary> summaries, string email)
        {
            var connectionString = _configuration.GetConnectionString("KaizenDb");
            await using var sqlConnection = new SqlConnection(connectionString);
            await sqlConnection.OpenAsync();
            var generalPayrollId = Guid.NewGuid();
            var generalData = BuildGeneralPayrollData(companyId, generalPayrollId, summaries);
            var payrollsTable = await BuildPayrollsTableAsync(generalPayrollId, summaries, email, _configuration);
            var deductionsTable = BuildOptionalDeductionsTable(summaries);

            await using var sqlCommand = new SqlCommand("SaveFullPayroll", sqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            sqlCommand.Parameters.AddWithValue("@GeneralPayrollsID", generalPayrollId);
            sqlCommand.Parameters.AddWithValue("@PaidBy", companyId);
            sqlCommand.Parameters.AddWithValue("@TotalDeductionsBenefits", generalData.TotalDeductionsBenefits);
            sqlCommand.Parameters.AddWithValue("@TotalObligatoryDeductions", generalData.TotalObligatoryDeductions);
            sqlCommand.Parameters.AddWithValue("@TotalLaborCharges", generalData.TotalLaborCharges);
            sqlCommand.Parameters.AddWithValue("@TotalMoneyPaid", generalData.TotalMoneyPaid);
            sqlCommand.Parameters.AddWithValue("@ExecutedOn", DateTime.Now);

            var payrollsParam = sqlCommand.Parameters.AddWithValue("@Payrolls", payrollsTable);
            payrollsParam.SqlDbType = SqlDbType.Structured;
            payrollsParam.TypeName = "dbo.PayrollsType";

            var deductionsParam = sqlCommand.Parameters.AddWithValue("@OptionalDeductions", deductionsTable);
            deductionsParam.SqlDbType = SqlDbType.Structured;
            deductionsParam.TypeName = "dbo.OptionalDeductionsType";

            await sqlCommand.ExecuteNonQueryAsync();
        }

        private static GeneralPayrollData BuildGeneralPayrollData(Guid companyId, Guid generalPayrollId, List<PayrollSummary> summaries)
        {
            var totalOptionalDeductions = summaries.Sum(summary =>
                summary.ApiDeductions.Values.Sum() + summary.BenefitDeductions.Sum(b => b.DeductionValue));

            var totalObligatoryDeductions = summaries.Sum(summary =>
                summary.CCSSDeduction + summary.IncomeTax);

            var totalLaborCharges = summaries.Sum(s => s.GrossSalary) * LaborChargeRate;
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

        private static async Task<DataTable> BuildPayrollsTableAsync(
            Guid generalPayrollId,
            List<PayrollSummary> summaries,
            string email,
            IConfiguration configuration)
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

            var executorPersonPk = await GetPersonPkByEmailAsync(email, configuration);

            foreach (var employeePayroll in summaries)
            {
                AddPayrollRowToTable(generalPayrollId, table, executorPersonPk, employeePayroll);
            }

            return table;
        }

        private static async Task<Guid> GetPersonPkByEmailAsync(string email, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("KaizenDb");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var cmdText = "SELECT PersonPK FROM dbo.Users WHERE Email = @Email";

            await using var command = new SqlCommand(cmdText, connection);
            command.Parameters.AddWithValue("@Email", email);

            var result = await command.ExecuteScalarAsync();

            if (result != null && result != DBNull.Value)
            {
                return (Guid)result;
            }
            else
            {
                throw new InvalidOperationException($"No user found with email: {email}");
            }
        }


        private static void AddPayrollRowToTable(Guid generalPayrollId, DataTable table, Guid executorPersonPk, PayrollSummary employeePayroll)
        {
            employeePayroll.PayrollId = Guid.NewGuid();
            table.Rows.Add(
                employeePayroll.PayrollId,
                employeePayroll.EmployeeId,
                executorPersonPk,
                false, // IsClosed Nani??
                employeePayroll.IncomeTax,
                employeePayroll.CCSSDeduction,
                DBNull.Value,
                generalPayrollId,
                employeePayroll.GrossSalary,
                employeePayroll.NetSalary);
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
                AddOptionalDeductionsToTable(table, payrollSummary);
            }

            return table;
        }

        private static void AddOptionalDeductionsToTable(DataTable table, PayrollSummary payrollSummary)
        {
            foreach (var apiDeduction in payrollSummary.ApiDeductions)
                AddApiDeductionToTable(table, payrollSummary, apiDeduction);

            foreach (var benefitDeduction in payrollSummary.BenefitDeductions)
                AddBenefitDeductionToTable(table, payrollSummary, benefitDeduction);
        }

        private static void AddBenefitDeductionToTable(DataTable table, PayrollSummary payrollSummary, BenefitDeductionResult benefitDeduction)
        {
            table.Rows.Add(Guid.NewGuid(), benefitDeduction.BenefitName, benefitDeduction.DeductionValue, payrollSummary.PayrollId);
        }

        private static void AddApiDeductionToTable(DataTable table, PayrollSummary payrollSummary, KeyValuePair<string, decimal> apiDeduction)
        {
            table.Rows.Add(Guid.NewGuid(), apiDeduction.Key, apiDeduction.Value, payrollSummary.PayrollId);
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

    public class EmployeePayroll
    {
        public Guid EmpID { get; set; }
        public decimal BruteSalary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FireDate { get; set; }
        /// <summary>
        /// Returns "Tiempo Completo", "Medio Tiempo", "Por Horas", "Servicios Profesionales"
        /// </summary>
        public string ContractType { get; set; }
        public bool RegistersHours { get; set; }
        /// <summary>
        /// Returns "Monthly", "Biweekly", "Weekly"
        /// </summary>
        public string PayrollTypeDescription { get; set; }
    }


    public class GeneralPayrollData
    {
        public decimal TotalDeductionsBenefits { get; set; }
        public decimal TotalObligatoryDeductions { get; set; }
        public decimal TotalLaborCharges { get; set; }
        public decimal TotalMoneyPaid { get; set; }
        public DateTime StartDate { get; set; }
    }
}
