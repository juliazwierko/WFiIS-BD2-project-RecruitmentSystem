using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Configuration;

using RecruitmentTypes;


namespace RecruitmentConsoleApp
{
    class Program
    {
        private static readonly string connectionString =
            @"Data Source=WINSERVER;Initial Catalog=HR;Integrated Security=True;Persist Security Info=False;Pooling=False;";

        static void Main()
        {
            Console.WriteLine("Checking DB connection...");
            if (TestDatabaseConnection())
            {
                Console.WriteLine("Connection successful!\n");

                ShowMenu();
            }
            else
            {
                Console.WriteLine("Connection failed. Check connection string or SQL Server availability.");
            }
        }

        static bool TestDatabaseConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return conn.State == ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during DB connection: " + ex.Message);
                return false;
            }
        }

        static void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Recruitment Console Menu ---");
                Console.WriteLine("1. Add Candidate");
                Console.WriteLine("2. Show All Candidates");
                Console.WriteLine("3. Delete Candidate");
                Console.WriteLine("4. Create Interview Task");
                Console.WriteLine("5. Show Unrated Interviews");
                Console.WriteLine("6. Evaluate Interview");
                Console.WriteLine("7. Add Final Verdict");
                Console.WriteLine("8. Show Candidate Decision");
                Console.WriteLine("9. Exit");
                Console.Write("Choose an option (1-9): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddCandidateFlow(); break;
                    case "2": DisplayAllCandidates(); break;
                    case "3": DeleteCandidateFlow(); break;
                    case "4": AssignTaskToInterviewFlow(); break;
                    case "5": ShowUnratedInterviews(); break;
                    case "6": EvaluateInterviewFlow(); break;
                    case "7": AddFinalVerdictFlow(); break; 
                    case "8": ShowCandidateDecisionFlow(); break;
                    case "9": return;
                    default: Console.WriteLine("Invalid choice. Please enter a number from 1 to 9."); break;
                }
            }
        }

        static void AddFinalVerdictFlow()
        {
            Console.Write("Enter Interview ID: ");
            if (!int.TryParse(Console.ReadLine(), out int interviewId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            Console.Write("Enter final verdict (Accepted / Rejected): ");
            string verdict = Console.ReadLine();

            if (verdict != "Accepted" && verdict != "Rejected")
            {
                Console.WriteLine("Verdict must be 'Accepted' or 'Rejected'.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    string updateQuery = "UPDATE Interviews SET FinalVerdict = @verdict WHERE Id = @id";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn, transaction))
                    {
                        updateCmd.Parameters.AddWithValue("@verdict", verdict);
                        updateCmd.Parameters.AddWithValue("@id", interviewId);
                        updateCmd.ExecuteNonQuery();
                    }

                    int candidateId;
                    string candidateName;
                    string selectQuery = @"
                        SELECT i.CandidateId, dbo.GetCandidateName(c.Info) AS CandidateName
                        FROM Interviews i
                        JOIN Candidates c ON i.CandidateId = c.Id
                        WHERE i.Id = @id";

                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn, transaction))
                    {
                        selectCmd.Parameters.AddWithValue("@id", interviewId);
                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                throw new Exception("Interview not found.");
                            }
                            candidateId = reader.GetInt32(0);
                            candidateName = reader.GetString(1);
                        }
                    }

                    string summaryString = $"{candidateName}|{verdict}";

                    string insertQuery = "INSERT INTO Summaries (CandidateId, Summary) VALUES (@candidateId, @summary)";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn, transaction))
                    {
                        insertCmd.Parameters.AddWithValue("@candidateId", candidateId);
                        insertCmd.Parameters.AddWithValue("@summary", summaryString);
                        insertCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    Console.WriteLine("Final verdict added and interview marked as evaluated.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error during verdict update: " + ex.Message);
                }
            }
        }




        static void AssignTaskToInterviewFlow()
        {
            Console.Write("Candidate ID: ");
            if (!int.TryParse(Console.ReadLine(), out int candidateId))
            {
                Console.WriteLine("Invalid Candidate ID.");
                return;
            }

            Console.Write("Interviewer Name: ");
            string interviewer = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(interviewer))
            {
                Console.WriteLine("Interviewer name cannot be empty.");
                return;
            }

            Console.Write("Interview Date (e.g., 2025-06-04): ");
            string dateStr = Console.ReadLine();

            if (!DateTime.TryParseExact(dateStr, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime interviewDate))
            {
                Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
                return;
            }
            if (interviewDate.Date < DateTime.Today)
            {
                Console.WriteLine("Interview date cannot be earlier than today.");
                return;
            }

            Console.Write("Task Title: ");
            string title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Task Title cannot be empty.");
                return;
            }

            Console.Write("Task Description: ");
            string description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("Task Description cannot be empty.");
                return;
            }

            var interview = new RecruitmentTypes.Interview
            {
                Interviewer = interviewer,
                Date = dateStr  
            };

            int interviewId;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand createCmd = new SqlCommand("CreateInterviewd", conn))
                {
                    createCmd.CommandType = CommandType.StoredProcedure;
                    createCmd.Parameters.AddWithValue("@CandidateId", candidateId);

                    var interviewParam = createCmd.Parameters.Add("@InterviewData", SqlDbType.Udt);
                    interviewParam.UdtTypeName = "Interview";
                    interviewParam.Value = interview;

                    var outputParam = new SqlParameter("@InterviewId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    createCmd.Parameters.Add(outputParam);

                    createCmd.ExecuteNonQuery();

                    interviewId = (int)outputParam.Value;
                }

                Console.WriteLine($"Interview created with ID: {interviewId}");

                // 2. Przypisanie zadania
                using (SqlCommand assignCmd = new SqlCommand("AssignTaskToInterview", conn))
                {
                    assignCmd.CommandType = CommandType.StoredProcedure;
                    assignCmd.Parameters.AddWithValue("@InterviewId", interviewId);
                    assignCmd.Parameters.AddWithValue("@CandidateId", candidateId);
                    assignCmd.Parameters.AddWithValue("@Title", title);
                    assignCmd.Parameters.AddWithValue("@Description", description);

                    assignCmd.ExecuteNonQuery();
                }

                Console.WriteLine("Task assigned to interview successfully.");
            }
        }




        static void ShowUnratedInterviews()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM UnratedInterviews", conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n--- Unrated Interviews ---");
                    while (reader.Read())
                    {
                        Console.WriteLine($"InterviewId: {reader["InterviewId"]}, CandidateId: {reader["CandidateId"]}, Name: {reader["Name"]}, Interviewer: {reader["Interviewer"]}, Date: {reader["InterviewDate"]}");
                    }
                }
            }
        }

        static void EvaluateInterviewFlow()
        {
            Console.Write("Interview ID: ");
            if (!int.TryParse(Console.ReadLine(), out int interviewId))
            {
                Console.WriteLine("Invalid Interview ID. Please enter a valid integer.");
                return;
            }

            Console.Write("Evaluator: ");
            string evaluator = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(evaluator))
            {
                Console.WriteLine("Evaluator name cannot be empty.");
                return;
            }

            Console.Write("Score (1-5): ");
            if (!int.TryParse(Console.ReadLine(), out int score) || score < 1 || score > 5)
            {
                Console.WriteLine("Score must be an integer between 1 and 5.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("EvaluateInterview", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InterviewId", interviewId);
                cmd.Parameters.AddWithValue("@Evaluator", evaluator);
                cmd.Parameters.AddWithValue("@Score", score);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Interview evaluated.");
        }


        static void ShowCandidateDecisionFlow()
        {
            Console.Write("Candidate ID: ");
            int candidateId = int.Parse(Console.ReadLine());

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("ShowCandidateDecision", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CandidateId", candidateId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Candidate: {reader["Name"]} ({reader["Email"]})");
                        Console.WriteLine($"Interviewer: {reader["Interviewer"]}, Date: {reader["InterviewDate"]}");
                        Console.WriteLine($"Task: {reader["TaskTitle"]}, Description: {reader["TaskDescription"]}");
                        Console.WriteLine($"Evaluator: {reader["Evaluator"]}, Score: {reader["Score"]}");
                        Console.WriteLine($"Final Status: {reader["FinalStatus"]}\n");
                    }
                }
            }
        }
        static void DeleteCandidate(int id)
        {
            string query = "DELETE FROM Candidates WHERE Id = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                conn.Open();
                int affectedRows = cmd.ExecuteNonQuery();

                if (affectedRows == 0)
                {
                    throw new Exception("No candidate deleted. Id might not exist.");
                }
            }
        }

        static bool CandidateExists(int id)
        {
            string query = "SELECT COUNT(*) FROM Candidates WHERE Id = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        static void DeleteCandidateFlow()
        {
            DisplayAllCandidates();

            Console.Write("Enter the Id of the candidate you want to delete: ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int candidateId))
            {
                Console.WriteLine("Invalid Id format. Please enter a numeric value.");
                return;
            }

            if (!CandidateExists(candidateId))
            {
                Console.WriteLine($"Candidate with Id {candidateId} does not exist.");
                return;
            }

            try
            {
                DeleteCandidate(candidateId);
                Console.WriteLine("Candidate deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting candidate: {ex.Message}");
            }
        }


        static void AddCandidateFlow()
        {
            Console.Write("Enter candidate name: ");
            string name = Console.ReadLine();

            Console.Write("Enter candidate email: ");
            string email = Console.ReadLine();

            try
            {
                InsertCandidate(name, email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting candidate: {ex.Message}");
            }
        }

        static void InsertCandidate(string name, string email)
        {
            string query = "INSERT INTO Candidates (Info) VALUES (CAST(@info AS Candidate))";
            string value = $"{name}|{email}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@info", SqlDbType.NVarChar).Value = value;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Inserted Candidate.");
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        Console.WriteLine("A candidate with this email already exists.");
                    }
                    if (ex.Number == 547) 
                    {
                        Console.WriteLine("Error: The email address you entered is invalid.");
                    }
                    else
                    {
                        Console.WriteLine($"SQL Error ({ex.Number}): {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General error: {ex.Message}");
                }
            }
        }

        static void DisplayAllCandidates()
        {
            string query = @"
                SELECT Id,
                       dbo.GetCandidateName(Info) AS Name,
                       dbo.GetCandidateEmail(Info) AS Email
                FROM Candidates";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\nCandidates:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"[{reader["Id"]}] {reader["Name"]} - {reader["Email"]}");
                    }
                }
            }
        }
    }
}