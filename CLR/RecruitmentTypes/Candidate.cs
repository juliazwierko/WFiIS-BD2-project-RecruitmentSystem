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
    public struct Candidate : INullable, IBinarySerialize
    {
        private bool isNull;
        public bool IsNull => isNull;

        public static Candidate Null => new Candidate { isNull = true };

        public string Name { get; set; }
        public string Email { get; set; }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString ToStringStatic(Candidate candidate)
        {
            if (candidate.IsNull)
                return SqlString.Null;
            return new SqlString(candidate.ToString());
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString GetName(Candidate candidate)
        {
            return candidate.IsNull ? SqlString.Null : new SqlString(candidate.Name);
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString GetEmail(Candidate candidate)
        {
            return candidate.IsNull ? SqlString.Null : new SqlString(candidate.Email);
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlBoolean IsValidEmail(SqlString email)
        {
            if (email.IsNull) return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return System.Text.RegularExpressions.Regex.IsMatch(email.Value, pattern);
        }
        public override string ToString() => $"{Name}|{Email}";

        public static Candidate Parse(SqlString s)
        {
            if (s.IsNull) return Null;
            var parts = s.Value.Split('|');
            return new Candidate { Name = parts[0], Email = parts[1] };
        }

        public void Read(BinaryReader r)
        {
            Name = r.ReadString();
            Email = r.ReadString();
        }

        public void Write(BinaryWriter w)
        {
            w.Write(Name ?? "");
            w.Write(Email ?? "");
        }
    }
}
