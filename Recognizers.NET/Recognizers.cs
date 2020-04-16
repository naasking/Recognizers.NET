using System;
using System.Collections.Generic;
using System.Text;

namespace Recognizers
{
    /// <summary>
    /// Extension methods that implement recognizers atop <see cref="Input"/>.
    /// </summary>
    public static class Recognizers
    {
        #region Internal methods
        /// <summary>
        /// Return false after initializing the captured variable.
        /// </summary>
        static bool Fail(out ReadOnlySpan<char> capture)
        {
            capture = default(ReadOnlySpan<char>);
            return false;
        }

        /// <summary>
        /// Return false after initializing the captured variable.
        /// </summary>
        static bool Fail(out List<string> capture)
        {
            capture = default(List<string>);
            return false;
        }
        #endregion

        #region Core recognizers
        /// <summary>
        /// Recognition proceeds even without a match.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool Optional(this Input x, bool match) => true;

        /// <summary>
        /// Recognize numbers.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Digit(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsNumber(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize numbers.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Digit(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsNumber(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);

        /// <summary>
        /// Recognize numbers.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Digits(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsNumber(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize numbers.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Digits(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsNumber(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a string value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Literal(this Input x, string text, ref Position pos) =>
            x.Literal(text, ref pos, StringComparison.Ordinal);

        /// <summary>
        /// Recognize a string value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Literal(this Input x, string text, ref Position pos, out ReadOnlySpan<char> capture) =>
            x.Literal(text, ref pos, StringComparison.Ordinal, out capture);

        /// <summary>
        /// Recognize a string value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Literal(this Input x, string text, ref Position pos, StringComparison compare) =>
               pos.Pos + text.Length < x.Length
            && text.AsSpan().Equals(x.Value.AsSpan(pos.Pos, text.Length), compare)
            && pos.AdvanceTo(new Position { Pos = pos.Pos + text.Length });

        /// <summary>
        /// Recognize a string value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Literal(this Input x, string text, ref Position pos, StringComparison compare, out ReadOnlySpan<char> capture) =>
               pos.Pos + text.Length < x.Length
            && text.AsSpan().Equals(x.Value.AsSpan(pos.Pos, text.Length), compare)
            && pos.AdvanceTo(new Position { Pos = pos.Pos + text.Length }, x, out capture)
            || Fail(out capture);

