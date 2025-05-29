DELETE FROM EmployeeApiParameters

ALTER TABLE ApiDeductionConfigs
drop column Endpoint;

ALTER TABLE ApiDeductionConfigs
drop column Name;

Alter table ApiDeductionConfigs
drop constraint FK_ADC_Benefits;

DELETE FROM ApiDeductionConfigs

DELETE FROM ChosenBenefits

DELETE FROM Benefits

Alter table ApiDeductionConfigs
drop column BenefitsID;

ALTER TABLE ApiDeductionConfigs
add Endpoint VARCHAR(500) NOT NULL;

ALTER TABLE ApiDeductionConfigs
add Name VARCHAR(100) NOT NULL;

INSERT INTO dbo.ApiDeductionConfigs (Name, Endpoint, HttpMethod, AuthHeaderName, AuthToken, ParametersJson, ExpectedDataType)
VALUES
('Seguro de vida', 'https://poliza-friends-grg0h9g5crf2hwh8.southcentralus-01.azurewebsites.net/api/LifeInsurance', 'GET', 'FRIENDS-API-TOKEN',
 '1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7',
 '{ "date of birth": "{dob}", "sex": "{sex}" }', 'json-path:$.monthlyCost'),

('Asociación solidarista', 'https://asociacion-geems-c3dfavfsapguhxbp.southcentralus-01.azurewebsites.net/api/public/calculator/calculate', 'POST', 'API-KEY',
 'Tralalerotralala', '{ "associationName": "{assocName}", "employeeSalary": "{salary}" }', 'json-path:$.amountToCharge'),

('MediSeguro', 'https://mediseguro-vorlagenersteller-d4hmbvf7frg7aqan.southcentralus-01.azurewebsites.net/api/MediSeguroMonto', 'POST', 'token',
 'TOKEN123', '{ "fechaNacimiento": "{dob}", "genero": "{sexo}", "cantidadDependientes": "{dependents}" }', 'decimal');

 CREATE TABLE dbo.OffersAPIs (
    CompanyPK   UNIQUEIDENTIFIER NOT NULL,
    ApiConfigId INT NOT NULL,
    
    PRIMARY KEY (CompanyPK, ApiConfigId),
    
    FOREIGN KEY (CompanyPK) REFERENCES dbo.Companies (CompanyPK),
    FOREIGN KEY (ApiConfigId) REFERENCES dbo.ApiDeductionConfigs (Id)
);

 CREATE TABLE dbo.ChosenAPIs (
    EmployeePK uniqueidentifier NOT NULL,
    ApiID int NOT NULL,
    
    PRIMARY KEY (EmployeePK, ApiID),
    
    FOREIGN KEY (EmployeePK) REFERENCES dbo.Employees (EmpId),
    FOREIGN KEY (ApiID) REFERENCES dbo.ApiDeductionConfigs (Id)
);