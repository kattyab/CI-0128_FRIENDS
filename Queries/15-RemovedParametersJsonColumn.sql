ALTER TABLE ApiDeductionConfigs
DROP CONSTRAINT CK_ApiDeductionConfigs_Json;

ALTER TABLE ApiDeductionConfigs
DROP COLUMN ParametersJson;