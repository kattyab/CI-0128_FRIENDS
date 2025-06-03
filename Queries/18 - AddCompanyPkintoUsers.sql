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
ADD COLUMN IsClosed bit NULL DEFAULT 0;