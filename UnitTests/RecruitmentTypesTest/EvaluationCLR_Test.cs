using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecruitmentTypes;
using System.Data.SqlTypes;
using System.IO;

namespace RecruitmentTypesTests
{
    [TestClass]
    public class EvaluationCLR_Test
    {
        [TestMethod]
        public void ToStringStatic_ShouldReturnFormattedString()
        {
            var evaluation = new Evaluation { Evaluator = "John Doe", Score = 85 };
            var result = Evaluation.ToStringStatic(evaluation);

            Assert.AreEqual("John Doe|85", result.Value);
        }

        [TestMethod]
        public void GetEvaluator_ShouldReturnEvaluatorName()
        {
            var evaluation = new Evaluation { Evaluator = "Anna Kowalska", Score = 90 };
            var result = Evaluation.GetEvaluator(evaluation);

            Assert.AreEqual("Anna Kowalska", result.Value);
        }

        [TestMethod]
        public void GetScore_ShouldReturnEvaluationScore()
        {
            var evaluation = new Evaluation { Evaluator = "Tester", Score = 75 };
            var result = Evaluation.GetScore(evaluation);

            Assert.AreEqual(75, result.Value);
        }

        [TestMethod]
        public void ToStringStatic_ShouldReturnNullForNullEvaluation()
        {
            var evaluation = Evaluation.Null;
            var result = Evaluation.ToStringStatic(evaluation);

            Assert.IsTrue(result.IsNull);
        }

        [TestMethod]
        public void GetEvaluator_ShouldReturnNullForNullEvaluation()
        {
            var evaluation = Evaluation.Null;
            var result = Evaluation.GetEvaluator(evaluation);

            Assert.IsTrue(result.IsNull);
        }

        [TestMethod]
        public void GetScore_ShouldReturnNullForNullEvaluation()
        {
            var evaluation = Evaluation.Null;
            var result = Evaluation.GetScore(evaluation);

            Assert.IsTrue(result.IsNull);
        }

        [TestMethod]
        public void Parse_ShouldReturnEvaluationFromString()
        {
            var input = new SqlString("Maria Skłodowska|100");
            var evaluation = Evaluation.Parse(input);

            Assert.AreEqual("Maria Skłodowska", evaluation.Evaluator);
            Assert.AreEqual(100, evaluation.Score);
        }

        [TestMethod]
        public void Parse_WithNull_ShouldReturnNullEvaluation()
        {
            var result = Evaluation.Parse(SqlString.Null);

            Assert.IsTrue(result.IsNull);
        }

        [TestMethod]
        public void BinarySerialization_ShouldPreserveValues()
        {
            var original = new Evaluation { Evaluator = "Binary Tester", Score = 55 };
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    original.Write(writer);
                }

                ms.Position = 0;

                var deserialized = new Evaluation();
                using (var reader = new BinaryReader(ms))
                {
                    deserialized.Read(reader);
                }

                Assert.AreEqual(original.Evaluator, deserialized.Evaluator);
                Assert.AreEqual(original.Score, deserialized.Score);
            }
        }
    }
}



