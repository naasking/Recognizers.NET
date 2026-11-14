using System;

namespace Recognizers.Tests
{
    /// <summary>
    /// The original left-recursive lambda calculus parser, with application guarded by
    /// <see cref="Rules"/> passed by value.
    /// </summary>
    /// <remarks>
    /// The guard stops left recursion rather than implementing it, so this parses application as
    /// right associative: <c>x y z</c> is <c>x (y z)</c>. <see cref="LcTerm.Parse(string)"/> is the
    /// left-associative parser. This one is kept as a test fixture for <see cref="Rules"/>.
    /// </remarks>
    static class LcLeftRecursive
    {
        const int ApplyRule = 1;

        static bool Var(Input inp, ref Position pos, out LcTerm x)
        {
            if (inp.Letters(ref pos, out var name))
            {
                x = new LcVar { Name = name.ToString(), Position = pos.Pos };
                return true;
            }
            return Recognizers.Fail(out x);
        }

        static bool Lambda(Input inp, ref Position pos, Rules rules, out LcTerm x)
        {
            var i = pos;
            if (inp.Letters(ref i, out var v) &&
                inp.Optional(inp.WhiteSpaces(ref i)) &&
                inp.Literal("->", ref i) &&
                Parse(inp, ref i, rules, out var body))
            {
                x = new LcLambda { Var = v.ToString(), Body = body };
                return pos.Seek(i);
            }
            return Recognizers.Fail(out x);
        }

        static bool Bracketed(Input inp, ref Position pos, Rules rules, out LcTerm x)
        {
            var i = pos;
            if (inp.Char('(', ref i) &&
                Parse(inp, ref i, rules, out x) &&
                inp.Optional(inp.WhiteSpaces(ref i)) &&
                inp.Char(')', ref i))
            {
                return pos.Seek(i);
            }
            return Recognizers.Fail(out x);
        }

        // e ::= e e, which may not call itself again at the same position
        static bool Apply(Input inp, ref Position pos, Rules rules, out LcTerm x)
        {
            var i = pos;
            if (!rules.IsLoop(i, ApplyRule) &&
                Parse(inp, ref i, rules, out var f) &&
                inp.WhiteSpaces(ref i) &&
                Parse(inp, ref i, rules, out var a))
            {
                x = new LcApply { Lambda = f, Arg = a };
                return pos.Seek(i);
            }
            return Recognizers.Fail(out x);
        }

        static bool Parse(Input inp, ref Position pos, Rules rules, out LcTerm x)
        {
            var i = pos;
            inp.WhiteSpaces(ref i);
            return (Lambda(inp, ref i, rules, out x)
                 || Apply(inp, ref i, rules, out x)
                 || Bracketed(inp, ref i, rules, out x)
                 || Var(inp, ref i, out x))
                && pos.Seek(i);
        }

        public static LcTerm Parse(string input)
        {
            var inp = new Input(input);
            var pos = new Position();
            return Parse(inp, ref pos, new Rules(), out var x) && inp.End(pos)
                ? x
                : throw new Exception("Parse error");
        }
    }
}
