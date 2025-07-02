using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Interfaces.Repositories;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneralPayrollReportController(IGeneralPayrollReportRepository repository) : ControllerBase
{
    private readonly IGeneralPayrollReportRepository _repository = repository;

    [HttpGet]
    public IActionResult GetAllReports()
    {
        var reports = _repository.GetAllReports();
        return Ok(reports);
    }

    [HttpGet("company/{companyPK:guid}")]
    public IActionResult GetCompanyReport(Guid companyPK)
    {
        var reports = _repository.GetCompanyReport(companyPK);
        return Ok(reports);
    }
}

