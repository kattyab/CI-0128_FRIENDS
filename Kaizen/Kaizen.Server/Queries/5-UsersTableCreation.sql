-- Creación de la tabla Usuarios, asociada con Personas mediante la cedula
CREATE TABLE Usuarios (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    cedula VARCHAR(20),
    contrasena VARCHAR(255) NOT NULL,
    correo VARCHAR(100) UNIQUE NOT NULL,
    FOREIGN KEY (cedula) REFERENCES Personas(cedula)
);
