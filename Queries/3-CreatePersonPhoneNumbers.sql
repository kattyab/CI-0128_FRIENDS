CREATE TABLE PersonPhoneNumbers (
    PersonPK UNIQUEIDENTIFIER NOT NULL,
    Number VARCHAR(15) NOT NULL,
    PRIMARY KEY (PersonPK, Number),
    FOREIGN KEY (PersonPK) REFERENCES Persons(PersonPK),
    CHECK (Number NOT LIKE '%[^0-9-]%')
);