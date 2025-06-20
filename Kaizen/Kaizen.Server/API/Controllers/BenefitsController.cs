﻿using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Infrastructure.Repositories;
using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Dtos.Auth;
using Kaizen.Server.Application.Interfaces.Services.Auth;

namespace Kaizen.Server.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BenefitsController(IAuthService authService, IBenefitsRepository benefitsRepository) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IBenefitsRepository _benefitsRepository = benefitsRepository;

        [HttpGet("{guid}")]
        public IActionResult Show(Guid guid)
        {
            if (this._authService.IsAuthenticated() == false)
            {
                return this.Unauthorized();
            }

            try
            {
                Guid companyPK = this._authService.GetAuthUserCompanyPK();
                BenefitDto? benefit = this._benefitsRepository.GetBenefit(guid, companyPK);
                if (benefit == null)
                {
                    return this.NotFound();
                }

                return this.Ok(benefit);
            }
            catch (Exception)
            {
                return this.StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("{guid}")]
        public IActionResult Update(BenefitDto benefit)
        {
            if (this._authService.IsAuthenticated() == false)
            {
                return this.Unauthorized();
            }

            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.BadRequest(this.ModelState);
                }

                Guid companyPK = this._authService.GetAuthUserCompanyPK();
                this._benefitsRepository.UpdateBenefit(benefit, companyPK);

                return this.Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
