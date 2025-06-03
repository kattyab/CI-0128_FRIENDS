using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.Payroll;


namespace Kaizen.Server.Application.Services.Payroll
{
    public class PayrollProcessingService : IPayrollProcessingService
    {
        private readonly IPayrollSummaryCalculator _payrollCalculator;
        private readonly IEmployeePayrollRepository _employeeRepository;
        private readonly IPayrollRepository _payrollRepository;
        private readonly IPayrollOutputService _outputService;
        private readonly IPayrollCore _payrollLogic;

        public PayrollProcessingService(
            IPayrollSummaryCalculator payrollCalculator,
            IEmployeePayrollRepository employeeRepository,
            IPayrollRepository payrollRepository,
            IPayrollOutputService outputService,
            IPayrollCore payrollLogic)
        {
            _payrollCalculator = payrollCalculator;
            _employeeRepository = employeeRepository;
            _payrollRepository = payrollRepository;
            _outputService = outputService;
            _payrollLogic = payrollLogic;
        }

        public async Task<PayrollResultSumary> ProcessCompanyPayrollAsync(PayrollRequest payrollInformation)
        {
            var payrollResults = await CalculateCompanyPayrollAsync(payrollInformation);
            var result = _payrollLogic.CreatePayrollResult(payrollInformation, payrollResults);

            if (!result.IsSuccess)
            {
                return result;
            }

            await _payrollRepository.SavePayrollAsync(payrollInformation.CompanyId, payrollResults, payrollInformation.Email);
#if DEBUG
            _outputService.PrintPayrollResults(payrollResults);
#else
#endif

            return result;
        }

        public async Task<List<PayrollSummary>> CalculateCompanyPayrollAsync(PayrollRequest payrollInformation)
        {
            var employeeData = await _employeeRepository.GetEmployeeDataAsync(payrollInformation);
            var payrollResults = new List<PayrollSummary>();

            foreach (var employee in employeeData)
            {
                var payrollSummary = await _payrollCalculator.CalculatePayrollAsync(employee, payrollInformation);
                payrollResults.Add(payrollSummary);
            }

            return payrollResults;
        }
    }
}