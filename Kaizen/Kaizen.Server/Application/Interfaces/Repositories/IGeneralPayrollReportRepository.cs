using Kaizen.Server.Application.Dtos;

namespace Kaizen.Server.Application.Interfaces.Repositories
{
    public interface IGeneralPayrollReportRepository
    {
        List<GeneralPayrollReportDto> GetAllReports();
        List<GeneralPayrollReportDto> GetCompanyReport(Guid companyPK);
    }
}
