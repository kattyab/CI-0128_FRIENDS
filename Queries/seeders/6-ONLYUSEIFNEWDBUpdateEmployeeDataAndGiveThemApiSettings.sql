WITH DevJobs AS (
    SELECT 
        e.EmpID,
        p.Name,
        ROW_NUMBER() OVER (ORDER BY p.Name) AS RowNum
    FROM Employees e
    INNER JOIN Persons p ON e.PersonPK = p.PersonPK
    WHERE e.IsDeleted = 0
)
UPDATE e
SET 
    JobPosition = CASE dj.RowNum
        WHEN 1 THEN 'Backend Developer'
        WHEN 2 THEN 'Frontend Developer'
        WHEN 3 THEN 'Full Stack Developer'
        WHEN 4 THEN 'QA Tester'
        WHEN 5 THEN 'DevOps Engineer'
        WHEN 6 THEN 'Mobile Developer'
        ELSE 'Software Engineer'
    END,
    BruteSalary = CASE dj.RowNum
        WHEN 1 THEN 2500000
        WHEN 2 THEN 2400000
        WHEN 3 THEN 2700000
        WHEN 4 THEN 1800000
        WHEN 5 THEN 3000000
        WHEN 6 THEN 2600000
        ELSE 2200000
    END
FROM Employees e
JOIN DevJobs dj ON e.EmpID = dj.EmpID;

UPDATE Employees
SET WorkHours = 48
WHERE IsDeleted = 0;

UPDATE Persons SET Name = 'Sofía' WHERE Id = '2-1111-1111';
UPDATE Persons SET LastName = 'Gómez' WHERE Id = '2-2222-2222';
UPDATE Persons SET LastName = 'Martínez' WHERE Id = '2-3333-3333';
UPDATE Persons SET LastName = 'Sánchez' WHERE Id = '2-6666-6666';
UPDATE Persons SET Name = 'Andrés' WHERE Id = '2-4444-4444';

WITH OrderedEmployees AS (
    SELECT EmpId, 
           ROW_NUMBER() OVER (ORDER BY EmpId) as RowNum
    FROM Employees
),
ParameterData AS (
    -- Employee 1
    SELECT 1 as EmployeeOrder, 1 as ApiConfigId, 'dob' as ParameterKey, '1985-03-22' as ParameterValue
    UNION ALL SELECT 1, 1, 'sex', 'female'
    UNION ALL SELECT 1, 3, 'dob', '1985-03-22'
    UNION ALL SELECT 1, 3, 'sexo', 'femenino'
    UNION ALL SELECT 1, 3, 'dependents', '2'
    UNION ALL SELECT 1, 2, 'assocName', 'Kaizen FRIENDS'
    UNION ALL SELECT 1, 2, 'salary', '2500000.00'
    
    -- Employee 2
    UNION ALL SELECT 2, 1, 'dob', '1985-03-17'
    UNION ALL SELECT 2, 1, 'sex', 'male'
    
    -- Employee 3
    UNION ALL SELECT 3, 3, 'dob', '1990-01-01'
    UNION ALL SELECT 3, 3, 'sexo', 'masculino'
    UNION ALL SELECT 3, 3, 'dependents', '1'
    
    -- Employee 4
    UNION ALL SELECT 4, 2, 'assocName', 'Kaizen FRIENDS'
    UNION ALL SELECT 4, 2, 'salary', '2700000.00'
    
    -- Employee 5
    UNION ALL SELECT 5, 1, 'dob', '1988-12-30'
    UNION ALL SELECT 5, 1, 'sex', 'male'
    UNION ALL SELECT 5, 3, 'dob', '1988-12-30'
    UNION ALL SELECT 5, 3, 'sexo', 'masculino'
    UNION ALL SELECT 5, 3, 'dependents', '1'
    
    -- Employee 6
    UNION ALL SELECT 6, 3, 'dob', '1992-09-25'
    UNION ALL SELECT 6, 3, 'sexo', 'femenino'
    UNION ALL SELECT 6, 3, 'dependents', '5'
    UNION ALL SELECT 6, 2, 'assocName', 'Kaizen FRIENDS'
    UNION ALL SELECT 6, 2, 'salary', '1800000.00'
    
    -- Employee 7
    UNION ALL SELECT 7, 1, 'dob', '1990-04-10'
    UNION ALL SELECT 7, 1, 'sex', 'male'
    UNION ALL SELECT 7, 2, 'assocName', 'Kaizen FRIENDS'
    UNION ALL SELECT 7, 2, 'salary', '2600000.00'
)
INSERT INTO EmployeeApiParameters (EmployeeId, ApiConfigId, ParameterKey, ParameterValue)
SELECT oe.EmpId, pd.ApiConfigId, pd.ParameterKey, pd.ParameterValue
FROM ParameterData pd
INNER JOIN OrderedEmployees oe ON oe.RowNum = pd.EmployeeOrder;

INSERT INTO ChosenAPIs (EmployeePK, ApiID)
SELECT DISTINCT
    eap.EmployeeId,
    eap.ApiConfigId
FROM EmployeeApiParameters eap
LEFT JOIN ChosenAPIs ca
    ON ca.EmployeePK = eap.EmployeeId AND ca.ApiID = eap.ApiConfigId
WHERE ca.EmployeePK IS NULL;
