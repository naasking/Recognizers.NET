using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Recognizers
{
    public static partial class Recognizers
    {

        /// <summary>
        /// Recognize a Letter.
        /// </summary>
        /// <param name="x">Apply the recognizer to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Letter(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsLetter(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize a Letter.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">Capture the input that was recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Letter(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsLetter(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);
            
        /// <summary>
        /// Recognize Letters.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Letters(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLetter(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize Letters.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Letters(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLetter(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Accept all characters until a Letter.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilLetter(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsLetter(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all characters until a Letter.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilLetter(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsLetter(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a Digit.
        /// </summary>
        /// <param name="x">Apply the recognizer to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Digit(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsDigit(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize a Digit.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">Capture the input that was recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Digit(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsDigit(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);
            
        /// <summary>
        /// Recognize Digits.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Digits(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsDigit(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize Digits.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Digits(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsDigit(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Accept all characters until a Digit.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilDigit(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsDigit(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all characters until a Digit.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilDigit(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsDigit(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a WhiteSpace.
        /// </summary>
        /// <param name="x">Apply the recognizer to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool WhiteSpace(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsWhiteSpace(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize a WhiteSpace.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">Capture the input that was recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool WhiteSpace(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsWhiteSpace(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);
            
        /// <summary>
        /// Recognize WhiteSpaces.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool WhiteSpaces(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsWhiteSpace(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize WhiteSpaces.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool WhiteSpaces(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsWhiteSpace(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Accept all characters until a WhiteSpace.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilWhiteSpace(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsWhiteSpace(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all characters until a WhiteSpace.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilWhiteSpace(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsWhiteSpace(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a Symbol.
        /// </summary>
        /// <param name="x">Apply the recognizer to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Symbol(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsSymbol(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize a Symbol.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">Capture the input that was recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Symbol(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsSymbol(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);
            
        /// <summary>
        /// Recognize Symbols.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Symbols(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsSymbol(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize Symbols.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Symbols(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsSymbol(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Accept all characters until a Symbol.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilSymbol(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsSymbol(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all characters until a Symbol.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilSymbol(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsSymbol(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a Separator.
        /// </summary>
        /// <param name="x">Apply the recognizer to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Separator(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsSeparator(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize a Separator.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">Capture the input that was recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Separator(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsSeparator(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);
            
        /// <summary>
        /// Recognize Separators.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Separators(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsSeparator(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize Separators.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Separators(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsSeparator(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Accept all characters until a Separator.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilSeparator(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsSeparator(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all characters until a Separator.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilSeparator(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsSeparator(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a Surrogate.
        /// </summary>
        /// <param name="x">Apply the recognizer to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Surrogate(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsSurrogate(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize a Surrogate.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">Capture the input that was recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Surrogate(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsSurrogate(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);
            
        /// <summary>
        /// Recognize Surrogates.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Surrogates(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize Surrogates.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Surrogates(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Accept all characters until a Surrogate.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilSurrogate(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all characters until a Surrogate.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilSurrogate(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a Punctuation.
        /// </summary>
        /// <param name="x">Apply the recognizer to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Punctuation(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsPunctuation(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize a Punctuation.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">Capture the input that was recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Punctuation(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsPunctuation(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);
            
        /// <summary>
        /// Recognize Punctuations.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Punctuations(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsPunctuation(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize Punctuations.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Punctuations(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsPunctuation(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Accept all characters until a Punctuation.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilPunctuation(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsPunctuation(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all characters until a Punctuation.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilPunctuation(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsPunctuation(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a LetterOrDigit.
        /// </summary>
        /// <param name="x">Apply the recognizer to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool LetterOrDigit(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsLetterOrDigit(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize a LetterOrDigit.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">Capture the input that was recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool LetterOrDigit(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsLetterOrDigit(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);
            
        /// <summary>
        /// Recognize LetterOrDigits.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool LettersOrDigits(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLetterOrDigit(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize LetterOrDigits.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool LettersOrDigits(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLetterOrDigit(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Accept all characters until a LetterOrDigit.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilLetterOrDigit(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsLetterOrDigit(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all characters until a LetterOrDigit.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilLetterOrDigit(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsLetterOrDigit(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a Lower.
        /// </summary>
        /// <param name="x">Apply the recognizer to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Lower(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsLower(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize a Lower.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">Capture the input that was recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Lower(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsLower(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);
            
        /// <summary>
        /// Recognize Lowers.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Lowers(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLower(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize Lowers.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Lowers(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLower(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Accept all characters until a Lower.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilLower(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsLower(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all characters until a Lower.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilLower(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsLower(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a Upper.
        /// </summary>
        /// <param name="x">Apply the recognizer to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Upper(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsUpper(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize a Upper.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">Capture the input that was recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Upper(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsUpper(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);
            
        /// <summary>
        /// Recognize Uppers.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Uppers(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsUpper(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize Uppers.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Uppers(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsUpper(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Accept all characters until a Upper.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilUpper(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsUpper(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all characters until a Upper.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilUpper(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsUpper(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a LowSurrogate.
        /// </summary>
        /// <param name="x">Apply the recognizer to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool LowSurrogate(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsLowSurrogate(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize a LowSurrogate.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">Capture the input that was recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool LowSurrogate(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsLowSurrogate(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);
            
        /// <summary>
        /// Recognize LowSurrogates.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool LowSurrogates(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLowSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize LowSurrogates.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool LowSurrogates(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsLowSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Accept all characters until a LowSurrogate.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilLowSurrogate(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsLowSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all characters until a LowSurrogate.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilLowSurrogate(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsLowSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a HighSurrogate.
        /// </summary>
        /// <param name="x">Apply the recognizer to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool HighSurrogate(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsHighSurrogate(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize a HighSurrogate.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">Capture the input that was recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool HighSurrogate(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsHighSurrogate(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);
            
        /// <summary>
        /// Recognize HighSurrogates.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool HighSurrogates(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsHighSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize HighSurrogates.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool HighSurrogates(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsHighSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Accept all characters until a HighSurrogate.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilHighSurrogate(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsHighSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all characters until a HighSurrogate.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilHighSurrogate(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsHighSurrogate(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Recognize a Control.
        /// </summary>
        /// <param name="x">Apply the recognizer to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Control(this Input x, ref Position pos) =>
            pos.Pos < x.Length && char.IsControl(x[pos]) && pos.Advance();

        /// <summary>
        /// Recognize a Control.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <param name="capture">Capture the input that was recognized.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Control(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Pos < x.Length && char.IsControl(x[pos]) && pos.Advance(x, out capture)
            || Fail(out capture);
            
        /// <summary>
        /// Recognize Controls.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Controls(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsControl(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Recognize Controls.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool Controls(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && char.IsControl(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }

        /// <summary>
        /// Accept all characters until a Control.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilControl(this Input x, ref Position pos)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsControl(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i);
        }

        /// <summary>
        /// Accept all characters until a Control.
        /// </summary>
        /// <param name="x">Apply the recognizers to this input.</param>
        /// <param name="pos">The position at which the recognizer should start processing.</param>
        /// <returns>True if the input at the given position matches the recognizer's rule.</returns>
        public static bool UntilControl(this Input x, ref Position pos, out ReadOnlySpan<char> capture)
        {
            var i = pos;
            while (i.Pos < x.Length && !char.IsControl(x[i]))
                ++i.Pos;
            return pos.AdvanceTo(i, x, out capture);
        }
    }
}