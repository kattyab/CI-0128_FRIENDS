using System.Collections.Generic;
using System.Threading.Tasks;
using Kaizen.Server.Application.Dtos.Reports;
using Kaizen.Server.Application.Interfaces.Reports;

namespace Kaizen.Server.Application.Services.Reports
{
    public class EmployeePayrollListService
    {
        private readonly IEmployeePayrollListRepository _repository;
        public EmployeePayrollListService(IEmployeePayrollListRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EmployeePayrollListDto>> GetAllEmployeePayrollsAsync()
        {
            return await _repository.GetAllEmployeePayrollsAsync();
        }
    }
}
