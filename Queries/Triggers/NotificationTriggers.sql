CREATE OR ALTER TRIGGER TR_NotifyNewEmployee
ON dbo.Employees
FOR INSERT
NOT FOR REPLICATION
AS

BEGIN
SET NOCOUNT ON;

DECLARE @NewEmployeePersonPK UNIQUEIDENTIFIER = (SELECT TOP 1 PersonPK FROM inserted);
DECLARE @CompanyPK UNIQUEIDENTIFIER = (SELECT TOP 1 WorksFor FROM inserted);
DECLARE @OwnerUserPK UNIQUEIDENTIFIER = (
    SELECT TOP 1 u.UserPK
    FROM Users u
    INNER JOIN Companies c ON c.OwnerPK = u.PersonPK
    WHERE c.CompanyPK = @CompanyPK
);

DECLARE @NewEmployeeName VARCHAR(75) = (SELECT TOP 1 Name FROM Persons WHERE PersonPK = @NewEmployeePersonPK);
DECLARE @NewEmployeeSurname VARCHAR(75) = (SELECT TOP 1 LastName FROM Persons WHERE PersonPK = @NewEmployeePersonPK);

DECLARE @NotificationContent VARCHAR(MAX) = 'Nuevo empleado agregado: {name} {surname}';
SELECT @NotificationContent = REPLACE(@NotificationContent, SearchText, ReplaceText)
FROM (VALUES
    ('{name}', @NewEmployeeName),
    ('{surname}', @NewEmployeeSurname)
) _ (SearchText, ReplaceText);

INSERT INTO Notifications
(Description, NotificationDate, UserPK)
VALUES
(@NotificationContent, GETDATE(), @OwnerUserPK);

DECLARE AdminNotificationCursor CURSOR FOR
SELECT u.UserPK FROM Users u
INNER JOIN Persons p ON p.PersonPK = u.PersonPK
INNER JOIN Admins a ON a.AdminPK = p.PersonPK
INNER JOIN Companies c ON c.CompanyPK = a.CompanyPK
WHERE c.CompanyPK = @CompanyPK;

DECLARE @AdminUserPK UNIQUEIDENTIFIER;

OPEN AdminNotificationCursor;
FETCH NEXT FROM AdminNotificationCursor INTO @AdminUserPK;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO Notifications
    (Description, NotificationDate, UserPK)
    VALUES
    (@NotificationContent, GETDATE(), @AdminUserPK);

    FETCH NEXT FROM AdminNotificationCursor INTO @AdminUserPK;
END

CLOSE AdminNotificationCursor;
DEALLOCATE AdminNotificationCursor;

END
