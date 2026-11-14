using System;
using Xunit;

namespace Recognizers.Tests
{
    /// <summary>
    /// Tests for <see cref="Recognizers.Grow{T}(Input, ref Position, LeftRecursiveRule{T}, out T)"/>.
    /// </summary>
    public static class LeftRecursionTests
    {
        #region e ::= e '-' t | t, t ::= t '*' f | f, f ::= letter | '(' e ')'

        static int factors;   // calls to Factor, to check that parsing stays linear

        static readonly LeftRecursiveRule<string> Minus = (Input x, ref Position pos, LeftRecursion<string> e, out string tree) =>
        {
            var i = pos;
            if (e.Recurse(x, ref i, out var left) && x.Char('-', ref i) && Product(x, ref i, out var right))
            {
                tree = $"({left}-{right})";
                return pos.Seek(i);
            }
            i = pos;
            return !e.Seeded && Product(x, ref i, out tree) && pos.Seek(i)
                || Recognizers.Fail(out tree);
        };

        static readonly LeftRecursiveRule<string> Times = (Input x, ref Position pos, LeftRecursion<string> t, out string tree) =>
        {
            var i = pos;
            if (t.Recurse(x, ref i, out var left) && x.Char('*', ref i) && Factor(x, ref i, out var right))
            {
                tree = $"({left}*{right})";
                return pos.Seek(i);
            }
            i = pos;
            return !t.Seeded && Factor(x, ref i, out tree) && pos.Seek(i)
                || Recognizers.Fail(out tree);
        };

        static bool Expr(Input x, ref Position pos, out string tree) =>
            x.Grow(ref pos, Minus, out tree);

        static bool Product(Input x, ref Position pos, out string tree) =>
            x.Grow(ref pos, Times, out tree);

        static bool Factor(Input x, ref Position pos, out string tree)
        {
            ++factors;
            var i = pos;
            if (x.Letter(ref i, out var name))
            {
                tree = name.ToString();
                return pos.Seek(i);
            }
            if (x.Char('(', ref i) && Expr(x, ref i, out tree) && x.Char(')', ref i))
                return pos.Seek(i);
            return Recognizers.Fail(out tree);
        }

        [Theory]
        [InlineData("a", "a")]
        [InlineData("a-b", "(a-b)")]
        [InlineData("a-b-c", "((a-b)-c)")]
        [InlineData("a-b-c-d", "(((a-b)-c)-d)")]
        [InlineData("a-(b-c)-d", "((a-(b-c))-d)")]
        [InlineData("(a-b)-(c-d)", "((a-b)-(c-d))")]
        [InlineData("a*b*c", "((a*b)*c)")]
        [InlineData("a-b*c-d", "((a-(b*c))-d)")]
        [InlineData("a*b-c*d", "((a*b)-(c*d))")]
        [InlineData("a*(b-c)*d", "((a*(b-c))*d)")]
        public static void LeftAssociative(string input, string tree)
        {
            var x = new Input(input);
            var pos = new Position();
            Assert.True(Expr(x, ref pos, out var parsed));
            Assert.Equal(tree, parsed);
            Assert.True(x.End(pos));
        }

        [Theory]
        [InlineData("", false, null, 0)]
        [InlineData("-", false, null, 0)]
        [InlineData("(a", false, null, 0)]
        [InlineData("a-", true, "a", 1)]
        [InlineData("a*", true, "a", 1)]
        [InlineData("a-b-", true, "(a-b)", 3)]
        [InlineData("a*b-", true, "(a*b)", 3)]
        [InlineData("a-b)", true, "(a-b)", 3)]
        public static void LongestParseWins(string input, bool expected, string tree, int end)
        {
            var x = new Input(input);
            var pos = new Position();
            Assert.Equal(expected, Expr(x, ref pos, out var parsed));
            Assert.Equal(tree, parsed);
            Assert.Equal(end, pos.Pos);
        }

        // Without the guards, the 10 nested brackets take 5,592,404 calls to Factor.
        [Theory]
        [InlineData("a", 1)]
        [InlineData("(a-b)*(c-d)-e", 7)]
        [InlineData("((((((((((a))))))))))", 11)]
        public static void ParsingStaysLinear(string input, int calls)
        {
            factors = 0;
            var pos = new Position();
            Assert.True(Expr(new Input(input), ref pos, out _));
            Assert.Equal(calls, factors);
        }

        #endregion

        #region A missing guard

        // e ::= e '-' letter | letter, without !e.Seeded on its second alternative
        static readonly LeftRecursiveRule<string> Unguarded = (Input x, ref Position pos, LeftRecursion<string> e, out string tree) =>
        {
            var i = pos;
            if (e.Recurse(x, ref i, out var left) && x.Char('-', ref i) && x.Letter(ref i, out var right))
            {
                tree = $"({left}-{right.ToString()})";
                return pos.Seek(i);
            }
            i = pos;
            if (x.Letter(ref i, out var name))
            {
                tree = name.ToString();
                return pos.Seek(i);
            }
            return Recognizers.Fail(out tree);
        };

