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
    public struct Evaluation : INullable, IBinarySerialize
    {
        private bool isNull;
        public bool IsNull => isNull;
        public static Evaluation Null => new Evaluation { isNull = true };

        public string Evaluator { get; set; }
        public int Score { get; set; }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString ToStringStatic(Evaluation evaluation)
        {
            if (evaluation.IsNull)
                return SqlString.Null;
            return new SqlString(evaluation.ToString());
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString GetEvaluator(Evaluation evaluation)
        {
            return evaluation.IsNull ? SqlString.Null : new SqlString(evaluation.Evaluator);
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlInt32 GetScore(Evaluation evaluation)
        {
            return evaluation.IsNull ? SqlInt32.Null : new SqlInt32(evaluation.Score);
        }

        public override string ToString() => $"{Evaluator}|{Score}";

        public static Evaluation Parse(SqlString s)
        {
            if (s.IsNull) return Null;
            var parts = s.Value.Split('|');
            return new Evaluation { Evaluator = parts[0], Score = int.Parse(parts[1]) };
        }

        public void Read(BinaryReader r)
        {
            Evaluator = r.ReadString();
            Score = r.ReadInt32();
        }

        public void Write(BinaryWriter w)
        {
            w.Write(Evaluator ?? "");
            w.Write(Score);
        }
    }
}
