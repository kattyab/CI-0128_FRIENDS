using System;
using Kaizen.Server.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeleteEmployerController(IDeleteEmployerRepository repository) : ControllerBase
    {
        private readonly IDeleteEmployerRepository _repository = repository;

        // GET: api/DeleteEmployer
        [HttpGet]
        public IActionResult GetEmployers()
        {
            var result = _repository.GetEmployersWithCompanyAndPersonData();
            return Ok(result);
        }

        // PUT: api/DeleteEmployer/{ownerPK}
        [HttpPut("{ownerPK:guid}")]
        public IActionResult SoftDeleteEmployer(Guid ownerPK)
        {
            var success = _repository.SoftDeleteEmployer(ownerPK);

            if (!success)
                return NotFound(new { message = "Empleador no encontrado o no se pudo eliminar." });

            return Ok(new { message = "Soft delete realizado con éxito." });
        }

        // DELETE: api/DeleteEmployer/{ownerPK}
        [HttpDelete("{ownerPK:guid}")]
        public IActionResult HardDeleteEmployer(Guid ownerPK)
        {
            var success = _repository.HardDeleteEmployer(ownerPK);

            if (!success)
                return NotFound(new { message = "Empleador no encontrado o no se pudo eliminar completamente." });

            return Ok(new { message = "Eliminación permanente realizada con éxito." });
        }
    }
}
