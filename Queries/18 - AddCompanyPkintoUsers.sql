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

-- Buscar el nombre de la constraint
-- Si se conoce el nombre de la constraint, se puede eliminar directamente:	
ALTER TABLE dbo.Payrolls
DROP CONSTRAINT FK__Payrolls__Execut__ejemplo;

ALTER TABLE dbo.Payrolls
DROP CONSTRAINT UQ__Payrolls__ejemplo;

ALTER TABLE dbo.Payrolls
ADD CONSTRAINT FK_Payrolls_ExecutedBy
Foreign KEY (ExecutedBy)
References Persons(PersonPK)



ALTER TABLE dbo.GeneralPayrolls
ADD PayrollMode CHAR(1)  NULL,
    Period      NVARCHAR(25) NULL;


GO

ALTER TABLE dbo.GeneralPayrolls
ADD CONSTRAINT CK_GeneralPayrolls_PayrollType
    CHECK (PayrollMode IN ('W','B','M'));   -- W = Weekly, B = Biweekly, M = Monthly

IF COL_LENGTH('dbo.GeneralPayrolls', 'InCharge') IS NULL
    ALTER TABLE dbo.GeneralPayrolls
    ADD InCharge NVARCHAR(150) NULL;  
GO
