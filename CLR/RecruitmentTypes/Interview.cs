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
    public struct Interview : INullable, IBinarySerialize
    {
        private bool isNull;
        public bool IsNull => isNull;
        public static Interview Null => new Interview { isNull = true };

        public string Interviewer { get; set; }
        public string Date { get; set; } // lub DateTime jeśli chcesz

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString ToStringStatic(Interview interview)
        {
            if (interview.IsNull)
                return SqlString.Null;
            return new SqlString(interview.ToString());
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString GetInterviewer(Interview interview)
        {
            return interview.IsNull ? SqlString.Null : new SqlString(interview.Interviewer);
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString GetDate(Interview interview)
        {
            return interview.IsNull ? SqlString.Null : new SqlString(interview.Date);
        }

        public override string ToString() => $"{Interviewer}|{Date}";

        public static Interview Parse(SqlString s)
        {
            if (s.IsNull) return Null;
            var parts = s.Value.Split('|');
            return new Interview { Interviewer = parts[0], Date = parts[1] };
        }

        public void Read(BinaryReader r)
        {
            Interviewer = r.ReadString();
            Date = r.ReadString();
        }

        public void Write(BinaryWriter w)
        {
            w.Write(Interviewer ?? "");
            w.Write(Date ?? "");
        }
    }
}
