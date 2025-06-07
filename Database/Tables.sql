USE HR;
GO

CREATE TABLE Candidates (
    Id INT IDENTITY PRIMARY KEY,
    Info Candidate NOT NULL
);

IF COL_LENGTH('dbo.Candidates', 'EmailComputed') IS NULL
BEGIN
    ALTER TABLE Candidates
    ADD EmailComputed AS dbo.GetCandidateEmail(Info) PERSISTED;
END
GO

CREATE TABLE Interviews (
    Id INT IDENTITY PRIMARY KEY,
    CandidateId INT FOREIGN KEY REFERENCES Candidates(Id),
    InterviewData Interview NOT NULL
);

CREATE TABLE Tasks (
    Id INT IDENTITY PRIMARY KEY,
    CandidateId INT NOT NULL FOREIGN KEY REFERENCES Candidates(Id),
    InterviewId INT NOT NULL FOREIGN KEY REFERENCES Interviews(Id),
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

CREATE TABLE HRNotes(
    Id INT IDENTITY PRIMARY KEY,
    CandidateId INT NOT NULL,
    Note HrNote NOT NULL
);

-- Add status to check is intervie is evaluated
ALTER TABLE Interviews
ADD IsEvaluated BIT NOT NULL DEFAULT 0;