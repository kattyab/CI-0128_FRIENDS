
CREATE TABLE Companies (
    CompanyPK UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyID VARCHAR(12) UNIQUE CHECK (CompanyID NOT LIKE '%[^0-9-]%'),
    OwnerPK UNIQUEIDENTIFIER NOT NULL,
    CompanyName NVARCHAR(100) NOT NULL, 
    BrandName NVARCHAR(100) NOT NULL, 
    Type VARCHAR(8) NOT NULL CHECK (Type IN ('Fisico', 'Juridico')), 
    FoundationDate DATE,
    MaxBenefits INT NOT NULL,
    WebPage VARCHAR(200),
    Logo VARCHAR(MAX),
    Description NVARCHAR(MAX), 
    PO VARCHAR(10) CHECK (PO NOT LIKE '%[^0-9]%'), 
    Province NVARCHAR(20), 
    Canton NVARCHAR(50), 
	Distrito NVARCHAR(50),
    OtherSigns NVARCHAR(MAX), 
    FOREIGN KEY (OwnerPK) REFERENCES Persons(PersonPK)
);