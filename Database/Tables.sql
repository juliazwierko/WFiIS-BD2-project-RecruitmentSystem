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

CREATE TABLE HRNotes(
    Id INT IDENTITY PRIMARY KEY,
    CandidateId INT NOT NULL,
    Note HrNote NOT NULL
);
