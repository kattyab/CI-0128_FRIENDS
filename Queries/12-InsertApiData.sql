-- ADD API DATA

-- GUIDs for benefits
DECLARE @Benefit_FRIENDS UNIQUEIDENTIFIER = 'D9E2A7D4-0E6C-4533-9D7B-3A44FE5E3081';
DECLARE @Benefit_GEEMS   UNIQUEIDENTIFIER = '5120C9D6-7350-4BE9-8A5E-AEF97A80B57D';
DECLARE @Benefit_VORLAG  UNIQUEIDENTIFIER = '40D9530F-5DC1-47ED-B3E6-34238C03F6F1';

INSERT INTO dbo.Benefits (ID, Name, MinWorkDurationMonths, OfferedBy, IsAPI, Path, NumParameters, IsFixed, IsPercetange, IsFullTime, IsPartTime, IsByHours, IsByService)
VALUES
(@Benefit_FRIENDS, 'FRIENDS', 0, '8A74DCCE-3A51-4A58-AAB8-AD81B9432901', 1, 'https://poliza-friends-grg0h9g5crf2hwh8.southcentralus-01.azurewebsites.net/api/LifeInsurance', 2, 0, 0, 1, 1, 1, 1),
(@Benefit_GEEMS,   'GEEMS',   0, '8A74DCCE-3A51-4A58-AAB8-AD81B9432901', 1, 'https://asociacion-geems-c3dfavfsapguhxbp.southcentralus-01.azurewebsites.net/api/public/calculator/calculate', 2, 0, 0, 1, 1, 1, 1),
(@Benefit_VORLAG,  'Vorlagenersteller S.A.', 0, '8A74DCCE-3A51-4A58-AAB8-AD81B9432901', 1, 'https://mediseguro-vorlagenersteller-d4hmbvf7frg7aqan.southcentralus-01.azurewebsites.net/api/MediSeguroMonto', 3, 0, 0, 1, 1, 1, 1);

INSERT INTO dbo.ApiDeductionConfigs (Name, Endpoint, HttpMethod, AuthHeaderName, AuthToken, ParametersJson, ExpectedDataType, BenefitsID)
VALUES
('FRIENDS', 'https://poliza-friends-grg0h9g5crf2hwh8.southcentralus-01.azurewebsites.net/api/LifeInsurance', 'GET', 'FRIENDS-API-TOKEN',
 '1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7',
 '{ "date of birth": "{dob}", "sex": "{sex}" }', 'json-path:$.monthlyCost', @Benefit_FRIENDS),

('GEEMS', 'https://asociacion-geems-c3dfavfsapguhxbp.southcentralus-01.azurewebsites.net/api/public/calculator/calculate', 'POST', 'API-KEY',
 'Tralalerotralala', '{ "associationName": "{assocName}", "employeeSalary": "{salary}" }', 'json-path:$.amountToCharge', @Benefit_GEEMS),

('Vorlagenersteller S.A.', 'https://mediseguro-vorlagenersteller-d4hmbvf7frg7aqan.southcentralus-01.azurewebsites.net/api/MediSeguroMonto', 'POST', 'token',
 'TOKEN123', '{ "fechaNacimiento": "{dob}", "genero": "{sexo}", "cantidadDependientes": "{dependents}" }', 'decimal', @Benefit_VORLAG);