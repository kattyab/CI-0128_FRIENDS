ALTER TABLE GeneralPayrolls 
ADD ExecutedOn DATETIME NULL;
GO;

UPDATE GeneralPayrolls 
SET ExecutedOn = CAST(StartDate AS DATETIME);

ALTER TABLE GeneralPayrolls 
DROP COLUMN StartDate;