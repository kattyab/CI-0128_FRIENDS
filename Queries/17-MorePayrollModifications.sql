ALTER TABLE GeneralPayrolls 
ADD ExecutedOn DATETIME NULL;
GO;

UPDATE GeneralPayrolls 
SET ExecutedOn = CAST(StartDate AS DATETIME);

ALTER TABLE GeneralPayrolls 
DROP COLUMN StartDate;

ALTER TABLE Employees
ADD IsDeleted bit default 0;

UPDATE Employees
SET IsDeleted = 0
WHERE IsDeleted IS NULL;
