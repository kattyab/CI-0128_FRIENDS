-- EmployeePayrollRepository GetEmployeeDataAsync
CREATE NONCLUSTERED INDEX IX_Employees_WorksFor_IsDeleted_RegistersHours
ON Employees (WorksFor, IsDeleted, RegistersHours);

--ApiBenefitDeductionRepository GetParametersForCompanyAsync
CREATE INDEX IX_Test ON Employees (WorksFor, IsDeleted);

-- ReportsRepository GetPayrollReportsByCompanyAsync
CREATE NONCLUSTERED INDEX IX_Payrolls_GeneralPayrollPk_PaidTo
ON Payrolls (GeneralPayrollPk, PaidTo);
