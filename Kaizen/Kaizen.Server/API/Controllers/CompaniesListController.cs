using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Repositories;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesListController(CompaniesListRepository companiesListRepository) : ControllerBase
{
    private readonly CompaniesListRepository _companiesListRepository = companiesListRepository;
    // GET: api/CompaniesList
    // This method handles HTTP GET requests to retrieve a list of companies
    [HttpGet("")]
    public IActionResult Index()

    {   // Fetch the list of companies from the repository
        List<CompaniesListDto> companies = this._companiesListRepository.GetCompaniesList();

        return this.Ok(companies);
    }
}
