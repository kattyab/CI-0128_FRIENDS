CREATE TRIGGER [dbo].[TR_Companies_AutoLinkAPIs]
ON [dbo].[Companies]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[OffersAPIs] (CompanyPK, ApiConfigId)
    SELECT 
        i.CompanyPK,
        a.Id
    FROM 
        inserted i
        CROSS JOIN [dbo].[ApiDeductionConfigs] a;
        
END;