CREATE TABLE Users (
    Email VARCHAR(100) PRIMARY KEY,
    Password VARCHAR(250) NOT NULL,
    Active BIT NOT NULL,
    Role VARCHAR(15) NOT NULL CHECK (Role IN ('Empleado', 'Administrador', 'Dueño', 'Superadmin')),
    PersonID VARCHAR(15),
    FOREIGN KEY (PersonID) REFERENCES Persons(ID)
);
