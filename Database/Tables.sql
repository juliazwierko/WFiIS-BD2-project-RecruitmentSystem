USE HR;
GO

CREATE TABLE Candidates (
    Id INT IDENTITY PRIMARY KEY,
    Info Candidate NOT NULL
);

CREATE TABLE Interviews (
    Id INT IDENTITY PRIMARY KEY,
    CandidateId INT FOREIGN KEY REFERENCES Candidates(Id),
    InterviewData Interview NOT NULL
);

CREATE TABLE Tasks (
    Id INT IDENTITY PRIMARY KEY,
    CandidateId INT FOREIGN KEY REFERENCES Candidates(Id),
    TaskDetails TaskItem NOT NULL
);

CREATE TABLE Evaluations (
    Id INT IDENTITY PRIMARY KEY,
    CandidateId INT FOREIGN KEY REFERENCES Candidates(Id),
    EvaluationData Evaluation NOT NULL
);

CREATE TABLE Summaries (
    Id INT IDENTITY PRIMARY KEY,
    CandidateId INT FOREIGN KEY REFERENCES Candidates(Id),
    Summary RecruitmentSummary NOT NULL
);


INSERT INTO Candidates (Info)
VALUES (CAST('Alice|alice@example.com' AS Candidate));

INSERT INTO Interviews (CandidateId, InterviewData)
VALUES (1, CAST('John Smith|2024-06-01' AS Interview));

INSERT INTO Tasks (CandidateId, TaskDetails)
VALUES (1, CAST('Technical Task|Implement BST in C#' AS TaskItem));

INSERT INTO Evaluations (CandidateId, EvaluationData)
VALUES (1, CAST('Manager|5' AS Evaluation));

INSERT INTO Summaries (CandidateId, Summary)
VALUES (1, CAST('Alice|Passed' AS RecruitmentSummary));


-- SELECT * FROM Candidates;
-- GO 
-- SELECT * FROM Interviews;
-- GO 
-- SELECT * FROM Tasks;
-- GO 
-- --Id	CandidateId	TaskDetails
-- --1	1	0x0E546563686E6963616C205461736B13496D706C656D656E742042535420696E204323
-- SELECT * FROM Evaluations;
-- GO 
-- SELECT * FROM Summaries;
-- GO