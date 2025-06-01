CREATE TABLE dbo.OptionalDeductions (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), -- PK
    [Name] NVARCHAR(100) NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    PayrollId UNIQUEIDENTIFIER NOT NULL, -- FK

    CONSTRAINT PK_Deductions PRIMARY KEY (Id),
    CONSTRAINT FK_Deductions_Payroll FOREIGN KEY (PayrollId) 
        REFERENCES dbo.Payrolls(PayrollID)
);

Alter table Payrolls
drop column SolidarityAssociationFund;

Alter table Payrolls
drop column LifeInsurancePlan;


CREATE TABLE dbo.GeneralPayrolls (
    GeneralPayrollsID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    PaidBy UNIQUEIDENTIFIER NOT NULL,
    TotalDeductionsBenefits DECIMAL(19,4),
    TotalObligatoryDeductions DECIMAL(19,4),
    TotalLaborCharges DECIMAL(19,4),
    TotalMoneyPaid DECIMAL(19,4),
    StartDate DATE
);

ALTER TABLE dbo.Payrolls
ADD GeneralPayrollPk UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();

ALTER TABLE dbo.Payrolls
ADD CONSTRAINT FK_Payrolls_GeneralPayrolls
FOREIGN KEY (GeneralPayrollPk)
REFERENCES dbo.GeneralPayrolls(GeneralPayrollsID);