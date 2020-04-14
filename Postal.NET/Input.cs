using System;
using System.Collections.Generic;
using System.Text;

namespace Postal
{
    /// <summary>
    /// Encapsulate an input.
    /// </summary>
    public struct Cursor
    {
        /// <summary>
        /// Construct a new cursor.
        /// </summary>
        /// <param name="input">The input to proces.</param>
        public Cursor(string input) : this()
        {
            this.Input = input;
        }

        /// <summary>
        /// The input being processed.
        /// </summary>
        public string Input { get; private set; }

        /// <summary>
        /// The character at the given position.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public char this[Position x] => Input[x.Pos];

        /// <summary>
        /// The input length.
        /// </summary>
        public int Length => Input.Length;

        /// <summary>
        /// Start processing a rule at the input's start.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool Begin(out Position pos)
        {
            pos = new Position { Pos = 0, Delta = 0 };
            return true;
        }

        /// <summary>
        /// Start processing a rule from the given position, <paramref name="cur"/>.
        /// </summary>
        /// <param name="cur"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool Begin(Position cur, out Position pos)
        {
            pos = new Position { Pos = cur.Pos, Delta = 0 };
            return true;
        }

        /// <summary>
        /// Advance <paramref name="pos"/> to the position given by <paramref name="newPosition"/>.
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool Advance(Position newPosition, ref Position pos)
        {
            if (pos.Pos == newPosition.Pos)
                return false;
            pos = newPosition;
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
    public struct Position
    {
        public int Pos;
        public int Delta;
    }
}
