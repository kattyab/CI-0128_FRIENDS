select * from users

select * from Companies

ALTER TABLE dbo.Users
ADD CompanyPK UNIQUEIDENTIFIER NULL;

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


