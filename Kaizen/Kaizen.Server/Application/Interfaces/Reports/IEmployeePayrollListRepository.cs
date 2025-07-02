using Kaizen.Server.Application.Dtos.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaizen.Server.Application.Interfaces.Reports
{
    public interface IEmployeePayrollListRepository
    {
        Task<List<EmployeePayrollListDto>> GetAllEmployeePayrollsAsync();
    }
}
