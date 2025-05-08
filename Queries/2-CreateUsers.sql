CREATE TABLE Users (
    UserPK UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),  
    Email VARCHAR(100) UNIQUE NOT NULL,  
    PasswordHash VARCHAR(84) NOT NULL,  
    Active BIT NOT NULL,  
    Role NVARCHAR(15) NOT NULL CHECK (Role IN ('Empleado', 'Administrador', 'Dueño', 'Superadmin', 'Supervisor')), 
    PersonPK UNIQUEIDENTIFIER NOT NULL, 
    FOREIGN KEY (PersonPK) REFERENCES Persons(PersonPK)
);

CREATE INDEX IDX_Users_Email ON Users(Email);