        /// <summary>
        /// Case insensitive recognition of a string.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool LiteralIgnoreCase(this Input x, string text, ref Position pos) =>
            x.Literal(text, ref pos, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Case insensitive recognition of a string.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool LiteralIgnoreCase(this Input x, string text, ref Position pos, out ReadOnlySpan<char> capture) =>
            x.Literal(text, ref pos, StringComparison.OrdinalIgnoreCase, out capture);

        /// <summary>
        /// Recognize whitespace.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool WhiteSpace(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsWhiteSpace(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize whitespace.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool WhiteSpace(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsWhiteSpace(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);

        /// <summary>
        /// Recognize whitespace.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool WhiteSpaces(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsWhiteSpace(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize whitespace.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <param name="capture"></param>
        /// <returns></returns>
        public static bool WhiteSpaces(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsWhiteSpace(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognise a character.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="c"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Char(this Input x, char c, ref Position pos) =>
            pos.Pos < x.Length && x[pos] == c && pos.Advance();


        /// <summary>
        /// Recognise a character.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="c"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Char(this Input x, char c, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && x[pos] == c && pos.Advance(x, out capture)
            || Fail(out capture);

        /// <summary>
        /// Recognise a character.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="c"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Chars(this Input x, char c, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && x[i] == c)
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
        public static bool Chars(this Input x, char c, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && x[i] == c)
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Consume all characters until the given character is seen.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="c"></param>
        /// <param name="pos"></param>
        public static bool CharsUntil(this Input x, char c, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && x[i] != c)
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Consume all characters until the given character is seen.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="c"></param>
        /// <param name="pos"></param>
        /// <param name="capture"></param>
        /// <returns></returns>
        public static bool CharsUntil(this Input x, char c, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && x[i] != c)
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognise a character.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="c"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Chars(this Input x, ref Position pos, params char[] c)
        {
            // statically expand the array access up to 4 entries
            var i = pos;
            switch (c.Length)
            {
                case 0:
                    return false;
                case 1:
                    return x.Chars(c[0], ref pos);
                case 2:
                    while (i.Pos < x.Length && (x[i] == c[0] || x[i] == c[1]))
                        ++i.Pos;
                    break;
                case 3:
                    while (i.Pos < x.Length && (x[i] == c[0] || x[i] == c[1] || x[i] == c[2]))
                        ++i.Pos;
                    break;
                default:
                    while (i.Pos < x.Length && Array.IndexOf(c, x[i]) >= 0)
                        ++i.Pos;
                    break;
            }
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognise a character.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="c"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Chars(this Input x, ref Position pos, out ReadOnlySpan<char> capture, params char[] c)
        {
            // statically expand the array access up to 4 entries
            var i = pos;
            switch (c.Length)
            {
                case 0:
                    return Fail(out capture);
                case 1:
                    return x.Chars(c[0], ref pos, out capture);
                case 2:
                    while (i.Pos < x.Length && (x[i] == c[0] || x[i] == c[1]))
                        ++i.Pos;
                    break;
                case 3:
                    while (i.Pos < x.Length && (x[i] == c[0] || x[i] == c[1] || x[i] == c[2]))
                        ++i.Pos;
                    break;
                default:
                    while (i.Pos < x.Length && Array.IndexOf(c, x[i]) >= 0)
                        ++i.Pos;
                    break;
            }
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize letters.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Letter(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsLetter(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize letters.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Letter(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsLetter(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);

        /// <summary>
        /// Recognize letters.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Letters(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLetter(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize letters.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Letters(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLetter(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a letter or a digit.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool LetterOrDigit(this Input x, ref Position pos) =>
            (x.Letter(ref pos) || x.Digit(ref pos)) && pos.Advance();


        /// <summary>
        /// Recognize a letter or a digit.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool LetterOrDigit(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
            x.Letter(ref pos, out capture) || x.Digit(ref pos, out capture);

        /// <summary>
        /// Recognize letters or digits.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool LettersOrDigits(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLetterOrDigit(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize letters or digits.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool LettersOrDigits(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLetterOrDigit(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }
        #endregion

        #region Extensions
        /// <summary>
        /// Recognize a phone number.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool PhoneNumber(this Input x, ref Position pos) =>
               pos.Save(out var i)
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Optional(x.Char('+', ref i))
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Optional(x.Digits(ref i))
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Optional(x.BracketedDigits(ref i))
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Optional(x.Char('/', ref i))
            && x.Optional(x.WhiteSpaces(ref i))
            && x.DelimitedDigits(ref i, ' ', '-')
            && pos.AdvanceTo(i);

        /// <summary>
        /// Recognize a phone number.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool PhoneNumber(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Save(out var i)
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Optional(x.Char('+', ref i))
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Optional(x.Digits(ref i))
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Optional(x.BracketedDigits(ref i))
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Optional(x.Char('/', ref i))
            && x.Optional(x.WhiteSpaces(ref i))
            && x.DelimitedDigits(ref i, ' ', '-')
            && pos.AdvanceTo(i, x, out capture)
            || Fail(out capture);

        /// <summary>
        /// Recognize a phone number.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool PhoneNumber(this Input x, ref Position pos, out List<string> capture) =>
               pos.Save(out var i)
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Optional(x.Char('+', ref i))
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Optional(x.Digits(ref i, out var d1))
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Optional(x.BracketedDigits(ref i, out var d2))
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Optional(x.Char('/', ref i))
            && x.Optional(x.WhiteSpaces(ref i))
            && x.DelimitedDigits(ref i, out capture, ' ', '-')
            && (d2.Length == 0 || capture.Push(d2))
            && (d1.Length == 0 || capture.Push(d1))
            && pos.AdvanceTo(i)
            || Fail(out capture);

        static bool Push(this List<string> capture, ReadOnlySpan<char> arg)
        {
            capture.Insert(0, arg.ToString());
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <param name="delimiters"></param>
        /// <returns></returns>
        public static bool DelimitedDigits(this Input x, ref Position pos, params char[] delimiters)
        {
            var i = pos;
            while (i.Pos < x.Length)
            {
                if (!x.Digits(ref i) && !x.Chars(ref i, delimiters))
                    break;
            }
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <param name="delimiters"></param>
        /// <returns></returns>
        public static bool DelimitedDigits(this Input x, ref Position pos, out List<string> capture, params char[] delimiters)
        {
            var i = pos;
            capture = null;
            while (i.Pos < x.Length)
            {
                if (x.Digits(ref i, out var data))
                    (capture ?? (capture = new List<string>())).Add(data.ToString());
                else if (!x.Chars(ref i, out data, delimiters))
                    break;
            }
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// A delimited sequence of digits with optional whitespace.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool DelimitedDigits(this Input x, char delimiter, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length)
            {
                if (!x.Digits(ref i) && !x.Chars(delimiter, ref i))
                    break;
            }
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// A delimited sequence of digits with optional whitespace.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool DelimitedDigits(this Input x, char delimiter, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length)
            {
                if (!x.Digits(ref i) && !x.Chars(delimiter, ref i))
                    break;
            }
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// A delimited sequence of digits with optional whitespace.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool DelimitedDigits(this Input x, char delimiter, ref Position pos, out List<string> capture)
        {
            var i = pos;
            capture = null;
            while (i.Pos < x.Length)
            {
                var data = default(ReadOnlySpan<char>);
                if (x.Digits(ref i, out data) || x.Chars(delimiter, ref i, out data))
                    (capture ?? (capture = new List<string>())).Add(data.ToString());
                else
                    break;
            }
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Digits surrounded by brackets.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool BracketedDigits(this Input x, ref Position pos) =>
            pos.Save(out var i) && x.Chars('(', ref i) && x.Digits(ref i) && x.Chars(')', ref i) && pos.AdvanceTo(i);

        /// <summary>
        /// Digits surrounded by brackets.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool BracketedDigits(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Save(out var i) && x.Chars('(', ref i) && x.Digits(ref i, out capture) && x.Chars(')', ref i) && pos.AdvanceTo(i)
            || Fail(out capture);

        /// <summary>
        /// A digit surrounded by brackets.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool BracketedDigit(this Input x, ref Position pos) =>
            pos.Save(out var i) && x.Chars('(', ref i) && x.Digit(ref i) && x.Chars(')', ref i) && pos.AdvanceTo(i);

        /// <summary>
        /// A digit surrounded by brackets.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool BracketedDigit(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Save(out var i) && x.Chars('(', ref i) && x.Digit(ref i, out capture) && x.Chars(')', ref i) && pos.AdvanceTo(i)
            || Fail(out capture);

        /// <summary>
        /// Recognize a zip code.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool ZipCode(this Input x, ref Position pos) =>
            x.Digits(ref pos);

        /// <summary>
        /// Recognize a zip code.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool ZipCode(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
            x.Digits(ref pos, out capture);

        /// <summary>
        /// Recognize a Canadian postal code.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool PostalCode(this Input x, ref Position pos)
        {
            var i = pos;
            if (!x.LettersOrDigits(ref i) || i.Delta != 3)
                return false;
            x.Optional(x.WhiteSpaces(ref i));
            return x.LettersOrDigits(ref i) && i.Delta == 3 && pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize a Canadian postal code.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool PostalCode(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            if (!x.LettersOrDigits(ref i) || i.Delta != 3)
                return Fail(out capture);
            x.Optional(x.WhiteSpaces(ref i));
            return x.LettersOrDigits(ref i) && i.Delta == 3 && pos.AdvanceTo(i, x, out capture)
                || Fail(out capture);
        }

        /// <summary>
        /// Recognize a postal or zip code.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool PostalOrZipCode(this Input x, ref Position pos) =>
            x.ZipCode(ref pos) || x.PostalCode(ref pos);

        /// <summary>
        /// Recognize a postal or zip code.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool PostalOrZipCode(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
            x.ZipCode(ref pos, out capture) || x.PostalCode(ref pos, out capture);

        /// <summary>
        /// Recognize an "attn" line.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Attention(this Input x, ref Position pos) =>
               pos.Save(out var i)
            && x.AttentionLabel(ref i)
            && x.Optional(x.Chars(':', ref i) || x.WhiteSpaces(ref i))
            && pos.AdvanceTo(i);

        /// <summary>
        /// Recognize an "Attention:" line.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Attention(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Save(out var i)
            && x.AttentionLabel(ref i, out capture)
            && x.Optional(x.Chars(':', ref i) || x.WhiteSpaces(ref i))
            && pos.AdvanceTo(i)
            || Fail(out capture);

        /// <summary>
        /// Recognize an attention label.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool AttentionLabel(this Input x, ref Position pos) =>
               pos.Save(out var j) && x.LiteralIgnoreCase("Attention", ref j) && pos.AdvanceTo(j)
            || pos.Save(out var i) && x.LiteralIgnoreCase("Attn", ref i) && pos.AdvanceTo(i)
            || pos.Save(out var k) && x.LiteralIgnoreCase("Att", ref k) && pos.AdvanceTo(k);

        /// <summary>
        /// Recognize an attention label.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool AttentionLabel(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Save(out var j) && x.LiteralIgnoreCase("Attention", ref j) && pos.AdvanceTo(j, x, out capture)
            || pos.Save(out var i) && x.LiteralIgnoreCase("Attn", ref i) && pos.AdvanceTo(i, x, out capture)
            || pos.Save(out var k) && x.LiteralIgnoreCase("Att", ref k) && pos.AdvanceTo(k, x, out capture)
            || Fail(out capture);

        /// <summary>
        /// Recognize a key-value pair, ie. foo="bar"
        /// </summary>
        /// <param name="x"></param>
        /// <param name="eq"></param>
        /// <param name="pos"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool KeyValuePair(this ref Input x, ref Position pos, out ReadOnlySpan<char> key, out ReadOnlySpan<char> value, char eq = '=') =>
               pos.Save(out var i)
            && x.CharsUntil(eq, ref i, out key)
            && x.Char(eq, ref i)
            && x.Char('"', ref i)
            && x.CharsUntil('"', ref i, out value)
            && x.Char('"', ref i)
            && pos.AdvanceTo(i)
            || Fail(out key) | Fail(out value);

        /// <summary>
        /// Recognize a key-value pair, ie. foo="bar"
        /// </summary>
        /// <param name="x"></param>
        /// <param name="eq"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool KeyValuePair(this ref Input x, ref Position pos, char eq = '=') =>
               pos.Save(out var i)
            && x.CharsUntil(eq, ref i)
            && x.Char(eq, ref i)
            && x.Char('"', ref i)
            && x.CharsUntil('"', ref i)
            && x.Char('"', ref i)
            && pos.AdvanceTo(i);

        /// <summary>
        /// Capture a set of key-value pairs.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="eq"></param>
        /// <param name="pos"></param>
        /// <param name="kv"></param>
        /// <returns></returns>
        public static bool KeyValuePairs(this ref Input x, ref Position pos, out Dictionary<string, string> kv, char eq = '=')
        {
            var i = pos;
            kv = new Dictionary<string, string>();
            while (i.Pos < x.Length)
            {
                if (x.KeyValuePair(ref i, out var key, out var value, eq))
                    kv.Add(key.ToString(), value.ToString());
                else
                    break;
            }
            return pos.AdvanceTo(i);
        }
        #endregion
    }
}
