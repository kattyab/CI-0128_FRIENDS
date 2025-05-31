CREATE TABLE dbo.OptionalDeductions (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), -- PK
    [Name] NVARCHAR(100) NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    PayrollId UNIQUEIDENTIFIER NOT NULL, -- FK

    CONSTRAINT PK_Deductions PRIMARY KEY (Id),
    CONSTRAINT FK_Deductions_Payroll FOREIGN KEY (PayrollId) 
        REFERENCES dbo.Payrolls(PayrollID)
);

