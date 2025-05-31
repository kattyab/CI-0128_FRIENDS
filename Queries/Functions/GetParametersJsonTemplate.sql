CREATE FUNCTION GetParametersJsonTemplate(@ApiConfigId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @result NVARCHAR(MAX);

    SET @result = CASE @ApiConfigId
        WHEN 9 THEN '{ "date of birth": "{dob}", "sex": "{sex}" }'
        WHEN 10 THEN '{ "associationName": "{assocName}", "employeeSalary": "{salary}" }'
        WHEN 11 THEN '{ "fechaNacimiento": "{dob}", "genero": "{sexo}", "cantidadDependientes": "{dependents}" }'
        ELSE '{}'
    END;

    RETURN @result;
END;
GO