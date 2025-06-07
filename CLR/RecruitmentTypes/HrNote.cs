using System;
using System.Data.SqlTypes;
using System.Globalization;
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

        public static HrNote Parse(SqlString input)
        {
            var parts = input.Value.Split('|');
            var text = parts[0];

            // To poprawnie obsługuje datę UTC
            var createdAt = DateTime.Parse(parts[1], null, DateTimeStyles.RoundtripKind);

            return new HrNote
            {
                Text = text,
                CreatedAt = createdAt
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
