-- Crear persona due�o
INSERT INTO Persons (Id, Name, LastName, Sex, BirthDate)
VALUES ('2-1111-1111', 'Sof�a', 'Navarro', 'Mujer', '1980-05-15');

-- Crear usuario del due�o
INSERT INTO Users (Email, PasswordHash, Active, Role, PersonPK)
VALUES (
  'sofia.navarro@example.com',
  'AQAAAAIAAYagAAAAEOBifWeNBudDjzVzfYLIzkT4FK+6XBP/BBvySlBybk5MLYxco9CNNkVpDOfQ8TgCEQ==',
  1,
  'Dueño',
  (SELECT PersonPK FROM Persons WHERE Id = '2-1111-1111')
);

-- Registrar como due�o
INSERT INTO Owners (OwnerPK)
VALUES ((SELECT PersonPK FROM Persons WHERE Id = '2-1111-1111'));

-- Crear empresa y asociarla al due�o
INSERT INTO Companies (
  CompanyID, OwnerPK, CompanyName, BrandName, Type, FoundationDate,
  MaxBenefits, WebPage, Description, PO
)
VALUES (
  '4-000-000020',
  (SELECT PersonPK FROM Persons WHERE Id = '2-1111-1111'),
  'NovaTech',
  'NovaTech',
  'Juridico',
  '2015-08-20',
  15,
  'http://novatech.com',
  'Empresa de soluciones tecnol�gicas',
  '54321'
);

-- Crear 5 empleados y usuarios
INSERT INTO Persons (Id, Name, LastName, Sex, BirthDate)
VALUES 
('2-2222-2222', 'Luis', 'G�mez', 'Hombre', '1990-04-10'),
('2-3333-3333', 'Elena', 'Mart�nez', 'Mujer', '1992-09-25'),
('2-4444-4444', 'Andr�s', 'Castro', 'Hombre', '1988-12-30'),
('2-5555-5555', 'Daniela', 'Vargas', 'Mujer', '1991-07-07'),
('2-6666-6666', 'Pablo', 'S�nchez', 'Hombre', '1985-03-17');

-- Usuarios (empleados)
INSERT INTO Users (Email, PasswordHash, Active, Role, PersonPK)
VALUES
('luis.gomez@example.com', 'AQAAAAIAAYagAAAAEOBifWeNBudDjzVzfYLIzkT4FK+6XBP/BBvySlBybk5MLYxco9CNNkVpDOfQ8TgCEQ==', 1, 'Empleado', (SELECT PersonPK FROM Persons WHERE Id = '2-2222-2222')),
('elena.martinez@example.com', 'AQAAAAIAAYagAAAAEOBifWeNBudDjzVzfYLIzkT4FK+6XBP/BBvySlBybk5MLYxco9CNNkVpDOfQ8TgCEQ==', 1, 'Empleado', (SELECT PersonPK FROM Persons WHERE Id = '2-3333-3333')),
('andres.castro@example.com', 'AQAAAAIAAYagAAAAEOBifWeNBudDjzVzfYLIzkT4FK+6XBP/BBvySlBybk5MLYxco9CNNkVpDOfQ8TgCEQ==', 1, 'Empleado', (SELECT PersonPK FROM Persons WHERE Id = '2-4444-4444')),
('daniela.vargas@example.com', 'AQAAAAIAAYagAAAAEOBifWeNBudDjzVzfYLIzkT4FK+6XBP/BBvySlBybk5MLYxco9CNNkVpDOfQ8TgCEQ==', 1, 'Empleado', (SELECT PersonPK FROM Persons WHERE Id = '2-5555-5555')),
('pablo.sanchez@example.com', 'AQAAAAIAAYagAAAAEOBifWeNBudDjzVzfYLIzkT4FK+6XBP/BBvySlBybk5MLYxco9CNNkVpDOfQ8TgCEQ==', 1, 'Empleado', (SELECT PersonPK FROM Persons WHERE Id = '2-6666-6666'));

-- Agregarlos como empleados de la empresa
INSERT INTO Employees (PersonPK, WorksFor, ContractType, StartDate, BankAccount, BruteSalary)
VALUES
((SELECT PersonPK FROM Persons WHERE Id = '2-2222-2222'), (SELECT CompanyPK FROM Companies WHERE CompanyID = '4-000-000020'), 'Tiempo Completo', '2021-01-01', '1111222233334444555501', 1100.00),
((SELECT PersonPK FROM Persons WHERE Id = '2-3333-3333'), (SELECT CompanyPK FROM Companies WHERE CompanyID = '4-000-000020'), 'Tiempo Completo', '2021-02-01', '1111222233334444555502', 1150.00),
((SELECT PersonPK FROM Persons WHERE Id = '2-4444-4444'), (SELECT CompanyPK FROM Companies WHERE CompanyID = '4-000-000020'), 'Tiempo Completo', '2021-03-01', '1111222233334444555503', 1200.00),
((SELECT PersonPK FROM Persons WHERE Id = '2-5555-5555'), (SELECT CompanyPK FROM Companies WHERE CompanyID = '4-000-000020'), 'Tiempo Completo', '2021-04-01', '1111222233334444555504', 1250.00),
((SELECT PersonPK FROM Persons WHERE Id = '2-6666-6666'), (SELECT CompanyPK FROM Companies WHERE CompanyID = '4-000-000020'), 'Tiempo Completo', '2021-05-01', '1111222233334444555505', 1300.00);
