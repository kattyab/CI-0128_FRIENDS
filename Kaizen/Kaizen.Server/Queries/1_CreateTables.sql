CREATE TABLE Personas (
    cedula VARCHAR(20) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    apellido VARCHAR(50) NOT NULL,
    sexo VARCHAR(10),
    fecha_nacimiento DATE,
    provincia VARCHAR(50),
    canton VARCHAR(50),
    otras_senas TEXT
);

CREATE TABLE Telefonos (
    id INT IDENTITY(1,1) PRIMARY KEY,
    cedula_persona VARCHAR(20) NOT NULL,
    telefono VARCHAR(20) NOT NULL,
    FOREIGN KEY (cedula_persona) REFERENCES Personas(cedula)
);


CREATE TABLE Empleados (
    cedula VARCHAR(20) PRIMARY KEY,
    tipo_contrato VARCHAR(50),
    fecha_ingreso DATE,
    cuenta_bancaria VARCHAR(50),
    salario_bruto DECIMAL(10, 2),
    periodicidad VARCHAR(30),
    horas_trabajadas INT,
    horas_extra INT,
    rol VARCHAR(50),
    FOREIGN KEY (cedula) REFERENCES Personas(cedula)
);


CREATE TABLE Beneficios (
    id INT IDENTITY(1,1) PRIMARY KEY,
    cedula_empleado VARCHAR(20) NOT NULL,
    beneficio VARCHAR(100) NOT NULL,
    FOREIGN KEY (cedula_empleado) REFERENCES Empleados(cedula)
);

