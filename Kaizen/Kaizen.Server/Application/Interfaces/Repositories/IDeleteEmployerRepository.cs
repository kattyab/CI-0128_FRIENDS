using System;
using System.Collections.Generic;
using Kaizen.Server.Application.Dtos.Employers;

namespace Kaizen.Server.Application.Interfaces.Repositories
{
    public interface IDeleteEmployerRepository
    {

        IEnumerable<DeleteEmployerDto> GetEmployersWithCompanyAndPersonData();


        bool SoftDeleteEmployer(Guid ownerPK);

        bool HardDeleteEmployer(Guid ownerPK);
    }
}
