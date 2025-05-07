INSERT INTO
    Notifications (Description, NotificationDate, UserPK)
VALUES
    (
        'Se ha registrado un nuevo empleado: Juan Pérez',
        '2023-10-01 10:00:00',
        (
            SELECT
                UserPK
            FROM
                Users
            WHERE
                Email = 'carlos.ramirez@example.com'
        )
    );
