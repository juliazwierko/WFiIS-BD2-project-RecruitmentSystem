USE HR;
GO

-- 1. CANDIDATE
CREATE FUNCTION dbo.GetCandidateName(@candidate Candidate)
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [RecruitmentTypes].[RecruitmentTypes.Candidate].GetName;
GO

CREATE FUNCTION dbo.GetCandidateEmail(@candidate Candidate)
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [RecruitmentTypes].[RecruitmentTypes.Candidate].GetEmail;
GO

SELECT
    Id,
    dbo.GetCandidateName(Info) AS Name,
    dbo.GetCandidateEmail(Info) AS Email
FROM Candidates;
--Id	Name	Email
--1	    Alice	alice@example.com


-- 2. EVALUATION
CREATE FUNCTION dbo.GetEvaluator(@evaluation Evaluation)
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [RecruitmentTypes].[RecruitmentTypes.Evaluation].GetEvaluator;
GO

CREATE FUNCTION dbo.GetScore(@evaluation Evaluation)
RETURNS INT
AS EXTERNAL NAME [RecruitmentTypes].[RecruitmentTypes.Evaluation].GetScore;
GO

SELECT
    Id,
    CandidateId,
    dbo.GetEvaluator(EvaluationData) AS Evaluator,
    dbo.GetScore(EvaluationData) AS Score
FROM Evaluations;
--Id	CandidateId	Evaluator	Score
--1	    1	        Manager	    5


-- 3. INTERVIEW 
CREATE FUNCTION dbo.GetInterviewer(@interview Interview)
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [RecruitmentTypes].[RecruitmentTypes.Interview].GetInterviewer;
GO

CREATE FUNCTION dbo.GetDate(@interview Interview)
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [RecruitmentTypes].[RecruitmentTypes.Interview].GetDate;
GO

SELECT
    Id,
    CandidateId,
    dbo.GetInterviewer(InterviewData) AS Interviewer,
    dbo.GetDate(InterviewData) AS InterviewDate
FROM Interviews;
--Id	CandidateId	Interviewer	InterviewDate
--1	    1	        John Smith	2024-06-01


-- 4. RECRUITMENT TYPES 
CREATE FUNCTION dbo.GetCandidateNameSum(@summary RecruitmentSummary)
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [RecruitmentTypes].[RecruitmentTypes.RecruitmentSummary].GetCandidateNameSum;
GO

CREATE FUNCTION dbo.GetStatusSum(@summary RecruitmentSummary)
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [RecruitmentTypes].[RecruitmentTypes.RecruitmentSummary].GetStatusSum;
GO

SELECT
    Id,
    dbo.GetCandidateNameSum(Summary) AS Candidate,
    dbo.GetStatusSum(Summary) AS RecruitmentStatus
FROM Summaries;
--Id	Candidate	RecruitmentStatus
--1	Alice	Passed


-- 5. TASKITEM
CREATE FUNCTION dbo.GetTaskTitle(@task TaskItem)
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [RecruitmentTypes].[RecruitmentTypes.TaskItem].GetTitle;
GO

CREATE FUNCTION dbo.GetTaskDescription(@task TaskItem)
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [RecruitmentTypes].[RecruitmentTypes.TaskItem].GetDescription;
GO

SELECT 
    dbo.GetTaskTitle(TaskDetails) AS TaskTitle,
    dbo.GetTaskDescription(TaskDetails) AS TaskDescription
FROM Tasks;
--TaskTitle	        TaskDescription
--Technical Task	Implement BST in C#
