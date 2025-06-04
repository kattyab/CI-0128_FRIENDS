using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.Payroll;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Infrastructure.Repositories.Payroll
{
    public class PayrollRepository : IPayrollRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IPayrollDataTransformer _dataTransformer;
        private readonly IEmployeePayrollRepository _employeeRepository;

        public PayrollRepository(
            IConfiguration configuration,
            IPayrollDataTransformer dataTransformer,
            IEmployeePayrollRepository employeeRepository)
        {
            _configuration = configuration;
            _dataTransformer = dataTransformer;
            _employeeRepository = employeeRepository;
        }

        public async Task SavePayrollAsync(Guid companyId, List<PayrollSummary> summaries, string email)
        {
            var connectionString = _configuration.GetConnectionString("KaizenDb");
            await using var sqlConnection = new SqlConnection(connectionString);
            await sqlConnection.OpenAsync();

            var generalPayrollId = Guid.NewGuid();
            var executorPersonPk = await _employeeRepository.GetPersonPkByEmailAsync(email);

            var generalData = _dataTransformer.BuildGeneralPayrollData(companyId, generalPayrollId, summaries, 0.2667m);
            var payrollsTable = _dataTransformer.BuildPayrollsTable(generalPayrollId, summaries, executorPersonPk);
            var deductionsTable = _dataTransformer.BuildOptionalDeductionsTable(summaries);

            await ExecuteSavePayrollCommand(sqlConnection, generalPayrollId, companyId, generalData, payrollsTable, deductionsTable);
        }

        private static async Task ExecuteSavePayrollCommand(
            SqlConnection connection,
            Guid generalPayrollId,
            Guid companyId,
            GeneralPayrollData generalData,
            DataTable payrollsTable,
            DataTable deductionsTable)
        {
            await using var sqlCommand = new SqlCommand("SaveFullPayroll", connection)
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
    }
}
