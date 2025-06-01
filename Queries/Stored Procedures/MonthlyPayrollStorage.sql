CREATE TYPE dbo.PayrollsType AS TABLE
(
    PayrollID UNIQUEIDENTIFIER,
    PaidTo UNIQUEIDENTIFIER,
    ExecutedByPersonPK UNIQUEIDENTIFIER,
    IsClosed BIT,
    IncomeTax DECIMAL(18,2),
    CCSS DECIMAL(18,2),
    ApprovalID UNIQUEIDENTIFIER NULL,
    GeneralPayrollPk UNIQUEIDENTIFIER,
    BrutePaid DECIMAL(18,2),
    NetPaid DECIMAL(18,2)
);
GO

CREATE TYPE dbo.OptionalDeductionsType AS TABLE
(
    Id UNIQUEIDENTIFIER,
    Name NVARCHAR(255),
    Amount DECIMAL(18,2),
    PayrollId UNIQUEIDENTIFIER
);
GO

CREATE PROCEDURE SaveFullPayroll
    @GeneralPayrollsID UNIQUEIDENTIFIER,
    @PaidBy UNIQUEIDENTIFIER,
    @TotalDeductionsBenefits DECIMAL(18,2),
    @TotalObligatoryDeductions DECIMAL(18,2),
    @TotalLaborCharges DECIMAL(18,2),
    @TotalMoneyPaid DECIMAL(18,2),
    @StartDate DATETIME,
    @Payrolls dbo.PayrollsType READONLY,
    @OptionalDeductions dbo.OptionalDeductionsType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO GeneralPayrolls (
            GeneralPayrollsID, PaidBy, TotalDeductionsBenefits,
            TotalObligatoryDeductions, TotalLaborCharges, TotalMoneyPaid, StartDate
        )
        VALUES (
            @GeneralPayrollsID, @PaidBy, @TotalDeductionsBenefits,
            @TotalObligatoryDeductions, @TotalLaborCharges, @TotalMoneyPaid, @StartDate
        );

        INSERT INTO Payrolls (
            PayrollID, PaidTo, ExecutedBy, IsClosed,
            IncomeTax, CCSS, ApprovalID, GeneralPayrollPk,
            BrutePaid, NetPaid
        )
        SELECT 
            PayrollID, PaidTo, ExecutedByPersonPK, IsClosed,
            IncomeTax, CCSS, ApprovalID, GeneralPayrollPk,
            BrutePaid, NetPaid
        FROM @Payrolls;

        INSERT INTO OptionalDeductions (
            Id, Name, Amount, PayrollId
        )
        SELECT *
        FROM @OptionalDeductions;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
