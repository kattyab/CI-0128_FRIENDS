using Kaizen.Server.Application.Dtos.Payroll;
using System.Data;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface IPayrollDataTransformer
    {
        DataTable BuildPayrollsTable(Guid generalPayrollId, List<PayrollSummary> summaries, Guid executorPersonPk);
        DataTable BuildOptionalDeductionsTable(List<PayrollSummary> summaries);
        GeneralPayrollData BuildGeneralPayrollData(Guid companyId, Guid generalPayrollId, List<PayrollSummary> summaries, decimal laborChargeRate);
    }
}
