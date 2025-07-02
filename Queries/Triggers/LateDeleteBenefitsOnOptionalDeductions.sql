CREATE TRIGGER trg_OnInsert_CheckIfHasToDeleteBenefit
ON OptionalDeductions
FOR INSERT
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @BenefitId UNIQUEIDENTIFIER

	SELECT @BenefitId = BenefitId FROM inserted

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