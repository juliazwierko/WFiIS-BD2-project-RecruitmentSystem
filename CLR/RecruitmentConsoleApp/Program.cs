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
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option (1-3): ");

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
                        Console.WriteLine("Exiting application.");
                        return;  // wyjście z programu
                    default:
                        Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                        break;
                }
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