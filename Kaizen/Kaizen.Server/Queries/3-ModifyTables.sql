-- Agregar columna estado
ALTER TABLE Empleados
ADD estado VARCHAR(10) CHECK (estado IN ('activo', 'inactivo'));

-- Modificar (o asegurar existencia de) columna periodicidad
-- Ya existe como VARCHAR(30), podrías agregar un CHECK si quieres restringir valores:
ALTER TABLE Empleados
ADD CONSTRAINT chk_periodicidad CHECK (periodicidad IN ('mensual', 'quincenal', 'semanal'));

-- Agregar columna puesto
ALTER TABLE Empleados
ADD puesto VARCHAR(50);

-- Modificar columna rol con restricción
-- Si ya existe, no se puede agregar otra igual, así que podrías eliminarla y volver a crearla con un CHECK:
ALTER TABLE Empleados
DROP COLUMN rol;

ALTER TABLE Empleados
ADD rol VARCHAR(20) CHECK (rol IN ('Empleado', 'Supervisor', 'Administrador'));
