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


-- 1. WALIDACJA MAILA DLA KANDYDATA
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


-- Needs to be checked
-- -- walidacja maila
-- -- Dodaj kolumnę obliczaną EmailComputed, jeśli jej nie ma
-- IF COL_LENGTH('dbo.Candidates', 'EmailComputed') IS NULL
-- BEGIN
--     ALTER TABLE Candidates
--     ADD EmailComputed AS dbo.GetCandidateEmail(Info) PERSISTED;
-- END
-- GO

-- --SELECT * FROM sys.objects WHERE name = 'IsValidEmail' AND type = 'FN';

-- CREATE FUNCTION dbo.IsValidEmail(@Email NVARCHAR(255))
-- RETURNS BIT
-- AS EXTERNAL NAME RecruitmentTypes.[RecruitmentTypes.Candidate].IsValidEmail;
-- GO


-- -- Sprawdzenie i dodanie ograniczenia CK_Candidates_EmailValid tylko jeśli go nie ma
-- IF NOT EXISTS (
--     SELECT * FROM sys.check_constraints 
--     WHERE name = 'CK_Candidates_EmailValid' AND parent_object_id = OBJECT_ID('dbo.Candidates')
-- )
-- BEGIN
--     ALTER TABLE Candidates
--     ADD CONSTRAINT CK_Candidates_EmailValid CHECK (dbo.IsValidEmail(EmailComputed) = 1);
-- END
-- GO

-- USE HR_;
-- GO
-- --- Logika apliakcji 
-- CREATE PROCEDURE dbo.RunRecruitmentProcess
--     @candidateData NVARCHAR(MAX),
--     @interviewData NVARCHAR(MAX),
--     @taskData NVARCHAR(MAX),
--     @evaluationData NVARCHAR(MAX),
--     @summaryData NVARCHAR(MAX)
-- AS
-- BEGIN
--     SET NOCOUNT ON;

--     DECLARE @candidate Candidate = CAST(@candidateData AS Candidate);
--     DECLARE @interview Interview = CAST(@interviewData AS Interview);
--     DECLARE @task TaskItem = CAST(@taskData AS TaskItem);
--     DECLARE @evaluation Evaluation = CAST(@evaluationData AS Evaluation);
--     DECLARE @summary RecruitmentSummary = CAST(@summaryData AS RecruitmentSummary);

--     -- Insert Candidate
--     INSERT INTO Candidates (Info)
--     VALUES (@candidate);

--     DECLARE @candidateId INT = SCOPE_IDENTITY();

--     -- Insert Interview
--     INSERT INTO Interviews (CandidateId, InterviewData)
--     VALUES (@candidateId, @interview);

--     -- Insert Task
--     INSERT INTO Tasks (CandidateId, TaskDetails)
--     VALUES (@candidateId, @task);

--     -- Insert Evaluation
--     INSERT INTO Evaluations (CandidateId, EvaluationData)
--     VALUES (@candidateId, @evaluation);

--     -- Insert Summary
--     INSERT INTO Summaries (CandidateId, Summary)
--     VALUES (@candidateId, @summary);

--     -- Optional: return something
--     SELECT 
--         @candidateId AS CandidateId,
--         dbo.GetCandidateName(@candidate) AS Name,
--         dbo.GetCandidateEmail(@candidate) AS Email;
-- END
-- GO

-- USE HR_;
-- GO
-- EXEC dbo.RunRecruitmentProcess 
--     @candidateData = 'Alice Paterkowska|alice@example.com',
--     @interviewData = 'John Smith|2024-06-01',
--     @taskData = 'Technical Task|Implement BST in C#',
--     @evaluationData = 'Manager|5',
--     @summaryData = 'Alice|Passed';
-- GO

-- --CandidateId	Name	Email
-- --9	Alice Paterkowska	alice@example.com

-- ALTER TABLE Interviews
-- ADD IsEvaluated BIT NOT NULL DEFAULT 0;

