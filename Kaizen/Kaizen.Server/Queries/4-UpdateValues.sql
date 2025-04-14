-- Ana Gómez
UPDATE Empleados
SET estado = 'activo',
    puesto = 'Contadora',
    rol = 'Empleado'
WHERE cedula = '123456789';

-- Luis Ramírez
UPDATE Empleados
SET estado = 'inactivo',
    puesto = 'Asistente Administrativo',
    rol = 'Empleado'
WHERE cedula = '987654321';

-- María Fernández
UPDATE Empleados
SET estado = 'activo',
    puesto = 'Jefa de Recursos Humanos',
    rol = 'Supervisor'
WHERE cedula = '456789123';
