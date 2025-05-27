ALTER TABLE Companies
ADD PayrollType CHAR(1)
    CHECK (PayrollType IN ('W','B','M'));  -- W=Weekly, B=Bi-Weekly, M=Monthly


CREATE TABLE dbo.ApiDeductionConfigs
(
    Id                 INT             IDENTITY(1,1)    NOT NULL,
    Name               VARCHAR(100)    NOT NULL,
    Endpoint           VARCHAR(500)    NOT NULL,
    HttpMethod         VARCHAR(10)     NOT NULL,
    AuthHeaderName     VARCHAR(100)    NULL,
    AuthToken          VARCHAR(500)    NULL,
    ParametersJson     NVARCHAR(MAX)   NULL,
    ExpectedDataType   VARCHAR(50)     NOT NULL,
    BenefitsID UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_ApiDeductionConfigs          PRIMARY KEY (Id),
    CONSTRAINT FK_ADC_Benefits
        FOREIGN KEY (BenefitsID) REFERENCES dbo.Benefits(ID),
    CONSTRAINT CK_ApiDeductionConfigs_HttpMethod
        CHECK (HttpMethod IN ('GET','POST')),
    CONSTRAINT CK_ApiDeductionConfigs_Json
        CHECK (ParametersJson IS NULL OR ISJSON(ParametersJson) = 1)
);
GO


CREATE TABLE dbo.EmployeeApiParameters
(
    Id              INT           IDENTITY(1,1) NOT NULL,
    EmployeeId      UNIQUEIDENTIFIER                   NOT NULL,
    ApiConfigId     INT                          NOT NULL,
    ParameterKey    VARCHAR(100)  NOT NULL,
    ParameterValue  VARCHAR(255)  NOT NULL, 
    CONSTRAINT PK_EmployeeApiParameters       PRIMARY KEY(Id),
    CONSTRAINT FK_EAP_Employees
        FOREIGN KEY (EmployeeId)  REFERENCES dbo.Employees(EmpID),
    CONSTRAINT FK_EAP_ApiDeductionConfigs
        FOREIGN KEY (ApiConfigId) REFERENCES dbo.ApiDeductionConfigs(Id),
    CONSTRAINT UQ_EAP_Employee_Api_Key
        UNIQUE (EmployeeId, ApiConfigId, ParameterKey)
);
GO

ALTER TABLE dbo.Benefits 
ALTER COLUMN Path NVARCHAR(500) NULL;
GO
