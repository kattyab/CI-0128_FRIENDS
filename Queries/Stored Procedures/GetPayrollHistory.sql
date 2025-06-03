CREATE OR ALTER PROCEDURE dbo.GetPayrollHistory
AS
BEGIN
    SET NOCOUNT ON;


    SELECT
        gp.GeneralPayrollsID                                                             AS Id,
        gp.InCharge                                                                      AS Manager,
        CASE gp.PayrollMode
             WHEN 'W' THEN 'Semanal'
             WHEN 'B' THEN 'Quincenal'
             WHEN 'M' THEN 'Mensual'
             ELSE 'Desconocido'
        END                                                                              AS [Type],
        gp.Period,


        (gp.TotalDeductionsBenefits + gp.TotalObligatoryDeductions)                      AS Deductions,
        gp.TotalLaborCharges                                                             AS SocialCharges,
        gp.TotalMoneyPaid                                                                AS Total,


        (gp.TotalMoneyPaid - gp.TotalLaborCharges)                                       AS Gross,

        (gp.TotalMoneyPaid - gp.TotalLaborCharges
         - (gp.TotalDeductionsBenefits + gp.TotalObligatoryDeductions))                  AS Net

    FROM dbo.GeneralPayrolls gp
    ORDER BY gp.ExecutedOn DESC;
END;
GO
EXEC dbo.GetPayrollHistory;
