-- 10
CREATE TABLE Supervises (
    SupervisorID UNIQUEIDENTIFIER NOT NULL,
    SupervisedID UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (SupervisorID, SupervisedID),
    FOREIGN KEY (SupervisorID) REFERENCES Employees(EmpID),
    FOREIGN KEY (SupervisedID) REFERENCES Employees(EmpID)
);

-- 11
CREATE TABLE ApprovedHours (
    ApprovalID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SupervisorID UNIQUEIDENTIFIER NOT NULL,
    SupervisedID UNIQUEIDENTIFIER NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    IsClosed BIT DEFAULT 0,
    HoursWorked DECIMAL(19,10) NOT NULL CHECK (HoursWorked > 0),
    FOREIGN KEY (SupervisorID, SupervisedID) REFERENCES Supervises(SupervisorID, SupervisedID)
);

-- 12
CREATE TABLE Payrolls (
    PayrollID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PaidTo UNIQUEIDENTIFIER NOT NULL,
    SolidarityAssociationFund DECIMAL(19,4),
    LifeInsurancePlan DECIMAL(19,4),
    IncomeTax DECIMAL(19,4),
    CCSS DECIMAL(19,4),
    ExecutedBy UNIQUEIDENTIFIER NOT NULL,
    ApprovalID UNIQUEIDENTIFIER NOT NULL UNIQUE,
    FOREIGN KEY (PaidTo) REFERENCES Employees(EmpID),
    FOREIGN KEY (ExecutedBy) REFERENCES Employees(EmpID),
    FOREIGN KEY (ApprovalID) REFERENCES ApprovedHours(ApprovalID)
);

-- 13
CREATE TABLE Oversees (
    SuperAdminPK UNIQUEIDENTIFIER,
    CompanyPK UNIQUEIDENTIFIER,
    PRIMARY KEY (SuperAdminPK, CompanyPK),
    FOREIGN KEY (SuperAdminPK) REFERENCES SuperAdmins(SuperAdminPK),
    FOREIGN KEY (CompanyPK) REFERENCES Companies(CompanyPK)
);

-- 14
CREATE TABLE CompaniesPhoneNumbers (
    CompanyPK UNIQUEIDENTIFIER,
    Number VARCHAR(15),
    PRIMARY KEY (CompanyPK, Number),
    FOREIGN KEY (CompanyPK) REFERENCES Companies(CompanyPK),
    CHECK (Number NOT LIKE '%[^0-9-]%')
);

-- 15
CREATE TABLE CompaniesEmails (
    CompanyPK UNIQUEIDENTIFIER,
    CompanyEmail VARCHAR(100),
    PRIMARY KEY (CompanyPK, CompanyEmail),
    FOREIGN KEY (CompanyPK) REFERENCES Companies(CompanyPK)
);

-- 16
CREATE TABLE Benefits (
    ID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    MinWorkDurationMonths INT NOT NULL,
    OfferedBy UNIQUEIDENTIFIER NOT NULL,
    IsFixed BIT NOT NULL DEFAULT 0,
    FixedValue DECIMAL(19,4) NULL,
    IsPercetange BIT NOT NULL DEFAULT 0,
    PercentageValue DECIMAL(5,2) NULL CHECK (PercentageValue > 0 AND PercentageValue < 100),
    IsAPI BIT NOT NULL DEFAULT 0,
    Path NVARCHAR(100) NULL,
    NumParameters INT NULL CHECK (NumParameters BETWEEN 1 AND 3),
    IsFullTime BIT NOT NULL DEFAULT 0,
    IsPartTime BIT NOT NULL DEFAULT 0,
    IsByHours BIT NOT NULL DEFAULT 0,
    IsByService BIT NOT NULL DEFAULT 0,
    PRIMARY KEY (ID),
    FOREIGN KEY (OfferedBy) REFERENCES dbo.Companies(CompanyPK)
);

-- 17
CREATE TABLE ChosenBenefits (
    EmployeeID UNIQUEIDENTIFIER NOT NULL,
    BenefitID UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (EmployeeID, BenefitID),
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmpID),
    FOREIGN KEY (BenefitID) REFERENCES Benefits(ID)
);

-- 18
CREATE TABLE BenefitsAuditData (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserPK UNIQUEIDENTIFIER,
    BenefitID UNIQUEIDENTIFIER,
    Details NVARCHAR(MAX) NOT NULL,
    DatePerformed DATETIME NOT NULL DEFAULT GETDATE(),
    FieldModified NVARCHAR(30) NOT NULL,
    FOREIGN KEY (BenefitID) REFERENCES Benefits(ID),
    FOREIGN KEY (UserPK) REFERENCES Users(UserPK)
);

-- 19
CREATE TABLE CompanyAuditData (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyPK UNIQUEIDENTIFIER,
    UserPK UNIQUEIDENTIFIER,
    Details NVARCHAR(MAX) NOT NULL,
    DatePerformed DATETIME NOT NULL DEFAULT GETDATE(),
    FieldModified NVARCHAR(30) NOT NULL,
    FOREIGN KEY (CompanyPK) REFERENCES Companies(CompanyPK),
    FOREIGN KEY (UserPK) REFERENCES Users(UserPK)
);