Alter table Employees
Drop column ExtraHours;

Alter table Employees
Add RegistersHours bit default 0;

Alter table Employees
Add FireDate date;

go

UPDATE Employees
SET RegistersHours = 0
WHERE RegistersHours IS NULL;

ALTER TABLE Employees
ALTER COLUMN RegistersHours bit NOT NULL;

