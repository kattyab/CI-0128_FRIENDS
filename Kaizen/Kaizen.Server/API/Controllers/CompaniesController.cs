using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Repositories;

namespace Kaizen.Server.API.Controllers;

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
