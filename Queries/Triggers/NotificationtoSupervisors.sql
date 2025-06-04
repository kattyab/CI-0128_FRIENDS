CREATE OR ALTER TRIGGER TR_NotifySupervisorsOnApprovedHoursUpdate
ON ApprovedHours
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;


    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN deleted d ON i.ApprovalID = d.ApprovalID
        WHERE i.IsSentForApproval = 1 AND d.IsSentForApproval = 0
    )
    BEGIN

        INSERT INTO Notifications (Description, NotificationDate, UserPK)
        SELECT
            'Horas a revisar pendientes',
            GETDATE(),
            u.UserPK
        FROM Users u
        WHERE u.Role = 'Supervisor' AND u.Active = 1;
    END
END;