using System;
using Kaizen.Server.Application.Dtos;

namespace Kaizen.Server.Application.Interfaces.Repositories;

public interface IEmployeeDashboardRepository
{
    EmployeeDashboardDto GetDashboardByUserPK(Guid userPK);
}
