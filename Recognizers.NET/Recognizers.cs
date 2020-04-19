using System;
using System.Collections.Generic;
using System.Text;

namespace Recognizers
{
    /// <summary>
    /// Extension methods that implement recognizers atop <see cref="Input"/>.
    /// </summary>
    public static partial class Recognizers
    {
        #region Core recognizers

        /// <summary>
        /// Return false after initializing the captured variable.
        /// </summary>
        public static bool Fail(out ReadOnlySpan<char> capture)
        {
            capture = default;
            return false;
        }

        /// <summary>
        /// Return false after initializing the captured variable.
        /// </summary>
        public static bool Fail<T>(out T capture)
        {
            capture = default;
            return false;
        }

        /// <summary>
        /// Recognition proceeds even without a match.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="match">The optional rule's result.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Optional(this Input x, bool match) => true;

        /// <summary>
        /// Recognize a string value.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="text"></param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Literal(this Input x, string text, ref Position pos) =>
            x.Literal(text, ref pos, StringComparison.Ordinal);

        /// <summary>
        /// Recognize a string value.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="text"></param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Literal(this Input x, string text, ref Position pos, out ReadOnlySpan<char> capture) =>
            x.Literal(text, ref pos, StringComparison.Ordinal, out capture);

        /// <summary>
        /// Recognize a string value.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="text"></param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Literal(this Input x, string text, ref Position pos, StringComparison compare) =>
               pos.Pos + text.Length < x.Length
            && text.AsSpan().Equals(x.Value.AsSpan(pos.Pos, text.Length), compare)
            && pos.AdvanceTo(new Position { Pos = pos.Pos + text.Length });

        /// <summary>
        /// Recognize a string value.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="text"></param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Literal(this Input x, string text, ref Position pos, StringComparison compare, out ReadOnlySpan<char> capture) =>
               pos.Pos + text.Length < x.Length
            && text.AsSpan().Equals(x.Value.AsSpan(pos.Pos, text.Length), compare)
            && pos.AdvanceTo(new Position { Pos = pos.Pos + text.Length }, x, out capture)
            || Fail(out capture);

