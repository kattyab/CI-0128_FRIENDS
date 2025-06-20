﻿using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Queries.Benefits;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BenefitEmployeeListController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BenefitEmployeeListController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<List<BenefitEmployeeListDto>>> GetEmployeeBenefitsByEmail(string email)
        {
            try
            {
                var query = new BenefitEmployeeListQuery(email);
                var benefits = await _mediator.Send(query);

                return Ok(benefits);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving benefits: {ex.Message}");
            }
        }
    }
}
