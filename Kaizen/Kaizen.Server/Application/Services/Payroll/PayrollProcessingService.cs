using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.Payroll;
using Kaizen.Server.Infrastructure.Contexts;


namespace Kaizen.Server.Application.Services.Payroll
{
    public class PayrollProcessingService : IPayrollProcessingService
    {
        private readonly IPayrollSummaryCalculator _payrollCalculator;
        private readonly IEmployeePayrollRepository _employeeRepository;
        private readonly IPayrollRepository _payrollRepository;
        private readonly IPayrollOutputService _outputService;
        private readonly IPayrollCore _payrollLogic;
        private readonly IConfiguration _configuration;

        public PayrollProcessingService(
            IPayrollSummaryCalculator payrollCalculator,
            IEmployeePayrollRepository employeeRepository,
            IPayrollRepository payrollRepository,
            IPayrollOutputService outputService,
            IPayrollCore payrollLogic,
            IConfiguration configuration)
        {
            _payrollCalculator = payrollCalculator;
            _employeeRepository = employeeRepository;
            _payrollRepository = payrollRepository;
            _outputService = outputService;
            _payrollLogic = payrollLogic;
            _configuration = configuration;
        }

        public async Task<PayrollResultSumary> ProcessCompanyPayrollAsync(PayrollRequest payrollInformation)
        {
            var connectionString = _configuration.GetConnectionString("KaizenDb");
            using var transactionContext = new PayrollTransactionContext(connectionString);

            try
            {
                await transactionContext.OpenAsync();

                var payrollResults = await CalculateCompanyPayrollAsync(payrollInformation, transactionContext);
                var result = _payrollLogic.CreatePayrollResult(payrollInformation, payrollResults);

                if (!result.IsSuccess)
                {
                    await transactionContext.RollbackAsync();
                    return result;
                }

                await _payrollRepository.SavePayrollAsync(payrollInformation.CompanyId, payrollResults, payrollInformation.Email, transactionContext);

                await transactionContext.CommitAsync();

#if DEBUG
                _outputService.PrintPayrollResults(payrollResults);
#endif
                return result;
            }
            catch (Exception ex)
            {
                await transactionContext.RollbackAsync();
                throw;
            }
        }

        public async Task<List<PayrollSummary>> CalculateCompanyPayrollAsync(PayrollRequest payrollInformation, PayrollTransactionContext context = null)
        {
            var employeeData = await _employeeRepository.GetEmployeeDataAsync(payrollInformation, context);
            var payrollResults = new List<PayrollSummary>();

            foreach (var employee in employeeData)
            {
                var payrollSummary = await _payrollCalculator.CalculatePayrollAsync(employee, payrollInformation, context);
                payrollResults.Add(payrollSummary);
            }

            return payrollResults;
        }
    }
}