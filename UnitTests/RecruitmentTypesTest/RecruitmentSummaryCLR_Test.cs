using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecruitmentTypes;
using System.Data.SqlTypes;
using System.IO;

namespace RecruitmentTypesTests
{
    [TestClass]
    public class RecruitmentSummaryCLR_Test
    {
        [TestMethod]
        public void ToStringStatic_ShouldReturnFormattedString()
        {
            var summary = new RecruitmentSummary { CandidateName = "Jan Kowalski", Status = "Accepted" };
            var result = RecruitmentSummary.ToStringStatic(summary);

            Assert.AreEqual("Jan Kowalski|Accepted", result.Value);
        }

        [TestMethod]
        public void GetCandidateNameSum_ShouldReturnCandidateName()
        {
            var summary = new RecruitmentSummary { CandidateName = "Anna Nowak", Status = "Pending" };
            var result = RecruitmentSummary.GetCandidateNameSum(summary);

            Assert.AreEqual("Anna Nowak", result.Value);
        }

        [TestMethod]
        public void GetStatusSum_ShouldReturnStatus()
        {
            var summary = new RecruitmentSummary { CandidateName = "Marek Malinowski", Status = "Rejected" };
            var result = RecruitmentSummary.GetStatusSum(summary);

            Assert.AreEqual("Rejected", result.Value);
        }

        [TestMethod]
        public void Parse_ShouldReturnRecruitmentSummaryFromString()
        {
            var input = new SqlString("Ewa Zielinska|Interview Scheduled");
            var summary = RecruitmentSummary.Parse(input);

            Assert.AreEqual("Ewa Zielinska", summary.CandidateName);
            Assert.AreEqual("Interview Scheduled", summary.Status);
        }

        [TestMethod]
        public void Parse_WithNull_ShouldReturnNullRecruitmentSummary()
        {
            var result = RecruitmentSummary.Parse(SqlString.Null);

            Assert.IsTrue(result.IsNull);
        }

        [TestMethod]
        public void BinarySerialization_ShouldPreserveValues()
        {
            var original = new RecruitmentSummary { CandidateName = "Binary Test", Status = "Completed" };
            using (var ms = new MemoryStream())
            {
                var writer = new BinaryWriter(ms);
                original.Write(writer);
                writer.Flush();

                ms.Position = 0;

                var deserialized = new RecruitmentSummary();
                using (var reader = new BinaryReader(ms))
                {
                    deserialized.Read(reader);
                }

                Assert.AreEqual(original.CandidateName, deserialized.CandidateName);
                Assert.AreEqual(original.Status, deserialized.Status);
            }
        }

        [TestMethod]
        public void ToString_ShouldReturnCorrectFormat()
        {
            var summary = new RecruitmentSummary { CandidateName = "Paweł Nowak", Status = "On Hold" };
            var result = summary.ToString();

            Assert.AreEqual("Paweł Nowak|On Hold", result);
        }
    }
}
