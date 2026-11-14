using System;
using System.Collections.Immutable;
using Xunit;

namespace Recognizers.Tests
{
    /// <summary>
    /// Acceptance tests for the fixes carried over from cparse, numbered as in fixes.md.
    /// </summary>
    public static class RegressionTests
    {
        #region 1. Literal matches when it ends at the end of the input

        [Theory]
        [InlineData("->", "->", 0, true, 2)]
        [InlineData("ab", "xab", 1, true, 3)]
        [InlineData("ab", "a", 0, false, 0)]
        [InlineData("ab", "ab", 5, false, 5)]   // past the end: no exception
        [InlineData("", "ab", 0, false, 0)]     // consumes nothing
        public static void Literal(string text, string input, int start, bool expected, int end)
        {
            var x = new Input(input);
            var pos = new Position { Pos = start };
            Assert.Equal(expected, x.Literal(text, ref pos));
            Assert.Equal(end, pos.Pos);

            var pos2 = new Position { Pos = start };
            Assert.Equal(expected, x.Literal(text, ref pos2, out var capture));
            Assert.Equal(end, pos2.Pos);
            Assert.Equal(expected ? text : "", capture.ToString());
        }

        [Fact]
        public static void Literals()
        {
            var x = new Input("abab");
            var pos = new Position();
            Assert.True(x.Literals("ab", ref pos));
            Assert.Equal(4, pos.Pos);

            var pos2 = new Position();
            Assert.True(x.Literals("ab", ref pos2, out var capture));
            Assert.Equal(4, pos2.Pos);
            Assert.Equal("abab", capture.ToString());
        }

        [Fact]
        public static void LiteralIgnoreCase()
        {
            var x = new Input("ATTN");
            var pos = new Position();
            Assert.True(x.LiteralIgnoreCase("Attn", ref pos));
            Assert.Equal(4, pos.Pos);

            var y = new Input("ATTNattn");
            var pos2 = new Position();
            Assert.True(y.LiteralsIgnoreCase("Attn", ref pos2, out var capture));
            Assert.Equal(8, pos2.Pos);
            Assert.Equal("ATTNattn", capture.ToString());
        }

        #endregion

        #region 2. UntilLiteral stops at the end of the input, and before the literal

        [Theory]
        [InlineData("ab->c", true, "ab", 2)]
        [InlineData("ab->", true, "ab", 2)]
        [InlineData("abc", true, "abc", 3)]
        [InlineData("a-b", true, "a-b", 3)]
        [InlineData("->c", false, "", 0)]
        [InlineData("", false, "", 0)]
        public static void UntilLiteral(string input, bool expected, string captured, int end)
        {
            var x = new Input(input);
            var pos = new Position();
            Assert.Equal(expected, x.UntilLiteral("->", ref pos));
            Assert.Equal(end, pos.Pos);

            var pos2 = new Position();
            Assert.Equal(expected, x.UntilLiteral("->", ref pos2, out var capture));
            Assert.Equal(end, pos2.Pos);
            Assert.Equal(captured, capture.ToString());
        }

        [Fact]
        public static void UntilLiteralIgnoreCase()
        {
            var x = new Input("abEnd");
            var pos = new Position();
            Assert.True(x.UntilLiteralIgnoreCase("END".AsSpan(), ref pos, out var capture));
            Assert.Equal(2, pos.Pos);
            Assert.Equal("ab", capture.ToString());
        }

        #endregion

        #region 3. Lambda calculus application is left associative

        [Theory]
        [InlineData("x y z", "x y z", "App(App(x, y), z)")]
        [InlineData("a b c d", "a b c d", "App(App(App(a, b), c), d)")]
        [InlineData("a (b c)", "a (b c)", "App(a, App(b, c))")]
        [InlineData("(a b) c", "a b c", "App(App(a, b), c)")]
        [InlineData("x->a b c", "(x->a b c)", "Lam(x, App(App(a, b), c))")]
        [InlineData("a x->x b", "a (x->x b)", "App(a, Lam(x, App(x, b)))")]
        [InlineData(" x", "x", "x")]
        [InlineData("(x->x x) (y->y) (z->k)", "(x->x x) (y->y) (z->k)", "App(App(Lam(x, App(x, x)), Lam(y, y)), Lam(z, k))")]
        public static void LambdaCalculusApplication(string program, string printed, string tree)
        {
            var term = LcTerm.Parse(program);
            Assert.Equal(printed, term.ToString());
            Assert.Equal(tree, Tree(term));
            Assert.Equal(tree, Tree(LcTerm.Parse(printed)));
        }

        [Theory]
        [InlineData("x y )")]
        [InlineData("x ")]
        [InlineData("x->(y->y y")]
        [InlineData("(x->x")]
        [InlineData("")]
        public static void LambdaCalculusParseErrors(string program)
        {
            var e = Assert.Throws<Exception>(() => LcTerm.Parse(program));
            Assert.Equal("Parse error", e.Message);
        }

        [Fact]
        public static void LambdaCalculusEvalLeftAssociative()
        {
            var term = LcTerm.Parse("(x->x x) (y->y) (z->k)");
            var env = ImmutableDictionary.Create<string, LcTerm>();
            Assert.Equal("(z->k)", term.Eval(env)?.ToString());
        }

        #endregion

        #region 4. Rules tracks left recursion per position, and is passed by value