-- CREATE VIEW UnratedInterviews AS
-- SELECT 
--     i.Id AS InterviewId,
--     c.Id AS CandidateId,
--     dbo.GetCandidateName(c.Info) AS Name,
--     dbo.GetInterviewer(i.InterviewData) AS Interviewer,
--     dbo.GetDate(i.InterviewData) AS InterviewDate
-- FROM Interviews i
-- JOIN Candidates c ON c.Id = i.CandidateId
-- WHERE i.IsEvaluated = 0;
-- GO

-- CREATE PROCEDURE ShowCandidateDecision
--     @CandidateId INT
-- AS
-- BEGIN
--     SELECT 
--         c.Id AS CandidateId,
--         dbo.GetCandidateName(c.Info) AS Name,
--         dbo.GetCandidateEmail(c.Info) AS Email,
--         dbo.GetInterviewer(i.InterviewData) AS Interviewer,
--         dbo.GetDate(i.InterviewData) AS InterviewDate,
--         dbo.GetTaskTitle(t.TaskDetails) AS TaskTitle,
--         dbo.GetTaskDescription(t.TaskDetails) AS TaskDescription,
--         dbo.GetEvaluator(e.EvaluationData) AS Evaluator,
--         dbo.GetScore(e.EvaluationData) AS Score,
--         dbo.GetStatusSum(s.Summary) AS FinalStatus
--     FROM Candidates c
--     LEFT JOIN Interviews i ON i.CandidateId = c.Id
--     LEFT JOIN Tasks t ON t.CandidateId = c.Id
--     LEFT JOIN Evaluations e ON e.CandidateId = c.Id
--     LEFT JOIN Summaries s ON s.CandidateId = c.Id
--     WHERE c.Id = @CandidateId;
-- END;

-- INSERT INTO Evaluations (CandidateId, EvaluationData)
-- VALUES (9, CAST('Manager|5' AS Evaluation));

-- -- i teraz ustawiamy, że rozmowa oceniona:
-- UPDATE Interviews
-- SET IsEvaluated = 1
-- WHERE CandidateId = 9;


-- CREATE PROCEDURE EvaluateInterview
--     @InterviewId INT,
--     @Evaluator NVARCHAR(MAX),
--     @Score INT
-- AS
-- BEGIN
--     SET NOCOUNT ON;

--     DECLARE @CandidateId INT;

--     -- Pobierz CandidateId z Interview
--     SELECT @CandidateId = CandidateId FROM Interviews WHERE Id = @InterviewId;

--     IF @CandidateId IS NULL
--     BEGIN
--         RAISERROR('Interview with given Id does not exist.', 16, 1);
--         RETURN;
--     END

--     -- Wstaw ocenę (załóżmy format 'Evaluator|Score' do typu Evaluation)
--     DECLARE @EvaluationData Evaluation;
--     SET @EvaluationData = CAST(CONCAT(@Evaluator, '|', CAST(@Score AS NVARCHAR(10))) AS Evaluation);

--     INSERT INTO Evaluations (CandidateId, EvaluationData)
--     VALUES (@CandidateId, @EvaluationData);

--     -- Ustaw flagę, że rozmowa oceniona
--     UPDATE Interviews
--     SET IsEvaluated = 1
--     WHERE Id = @InterviewId;
-- END;
-- GO

-- EXEC EvaluateInterview @InterviewId = 1, @Evaluator = 'Manager', @Score = 4;
-- GO


-- DROP TABLE IF EXISTS Tasks;
-- GO

-- CREATE TABLE Tasks (
--     Id INT IDENTITY PRIMARY KEY,
--     CandidateId INT NOT NULL FOREIGN KEY REFERENCES Candidates(Id),
--     InterviewId INT NOT NULL FOREIGN KEY REFERENCES Interviews(Id),
--     TaskDetails TaskItem NOT NULL
-- );


-- INSERT INTO Tasks (CandidateId, InterviewId, TaskDetails)
-- VALUES (
--     1, -- CandidateId
--     1, -- InterviewId
--     CAST('Technical Task|Implement BST in C#' AS TaskItem)
-- );


-- CREATE FUNCTION dbo.GetTasksForInterview(@InterviewId INT)
-- RETURNS TABLE
-- AS
-- RETURN
-- SELECT
--     Id,
--     dbo.GetTaskTitle(TaskDetails) AS Title,
--     dbo.GetTaskDescription(TaskDetails) AS Description
-- FROM Tasks
-- WHERE InterviewId = @InterviewId;
-- GO


