using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Dtos.Companies;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Kaizen.Server.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController(
    IAuthService authService,
    CompaniesRepository companiesRepository) : ControllerBase
{
    private readonly IAuthService _authService = authService;
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

    [HttpPost("{companyPK}")]
    public IActionResult EditCompany(Guid companyPK, CompanyEditDto companyEditDto)
    {
        try
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.BadRequest(this.ModelState);
                }

                this._companiesRepository.UpdateCompany(companyPK, companyEditDto);

                return this.Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(500, "An error occurred while processing your request.");
            }
        }
        catch (Exception)
        {
            // Ignore, return not found if any error occurs
        }

        return this.NotFound();
    }

    [HttpPost("user")]
    public IActionResult EditUserCompany(CompanyEditDto companyEditDto)
    {
        try
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.BadRequest(this.ModelState);
                }

                Guid companyPK = this._authService.GetAuthUserCompanyPK();
                this._companiesRepository.UpdateCompany(companyPK, companyEditDto);

                return this.Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(500, "An error occurred while processing your request.");
            }
        }
        catch (Exception)
        {
            // Ignore, return not found if any error occurs
        }

        return this.NotFound();
    }
}
