CREATE TRIGGER tr_OnCompanyDeactivation_SetDatetime
ON Companies
FOR UPDATE
AS
BEGIN
	SET NOCOUNT ON

	UPDATE c
	SET DeletedAt = GETDATE()
	FROM Companies c
	INNER JOIN inserted i ON c.CompanyPK = i.CompanyPK 
	INNER JOIN deleted d ON c.CompanyPK = d.CompanyPK
	WHERE d.IsDeleted = 1 AND i.IsDeleted = 0
END