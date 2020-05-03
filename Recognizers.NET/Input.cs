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
    /// Track a set of left-recursive rules.
    /// </summary>
    public struct Rules
    {
        uint flags;

        /// <summary>
        /// Check if the given rule number has been applied already.
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns>True if the rule has already been applied, false otherwise.</returns>
        public bool IsLoop(int ruleId)
        {
            uint mask =  1U << ruleId;
            if (0 != (flags & mask))
                return true;
            flags |= mask;
            return false;
        }

        internal void Reset() =>
            flags = 0;
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
        public bool Seek(Position newPosition, ref Rules rules)
        {
            if (Pos == newPosition.Pos)
            {
                return false;
            }
            else
            {
                rules.Reset();
                Pos = newPosition.Pos;
                return true;
            }
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
        /// Advance <paramref name="pos"/> to the position given by <paramref name="newPosition"/>.
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool Seek(Position newPosition, Input input, out ReadOnlySpan<char> capture, ref Rules rules)
        {
            if (Pos == newPosition.Pos)
            {
                capture = default;
                return false;
            }
            else
            {
                capture = input.Value.AsSpan(Pos, newPosition.Pos - Pos);
                rules.Reset();
                Pos = newPosition.Pos;
                return true;
            }
        }

        /// <summary>
        /// Increment.
        /// </summary>
        /// <returns></returns>
        public bool Step()
        {
            Pos = Pos + 1;
            return true;
        }

        /// <summary>
        /// Increment.
        /// </summary>
        /// <returns></returns>
        public bool Step(ref Rules rules)
        {
            rules.Reset();
            return Step();
        }

        /// <summary>
        /// Increment.
        /// </summary>
        /// <returns></returns>
        public bool Step(Input input, out ReadOnlySpan<char> capture)
        {
            capture = input.Value.AsSpan(Pos, 1);
            Pos = Pos + 1;
            return true;
        }

        /// <summary>
        /// Increment.
        /// </summary>
        /// <returns></returns>
        public bool Step(Input input, out ReadOnlySpan<char> capture, ref Rules rules)
        {
            rules.Reset();
            return Step(input, out capture);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string ToString() => Pos.ToString();
    }
}
