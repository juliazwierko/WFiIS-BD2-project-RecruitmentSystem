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


IF COL_LENGTH('dbo.Candidates', 'EmailComputed') IS NULL
BEGIN
    ALTER TABLE Candidates
    ADD EmailComputed AS dbo.GetCandidateEmail(Info) PERSISTED;
END
GO

--SELECT * FROM sys.objects WHERE name = 'IsValidEmail' AND type = 'FN';

CREATE FUNCTION dbo.IsValidEmail(@Email NVARCHAR(255))
RETURNS BIT
AS EXTERNAL NAME RecruitmentTypes.[RecruitmentTypes.Candidate].IsValidEmail;
GO

-- Sprawdzenie i dodanie ograniczenia CK_Candidates_EmailValid
IF NOT EXISTS (
    SELECT * FROM sys.check_constraints 
    WHERE name = 'CK_Candidates_EmailValid' AND parent_object_id = OBJECT_ID('dbo.Candidates')
)
BEGIN
    ALTER TABLE Candidates
    ADD CONSTRAINT CK_Candidates_EmailValid CHECK (dbo.IsValidEmail(EmailComputed) = 1);
END
GO

-- dbo.HrNote_GetText
CREATE FUNCTION dbo.HrNote_GetText (@note HRNote)
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME RecruitmentTypes.[RecruitmentTypes.HrNote].GetText;
GO

-- dbo.HrNote_GetCreatedAt
CREATE FUNCTION dbo.HrNote_GetCreatedAt (@note HRNote)
RETURNS DATETIME
AS EXTERNAL NAME RecruitmentTypes.[RecruitmentTypes.HrNote].GetCreatedAt;
GO

-- Email validation
IF COL_LENGTH('dbo.Candidates', 'EmailComputed') IS NULL
BEGIN
    ALTER TABLE Candidates
    ADD EmailComputed AS dbo.GetCandidateEmail(Info) PERSISTED;
END
GO

-- SELECT * FROM sys.objects WHERE name = 'IsValidEmail' AND type = 'FN';

CREATE FUNCTION dbo.IsValidEmail(@Email NVARCHAR(255))
RETURNS BIT
AS EXTERNAL NAME RecruitmentTypes.[RecruitmentTypes.Candidate].IsValidEmail;
GO

IF NOT EXISTS (
    SELECT * FROM sys.check_constraints 
    WHERE name = 'CK_Candidates_EmailValid' AND parent_object_id = OBJECT_ID('dbo.Candidates')
)
BEGIN
    ALTER TABLE Candidates
    ADD CONSTRAINT CK_Candidates_EmailValid CHECK (dbo.IsValidEmail(EmailComputed) = 1);
END
GO

-- Main API logic
USE HR_;
GO
 
CREATE PROCEDURE dbo.RunRecruitmentProcess
    @candidateData NVARCHAR(MAX),
    @interviewData NVARCHAR(MAX),
    @taskData NVARCHAR(MAX),
    @evaluationData NVARCHAR(MAX),
    @summaryData NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @candidate Candidate = CAST(@candidateData AS Candidate);
    DECLARE @interview Interview = CAST(@interviewData AS Interview);
    DECLARE @task TaskItem = CAST(@taskData AS TaskItem);
    DECLARE @evaluation Evaluation = CAST(@evaluationData AS Evaluation);
    DECLARE @summary RecruitmentSummary = CAST(@summaryData AS RecruitmentSummary);

    -- Insert Candidate
    INSERT INTO Candidates (Info)
    VALUES (@candidate);

    DECLARE @candidateId INT = SCOPE_IDENTITY();

    -- Insert Interview
    INSERT INTO Interviews (CandidateId, InterviewData)
    VALUES (@candidateId, @interview);

    -- Insert Task
    INSERT INTO Tasks (CandidateId, TaskDetails)
    VALUES (@candidateId, @task);

    -- Insert Evaluation
    INSERT INTO Evaluations (CandidateId, EvaluationData)
    VALUES (@candidateId, @evaluation);

    -- Insert Summary
    INSERT INTO Summaries (CandidateId, Summary)
    VALUES (@candidateId, @summary);

    SELECT 
        @candidateId AS CandidateId,
        dbo.GetCandidateName(@candidate) AS Name,
        dbo.GetCandidateEmail(@candidate) AS Email;
END
GO

