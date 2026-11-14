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
                    // application is left associative, so only an application in argument position needs brackets
                    return app.Arg is LcApply
                        ? $"{app.Lambda} ({app.Arg})"
                        : $"{app.Lambda} {app.Arg}";
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

        static bool LcLambda(Input inp, ref Position pos, out LcTerm x)
        {
            var i = pos;
            if (inp.Letters(ref i, out var v) &&
                inp.Optional(inp.WhiteSpaces(ref i)) &&
                inp.Literal("->", ref i) &&
                Parse(inp, ref i, out var body))
            {
                x = new LcLambda { Var = v.ToString(), Body = body };
                return pos.Seek(i);
            }
            return Recognizers.Fail(out x);
        }

        static bool LcBracketed(Input inp, ref Position pos, out LcTerm x)
        {
            var i = pos;
            if (inp.Char('(', ref i) &&
                Parse(inp, ref i, out x) &&
                inp.Optional(inp.WhiteSpaces(ref i)) &&
                inp.Char(')', ref i))
            {
                return pos.Seek(i);
            }
            return Recognizers.Fail(out x);
        }

        // operand ::= v->e | (e) | v
        // A lambda's body extends as far right as possible.
        static bool LcOperand(Input inp, ref Position pos, out LcTerm x) =>
               LcLambda(inp, ref pos, out x)
            || LcBracketed(inp, ref pos, out x)
            || LcVar(inp, ref pos, out x);

        // e ::= operand (whitespace operand)*
        // Application is folded to the left: a b c = (a b) c.
        static bool Parse(Input inp, ref Position pos, out LcTerm x)
        {
            var i = pos;
            inp.WhiteSpaces(ref i);
            if (!LcOperand(inp, ref i, out x))
                return false;
            var j = i;
            while (inp.WhiteSpaces(ref j) && LcOperand(inp, ref j, out var arg))
            {
                x = new LcApply { Lambda = x, Arg = arg };
                i = j;
            }
            return pos.Seek(i);
        }

        public static LcTerm Parse(string input)
        {
            var inp = new Input(input);
            var pos = new Position();
            return Parse(inp, ref pos, out var x) && inp.End(pos)
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
