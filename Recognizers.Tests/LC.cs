using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Recognizers.Tests
{
    public abstract class LcTerm
    {
        public LcTerm Eval(ImmutableDictionary<string, LcTerm> env)
        {
            switch (this)
            {
                case LcVar v:
                    return env.TryGetValue(v.Name, out var x) ? x : null;
                case LcLambda lam:
                    return lam;
                case LcApply app:
                    var arg = app.Arg.Eval(env);
                    if (app.Lambda.Eval(env) is LcLambda l)
                        return l.Body.Eval(env.Add(l.Var, arg));
                    goto default;
                default:
                    throw new Exception("Impossible!");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string ToString()
        {
            switch (this)
            {
                case LcVar v:
                    return v.Name;
                case LcLambda lam:
                    return $"({lam.Var}->{lam.Body})";
                case LcApply app:
                    return $"{app.Lambda} {app.Arg}";
                default:
                    throw new Exception("Impossible!");
            }
        }

        static bool LcVar(Input inp, ref Position pos, out LcTerm x)
        {
            if (inp.Letters(ref pos, out var name))
            {
                x = new LcVar { Name = name.ToString(), Position = pos.Pos };
                return true;
            }
            x = default;
            return false;
        }

        static bool LcLambda(Input inp, ref Position pos, ref Rules rules, out LcTerm x)
        {
            var i = pos;
            if (inp.Letters(ref i, out var v) &&
                inp.Optional(inp.WhiteSpaces(ref i)) &&
                inp.Literal("->", ref i) &&
                Parse(inp, ref i, ref rules, out var body))
            {
                x = new LcLambda { Var = v.ToString(), Body = body };
                return pos.Seek(i, ref rules);
            }
            return Recognizers.Fail(out x);
        }

        static bool LcBracketed(Input inp, ref Position pos, ref Rules rules, out LcTerm x)
        {
            var i = pos;
            if (inp.Char('(', ref i) &&
                Parse(inp, ref i, ref rules, out x) &&
                inp.Optional(inp.WhiteSpaces(ref i)) &&
                inp.Char(')', ref i))
            {
                return pos.Seek(i, ref rules);
            }
            return Recognizers.Fail(out x);
        }

        static bool LcApply(Input inp, ref Position pos, ref Rules rules, out LcTerm x)
        {
            var i = pos;
            if (!rules.IsLoop(1) &&
                Parse(inp, ref i, ref rules, out var lambda) &&
                inp.WhiteSpaces(ref i) &&
                Parse(inp, ref i, ref rules, out var arg))
            {
                x = new LcApply { Lambda = lambda, Arg = arg };
                return pos.Seek(i, ref rules);
            }
            return Recognizers.Fail(out x);
        }

        static bool Parse(Input inp, ref Position pos, ref Rules rules, out LcTerm x)
        {
            inp.WhiteSpaces(ref pos);
            return LcApply(inp, ref pos, ref rules, out x)
                || LcLambda(inp, ref pos, ref rules, out x)
                || LcBracketed(inp, ref pos, ref rules, out x)
                || LcVar(inp, ref pos, out x);
        }

        public static LcTerm Parse(string input)
        {
            var inp = new Input(input);
            var pos = new Position();
            var rules = new Rules();
            return Parse(inp, ref pos, ref rules, out var x)
                ? x
                : throw new Exception("Parse error");
        }
    }

    sealed class LcVar : LcTerm
    {
        public string Name { get; set; }
        public int Position { get; set; }
    }

    sealed class LcLambda : LcTerm
    {
        public string Var { get; set; }
        public LcTerm Body { get; set; }
    }

    sealed class LcApply : LcTerm
    {
        public LcTerm Lambda { get; set; }
        public LcTerm Arg { get; set; }
    }
}
