-- 8
CREATE TABLE Owners (
    OwnerPK UNIQUEIDENTIFIER PRIMARY KEY,
    FOREIGN KEY (OwnerPK) REFERENCES Persons(PersonPK)
);

-- 9
CREATE TABLE SuperAdmins (
    SuperAdminPK UNIQUEIDENTIFIER PRIMARY KEY,
    FOREIGN KEY (SuperAdminPK) REFERENCES Persons(PersonPK)
);