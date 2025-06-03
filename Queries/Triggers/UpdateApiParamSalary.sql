CREATE TRIGGER trg_UpdateSalaryInApiParameters
ON Employees
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF UPDATE(BruteSalary)
    BEGIN
        UPDATE eap
        SET eap.ParameterValue = CAST(CAST(i.BruteSalary AS INT) AS VARCHAR(255))
        FROM EmployeeApiParameters eap
        INNER JOIN inserted i ON eap.EmployeeId = i.EmpId
        INNER JOIN deleted d ON i.EmpId = d.EmpId
        WHERE eap.ParameterKey = 'salary'
        AND i.BruteSalary <> d.BruteSalary;
    END
END;