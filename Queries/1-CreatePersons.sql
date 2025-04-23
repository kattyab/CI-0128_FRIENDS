CREATE TABLE Persons (
    ID VARCHAR(15) PRIMARY KEY,
    Name VARCHAR(75) NOT NULL,
    LastName VARCHAR(75) NOT NULL,
    Sex VARCHAR(6) NOT NULL CHECK (Sex IN ('Hombre', 'Mujer')),
    BirthDate DATE NOT NULL,
    Province VARCHAR(20),
    Canton VARCHAR(50),
    OtherSigns TEXT
);
