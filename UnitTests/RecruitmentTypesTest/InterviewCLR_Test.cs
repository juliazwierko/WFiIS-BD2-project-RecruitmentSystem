using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecruitmentTypes;
using System.Data.SqlTypes;
using System.IO;

namespace RecruitmentTypesTests
{
    [TestClass]
    public class InterviewTests
    {
        [TestMethod]
        public void ToStringStatic_ShouldReturnFormattedString()
        {
            var interview = new Interview { Interviewer = "Krzysztof Kowalski", Date = "2025-06-04" };
            var result = Interview.ToStringStatic(interview);

            Assert.AreEqual("Krzysztof Kowalski|2025-06-04", result.Value);
        }

        [TestMethod]
        public void GetInterviewer_ShouldReturnInterviewer()
        {
            var interview = new Interview { Interviewer = "Anna Nowak", Date = "2025-01-15" };
            var result = Interview.GetInterviewer(interview);

            Assert.AreEqual("Anna Nowak", result.Value);
        }

        [TestMethod]
        public void GetDate_ShouldReturnDate()
        {
            var interview = new Interview { Interviewer = "Jan Testowy", Date = "2024-12-31" };
            var result = Interview.GetDate(interview);

            Assert.AreEqual("2024-12-31", result.Value);
        }

        [TestMethod]
        public void Parse_ShouldReturnInterviewFromString()
        {
            var input = new SqlString("Maria Skłodowska|2025-03-01");
            var interview = Interview.Parse(input);

            Assert.AreEqual("Maria Skłodowska", interview.Interviewer);
            Assert.AreEqual("2025-03-01", interview.Date);
        }

        [TestMethod]
        public void Parse_WithNull_ShouldReturnNullInterview()
        {
            var result = Interview.Parse(SqlString.Null);

            Assert.IsTrue(result.IsNull);
        }

        [TestMethod]
        public void BinarySerialization_ShouldPreserveValues()
        {
            var original = new Interview { Interviewer = "Binary Tester", Date = "2025-06-04" };
            using (var ms = new MemoryStream())
            {
                var writer = new BinaryWriter(ms);
                original.Write(writer);
                writer.Flush();

                ms.Position = 0;

                var deserialized = new Interview();
                using (var reader = new BinaryReader(ms))
                {
                    deserialized.Read(reader);
                }

                Assert.AreEqual(original.Interviewer, deserialized.Interviewer);
                Assert.AreEqual(original.Date, deserialized.Date);
            }
        }

        [TestMethod]
        public void ToString_ShouldReturnCorrectFormat()
        {
            var interview = new Interview { Interviewer = "Ewa Malinowska", Date = "2024-11-11" };
            var result = interview.ToString();

            Assert.AreEqual("Ewa Malinowska|2024-11-11", result);
        }
    }
}
