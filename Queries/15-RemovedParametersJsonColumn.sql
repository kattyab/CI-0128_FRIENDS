ALTER TABLE ApiDeductionConfigs
DROP CONSTRAINT CK_ApiDeductionConfigs_Json;

ALTER TABLE ApiDeductionConfigs
DROP COLUMN ParametersJson;

ALTER TABLE ApiDeductionConfigs
ADD CONSTRAINT UK_ApiDeductionConfigs_Endpoint UNIQUE (Endpoint);