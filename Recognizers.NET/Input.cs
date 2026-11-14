using System;
using System.Collections.Generic;
using System.Text;

namespace Recognizers
{
    /// <summary>
    /// Encapsulate an input.
    /// </summary>
    public readonly struct Input
    {
        /// <summary>
        /// Construct a new cursor.
        /// </summary>
        /// <param name="input">The input to proces.</param>
        public Input(string input) : this(input?.ToCharArray())
        {
        }

        /// <summary>
        /// Construct a new cursor.
        /// </summary>
        /// <param name="input">The input to proces.</param>
        public Input(char[] input) : this()
        {
            this.Value = input ?? throw new ArgumentNullException(nameof(input));
        }

        /// <summary>
        /// The input being processed.
        /// </summary>
        public char[] Value { get; }

        /// <summary>
        /// The character at the given position.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public char this[Position x] => Value[x.Pos];

        /// <summary>
        /// The input length.
        /// </summary>
        public int Length => Value.Length;

        /// <summary>
        /// Start processing a rule at the input's start.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool Begin(out Position pos)
        {
            pos = new Position { Pos = 0, };
            return true;
        }

        /// <summary>
        /// Terminate this cursor by checking that all input has been consumed.
        /// </summary>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        public bool End(Position newPosition) =>
            newPosition.Pos == Length;
    }

    /// <summary>
    /// Track the left-recursive rules that are active at one input position.
    /// </summary>
    /// <remarks>
    /// Pass <see cref="Rules"/> by value. Each rule checks <see cref="IsLoop"/> on its own copy and
    /// passes that copy to the rules it calls, so its marks are seen by those rules, and never by its
    /// caller or by a sibling alternative. Marks expire as soon as a check is made at a different
    /// position, so nothing needs to reset them.
    ///
    /// This stops left recursion rather than implementing it: a rule's leftmost operand can never be
    /// the rule itself, so <c>e ::= e e</c> parses <c>x y z</c> as <c>x (y z)</c>. Parse
    /// left-associative syntax with a loop instead.
    /// </remarks>
    public struct Rules
    {
        uint flags;
        int pos;

        /// <summary>
        /// Check if the given rule has already been applied at the given position, and mark it as
        /// applied if not.
        /// </summary>
        /// <param name="at">The position at which the rule is being applied.</param>
        /// <param name="ruleId">The rule number, in [0, 32).</param>
        /// <returns>True if the rule has already been applied at <paramref name="at"/>, false otherwise.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="ruleId"/> is outside [0, 32).</exception>
        public bool IsLoop(Position at, int ruleId)
        {
            if ((uint)ruleId >= 32)
                throw new ArgumentOutOfRangeException(nameof(ruleId), "Rule numbers must be in [0, 32).");
            if (pos != at.Pos)
            {
                flags = 0;
                pos = at.Pos;
            }
            uint mask = 1U << ruleId;
            if (0 != (flags & mask))
                return true;
            flags |= mask;
            return false;
        }
    }

    /// <summary>
    /// The current cursor's position.
    /// </summary>
    public ref struct Position
    {
        public int Pos;

        /// <summary>
        /// Start processing a rule at the input's start.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool Save(out Position pos)
        {
            pos = this;
            return true;
        }

        /// <summary>
        /// Advance <paramref name="pos"/> to the position given by <paramref name="newPosition"/>.
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool Seek(Position newPosition)
        {
            if (Pos == newPosition.Pos)
                return false;
            Pos = newPosition.Pos;
            return true;
        }

        /// <summary>
        /// Advance <paramref name="pos"/> to the position given by <paramref name="newPosition"/>.
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool Seek(Position newPosition, Input input, out ReadOnlySpan<char> capture)
        {
            if (Pos == newPosition.Pos)
            {
                capture = default;
                return false;
            }
            else
            {
                capture = input.Value.AsSpan(Pos, newPosition.Pos - Pos);
                Pos = newPosition.Pos;
                return true;
            }
        }

        /// <summary>
        /// Advance one character, unless at the end of the input.
        /// </summary>
        /// <param name="input">The input being processed.</param>
        /// <returns>True if a character was consumed, false at the end of the input.</returns>
        public bool Step(Input input)
        {
            if (Pos >= input.Length)
                return false;
            ++Pos;
            return true;
        }

        /// <summary>
        /// Advance one character, unless at the end of the input.
        /// </summary>
        /// <param name="input">The input being processed.</param>
        /// <param name="capture">The character consumed, or empty at the end of the input.</param>
        /// <returns>True if a character was consumed, false at the end of the input.</returns>
        public bool Step(Input input, out ReadOnlySpan<char> capture)
        {
            if (Pos >= input.Length)
            {
                capture = default;
                return false;
            }
            capture = input.Value.AsSpan(Pos, 1);
            ++Pos;
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string ToString() => Pos.ToString();
    }
}