-- CREATE PROCEDURE AssignTaskToInterview
--     @InterviewId INT,
--     @CandidateId INT,
--     @Title NVARCHAR(MAX),
--     @Description NVARCHAR(MAX)
-- AS
-- BEGIN
--     SET NOCOUNT ON;

--     DECLARE @TaskData TaskItem;
--     SET @TaskData = CAST(CONCAT(@Title, '|', @Description) AS TaskItem);

--     INSERT INTO Tasks (CandidateId, InterviewId, TaskDetails)
--     VALUES (@CandidateId, @InterviewId, @TaskData);
-- END;
-- GO


-- -- test
-- -- Test insert and select for task assigned to interview

-- BEGIN TRANSACTION;

-- -- 1. Insert candidate
-- INSERT INTO Candidates (Info)
-- VALUES (CAST('Test User|testuser@example.com' AS Candidate));

-- DECLARE @CandidateId INT = SCOPE_IDENTITY();

-- -- 2. Insert interview for this candidate
-- INSERT INTO Interviews (CandidateId, InterviewData)
-- VALUES (@CandidateId, CAST('Interviewer Test|2025-06-04' AS Interview));

-- DECLARE @InterviewId INT = SCOPE_IDENTITY();

-- -- 3. Assign task to this interview
-- DECLARE @TaskData TaskItem;
-- SET @TaskData = CAST('Test Task|Description of test task' AS TaskItem);

-- INSERT INTO Tasks (CandidateId, InterviewId, TaskDetails)
-- VALUES (@CandidateId, @InterviewId, @TaskData);

-- -- 4. Check if task is stored correctly
-- SELECT 
--     t.Id AS TaskId,
--     c.Id AS CandidateId,
--     i.Id AS InterviewId,
--     dbo.GetTaskTitle(t.TaskDetails) AS TaskTitle,
--     dbo.GetTaskDescription(t.TaskDetails) AS TaskDescription
-- FROM Tasks t
-- JOIN Candidates c ON c.Id = t.CandidateId
-- JOIN Interviews i ON i.Id = t.InterviewId
-- WHERE t.CandidateId = @CandidateId AND t.InterviewId = @InterviewId;

-- ROLLBACK TRANSACTION;


-- -- TaskId	CandidateId	InterviewId	TaskTitle	TaskDescription
-- -- 4	12	8	Test Task	Description of test task
-- USE HR_;
-- GO

-- CREATE PROCEDURE CreateInterviewd
--     @CandidateId INT,
--     @InterviewData Interview,
--     @InterviewId INT OUTPUT
-- AS
-- BEGIN
--     SET NOCOUNT ON;

--     INSERT INTO Interviews (CandidateId, InterviewData)
--     VALUES (@CandidateId, @InterviewData);

--     SET @InterviewId = SCOPE_IDENTITY();
-- END;
-- GO


-- -- To teraz będzie działać poprawnie:
-- SELECT dbo.GetStatusSum(s.Summary)
-- FROM Summaries s


-- SELECT * FROM Interviews; -- lub odpowiednia tabela
-- ALTER TABLE Interviews ADD FinalVerdict NVARCHAR(50);

-- --SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Summaries';



-- BEGIN TRANSACTION;

-- -- 1. Usuń Tasks powiązane z Interviews powiązanymi z Candidates
-- DELETE FROM Tasks
-- WHERE InterviewId IN (
--     SELECT Id FROM Interviews
--     WHERE CandidateId IN (SELECT Id FROM Candidates)
-- );

-- -- 2. Usuń Interviews powiązane z Candidates
-- DELETE FROM Interviews
-- WHERE CandidateId IN (SELECT Id FROM Candidates);


-- -- 3. Usuń Evaluations powiązane z Candidates
-- DELETE FROM Evaluations
-- WHERE CandidateId IN (SELECT Id FROM Candidates);

-- DELETE FROM Summaries
-- WHERE CandidateId IN (SELECT Id FROM Candidates);
-- -- 3. Usuń wszystkich Candidates
-- DELETE FROM Candidates;


-- COMMIT TRANSACTION;