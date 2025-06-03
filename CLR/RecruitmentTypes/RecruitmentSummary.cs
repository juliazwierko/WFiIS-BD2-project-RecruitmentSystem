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
    public struct RecruitmentSummary : INullable, IBinarySerialize
    {
        private bool isNull;
        public bool IsNull => isNull;
        public static RecruitmentSummary Null => new RecruitmentSummary { isNull = true };

        public string CandidateName { get; set; }
        public string Status { get; set; }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString ToStringStatic(RecruitmentSummary recruitmentSummary)
        {
            if (recruitmentSummary.IsNull)
                return SqlString.Null;
            return new SqlString(recruitmentSummary.ToString());
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString GetCandidateNameSum(RecruitmentSummary recruitmentSummary)
        {
            return recruitmentSummary.IsNull ? SqlString.Null : new SqlString(recruitmentSummary.CandidateName);
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString GetStatusSum(RecruitmentSummary recruitmentSummary)
        {
            return recruitmentSummary.IsNull ? SqlString.Null : new SqlString(recruitmentSummary.Status);
        }

        public override string ToString() => $"{CandidateName}|{Status}";

        public static RecruitmentSummary Parse(SqlString s)
        {
            if (s.IsNull) return Null;
            var parts = s.Value.Split('|');
            return new RecruitmentSummary { CandidateName = parts[0], Status = parts[1] };
        }

        public void Read(BinaryReader r)
        {
            CandidateName = r.ReadString();
            Status = r.ReadString();
        }

        public void Write(BinaryWriter w)
        {
            w.Write(CandidateName ?? "");
            w.Write(Status ?? "");
        }
    }
}
