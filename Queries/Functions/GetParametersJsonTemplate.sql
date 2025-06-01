CREATE FUNCTION GetParametersJsonTemplate(@Endpoint VARCHAR(500))
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @result NVARCHAR(MAX);
    SET @result = CASE @Endpoint
        WHEN 'https://poliza-friends-grg0h9g5crf2hwh8.southcentralus-01.azurewebsites.net/api/LifeInsurance' THEN '{ "date of birth": "{dob}", "sex": "{sex}" }'
        WHEN 'https://asociacion-geems-c3dfavfsapguhxbp.southcentralus-01.azurewebsites.net/api/public/calculator/calculate' THEN '{ "associationName": "{assocName}", "employeeSalary": "{salary}" }'
        WHEN 'https://mediseguro-vorlagenersteller-d4hmbvf7frg7aqan.southcentralus-01.azurewebsites.net/api/MediSeguroMonto' THEN '{ "fechaNacimiento": "{dob}", "genero": "{sexo}", "cantidadDependientes": "{dependents}" }'
        ELSE '{}'
    END;
    RETURN @result;
END;