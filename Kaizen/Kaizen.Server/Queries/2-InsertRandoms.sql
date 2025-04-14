INSERT INTO Personas (cedula, nombre, apellido, sexo, fecha_nacimiento, provincia, canton, otras_senas)
VALUES 
('123456789', 'Ana', 'Gómez', 'F', '1990-05-15', 'San José', 'Montes de Oca', 'Calle 5, casa esquinera'),
('987654321', 'Luis', 'Ramírez', 'M', '1985-09-10', 'Alajuela', 'San Ramón', 'Del parque 200m norte'),
('456789123', 'María', 'Fernández', 'F', '1992-02-25', 'Heredia', 'Santo Domingo', 'Frente a la iglesia');

INSERT INTO Telefonos (cedula_persona, telefono)
VALUES 
('123456789', '8888-1234'),
('123456789', '2222-5678'),
('987654321', '8999-4321'),
('456789123', '8711-9090');


INSERT INTO Empleados (cedula, tipo_contrato, fecha_ingreso, cuenta_bancaria, salario_bruto, periodicidad, horas_trabajadas, horas_extra, rol)
VALUES
('123456789', 'Tiempo completo', '2020-01-10', 'CR123456789001', 850000.00, 'Mensual', 160, 5, 'Contadora'),
('987654321', 'Medio tiempo', '2021-06-01', 'CR987654321002', 400000.00, 'Quincenal', 80, 0, 'Asistente'),
('456789123', 'Tiempo completo', '2019-11-15', 'CR456789123003', 950000.00, 'Mensual', 170, 8, 'Jefa de Recursos Humanos');


INSERT INTO Beneficios (cedula_empleado, beneficio)
VALUES
('123456789', 'Transporte'),
('123456789', 'Almuerzo'),
('987654321', 'Capacitación'),
('456789123', 'Seguro Médico'),
('456789123', 'Parqueo');

