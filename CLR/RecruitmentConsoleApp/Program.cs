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
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option (1-4): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddCandidateFlow();
                        break;
                    case "2":
                        DisplayAllCandidates();
                        break;
                    case "3":
                        DeleteCandidateFlow();
                        break;
                    case "4":
                        Console.WriteLine("Exiting application.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number from 1 to 4.");
                        break;
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

                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Inserted Candidate.");
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
