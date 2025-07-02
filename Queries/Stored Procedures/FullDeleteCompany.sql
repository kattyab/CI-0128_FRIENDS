
CREATE PROCEDURE sp_FullDeleteCompany
	@CompanyPK UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON 
	
	DELETE ah
	FROM ApprovedHours ah
	INNER JOIN Employees e ON ah.EmpID = e.EmpID
	INNER JOIN Companies c ON e.WorksFor = c.CompanyPK
	WHERE c.CompanyPK = @CompanyPK

	DELETE FROM Benefits
	WHERE OfferedBy = @CompanyPK

	DELETE FROM OffersAPIs
	WHERE CompanyPK = @CompanyPK


	DELETE eap FROM EmployeeApiParameters eap
	INNER JOIN Employees e ON eap.EmployeeId = e.EmpID
	INNER JOIN Companies c ON e.WorksFor = c.CompanyPK
	WHERE c.CompanyPK = @CompanyPK

	DELETE FROM Employees
	WHERE WorksFor = @CompanyPK

	DELETE FROM Oversees
	WHERE CompanyPK = @CompanyPK

	DELETE FROM Admins
	WHERE CompanyPK = @CompanyPK

	DELETE FROM CompaniesPhoneNumbers
	WHERE CompanyPK = @CompanyPK

	DELETE FROM CompaniesEmails
	WHERE CompanyPK = @CompanyPK

	DELETE FROM Admins
	WHERE CompanyPK = @CompanyPK

	DELETE FROM CompanyAuditData
	WHERE CompanyPK = @CompanyPK
			
	DELETE n FROM Notifications n
	INNER JOIN Users u on n.UserPK = u.UserPK
	WHERE u.CompanyPK = @CompanyPK

	DELETE FROM Users
	WHERE CompanyPK = @CompanyPK
	AND Role != 'Dueño'

	DELETE FROM Companies
	WHERE CompanyPK = @CompanyPK 
END