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