        [Theory]
        [InlineData("a")]
        [InlineData("a-b-c")]
        public static void MissingGuardThrows(string input)
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var pos = new Position();
                return new Input(input).Grow(ref pos, Unguarded, out _);
            });
        }

        [Fact]
        public static void MissingGuardOnlyThrowsAfterAMatch()
        {
            var pos = new Position();
            Assert.False(new Input("-").Grow(ref pos, Unguarded, out _));
            Assert.Equal(0, pos.Pos);
        }

        #endregion

        #region Application: left recursive only, and both left and right recursive

        // a ::= a ' ' v | v
        static readonly LeftRecursiveRule<string> LeftApply = (Input x, ref Position pos, LeftRecursion<string> a, out string tree) =>
        {
            var i = pos;
            if (a.Recurse(x, ref i, out var f) && x.Char(' ', ref i) && x.Letter(ref i, out var arg))
            {
                tree = $"({f} {arg.ToString()})";
                return pos.Seek(i);
            }
            i = pos;
            if (!a.Seeded && x.Letter(ref i, out var v))
            {
                tree = v.ToString();
                return pos.Seek(i);
            }
            return Recognizers.Fail(out tree);
        };

        // b ::= b ' ' b | v
        static readonly LeftRecursiveRule<string> BothApply = (Input x, ref Position pos, LeftRecursion<string> b, out string tree) =>
        {
            var i = pos;
            if (b.Recurse(x, ref i, out var f) && x.Char(' ', ref i) && b.Recurse(x, ref i, out var arg))
            {
                tree = $"({f} {arg})";
                return pos.Seek(i);
            }
            i = pos;
            if (!b.Seeded && x.Letter(ref i, out var v))
            {
                tree = v.ToString();
                return pos.Seek(i);
            }
            return Recognizers.Fail(out tree);
        };

        [Theory]
        [InlineData("x y z", "((x y) z)", "(x (y z))")]
        [InlineData("w x y z", "(((w x) y) z)", "(w (x (y z)))")]
        public static void Associativity(string input, string leftOnly, string both)
        {
            var x = new Input(input);
            var pos = new Position();
            Assert.True(x.Grow(ref pos, LeftApply, out var tree));
            Assert.Equal(leftOnly, tree);
            Assert.True(x.End(pos));

            // the right operand grows greedily too, so this is right associative
            var pos2 = new Position();
            Assert.True(x.Grow(ref pos2, BothApply, out var tree2));
            Assert.Equal(both, tree2);
            Assert.True(x.End(pos2));
        }

        #endregion

        #region n ::= n '-' digits | digits, evaluated

        static readonly LeftRecursiveRule<int> Difference = (Input x, ref Position pos, LeftRecursion<int> n, out int value) =>
        {
            var i = pos;
            if (n.Recurse(x, ref i, out var left) && x.Char('-', ref i) && Number(x, ref i, out var right))
            {
                value = left - right;
                return pos.Seek(i);
            }
            i = pos;
            return !n.Seeded && Number(x, ref i, out value) && pos.Seek(i)
                || Recognizers.Fail(out value);
        };

        static bool Number(Input x, ref Position pos, out int value)
        {
            value = 0;
            if (!x.Digits(ref pos, out var digits))
                return false;
            foreach (var c in digits)
                value = value * 10 + (c - '0');
            return true;
        }

        [Theory]
        [InlineData("9", 0, 9, 1)]
        [InlineData("9-3-2", 0, 4, 5)]
        [InlineData("10-2-3-4", 0, 1, 8)]
        [InlineData("x9-3-2", 1, 4, 6)]
        public static void Evaluate(string input, int start, int expected, int end)
        {
            var x = new Input(input);
            var pos = new Position { Pos = start };
            Assert.True(x.Grow(ref pos, Difference, out var value));
            Assert.Equal(expected, value);
            Assert.Equal(end, pos.Pos);
        }

        [Fact]
        public static void DoesNotAllocate()
        {
            var x = new Input("10-2-3-4");
            var warm = new Position();
            x.Grow(ref warm, Difference, out _);

            var before = GC.GetAllocatedBytesForCurrentThread();
            var pos = new Position();
            var ok = x.Grow(ref pos, Difference, out var value);
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.True(ok);
            Assert.Equal(1, value);
            Assert.Equal(0, allocated);
        }

        #endregion

        [Fact]
        public static void NullRule()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var pos = new Position();
                return new Input("a").Grow<string>(ref pos, null, out _);
            });
        }
    }
}
