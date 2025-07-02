ALTER PROCEDURE SaveFullPayroll
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
        BEGIN TRANSACTION;
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
        SELECT Id, Name, Amount, PayrollId
        FROM @OptionalDeductions;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

ALTER TABLE OptionalDeductions
ADD
	BenefitId UNIQUEIDENTIFIER NULL,
	APIBenefitId INT NULL

ALTER TABLE OptionalDeductions
ADD CONSTRAINT FK_BenefitId_Benefits
FOREIGN KEY (BenefitId) REFERENCES Benefits(ID)
ON DELETE SET NULL

ALTER TABLE OptionalDeductions
ADD CONSTRAINT FK_APIBenefitId_ApiDeductionConfigs
FOREIGN KEY (APIBenefitId) REFERENCES ApiDeductionConfigs(Id)
ON DELETE SET NULL

GO

ALTER TABLE Benefits
ADD Active BIT NOT NULL DEFAULT 1

ALTER TABLE Benefits
ADD IsOut BIT NOT NULL DEFAULT 0

GO
