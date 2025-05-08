CREATE TABLE Persons (
    PersonPK UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Id VARCHAR(15) UNIQUE, 
    Name NVARCHAR(75) NOT NULL,
    LastName NVARCHAR(75) NOT NULL,  
    Sex VARCHAR(6) NOT NULL CHECK (Sex IN ('Hombre', 'Mujer')),  
    BirthDate DATE NOT NULL,  
    Province NVARCHAR(20),
    Canton NVARCHAR(50),
    OtherSigns NVARCHAR(MAX)
);

CREATE INDEX IDX_Persons_Id ON Persons(Id);