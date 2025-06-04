CREATE OR ALTER TRIGGER TR_NotifyEmployeeOnStatusChange
ON ApprovedHours
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Notifications (Description, NotificationDate, UserPK)
    SELECT
        CASE i.Status
            WHEN 'Approved' THEN N'Tus horas han sido aprobadas'
            WHEN 'Rejected' THEN N'Tus horas han sido rechazadas'
        END,
        GETDATE(),
        u.UserPK
    FROM inserted i
    INNER JOIN deleted d ON i.ApprovalID = d.ApprovalID
    INNER JOIN Employees e ON i.EmpID = e.EmpID
    INNER JOIN Users u ON u.PersonPK = e.PersonPK
    WHERE i.Status IN ('Approved', 'Rejected')
      AND i.Status <> d.Status
      AND u.Role = 'Empleado';
END;
