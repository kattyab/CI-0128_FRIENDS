-- User passwords are "Password"

INSERT INTO
    Users (Email, PasswordHash, Active, Role, PersonPK)
VALUES
    (
        'juan.perez@example.com',
        'AQAAAAIAAYagAAAAEF72g/ype6C+EQBDSbylQy9STe3YPAUQZasIzfanStRWBo2IGU2q/cYBZys5x9RFjA==',
        1,
        'Empleado',
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '1-2345-6789'
        )
    ),
    (
        'ana.lopez@example.com',
        'AQAAAAIAAYagAAAAEDKkW2RENG+AwNUZqwLMG4ebW/GDvjSIZ6cCndSCZgtK5nzm0skmORCVK79BGhJsvA==',
        1,
        'Administrador',
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '4-5678-9876'
        )
    ),
    (
        'carlos.ramirez@example.com',
        'AQAAAAIAAYagAAAAEE5jDqVjhuMM5H/T6+GqAQE0WGHZ01p5rQ/dn8B+ewVzyyMVpf0cOEAcTE+dikxXWg==',
        1,
        'Dueño',
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '7-8987-6543'
        )
    ),
    (
        'laura.jimenez@example.com',
        'AQAAAAIAAYagAAAAEI/NUUKTJdLki+qRy/OwEFO/J/N6y3X43sePa4Sw3kAEXqLjcsjKHtyI7maP6MroPg==',
        1,
        'Superadmin',
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '3-4567-8912'
        )
    ),
    (
        'miguel.torres@example.com',
        'AQAAAAIAAYagAAAAEOVLPt0HIWEtZP03QkcfMEfEjhVERqG0j8TUw3WfXskv9yLuBOj58Zuf07K9dzhD3w==',
        1,
        'Supervisor',
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '5-6789-1234'
        )
    );
