using Kaizen.Server.Application.Dtos.Reports;
using Kaizen.Server.Application.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers.Reports
{
    [ApiController]
    [Route("api/reports/employee-total")]
    [Authorize(Roles = "Dueño")]
    public class EmployeePayrollListController : ControllerBase
    {
        private readonly EmployeePayrollListService _service;
        public EmployeePayrollListController(EmployeePayrollListService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllEmployeePayrollsAsync();
            return Ok(result);
        }
    }
}
