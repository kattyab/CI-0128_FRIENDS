using Kaizen.Server.Repository;
using Kaizen.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.Controllers;

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
