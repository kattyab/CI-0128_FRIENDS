CREATE OR ALTER TRIGGER trg_OnInsert_CheckIfHasToDeleteBenefit
ON OptionalDeductions
FOR INSERT
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @BenefitId UNIQUEIDENTIFIER

	SELECT @BenefitId = b.Id FROM OptionalDeductions od
	INNER JOIN inserted i ON od.Id = i.Id
	INNER JOIN Payrolls p ON i.PayrollId = p.PayrollID
	INNER JOIN Employees e ON p.PaidTo = e.EmpID
	INNER JOIN Benefits b ON b.OfferedBy = e.WorksFor AND b.Name = i.Name

	IF (
		SELECT IsOut FROM Benefits b
		WHERE b.ID = @BenefitId
		) = 1
	BEGIN
		DELETE FROM ChosenBenefits WHERE BenefitID = @BenefitId
		UPDATE Benefits SET Active = 0 WHERE Benefits.Id = @BenefitId 
	END
END

GO