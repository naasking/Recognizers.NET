using System;
using System.Collections.Generic;
using System.Text;

namespace Postal
{
    /// <summary>
    /// Extension methods that implement recognizers atop <see cref="Input"/>.
    /// </summary>
    public static class Recognizers
    {
        /// <summary>
        /// Recognition proceeds even without a match.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool Optional(ref this Cursor x, bool match) => true;

        /// <summary>
        /// Recognize numbers.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Digits(ref this Cursor x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsNumber(x.Input, i.Pos))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Case sensitive recognition of a string.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool CaseSensitive(ref this Cursor x, string text, ref Position pos)
        {
            var i = pos;
            for (int j = 0; j < text.Length && i.Pos < x.Length; ++j)
            {
                if (x[i] != text[j])
                    return false;
                ++i.Pos;
            }
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Case insensitive recognition of a string.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool CaseInsensitive(ref this Cursor x, string text, ref Position pos)
        {
            var i = pos;
            for (int j = 0; j < text.Length && i.Pos < x.Length; ++j)
            {
                if (char.ToUpperInvariant(x[i]) != char.ToUpperInvariant(text[j]))
                    return false;
                ++i.Pos;
            }
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize whitespace.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Whitespace(ref this Cursor x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsWhiteSpace(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognise a character.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="c"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Char(ref this Cursor x, char c, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && x[i] == c)
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize letters.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Letters(ref this Cursor x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLetter(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize letters or digits.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool LettersOrDigits(ref this Cursor x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLetterOrDigit(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize a phone number.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool PhoneNumber(this ref Cursor x, ref Position pos) =>
               pos.Save(out var i)
            && x.Optional(x.Whitespace(ref i))
            && x.Optional(x.Char('+', ref i))
            && x.Optional(x.Whitespace(ref i))
            && x.Optional(x.Digits(ref i))
            && x.Optional(x.Whitespace(ref i))
            && (x.BracketedDigits(ref i) || x.Digits(ref i))
            && x.Optional(x.Whitespace(ref i))
            && x.Optional(x.Char('/', ref i))
            && x.Optional(x.Whitespace(ref i))
            && x.DelimitedDigits('-', ref i)
            && pos.AdvanceTo(i);

        /// <summary>
        /// A delimited sequence of digits with optional whitespace.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool DelimitedDigits(this ref Cursor x, char c, ref Position pos)
        {
            var i = pos;
            do
            {
                x.Whitespace(ref i);
                x.Char(c, ref i);
                x.Whitespace(ref i);
            } while (i.Pos < x.Length && i.Save(out var loop) && x.Digits(ref loop) && i.AdvanceTo(loop));
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Digits surrounded by brackets.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool BracketedDigits(this ref Cursor x, ref Position pos) =>
            pos.Save(out var i) && x.Char('(', ref i) && x.Digits(ref i) && x.Char(')', ref i) && pos.AdvanceTo(i);

        /// <summary>
        /// Recognize a zip code.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool ZipCode(this ref Cursor x, ref Position pos) =>
            x.Digits(ref pos);

        /// <summary>
        /// Recognize a Canadian postal code.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool PostalCode(this ref Cursor x, ref Position pos)
        {
            var i = pos;
            if (!x.LettersOrDigits(ref i) || i.Delta != 3)
                return false;
            x.Optional(x.Whitespace(ref i));
            return x.LettersOrDigits(ref i) && i.Delta == 3 && pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize a postal or zip code.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool PostalOrZipCode(this ref Cursor x, ref Position pos) =>
            x.ZipCode(ref pos) || x.PostalCode(ref pos);

        /// <summary>
        /// Recognize an "attn" line.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Attention(this ref Cursor x, ref Position pos) =>
               pos.Save(out var i)
            && x.AttentionLabel(ref i)
            && x.Optional(x.Char(':', ref i) || x.Whitespace(ref i))
            && pos.AdvanceTo(i);

        /// <summary>
        /// Recognize an attention label.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool AttentionLabel(this ref Cursor x, ref Position pos) =>
               pos.Save(out var j) && x.CaseInsensitive("Attention", ref j) && pos.AdvanceTo(j)
            || pos.Save(out var i) && x.CaseInsensitive("Attn", ref i) && pos.AdvanceTo(i)
            || pos.Save(out var k) && x.CaseInsensitive("Att", ref k) && pos.AdvanceTo(k);
    }
}
