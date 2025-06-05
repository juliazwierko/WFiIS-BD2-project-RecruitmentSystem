using System;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;

namespace RecruitmentTypes
{
    [Serializable]
    [SqlUserDefinedType(Format.UserDefined, MaxByteSize = 8000)]
    public struct HrNote : INullable, IBinarySerialize
    {
        private bool isNull;
        public bool IsNull => isNull;
        public static HrNote Null => new HrNote { isNull = true };

        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString() => $"{Text}|{CreatedAt:o}";

        public static HrNote Parse(SqlString s)
        {
            if (s.IsNull) return Null;

            var parts = s.Value.Split('|');
            return new HrNote
            {
                Text = parts[0],
                CreatedAt = DateTime.Parse(parts[1])
            };
        }

        public void Read(BinaryReader r)
        {
            Text = r.ReadString();
            CreatedAt = DateTime.Parse(r.ReadString());
        }

        public void Write(BinaryWriter w)
        {
            w.Write(Text ?? "");
            w.Write(CreatedAt.ToString("o")); 
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString GetText(HrNote note) =>
            note.IsNull ? SqlString.Null : new SqlString(note.Text);

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlDateTime GetCreatedAt(HrNote note) =>
            note.IsNull ? SqlDateTime.Null : new SqlDateTime(note.CreatedAt);
    }
}
