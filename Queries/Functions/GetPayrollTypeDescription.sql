CREATE FUNCTION GetPayrollTypeDescription(@payrollType CHAR(1))
RETURNS NVARCHAR(20)
AS
BEGIN
    RETURN (
        SELECT CASE @payrollType
            WHEN 'M' THEN 'Monthly'
            WHEN 'B' THEN 'Biweekly'
            WHEN 'W' THEN 'Weekly'
            ELSE 'Unknown'
        END
    )
END