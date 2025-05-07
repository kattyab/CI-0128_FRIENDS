INSERT INTO
    Persons (
        Id,
        Name,
        LastName,
        Sex,
        BirthDate,
        Province,
        Canton,
        OtherSigns
    )
VALUES
    (
        '1-2345-6789',
        'Juan',
        'Pérez',
        'Hombre',
        '1990-01-01',
        'San José',
        'Escazú',
        '50m oeste de La Casona de Laly'
    ),
    (
        '4-5678-9876',
        'Ana',
        'López',
        'Mujer',
        '1985-03-22',
        'Alajuela',
        'Palmares',
        NULL
    ),
    (
        '7-8987-6543',
        'Carlos',
        'Ramírez',
        'Hombre',
        '1975-07-12',
        'Cartago',
        'Turrialba',
        NULL
    ),
    (
        '3-4567-8912',
        'Laura',
        'Jiménez',
        'Mujer',
        '1992-11-05',
        'Heredia',
        'Barva',
        NULL
    ),
    (
        '5-6789-1234',
        'Miguel',
        'Torres',
        'Hombre',
        '1988-06-18',
        'Puntarenas',
        'Esparza',
        NULL
    );

INSERT INTO
    PersonPhoneNumbers (PersonPK, Number)
VALUES
    (
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '1-2345-6789'
        ),
        '8888-1111'
    ),
    (
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '1-2345-6789'
        ),
        '8888-2222'
    ),
    (
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '4-5678-9876'
        ),
        '8888-3333'
    ),
    (
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '7-8987-6543'
        ),
        '8888-4444'
    ),
    (
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '7-8987-6543'
        ),
        '8888-5555'
    ),
    (
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '3-4567-8912'
        ),
        '8888-6666'
    ),
    (
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '5-6789-1234'
        ),
        '8888-7777'
    ),
    (
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '5-6789-1234'
        ),
        '8888-8888'
    );
