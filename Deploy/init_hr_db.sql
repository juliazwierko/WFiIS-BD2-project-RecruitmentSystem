CREATE DATABASE HR_;
GO

USE HR_;
GO

EXEC sp_configure 'clr enabled', 1;
RECONFIGURE;

DECLARE @AssemblyName NVARCHAR(100) = N'RecruitmentTypes';

DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql += 'DROP FUNCTION [' + SCHEMA_NAME(o.schema_id) + '].[' + o.name + '];' + CHAR(13)
FROM sys.assembly_modules m
JOIN sys.objects o ON m.object_id = o.object_id
WHERE m.assembly_id = (SELECT assembly_id FROM sys.assemblies WHERE name = @AssemblyName);
EXEC sp_executesql @sql;

DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += 'DROP FUNCTION [' + SCHEMA_NAME(o.schema_id) + '].[' + o.name + '];' + CHAR(13)
FROM sys.objects o
WHERE o.type IN ('FN', 'TF', 'IF') 
AND o.is_ms_shipped = 0; 

IF LEN(@sql) > 0
BEGIN
    PRINT 'Usuwam funkcje:'
    PRINT @sql
    EXEC sp_executesql @sql;
END
ELSE
BEGIN
    PRINT 'Brak funkcji do usunięcia w bazie HR_.'
END

BEGIN TRY DROP TABLE Summaries;        END TRY BEGIN CATCH END CATCH
BEGIN TRY DROP TABLE Evaluations;      END TRY BEGIN CATCH END CATCH
BEGIN TRY DROP TABLE Tasks;            END TRY BEGIN CATCH END CATCH
BEGIN TRY DROP TABLE Interviews;       END TRY BEGIN CATCH END CATCH
BEGIN TRY DROP TABLE Candidates;       END TRY BEGIN CATCH END CATCH

BEGIN TRY DROP TYPE Candidate;         END TRY BEGIN CATCH END CATCH
BEGIN TRY DROP TYPE Interview;         END TRY BEGIN CATCH END CATCH
BEGIN TRY DROP TYPE TaskItem;          END TRY BEGIN CATCH END CATCH
BEGIN TRY DROP TYPE Evaluation;        END TRY BEGIN CATCH END CATCH
BEGIN TRY DROP TYPE RecruitmentSummary;END TRY BEGIN CATCH END CATCH

BEGIN TRY DROP ASSEMBLY RecruitmentTypes; END TRY BEGIN CATCH END CATCH

CREATE ASSEMBLY RecruitmentTypes
FROM 'C:\Users\Administrator\Desktop\RecruitmentTypes\bin\Debug\RecruitmentTypes.dll'
WITH PERMISSION_SET = SAFE;
GO

BEGIN TRANSACTION;
DELETE FROM Tasks
WHERE InterviewId IN (
    SELECT Id FROM Interviews
    WHERE CandidateId IN (SELECT Id FROM Candidates)
);

DELETE FROM Interviews
WHERE CandidateId IN (SELECT Id FROM Candidates);

DELETE FROM Evaluations
WHERE CandidateId IN (SELECT Id FROM Candidates);

DELETE FROM Summaries
WHERE CandidateId IN (SELECT Id FROM Candidates);

DELETE FROM Candidates;
COMMIT TRANSACTION;