        /// <summary>
        /// Case insensitive recognition of a string.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="text"></param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool LiteralIgnoreCase(this Input x, string text, ref Position pos) =>
            x.Literal(text, ref pos, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Case insensitive recognition of a string.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="text"></param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool LiteralIgnoreCase(this Input x, string text, ref Position pos, out ReadOnlySpan<char> capture) =>
            x.Literal(text, ref pos, StringComparison.OrdinalIgnoreCase, out capture);

        /// <summary>
        /// Recognise a character.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="c">The character to accept.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Char(this Input x, ref Position pos, params char[] c)
        {
            switch (c.Length)
            {
                case 0:
                    return false;
                case 1:
                    return x.Char(c[0], ref pos);
                case 2:
                    return x.Char(c[0], ref pos)
                        || x.Char(c[1], ref pos);
                case 3:
                    return x.Char(c[0], ref pos)
                        || x.Char(c[1], ref pos)
                        || x.Char(c[2], ref pos);
                default:
                    return Array.IndexOf(c, x[pos]) >= 0 && pos.Advance();
            }
        }

        /// <summary>
        /// Recognise a character.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="c">The character to accept.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Char(this Input x, ref Position pos, out ReadOnlySpan<char> capture, params char[] c)
        {
            switch (c.Length)
            {
                case 0:
                    return Fail(out capture);
                case 1:
                    return x.Char(c[0], ref pos, out capture);
                case 2:
                    return x.Char(c[0], ref pos, out capture)
                        || x.Char(c[1], ref pos, out capture);
                case 3:
                    return x.Char(c[0], ref pos, out capture)
                        || x.Char(c[1], ref pos, out capture)
                        || x.Char(c[2], ref pos, out capture);
                default:
                    return Array.IndexOf(c, x[pos]) >= 0 && pos.Advance(x, out capture)
                        || Fail(out capture);
            }
        }

        /// <summary>
        /// Recognise a character.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="c">The character to accept.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Char(this Input x, char c, ref Position pos) =>
            pos.Pos < x.Length && x[pos] == c && pos.Advance();

        /// <summary>
        /// Recognise a character.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="c">The character to accept.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Char(this Input x, char c, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && x[pos] == c && pos.Advance(x, out capture)
            || Fail(out capture);

        /// <summary>
        /// Recognise a character.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="c">The character to accept.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool WhileChar(this Input x, char c, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && x[i] == c)
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognise a character.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="c">The character to accept.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool WhileChar(this Input x, char c, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && x[i] == c)
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognise a character.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="c">The characters to accept.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool WhileChar(this Input x, ref Position pos, params char[] c)
        {
            // statically expand the array access up to 4 entries
            var i = pos;
            switch (c.Length)
            {
                case 0:
                    return false;
                case 1:
                    return x.WhileChar(c[0], ref pos);
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
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="c">The characters to accept.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool WhileChar(this Input x, ref Position pos, out ReadOnlySpan<char> capture, params char[] c)
        {
            // statically expand the array access up to 4 entries
            var i = pos;
            switch (c.Length)
            {
                case 0:
                    return Fail(out capture);
                case 1:
                    return x.WhileChar(c[0], ref pos, out capture);
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
        /// Consume all characters until the given character is seen.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="c">The character to accept.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        public static bool UntilChar(this Input x, char c, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && x[i] != c)
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Consume all characters until the given character is seen.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="c">The character to accept.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">The characters that were recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilChar(this Input x, char c, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && x[i] != c)
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Consume all characters until the given character is seen.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="c">The character to accept.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">The characters that were recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilChar(this Input x, ref Position pos, params char[] c)
        {
            var i = pos;
            switch (c.Length)
            {
                case 0:
                    return false;
                case 1:
                    return x.UntilChar(c[0], ref pos);
                case 2:
                    while (i.Pos < x.Length && x[i] != c[0] && x[i] != c[1])
                        ++i.Pos;
                    break;
                case 3:
                    while (i.Pos < x.Length && x[i] != c[0] && x[i] != c[1] && x[i] != c[2])
                        ++i.Pos;
                    break;
                default:
                    while (i.Pos < x.Length && Array.IndexOf(c, x[i]) < 0)
                        ++i.Pos;
                    break;
            }
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Consume all characters until the given character is seen.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="c">The character to accept.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">The characters that were recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilChar(this Input x, ref Position pos, out ReadOnlySpan<char> capture, params char[] c)
        {
            var i = pos;
            switch (c.Length)
            {
                case 0:
                    return Fail(out capture);
                case 1:
                    return x.UntilChar(c[0], ref pos, out capture);
                case 2:
                    while (i.Pos < x.Length && x[i] != c[0] && x[i] != c[1])
                        ++i.Pos;
                    break;
                case 3:
                    while (i.Pos < x.Length && x[i] != c[0] && x[i] != c[1] && x[i] != c[2])
                        ++i.Pos;
                    break;
                default:
                    while (i.Pos < x.Length && Array.IndexOf(c, x[i]) < 0)
                        ++i.Pos;
                    break;
            }
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize high surrogates.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool SurrogatePair(this Input x, ref Position pos) =>
            pos.Save(out var i) && x.HighSurrogate(ref i) && x.LowSurrogate(ref i) && pos.AdvanceTo(i);
            
        /// <summary>
        /// Recognize high surrogates.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool SurrogatePair(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Save(out var i) && x.HighSurrogate(ref i) && x.LowSurrogate(ref i) && pos.AdvanceTo(i, x, out capture)
            || Fail(out capture);

        /// <summary>
        /// Recognize surrogate pairs.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool WhileSurrogatePair(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsHighSurrogate(x[i]) && i.Advance() && char.IsLowSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize surrogate pairs.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool WhileSurrogatePair(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsHighSurrogate(x[i]) && i.Advance() && char.IsLowSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }
        /// <summary>
        /// Accept all chars up to a surrogate pair.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilSurrogatePair(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !(char.IsHighSurrogate(x[i]) && i.Advance() && char.IsLowSurrogate(x[i])))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all chars up to a surrogate pair.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilSurrogatePair(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !(char.IsHighSurrogate(x[i]) && i.Advance() && char.IsLowSurrogate(x[i])))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }
        #endregion

        #region Extensions
        /// <summary>
        /// Recognize a phone number.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool PhoneNumber(this Input x, ref Position pos) =>
               pos.Save(out var i)
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.Char('+', ref i))
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.WhileDigit(ref i))
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.BracketedDigits(ref i))
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.Char('/', ref i))
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.DelimitedDigits(ref i, ' ', '-')
            && pos.AdvanceTo(i);

        /// <summary>
        /// Recognize a phone number.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool PhoneNumber(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Save(out var i)
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.Char('+', ref i))
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.WhileDigit(ref i))
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.BracketedDigits(ref i))
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.Char('/', ref i))
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.DelimitedDigits(ref i, ' ', '-')
            && pos.AdvanceTo(i, x, out capture)
            || Fail(out capture);