USE HR_;
GO
EXEC dbo.RunRecruitmentProcess 
    @candidateData = 'Alice Paterkowska|alice@example.com',
    @interviewData = 'John Smith|2024-06-01',
    @taskData = 'Technical Task|Implement BST in C#',
    @evaluationData = 'Manager|5',
    @summaryData = 'Alice|Passed';
GO

-- Test:
-- --CandidateId	Name	Email
-- --9	Alice Paterkowska	alice@example.com

-- View to see all unrated interviews
CREATE VIEW UnratedInterviews AS
SELECT 
    i.Id AS InterviewId,
    c.Id AS CandidateId,
    dbo.GetCandidateName(c.Info) AS Name,
    dbo.GetInterviewer(i.InterviewData) AS Interviewer,
    dbo.GetDate(i.InterviewData) AS InterviewDate
FROM Interviews i
JOIN Candidates c ON c.Id = i.CandidateId
WHERE i.IsEvaluated = 0;
GO

-- Procedure to see candidate decision
CREATE PROCEDURE ShowCandidateDecision
    @CandidateId INT
AS
BEGIN
    SELECT 
        c.Id AS CandidateId,
        dbo.GetCandidateName(c.Info) AS Name,
        dbo.GetCandidateEmail(c.Info) AS Email,
        dbo.GetInterviewer(i.InterviewData) AS Interviewer,
        dbo.GetDate(i.InterviewData) AS InterviewDate,
        dbo.GetTaskTitle(t.TaskDetails) AS TaskTitle,
        dbo.GetTaskDescription(t.TaskDetails) AS TaskDescription,
        dbo.GetEvaluator(e.EvaluationData) AS Evaluator,
        dbo.GetScore(e.EvaluationData) AS Score,
        dbo.GetStatusSum(s.Summary) AS FinalStatus
    FROM Candidates c
    LEFT JOIN Interviews i ON i.CandidateId = c.Id
    LEFT JOIN Tasks t ON t.CandidateId = c.Id
    LEFT JOIN Evaluations e ON e.CandidateId = c.Id
    LEFT JOIN Summaries s ON s.CandidateId = c.Id
    WHERE c.Id = @CandidateId;
END;


-- Procedure to evaluate interview
CREATE PROCEDURE EvaluateInterview
    @InterviewId INT,
    @Evaluator NVARCHAR(MAX),
    @Score INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CandidateId INT;

    SELECT @CandidateId = CandidateId FROM Interviews WHERE Id = @InterviewId;

    IF @CandidateId IS NULL
    BEGIN
        RAISERROR('Interview with given Id does not exist.', 16, 1);
        RETURN;
    END

    DECLARE @EvaluationData Evaluation;
    SET @EvaluationData = CAST(CONCAT(@Evaluator, '|', CAST(@Score AS NVARCHAR(10))) AS Evaluation);

    INSERT INTO Evaluations (CandidateId, EvaluationData)
    VALUES (@CandidateId, @EvaluationData);

    UPDATE Interviews
    SET IsEvaluated = 1
    WHERE Id = @InterviewId;
END;
GO

-- EXEC EvaluateInterview @InterviewId = 1, @Evaluator = 'Manager', @Score = 4;
-- GO

CREATE FUNCTION dbo.GetTasksForInterview(@InterviewId INT)
RETURNS TABLE
AS
RETURN
SELECT
    Id,
    dbo.GetTaskTitle(TaskDetails) AS Title,
    dbo.GetTaskDescription(TaskDetails) AS Description
FROM Tasks
WHERE InterviewId = @InterviewId;
GO

CREATE PROCEDURE AssignTaskToInterview
    @InterviewId INT,
    @CandidateId INT,
    @Title NVARCHAR(MAX),
    @Description NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TaskData TaskItem;
    SET @TaskData = CAST(CONCAT(@Title, '|', @Description) AS TaskItem);

    INSERT INTO Tasks (CandidateId, InterviewId, TaskDetails)
    VALUES (@CandidateId, @InterviewId, @TaskData);
END;
GO

CREATE PROCEDURE CreateInterviewd
    @CandidateId INT,
    @InterviewData Interview,
    @InterviewId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Interviews (CandidateId, InterviewData)
    VALUES (@CandidateId, @InterviewData);

    SET @InterviewId = SCOPE_IDENTITY();
END;
GO