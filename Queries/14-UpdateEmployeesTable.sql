Alter table Employees
Drop column ExtraHours;

Alter table Employees
Add RegistersHours bit default 0;

Alter table Employees
Add FireDate date;

UPDATE Employees
SET RegistersHours = 0
WHERE RegistersHours IS NULL;
