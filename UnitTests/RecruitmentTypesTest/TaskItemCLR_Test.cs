using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecruitmentTypes;
using System.Data.SqlTypes;
using System.IO;

namespace RecruitmentTypesTests
{
    [TestClass]
    public class TaskItemCLR_Test
    {
        [TestMethod]
        public void ToStringStatic_ShouldReturnFormattedString()
        {
            var task = new TaskItem { Title = "Task 1", Description = "Description 1" };
            var result = TaskItem.ToStringStatic(task);

            Assert.AreEqual("Task 1|Description 1", result.Value);
        }

        [TestMethod]
        public void GetTitle_ShouldReturnTitle()
        {
            var task = new TaskItem { Title = "Important Task", Description = "Some description" };
            var result = TaskItem.GetTitle(task);

            Assert.AreEqual("Important Task", result.Value);
        }

        [TestMethod]
        public void GetDescription_ShouldReturnDescription()
        {
            var task = new TaskItem { Title = "Another Task", Description = "Do it fast" };
            var result = TaskItem.GetDescription(task);

            Assert.AreEqual("Do it fast", result.Value);
        }

        [TestMethod]
        public void Parse_ShouldReturnTaskItemFromString()
        {
            var input = new SqlString("Fix Bug|Fix the login bug");
            var task = TaskItem.Parse(input);

            Assert.AreEqual("Fix Bug", task.Title);
            Assert.AreEqual("Fix the login bug", task.Description);
        }

        [TestMethod]
        public void Parse_WithNull_ShouldReturnNullTaskItem()
        {
            var result = TaskItem.Parse(SqlString.Null);

            Assert.IsTrue(result.IsNull);
        }

        [TestMethod]
        public void BinarySerialization_ShouldPreserveValues()
        {
            var original = new TaskItem { Title = "Serialize Task", Description = "Serialization test" };
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            original.Write(writer);
            writer.Flush();

            ms.Position = 0;

            TaskItem deserialized = new TaskItem();
            BinaryReader reader = new BinaryReader(ms);
            deserialized.Read(reader);

            Assert.AreEqual(original.Title, deserialized.Title);
            Assert.AreEqual(original.Description, deserialized.Description);
        }

        [TestMethod]
        public void ToString_ShouldReturnCorrectFormat()
        {
            var task = new TaskItem { Title = "Review Code", Description = "Review before release" };
            var result = task.ToString();

            Assert.AreEqual("Review Code|Review before release", result);
        }
    }
}
