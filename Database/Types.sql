-- definicje typów UDT
USE HR;
GO

CREATE TYPE Candidate         EXTERNAL NAME RecruitmentTypes.[RecruitmentTypes.Candidate];
CREATE TYPE Interview         EXTERNAL NAME RecruitmentTypes.[RecruitmentTypes.Interview];
CREATE TYPE TaskItem          EXTERNAL NAME RecruitmentTypes.[RecruitmentTypes.TaskItem];
CREATE TYPE Evaluation        EXTERNAL NAME RecruitmentTypes.[RecruitmentTypes.Evaluation];
CREATE TYPE RecruitmentSummary EXTERNAL NAME RecruitmentTypes.[RecruitmentTypes.RecruitmentSummary];
GO
