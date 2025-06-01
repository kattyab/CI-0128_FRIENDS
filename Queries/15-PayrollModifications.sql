ALTER TABLE Payrolls 
DROP CONSTRAINT UQ_Payrolls_ApprovalID;

ALTER TABLE Payrolls 
ADD GrossPaid decimal(19,2) NOT NULL;

ALTER TABLE Payrolls 
ADD NetPaid decimal(19,2) NOT NULL;

EXEC sp_rename 'Payrolls.GrossPaid', 'BrutePaid', 'COLUMN';

ALTER TABLE Payrolls DROP CONSTRAINT FK_Payrolls_ExecutedBy;

ALTER TABLE Payrolls
ADD CONSTRAINT FK_Payrolls_ExecutedBy FOREIGN KEY (ExecutedBy) REFERENCES dbo.Persons (PersonPK);
