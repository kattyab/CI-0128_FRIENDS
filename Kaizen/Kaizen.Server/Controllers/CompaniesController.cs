using Kaizen.Server.Repository;
using Kaizen.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController(CompaniesRepository companiesRepository) : ControllerBase
{
    private readonly CompaniesRepository _companiesRepository = companiesRepository;

    [HttpGet("")]
    public IActionResult Index()
    {
        List<CompanyDto> companies = this._companiesRepository.GetCompanies();

        return this.Ok(companies);
    }

    [HttpGet("{companyPK}")]
    public IActionResult Show(Guid companyPK)
    {
        CompanyDto? company = this._companiesRepository.GetCompany(companyPK);

        if (company == null)
        {
            return this.NotFound();
        }

        return this.Ok(company);
    }
}
