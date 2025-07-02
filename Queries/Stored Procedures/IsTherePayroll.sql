CREATE PROCEDURE sp_IsTherePayroll
	@CompanyPK UNIQUEIDENTIFIER,
	@IsTherePayroll BIT OUTPUT
AS
BEGIN
	 SET NOCOUNT ON

	 SET @IsTherePayroll = 0

	 IF EXISTS(
		SELECT 1 FROM Payrolls p
		INNER JOIN Employees e ON p.PaidTo = e.EmpID
		INNER JOIN Companies c ON e.WorksFor = c.CompanyPK
		WHERE c.CompanyPK = @CompanyPK
		)
	BEGIN
		SET @IsTherePayroll = 1
	END
END