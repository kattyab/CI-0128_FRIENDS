ALTER TABLE dbo.Users
ADD CompanyPK UNIQUEIDENTIFIER NULL;
GO
--------------------------------------------------------------------- opcional--------------------------------------
-- Si se quiere poner todos los trabajadores en la empresa Kaizen, se puede hacer de la siguiente manera:

DECLARE @KaizenPK UNIQUEIDENTIFIER;

SELECT  @KaizenPK = CompanyPK
FROM    dbo.Companies
WHERE   CompanyName = 'Kaizen'; 


UPDATE  dbo.Users
SET     CompanyPK = @KaizenPK;


SELECT  CompanyPK, COUNT(*) AS TotalUsuarios
FROM    dbo.Users
GROUP BY CompanyPK;

---------------------------------------------------------------------

ALTER TABLE dbo.Users
ADD CONSTRAINT FK_Users_Companies
    FOREIGN KEY (CompanyPK)
    REFERENCES dbo.Companies (CompanyPK)

	select * from users
	select * from companies
	select * from persons
	select * from employees

ALTER TABLE dbo.Payrolls
ADD IsClosed bit NULL DEFAULT 0;


Update Companies
Set PayrollType = 'M'
WHERE BrandName = 'Kaizen'

EXEC sp_rename 'Benefits.IsPercetange', 'IsPercentage', 'COLUMN';

ALTER TABLE dbo.Payrolls
ALTER COLUMN ApprovalID UNIQUEIDENTIFIER NULL;

ALTER TABLE dbo.Payrolls
DROP CONSTRAINT FK_Payrolls_ExecutedBy;

-- Correr estos
ALTER TABLE dbo.Payrolls
DROP CONSTRAINT FK__Payrolls__Execut__70DDC3D8;

ALTER TABLE dbo.Payrolls
DROP CONSTRAINT UQ__Payrolls__328477D570360F7C;

-- Este ya lo corrio
ALTER TABLE dbo.Payrolls
ADD CONSTRAINT FK_Payrolls_ExecutedBy
Foreign KEY (ExecutedBy)
References Persons(PersonPK)
