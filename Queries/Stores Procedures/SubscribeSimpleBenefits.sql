CREATE PROCEDURE sp_InsertChosenBenefit
    @Email VARCHAR(100),
    @BenefitID UNIQUEIDENTIFIER
AS
BEGIN
    INSERT INTO ChosenBenefits (EmployeeID, BenefitID)
    SELECT 
        e.EmpId, 
        @BenefitID
    FROM Employees e
    INNER JOIN Users u ON e.PersonPK = u.PersonPK
    WHERE u.Email = @Email;
END