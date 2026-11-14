using System;

namespace Recognizers
{
    /// <summary>
    /// A left-recursive rule, recognized by <see cref="Recognizers.Grow{T}(Input, ref Position, LeftRecursiveRule{T}, out T)"/>.
    /// </summary>
    /// <typeparam name="T">The type of value the rule produces.</typeparam>
    /// <param name="x">Apply the rule to this input.</param>
    /// <param name="pos">The position at which the rule should start processing.</param>
    /// <param name="self">
    /// The rule's recursive calls to itself go through <see cref="LeftRecursion{T}.Recurse"/> on this.
    /// Once <see cref="LeftRecursion{T}.Seeded"/> is true, the rule must skip its other alternatives.
    /// </param>
    /// <param name="value">The value the rule produces.</param>
    /// <returns>True if the input at the given position matches the rule.</returns>
    public delegate bool LeftRecursiveRule<T>(Input x, ref Position pos, LeftRecursion<T> self, out T value);

    /// <summary>
    /// The handle through which a <see cref="LeftRecursiveRule{T}"/> calls itself.
    /// </summary>
    /// <remarks>
    /// <see cref="Recognizers.Grow{T}(Input, ref Position, LeftRecursiveRule{T}, out T)"/> passes the rule a
    /// new handle on each pass, holding the longest parse found so far.
    /// </remarks>
    /// <typeparam name="T">The type of value the rule produces.</typeparam>
    public readonly ref struct LeftRecursion<T>
    {
        readonly LeftRecursiveRule<T> rule;
        readonly int start;
        readonly int end;
        readonly T seed;

        internal LeftRecursion(LeftRecursiveRule<T> rule, int start, int end, T seed)
        {
            this.rule = rule;
            this.start = start;
            this.end = end;
            this.seed = seed;
        }

        /// <summary>
        /// True once there's a parse to extend, which is on every pass after the first.
        /// </summary>
        /// <remarks>
        /// Once this is true, the rule must skip its alternatives that don't start with
        /// <see cref="Recurse"/>. They can only repeat the first pass, whose parse can't grow.
        /// </remarks>
        public bool Seeded => end != start;

        /// <summary>
        /// Recognize the rule recursively.
        /// </summary>
        /// <remarks>
        /// At the position where the growth started, this fails on the first pass, and on later passes
        /// answers with the previous pass's parse without running the rule. At any other position it
        /// grows the rule afresh from there.
        /// </remarks>
        /// <param name="x">Apply the rule to this input.</param>
        /// <param name="pos">The position at which the rule should start processing.</param>
        /// <param name="value">The value the rule produces.</param>
        /// <returns>True if the input at the given position matches the rule.</returns>
        public bool Recurse(Input x, ref Position pos, out T value)
        {
            if (pos.Pos != start)
                return x.Grow(ref pos, rule, out value);
            if (end == start)
            {
                value = default;
                return false;
            }
            value = seed;
            return pos.Seek(new Position { Pos = end });
        }
    }

    public static partial class Recognizers
    {
        /// <summary>
        /// Recognize a left-recursive rule, such as <c>e ::= e '-' t | t</c>, greedily.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Runs <paramref name="rule"/> repeatedly at <paramref name="pos"/>. On the first pass the
        /// rule's left-recursive call fails, so only its other alternatives can match, and that gives a
        /// seed. On each later pass the left-recursive call answers with the previous pass's parse, so
        /// the rule can extend it. Growth stops when a pass fails or ends no further along than the one
        /// before, and the longest parse wins. Left-recursive rules are therefore left associative:
        /// <c>a-b-c</c> is <c>(a-b)-c</c>. In a rule that is also right recursive, like
        /// <c>e ::= e e</c>, the right operand grows greedily too, so <c>x y z</c> is <c>x (y z)</c>.
        /// </para>
        /// <para>
        /// The rule must make its left-recursive call through <see cref="LeftRecursion{T}.Recurse"/>.
        /// Only direct left recursion is supported: a rule that reaches itself through another rule,
        /// before consuming anything, recurses forever.
        /// </para>
        /// <para>
        /// Once <see cref="LeftRecursion{T}.Seeded"/> is true, the rule must skip its alternatives that
        /// don't start with <see cref="LeftRecursion{T}.Recurse"/>. They can only repeat the first pass,
        /// whose parse can't grow, and running them again doubles the work at every level of nesting.
        /// A pass that matches without growing means the rule doesn't do this, so it throws.
        /// </para>
        /// <para>
        /// This allocates nothing itself, provided the delegate is cached: use a lambda that captures
        /// nothing, or a static field. Converting a method group to a delegate allocates on every call.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The type of value the rule produces.</typeparam>
        /// <param name="x">Apply the rule to this input.</param>
        /// <param name="pos">The position at which the rule should start processing.</param>
        /// <param name="rule">The left-recursive rule.</param>
        /// <param name="value">The value produced by the longest parse, or default if the rule fails.</param>
        /// <returns>True if the input at the given position matches the rule.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rule"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// A pass matched without extending the previous one: the rule doesn't skip its other
        /// alternatives once <see cref="LeftRecursion{T}.Seeded"/> is true, or its left-recursive
        /// alternative can match nothing after the left-recursive call.
        /// </exception>
        public static bool Grow<T>(this Input x, ref Position pos, LeftRecursiveRule<T> rule, out T value)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));
            var end = pos.Pos;
            var seed = default(T);
            while (true)
            {
                var i = pos;
                var matched = rule(x, ref i, new LeftRecursion<T>(rule, pos.Pos, end, seed), out var v);
                if (matched && end != pos.Pos && i.Pos <= end)
                    throw new InvalidOperationException(
                        "A left-recursive rule matched without extending the parse found so far. Once " +
                        "self.Seeded is true, the rule must skip the alternatives that don't start with " +
                        "self.Recurse: they repeat the first pass, which makes parsing exponential.");
                if (!matched || i.Pos <= end)
                    break;
                end = i.Pos;
                seed = v;
            }
            if (end == pos.Pos)
                return Fail(out value);
            value = seed;
            return pos.Seek(new Position { Pos = end });
        }
    }
}
