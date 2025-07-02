using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Dtos.Reports;
using Kaizen.Server.Application.Interfaces.Payroll;
using Kaizen.Server.Infrastructure.Contexts;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Infrastructure.Repositories.Payroll
{
    public class PayrollRepository : IPayrollRepository
    {
        private readonly string _connectionString;

        private readonly IConfiguration _configuration;
        private readonly IPayrollDataTransformer _dataTransformer;
        private readonly IEmployeePayrollRepository _employeeRepository;

        public PayrollRepository(
            IConfiguration configuration,
            IPayrollDataTransformer dataTransformer,
            IEmployeePayrollRepository employeeRepository)
        {
            this._connectionString = configuration.GetConnectionString("KaizenDb")
                ?? throw new InvalidOperationException(
                    "La cadena de conexion 'KaizenDb' no está definida en appsettings.json");

            _configuration = configuration;
            _dataTransformer = dataTransformer;
            _employeeRepository = employeeRepository;
        }

        public async Task SavePayrollAsync(Guid companyId, List<PayrollSummary> summaries, string email, PayrollTransactionContext context = null)
        {
            if (context != null)
            {
                await SavePayrollWithTransactionAsync(context, companyId, summaries, email);
            }
            else
            {
                await SavePayrollWithoutTransactionAsync(companyId, summaries, email);
            }
        }

        private async Task SavePayrollWithTransactionAsync(PayrollTransactionContext context, Guid companyId, List<PayrollSummary> summaries, string email)
        {
            var generalPayrollId = Guid.NewGuid();

            var executorPersonPk = await _employeeRepository.GetPersonPkByEmailAsync(email, context);

            var generalData = _dataTransformer.BuildGeneralPayrollData(companyId, generalPayrollId, summaries, 0.2667m);
            var payrollsTable = _dataTransformer.BuildPayrollsTable(generalPayrollId, summaries, executorPersonPk);
            var deductionsTable = _dataTransformer.BuildOptionalDeductionsTable(summaries);

            await ExecuteSavePayrollCommand(
                context.Connection,
                context.Transaction,
                generalPayrollId,
                companyId,
                generalData,
                payrollsTable,
                deductionsTable);
        }

        private async Task SavePayrollWithoutTransactionAsync(Guid companyId, List<PayrollSummary> summaries, string email)
        {
            var connectionString = _configuration.GetConnectionString("KaizenDb");
            await using var sqlConnection = new SqlConnection(connectionString);
            await sqlConnection.OpenAsync();

            var generalPayrollId = Guid.NewGuid();
            var executorPersonPk = await _employeeRepository.GetPersonPkByEmailAsync(email);
            var generalData = _dataTransformer.BuildGeneralPayrollData(companyId, generalPayrollId, summaries, 0.2667m);
            var payrollsTable = _dataTransformer.BuildPayrollsTable(generalPayrollId, summaries, executorPersonPk);
            var deductionsTable = _dataTransformer.BuildOptionalDeductionsTable(summaries);

            await ExecuteSavePayrollCommand(sqlConnection, null, generalPayrollId, companyId, generalData, payrollsTable, deductionsTable);
        }

        private static async Task ExecuteSavePayrollCommand(
            SqlConnection connection,
            SqlTransaction transaction,
            Guid generalPayrollId,
            Guid companyId,
            GeneralPayrollData generalData,
            DataTable payrollsTable,
            DataTable deductionsTable)
        {
            await using var sqlCommand = new SqlCommand("SaveFullPayroll", connection, transaction)
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

            var payrollsParameters = sqlCommand.Parameters.AddWithValue("@Payrolls", payrollsTable);
            payrollsParameters.SqlDbType = SqlDbType.Structured;
            payrollsParameters.TypeName = "dbo.PayrollsType";

            var deductionsParameters = sqlCommand.Parameters.AddWithValue("@OptionalDeductions", deductionsTable);
            deductionsParameters.SqlDbType = SqlDbType.Structured;
            deductionsParameters.TypeName = "dbo.OptionalDeductionsType";

            await sqlCommand.ExecuteNonQueryAsync();
        }

        public List<HistoricRangePayroll> GetEmployeeHistoricRangePayrolls(Guid employeeId, DateTime start, DateTime end)
        {
            const string getEmployeeHistoricRangePayrollsCommandText = @"
                SELECT
                    e.ContractType,
                    e.JobPosition,
                    gp.ExecutedOn AS PayrollDate,
                    p.BrutePaid AS BruteSalary,
                    p.NetPaid AS NetSalary,
                    (p.IncomeTax + p.CCSS) AS ObligatoryDeductions,
                    ISNULL(SUM(od.Amount), 0) AS OptionalDeductions
                FROM
                    Payrolls p
                INNER JOIN
                    Employees e ON p.PaidTo = e.EmpID
                INNER JOIN
                    GeneralPayrolls gp ON p.GeneralPayrollPk = gp.GeneralPayrollsID
                LEFT JOIN
                    OptionalDeductions od ON p.PayrollID = od.PayrollID
                WHERE
                    p.PaidTo = @EmployeeId
                    AND CAST(gp.ExecutedOn AS DATE) BETWEEN @Start AND @End
                GROUP BY
                    e.ContractType, e.JobPosition, gp.ExecutedOn, p.PayrollID, p.BrutePaid, p.NetPaid, p.IncomeTax, p.CCSS
                ORDER BY gp.ExecutedOn DESC
            ";

            SqlParameter[] getEmployeeHistoricRangePayrollsParameters = [
                new SqlParameter("@EmployeeId", employeeId),
                new SqlParameter("@Start", start),
                new SqlParameter("@End", end)
            ];

            using SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString,
                getEmployeeHistoricRangePayrollsCommandText,
                CommandType.Text,
                getEmployeeHistoricRangePayrollsParameters);

            List<HistoricRangePayroll> historicPayrolls = [];
            while (reader.Read())
            {
                HistoricRangePayroll historicPayroll = new()
                {
                    ContractType = reader.GetString(reader.GetOrdinal("ContractType")),
                    JobPosition = reader.GetString(reader.GetOrdinal("JobPosition")),
                    PayrollDate = reader.GetDateTime(reader.GetOrdinal("PayrollDate")),
                    BruteSalary = reader.GetDecimal(reader.GetOrdinal("BruteSalary")),
                    NetSalary = reader.GetDecimal(reader.GetOrdinal("NetSalary")),
                    ObligatoryDeductions = reader.GetDecimal(reader.GetOrdinal("ObligatoryDeductions")),
                    OptionalDeductions = reader.IsDBNull(reader.GetOrdinal("OptionalDeductions"))
                        ? 0
                        : reader.GetDecimal(reader.GetOrdinal("OptionalDeductions"))
                };

                historicPayrolls.Add(historicPayroll);
            }
            return historicPayrolls;
        }
    }
}