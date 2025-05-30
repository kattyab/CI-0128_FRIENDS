INSERT INTO
    Employees (
        PersonPK,
        WorksFor,
        JobPosition,
        ContractType,
        StartDate,
        BankAccount,
        BruteSalary
    )
VALUES
    (
        (
            SELECT
                PersonPK
            FROM
                Persons
            WHERE
                Id = '4-5678-9876'
        ),
        (
            SELECT
                CompanyPK
            FROM
                Companies
            WHERE
                CompanyID = '4-000-000019'
        ),
        'Administrador',
        'Tiempo Completo',
        '2021-01-01',
        '1111222233334444555500',
        2000.00
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
        (
            SELECT
                CompanyPK
            FROM
                Companies
            WHERE
                CompanyID = '4-000-000019'
        ),
        'Empleado',
        'Tiempo Completo',
        '2021-01-01',
        '1111222233334444555501',
        1100.00
    );
