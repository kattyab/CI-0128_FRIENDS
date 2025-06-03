using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.Payroll;
using System.Data;

namespace Kaizen.Server.Infrastructure.Helpers.Payroll
{
    public class PayrollDataTransformer : IPayrollDataTransformer
    {
        public DataTable BuildPayrollsTable(Guid generalPayrollId, List<PayrollSummary> summaries, Guid executorPersonPk)
        {
            var table = CreatePayrollsTableStructure();

            foreach (var employeePayroll in summaries)
            {
                AddPayrollRowToTable(generalPayrollId, table, executorPersonPk, employeePayroll);
            }

            return table;
        }

        public DataTable BuildOptionalDeductionsTable(List<PayrollSummary> summaries)
        {
            var table = CreateOptionalDeductionsTableStructure();

            foreach (var payrollSummary in summaries)
            {
                AddOptionalDeductionsToTable(table, payrollSummary);
            }

            return table;
        }

        public GeneralPayrollData BuildGeneralPayrollData(Guid companyId, Guid generalPayrollId, List<PayrollSummary> summaries, decimal laborChargeRate)
        {
            var totalOptionalDeductions = summaries.Sum(summary =>
                summary.ApiDeductions.Values.Sum() + summary.BenefitDeductions.Sum(b => b.DeductionValue));

            var totalObligatoryDeductions = summaries.Sum(summary =>
                summary.CCSSDeduction + summary.IncomeTax);

            var totalLaborCharges = summaries.Sum(s => s.GrossSalary) * laborChargeRate;
            var totalMoneyPaid = totalLaborCharges + summaries.Sum(s => s.GrossSalary);

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
            var startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            return new GeneralPayrollData
            {
                TotalDeductionsBenefits = totalOptionalDeductions,
                TotalObligatoryDeductions = totalObligatoryDeductions,
                TotalLaborCharges = totalLaborCharges,
                TotalMoneyPaid = totalMoneyPaid,
                StartDate = startDate
            };
        }

        private static DataTable CreatePayrollsTableStructure()
        {
            var table = new DataTable();
            table.Columns.Add("PayrollID", typeof(Guid));
            table.Columns.Add("PaidTo", typeof(Guid));
            table.Columns.Add("ExecutedBy", typeof(Guid));
            table.Columns.Add("IsClosed", typeof(bool));
            table.Columns.Add("IncomeTax", typeof(decimal));
            table.Columns.Add("CCSS", typeof(decimal));
            table.Columns.Add("ApprovalID", typeof(Guid));
            table.Columns.Add("GeneralPayrollPk", typeof(Guid));
            table.Columns.Add("BrutePaid", typeof(decimal));
            table.Columns.Add("NetPaid", typeof(decimal));
            return table;
        }

        private static DataTable CreateOptionalDeductionsTableStructure()
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(Guid));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Amount", typeof(decimal));
            table.Columns.Add("PayrollId", typeof(Guid));
            return table;
        }

        private static void AddPayrollRowToTable(Guid generalPayrollId, DataTable table, Guid executorPersonPk, PayrollSummary employeePayroll)
        {
            employeePayroll.PayrollId = Guid.NewGuid();
            table.Rows.Add(
                employeePayroll.PayrollId,
                employeePayroll.EmployeeId,
                executorPersonPk,
                false,
                employeePayroll.IncomeTax,
                employeePayroll.CCSSDeduction,
                DBNull.Value,
                generalPayrollId,
                employeePayroll.GrossSalary,
                employeePayroll.NetSalary);
        }

        private static void AddOptionalDeductionsToTable(DataTable table, PayrollSummary payrollSummary)
        {
            foreach (var apiDeduction in payrollSummary.ApiDeductions)
                AddApiDeductionToTable(table, payrollSummary, apiDeduction);

            foreach (var benefitDeduction in payrollSummary.BenefitDeductions)
                AddBenefitDeductionToTable(table, payrollSummary, benefitDeduction);
        }

        private static void AddBenefitDeductionToTable(DataTable table, PayrollSummary payrollSummary, BenefitDeductionResult benefitDeduction)
        {
            table.Rows.Add(Guid.NewGuid(), benefitDeduction.BenefitName, benefitDeduction.DeductionValue, payrollSummary.PayrollId);
        }

        private static void AddApiDeductionToTable(DataTable table, PayrollSummary payrollSummary, KeyValuePair<string, decimal> apiDeduction)
        {
            table.Rows.Add(Guid.NewGuid(), apiDeduction.Key, apiDeduction.Value, payrollSummary.PayrollId);
        }
    }
}