        [Fact]
        public static void RulesMarksArePerPosition()
        {
            var rules = new Rules();
            var at0 = new Position { Pos = 0 };
            var at1 = new Position { Pos = 1 };
            Assert.False(rules.IsLoop(at0, 3));
            Assert.True(rules.IsLoop(at0, 3));
            Assert.False(rules.IsLoop(at0, 4));
            Assert.False(rules.IsLoop(at1, 3));   // moving on clears the marks
            Assert.False(rules.IsLoop(at1, 4));
        }

        [Theory]
        [InlineData(0, 32)]
        [InlineData(7, -1)]
        public static void RulesRejectOutOfRangeIds(int at, int ruleId)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Rules().IsLoop(new Position { Pos = at }, ruleId));
        }

        [Theory]
        [InlineData("x y z", "App(x, App(y, z))")]
        [InlineData("(x->x) (y->y) (z->k)", "App(Lam(x, x), App(Lam(y, y), Lam(z, k)))")]
        [InlineData("x->(y->y y)", "Lam(x, Lam(y, App(y, y)))")]
        public static void LeftRecursiveParser(string program, string tree)
        {
            Assert.Equal(tree, Tree(LcLeftRecursive.Parse(program)));
        }

        [Theory]
        [InlineData("x->(y->y y")]
        [InlineData("(x->x")]
        [InlineData("x y )")]
        public static void LeftRecursiveParserErrors(string program)
        {
            var e = Assert.Throws<Exception>(() => LcLeftRecursive.Parse(program));
            Assert.Equal("Parse error", e.Message);
        }

        #endregion

        #region 5. Every single-character step is bounds-checked

        [Theory]
        [InlineData("a", 0, true, 1)]
        [InlineData("a", 1, false, 1)]
        [InlineData("", 0, false, 0)]
        public static void Step(string input, int start, bool expected, int end)
        {
            var x = new Input(input);
            var pos = new Position { Pos = start };
            Assert.Equal(expected, pos.Step(x));
            Assert.Equal(end, pos.Pos);

            var pos2 = new Position { Pos = start };
            Assert.Equal(expected, pos2.Step(x, out var capture));
            Assert.Equal(end, pos2.Pos);
            Assert.Equal(expected ? input.Substring(start, 1) : "", capture.ToString());
        }

        [Theory]
        [InlineData("", 0, false, 0)]
        [InlineData("z", 1, false, 1)]
        [InlineData("z", 0, false, 0)]
        [InlineData("d", 0, true, 1)]
        public static void CharWithFourAlternatives(string input, int start, bool expected, int end)
        {
            var x = new Input(input);
            var pos = new Position { Pos = start };
            Assert.Equal(expected, x.Char(ref pos, 'a', 'b', 'c', 'd'));
            Assert.Equal(end, pos.Pos);

            var pos2 = new Position { Pos = start };
            Assert.Equal(expected, x.Char(ref pos2, out var capture, 'a', 'b', 'c', 'd'));
            Assert.Equal(end, pos2.Pos);
            Assert.Equal(expected ? 1 : 0, capture.Length);
        }

        // H and L stand for a high and a low surrogate. They are substituted at run time, because
        // InlineData arguments are stored as UTF-8, which can't represent a lone surrogate.
        [Theory]
        [InlineData("H", false, 0)]
        [InlineData("Ha", false, 0)]
        [InlineData("LH", false, 0)]
        [InlineData("HL", true, 2)]
        [InlineData("HLHLx", true, 4)]
        public static void WhileSurrogatePair(string pattern, bool expected, int end)
        {
            var input = Surrogates(pattern);
            var x = new Input(input);
            var pos = new Position();
            Assert.Equal(expected, x.WhileSurrogatePair(ref pos));
            Assert.Equal(end, pos.Pos);

            var pos2 = new Position();
            Assert.Equal(expected, x.WhileSurrogatePair(ref pos2, out var capture));
            Assert.Equal(end, pos2.Pos);
            Assert.Equal(input.Substring(0, end), capture.ToString());
        }

        [Theory]
        [InlineData("aH", true, 2)]
        [InlineData("abHL", true, 2)]
        [InlineData("HL", false, 0)]
        [InlineData("HaHL", true, 2)]
        public static void UntilSurrogatePair(string pattern, bool expected, int end)
        {
            var input = Surrogates(pattern);
            var x = new Input(input);
            var pos = new Position();
            Assert.Equal(expected, x.UntilSurrogatePair(ref pos));
            Assert.Equal(end, pos.Pos);

            var pos2 = new Position();
            Assert.Equal(expected, x.UntilSurrogatePair(ref pos2, out var capture));
            Assert.Equal(end, pos2.Pos);
            Assert.Equal(input.Substring(0, end), capture.ToString());
        }

        static string Surrogates(string pattern) =>
            pattern.Replace('H', '\uD83D').Replace('L', '\uDE00');

        #endregion

        static string Tree(LcTerm term)
        {
            switch (term)
            {
                case LcVar v:
                    return v.Name;
                case LcLambda lam:
                    return $"Lam({lam.Var}, {Tree(lam.Body)})";
                case LcApply app:
                    return $"App({Tree(app.Lambda)}, {Tree(app.Arg)})";
                default:
                    throw new ArgumentException("Unknown term: " + term, nameof(term));
            }
        }
    }
}
