using Kaizen.Server.Application.Dtos.Reports;

namespace Kaizen.Server.Application.Interfaces.Services;

public interface IReportService
{
    HistoricRangeInitialDto GetHistoricRangeInitialDataAsync(Guid companyId);
    Task<List<HistoricRangePayroll>> GetHistoricRangePayroll(Guid companyPK, HistoricRangeSearch historicRangeSearch);
    Task SendHistoricRangeEmail(Guid companyPK, HistoricRangeSearch historicRangeSearch);
    Task SendCompaniesHistoricEmail(string csvContent);

}
