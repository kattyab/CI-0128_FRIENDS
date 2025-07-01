using Kaizen.Server.Application.Dtos;

namespace Kaizen.Server.Application.Interfaces.Repositories
{
    public interface IGeneralPayrollReportRepository
    {
        List<GeneralPayrollReportDto> GetAllReports();
    }
}