        /// <summary>
        /// Recognize a phone number.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool PhoneNumber(this Input x, ref Position pos, out List<string> capture) =>
               pos.Save(out var i)
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.Char('+', ref i))
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.WhileDigit(ref i, out var d1))
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.BracketedDigits(ref i, out var d2))
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.Char('/', ref i))
            && x.Optional(x.WhileWhiteSpace(ref i))
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
        /// A delimited sequence of digits with optional whitespace.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="delimiters"></param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool DelimitedDigits(this Input x, ref Position pos, params char[] delimiters)
        {
            //FIXME: this is probably not correct, it needs only a single delimiter per digit? May need a loop "state" variable to track this.
            var i = pos;
            while (i.Pos < x.Length)
            {
                if (!x.WhileDigit(ref i) && !x.Char(ref i, delimiters))
                    break;
            }
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// A delimited sequence of digits with optional whitespace.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="delimiters"></param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool DelimitedDigits(this Input x, ref Position pos, out List<string> capture, params char[] delimiters)
        {
            var i = pos;
            capture = null;
            while (i.Pos < x.Length)
            {
                if (x.WhileDigit(ref i, out var data))
                    (capture ?? (capture = new List<string>())).Add(data.ToString());
                else if (!x.Char(ref i, out data, delimiters))
                    break;
            }
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// A delimited sequence of digits with optional whitespace.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool DelimitedDigits(this Input x, char delimiter, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length)
            {
                if (!x.WhileDigit(ref i) && !x.Char(delimiter, ref i))
                    break;
            }
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// A delimited sequence of digits with optional whitespace.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool DelimitedDigits(this Input x, char delimiter, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length)
            {
                if (!x.WhileDigit(ref i) && !x.Char(delimiter, ref i))
                    break;
            }
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// A delimited sequence of digits with optional whitespace.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool DelimitedDigits(this Input x, char delimiter, ref Position pos, out List<string> capture)
        {
            var i = pos;
            capture = null;
            while (i.Pos < x.Length)
            {
                var data = default(ReadOnlySpan<char>);
                if (x.WhileDigit(ref i, out data) || x.WhileChar(delimiter, ref i, out data))
                    (capture ?? (capture = new List<string>())).Add(data.ToString());
                else
                    break;
            }
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Digits surrounded by brackets.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool BracketedDigits(this Input x, ref Position pos) =>
            pos.Save(out var i) && x.Char('(', ref i) && x.WhileDigit(ref i) && x.Char(')', ref i) && pos.AdvanceTo(i);

        /// <summary>
        /// Digits surrounded by brackets.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool BracketedDigits(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Save(out var i) && x.Char('(', ref i) && x.WhileDigit(ref i, out capture) && x.Char(')', ref i) && pos.AdvanceTo(i)
            || Fail(out capture);

        /// <summary>
        /// A digit surrounded by brackets.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool BracketedDigit(this Input x, ref Position pos) =>
            pos.Save(out var i) && x.Char('(', ref i) && x.Digit(ref i) && x.Char(')', ref i) && pos.AdvanceTo(i);

        /// <summary>
        /// A digit surrounded by brackets.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool BracketedDigit(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Save(out var i) && x.Char('(', ref i) && x.Digit(ref i, out capture) && x.Char(')', ref i) && pos.AdvanceTo(i)
            || Fail(out capture);

        /// <summary>
        /// Recognize a key-value pair, ie. foo="bar"
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="eq"></param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool KeyValuePair(this Input x, ref Position pos, out ReadOnlySpan<char> key, out ReadOnlySpan<char> value, char eq = '=') =>
               pos.Save(out var i)
            && x.LetterOrDigit(ref i) // ensure starts with letter or digit
            && pos.Save(out i)        // backtrack one and resume
            && x.UntilChar(eq, ref i, out key)
            && x.Char(eq, ref i)
            && x.Char('"', ref i)
            && x.UntilChar('"', ref i, out value)
            && x.Char('"', ref i)
            && pos.AdvanceTo(i)
            || Fail(out key) | Fail(out value);

        /// <summary>
        /// Recognize a key-value pair, ie. foo="bar"
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="eq"></param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool KeyValuePair(this Input x, ref Position pos, char eq = '=') =>
               pos.Save(out var i)
            && x.UntilChar(eq, ref i)
            && x.Char(eq, ref i)
            && x.Char('"', ref i)
            && x.UntilChar('"', ref i)
            && x.Char('"', ref i)
            && pos.AdvanceTo(i);

        /// <summary>
        /// Capture a set of key-value pairs.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="eq"></param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="kv"></param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool KeyValuePairs(this Input x, ref Position pos, out Dictionary<string, string> kv, char eq = '=')
        {
            var i = pos;
            kv = new Dictionary<string, string>();
            while (i.Pos < x.Length)
            {
                if (x.Optional(x.WhileWhiteSpace(ref i)) && x.KeyValuePair(ref i, out var key, out var value, eq))
                    kv.Add(key.ToString(), value.ToString());
                else
                    break;
            }
            return pos.AdvanceTo(i);
        }
        #endregion
    }
}
