using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecruitmentTypes;
using System.Data.SqlTypes;
using System.IO;

namespace RecruitmentTypesTests
{
    [TestClass]
    public class CandidateTests
    {
        [TestMethod]
        public void ToStringStatic_ShouldReturnFormattedString()
        {
            var candidate = new Candidate { Name = "Anna Nowak", Email = "anna@example.com" };
            var result = Candidate.ToStringStatic(candidate);

            Assert.AreEqual("Anna Nowak|anna@example.com", result.Value);
        }

        [TestMethod]
        public void GetName_ShouldReturnCandidateName()
        {
            var candidate = new Candidate { Name = "Jan Testowy", Email = "jan@test.pl" };
            var result = Candidate.GetName(candidate);

            Assert.AreEqual("Jan Testowy", result.Value);
        }

        [TestMethod]
        public void GetEmail_ShouldReturnCandidateEmail()
        {
            var candidate = new Candidate { Name = "Test", Email = "test@mail.com" };
            var result = Candidate.GetEmail(candidate);

            Assert.AreEqual("test@mail.com", result.Value);
        }

        [DataTestMethod]
        [DataRow("valid@example.com", true)]
        [DataRow("invalidexample.com", false)]
        [DataRow("invalid@.com", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public void IsValidEmail_ShouldValidateCorrectly(string email, bool expected)
        {
            var sqlEmail = email == null ? SqlString.Null : new SqlString(email);
            var result = Candidate.IsValidEmail(sqlEmail);

            Assert.AreEqual(expected, result.Value);
        }

        [TestMethod]
        public void Parse_ShouldReturnCandidateFromString()
        {
            var input = new SqlString("Maria Skłodowska|maria@lab.pl");
            var candidate = Candidate.Parse(input);

            Assert.AreEqual("Maria Skłodowska", candidate.Name);
            Assert.AreEqual("maria@lab.pl", candidate.Email);
        }

        [TestMethod]
        public void Parse_WithNull_ShouldReturnNullCandidate()
        {
            var result = Candidate.Parse(SqlString.Null);

            Assert.IsTrue(result.IsNull);
        }

        [TestMethod]
        public void BinarySerialization_ShouldPreserveValues()
        {
            var original = new Candidate { Name = "Test User", Email = "user@test.com" };
            var ms = new MemoryStream();
            try
            {
                var writer = new BinaryWriter(ms);
                try
                {
                    original.Write(writer);
                    writer.Flush();
                    ms.Position = 0;

                    var reader = new BinaryReader(ms);
                    var deserialized = new Candidate();
                    deserialized.Read(reader);

                    Assert.AreEqual(original.Name, deserialized.Name);
                    Assert.AreEqual(original.Email, deserialized.Email);
                }
                finally
                {
                    writer.Dispose();
                }
            }
            finally
            {
                ms.Dispose();
            }
        }
    }
}
