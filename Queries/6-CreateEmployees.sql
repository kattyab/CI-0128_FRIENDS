CREATE TABLE Employees (
    EmpID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonPK UNIQUEIDENTIFIER NOT NULL UNIQUE,
    WorksFor UNIQUEIDENTIFIER NOT NULL,
    JobPosition NVARCHAR(75),
    ContractType VARCHAR(25) NOT NULL CHECK (ContractType IN ('Tiempo Completo', 'Medio Tiempo', 'Por Horas', 'Servicios Profesionales')),
    WorkHours TINYINT,
    ExtraHours TINYINT,
    StartDate DATE NOT NULL,
    BankAccount VARCHAR(22) NOT NULL,
    BruteSalary DECIMAL(18, 2) NOT NULL CHECK (BruteSalary >= 0),
    PayCycleType VARCHAR(20) CHECK (PayCycleType IN ('Mensual', 'Quincenal', 'Bisemanal', 'Semanal')),
    FOREIGN KEY (PersonPK) REFERENCES Persons(PersonPK),
    FOREIGN KEY (WorksFor) REFERENCES Companies(CompanyPK)
);