using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.IO;

namespace RecruitmentTypes
{
    [Serializable]
    [SqlUserDefinedType(Format.UserDefined, MaxByteSize = 8000)]
    public struct TaskItem : INullable, IBinarySerialize
    {
        private bool isNull;
        public bool IsNull => isNull;
        public static TaskItem Null => new TaskItem { isNull = true };

        public string Title { get; set; }
        public string Description { get; set; }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString ToStringStatic(TaskItem taskItem)
        {
            if (taskItem.IsNull)
                return SqlString.Null;
            return new SqlString(taskItem.ToString());
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString GetTitle(TaskItem taskItem)
        {
            return taskItem.IsNull ? SqlString.Null : new SqlString(taskItem.Title);
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString GetDescription(TaskItem taskItem)
        {
            return taskItem.IsNull ? SqlString.Null : new SqlString(taskItem.Description);
        }

        public override string ToString() => $"{Title}|{Description}";

        public static TaskItem Parse(SqlString s)
        {
            if (s.IsNull) return Null;
            var parts = s.Value.Split('|');
            return new TaskItem { Title = parts[0], Description = parts[1] };
        }

        public void Read(BinaryReader r)
        {
            Title = r.ReadString();
            Description = r.ReadString();
        }

        public void Write(BinaryWriter w)
        {
            w.Write(Title ?? "");
            w.Write(Description ?? "");
        }
    }
}
