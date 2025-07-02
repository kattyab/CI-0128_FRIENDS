CREATE PROCEDURE sp_SoftDeleteCompany
	@CompanyPK UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;
		UPDATE Companies
		SET IsDeleted = 1
		WHERE CompanyPK = @CompanyPK

		UPDATE Employees
		SET IsDeleted = 1
		WHERE WorksFor = @CompanyPK

		UPDATE Users
		SET Active = 0
		WHERE CompanyPK = @CompanyPK
END