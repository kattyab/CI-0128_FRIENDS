INSERT INTO
    Notifications (Description, NotificationDate, UserPK)
VALUES
    (
        'Esta es una notification de prueba',
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
