CREATE OR ALTER PROCEDURE SaveFullPayroll
    @GeneralPayrollsID UNIQUEIDENTIFIER,
    @PaidBy UNIQUEIDENTIFIER,
    @TotalDeductionsBenefits DECIMAL(18,2),
    @TotalObligatoryDeductions DECIMAL(18,2),
    @TotalLaborCharges DECIMAL(18,2),
    @TotalMoneyPaid DECIMAL(18,2),
    @ExecutedOn DATETIME,
    @Payrolls dbo.PayrollsType READONLY,
    @OptionalDeductions dbo.OptionalDeductionsType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO GeneralPayrolls (
            GeneralPayrollsID, PaidBy, TotalDeductionsBenefits,
            TotalObligatoryDeductions, TotalLaborCharges, TotalMoneyPaid, ExecutedOn
        )
        VALUES (
            @GeneralPayrollsID, @PaidBy, @TotalDeductionsBenefits,
            @TotalObligatoryDeductions, @TotalLaborCharges, @TotalMoneyPaid, @ExecutedOn
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
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;