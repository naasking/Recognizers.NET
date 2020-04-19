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
        public bool AdvanceTo(Position newPosition)
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
        public bool AdvanceTo(Position newPosition, Input input, out ReadOnlySpan<char> capture)
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
        /// Increment.
        /// </summary>
        /// <returns></returns>
        public bool Advance()
        {
            Pos = Pos + 1;
            return true;
        }

        /// <summary>
        /// Increment.
        /// </summary>
        /// <returns></returns>
        public bool Advance(Input input, out ReadOnlySpan<char> capture)
        {
            capture = input.Value.AsSpan(Pos, 1);
            Pos = Pos + 1;
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string ToString() => Pos.ToString();
    }
}
