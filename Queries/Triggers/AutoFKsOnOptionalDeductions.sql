CREATE TRIGGER trg_ForInsert_OptionalDeductions_AddFKs
ON OptionalDeductions
FOR INSERT
AS
BEGIN
	SET NOCOUNT ON

	UPDATE od
	SET BenefitId = b.Id
	FROM OptionalDeductions od
	INNER JOIN inserted i ON od.Id = i.Id
	INNER JOIN Payrolls p ON i.PayrollId = p.PayrollID
	INNER JOIN Employees e ON p.PaidTo = e.EmpID
	INNER JOIN Benefits b ON b.OfferedBy = e.WorksFor AND b.Name = i.Name

	UPDATE od
    SET APIBenefitId = adc.Id
    FROM OptionalDeductions od
    INNER JOIN inserted i ON od.Id = i.Id
    INNER JOIN ApiDeductionConfigs adc ON adc.Name = i.Name
    WHERE od.BenefitId IS NULL;
END
GO