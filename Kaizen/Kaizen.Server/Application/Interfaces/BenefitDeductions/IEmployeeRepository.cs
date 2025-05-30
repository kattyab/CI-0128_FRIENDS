using Kaizen.Server.Application.Dtos.BenefitDeductions;

namespace Kaizen.Server.Application.Interfaces.BenefitDeductions
{
    public interface IEmployeeRepository
    {
        Employee GetById(Guid employeeID);
        List<Guid> GetChosenBenefitIDs(Guid employeeID);
    }
}
