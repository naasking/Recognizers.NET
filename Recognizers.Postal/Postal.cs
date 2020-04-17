using System;
using System.Linq;

namespace Recognizers.Postal
{
    public static class Postal
    {
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
            if (!x.LettersOrDigits(ref i, out var part1) || part1.Length != 3)
                return false;
            x.Optional(x.WhiteSpaces(ref i));
            return x.LettersOrDigits(ref i, out var part2) && part2.Length == 3 && pos.AdvanceTo(i);
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
            if (!x.LettersOrDigits(ref i, out var part1) || part1.Length != 3)
                return Recognizers.Fail(out capture);
            x.Optional(x.WhiteSpaces(ref i));
            return x.LettersOrDigits(ref i, out var part2) && part2.Length == 3 && pos.AdvanceTo(i, x, out capture)
                || Recognizers.Fail(out capture);
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
            || Recognizers.Fail(out capture);

        /// <summary>
        /// Recognize an attention label.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool AttentionLabel(this Input x, ref Position pos) =>
               pos.Save(out var i) && x.LiteralIgnoreCase("Attention", ref i) && pos.AdvanceTo(i)
            || pos.Save(out i) && x.LiteralIgnoreCase("Attn", ref i) && pos.AdvanceTo(i)
            || pos.Save(out i) && x.LiteralIgnoreCase("Att", ref i) && pos.AdvanceTo(i);

        /// <summary>
        /// Recognize an attention label.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool AttentionLabel(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Save(out var i) && x.LiteralIgnoreCase("Attention", ref i) && pos.AdvanceTo(i, x, out capture)
            || pos.Save(out i) && x.LiteralIgnoreCase("Attn", ref i) && pos.AdvanceTo(i, x, out capture)
            || pos.Save(out i) && x.LiteralIgnoreCase("Att", ref i) && pos.AdvanceTo(i, x, out capture)
            || Recognizers.Fail(out capture);

    }
}
