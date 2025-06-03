-- Esto hay que correrlo
ALTER TABLE ApprovedHours
ADD IsSentForApproval bit NOT NULL DEFAULT 0;

-- Borrar las fk de supervises

DECLARE @fkName NVARCHAR(128)
DECLARE @sql NVARCHAR(MAX)


DECLARE fk_cursor CURSOR FOR
SELECT fk.name
FROM sys.foreign_keys fk
JOIN sys.tables t ON fk.parent_object_id = t.object_id
WHERE t.name = 'ApprovedHours' AND fk.referenced_object_id = OBJECT_ID('Supervises')

OPEN fk_cursor
FETCH NEXT FROM fk_cursor INTO @fkName

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = 'ALTER TABLE ApprovedHours DROP CONSTRAINT [' + @fkName + '];'
    EXEC sp_executesql @sql
    FETCH NEXT FROM fk_cursor INTO @fkName
END

CLOSE fk_cursor
DEALLOCATE fk_cursor

-- 2. Renombra las columnas
EXEC sp_rename 'ApprovedHours.SupervisorID', 'SupID', 'COLUMN';
EXEC sp_rename 'ApprovedHours.SupervisedID', 'EmpID', 'COLUMN';

-- 3. Añadir FKS a Employees
ALTER TABLE ApprovedHours
ADD CONSTRAINT FK_ApprovedHours_SupID_Employees FOREIGN KEY (SupID) REFERENCES Employees(EmpId);

ALTER TABLE ApprovedHours
ADD CONSTRAINT FK_ApprovedHours_EmpID_Employees FOREIGN KEY (EmpID) REFERENCES Employees(EmpId);

-- 4. Drop table de supervises
DROP TABLE Supervises;

-- 5. SupID puede ser nulo
ALTER TABLE ApprovedHours
ALTER COLUMN SupID uniqueidentifier NULL;

-- 6. Renombrar isClosed
EXEC sp_rename 'ApprovedHours.IsClosed', 'Status', 'COLUMN';

-- 7. Quitar la constraint
ALTER TABLE ApprovedHours
DROP CONSTRAINT DF__ApprovedH__IsClo__17F790F9;


-- 8. Estados
ALTER TABLE ApprovedHours
ALTER COLUMN Status VARCHAR(10) NULL;

-- 9. Nuevos estados
ALTER TABLE ApprovedHours
ADD CONSTRAINT CHK_ApprovedHours_Status
CHECK (Status IS NULL OR Status IN ('Approved', 'Rejected', 'Waiting'));
