DROP TRIGGER trg_OnUpdateChosenBenefits_CreateParameters

GO

CREATE TRIGGER trg_OnUpdateChosenBenefits_CreateParameters
ON ChosenAPIs
FOR INSERT
AS 
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO EmployeeApiParameters (EmployeeID, ApiConfigID, parameterKey, parameterValue)
    SELECT 
        i.EmployeePK, 
        i.ApiId,
        param.parameterKey,
        CASE 
            WHEN param.parameterKey = 'dob' THEN CONVERT(VARCHAR(10), p.Birthdate, 120)
            WHEN param.parameterKey = 'sex' THEN p.Sex
            WHEN param.parameterKey = 'sexo' THEN p.Sex
            WHEN param.parameterKey = 'salary' THEN CONVERT(VARCHAR(50), CAST(ROUND(e.brutesalary, 0) AS INT))
        END AS parameterValue
    FROM 
        inserted i
    INNER JOIN 
        Employees e ON i.EmployeePK = e.EmpId
    INNER JOIN 
        Persons p ON e.PersonPK = p.PersonPK
    INNER JOIN (
        VALUES 
            (1, 'dob'),
            (1, 'sex'),
            (2, 'salary'),
            (3, 'dob'),
            (3, 'sexo')
    ) param(ApiId, parameterKey) ON i.ApiId = param.ApiId
    WHERE NOT EXISTS (
        SELECT 1 FROM EmployeeApiParameters eap
        WHERE eap.EmployeeID = i.EmployeePK
          AND eap.ApiConfigID = i.ApiId
          AND eap.parameterKey = param.parameterKey
    );
END

GO
