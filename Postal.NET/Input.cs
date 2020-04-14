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
        //int pos;
        public string Input { get; private set; }
        //public int Delta { get; private set; }
        //public int Position => pos;

        public Cursor(string input) : this()
        {
            this.Input = input;
        }

        public char this[Position x] => Input[x.Pos];

        public int Length => Input.Length;

        public bool Advance(Position newPosition, ref Position pos)
        {
            if (pos.Pos == newPosition.Pos)
                return false;
            pos = newPosition;
            return true;
        }

        public bool Begin(out Position pos)
        {
            pos = new Position { Pos = 0, Delta = 0 };
            return true;
        }

        public bool Begin(Position cur, out Position pos)
        {
            pos = new Position { Pos = cur.Pos, Delta = 0 };
            return true;
        }

        public bool End(Position newPosition) =>
            newPosition.Pos == Length;
    }

    public struct Position
    {
        public int Pos;
        public int Delta;
    }
}
