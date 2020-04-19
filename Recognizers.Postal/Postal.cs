using System;
using System.Collections.Generic;
using System.Linq;

namespace Recognizers.Postal
{
    /// <summary>
    /// Recognizers specific to mailing/postal addresses.
    /// </summary>
    public static partial class Postal
    {
        #region Core postal address recognizers
        /// <summary>
        /// Recognize a zip code.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool ZipCode(this Input x, ref Position pos) =>
            x.WhileDigit(ref pos);

        /// <summary>
        /// Recognize a zip code.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool ZipCode(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
            x.WhileDigit(ref pos, out capture);

        /// <summary>
        /// Recognize a Canadian postal code.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool PostalCode(this Input x, ref Position pos)
        {
            var i = pos;
            if (!x.WhileLetterOrDigit(ref i, out var part1) || part1.Length != 3)
                return false;
            x.Optional(x.WhileWhiteSpace(ref i));
            return x.WhileLetterOrDigit(ref i, out var part2) && part2.Length == 3 && pos.AdvanceTo(i);
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
            if (!x.WhileLetterOrDigit(ref i, out var part1) || part1.Length != 3)
                return Recognizers.Fail(out capture);
            x.Optional(x.WhileWhiteSpace(ref i));
            return x.WhileLetterOrDigit(ref i, out var part2) && part2.Length == 3 && pos.AdvanceTo(i, x, out capture)
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
            && x.Optional(x.WhileChar(':', ref i) || x.WhileWhiteSpace(ref i))
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
            && x.Optional(x.WhileChar(':', ref i) || x.WhileWhiteSpace(ref i))
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

        /// <summary>
        /// Recognize a c/o label.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool CareOf(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Save(out var i)
            && x.Optional(x.WhileWhiteSpace(ref i))
            && (x.LiteralIgnoreCase("C\\O", ref i) || x.LiteralIgnoreCase("C/O", ref i))
            && pos.AdvanceTo(i, x, out capture)
            || Recognizers.Fail(out capture);

        /// <summary>
        /// Recognize a c/o label.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool CareOf(this Input x, ref Position pos) =>
               pos.Save(out var i)
            && x.Optional(x.WhileWhiteSpace(ref i))
            && (x.LiteralIgnoreCase("C\\O", ref i) || x.LiteralIgnoreCase("C/O", ref i))
            && pos.AdvanceTo(i);

        /// <summary>
        /// Recognize a city name.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool City(this Input x, ref Position pos, out ReadOnlySpan<char> capture) =>
               pos.Save(out var i)
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.WhileLetter(ref i, out capture)
            && pos.AdvanceTo(i)
            || Recognizers.Fail(out capture);

        /// <summary>
        /// Recognize an input that matches a known country.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool Country(this Input x, Dictionary<string, string> countries, out string country, ref Position pos) =>
               pos.Save(out var i)
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.WhileLetter(ref i, out var word)
            && countries.TryGetValue(word.ToString(), out country)
            && pos.AdvanceTo(i)
            || Recognizers.Fail(out country);

        /// <summary>
        /// Recognize a "state[,][ ]*province" input.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool StateOrProvince(this Input x, Dictionary<string, string> provinces, out string province, ref Position pos) =>
               pos.Save(out var i)
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.WhileLetter(ref i, out var word)
            && provinces.TryGetValue(word.ToString(), out province)
            && pos.AdvanceTo(i)
            || Recognizers.Fail(out province);

        public static bool CityProvinceCountry(this Input x, out ReadOnlySpan<char> city, Dictionary<string, string> provinces, out string province, Dictionary<string, string> countries, out string country, ref Position pos) =>
               pos.Save(out var i)
            && x.CityProvince(out city, provinces, out province, ref i)
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.Char(',', ref i))
            && x.Country(countries, out country, ref i)
            || (Recognizers.Fail(out city) | Recognizers.Fail(out province) | Recognizers.Fail(out country));

        public static bool CityProvince(this Input x, out ReadOnlySpan<char> city, Dictionary<string, string> provinces, out string province, ref Position pos) =>
               pos.Save(out var i)
            && x.City(ref i, out city)
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.Char(',', ref i))
            && x.StateOrProvince(provinces, out province, ref i)
            && x.Optional(x.WhileWhiteSpace(ref i))
            && x.Optional(x.Char(',', ref i))
            || (Recognizers.Fail(out city) | Recognizers.Fail(out province));
        #endregion

        #region Extract lines that match the rules
        /// <summary>
        /// Include only lines that match phone numbers.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IEnumerable<Input> PhoneNos(this IEnumerable<Input> lines) =>
            lines.Where(x => x.Begin(out var pos)
                          && x.Optional(x.WhileWhiteSpace(ref pos))
                          && (x.LiteralIgnoreCase("Fax", ref pos) || x.LiteralIgnoreCase("Phone", ref pos))
                          && x.Optional(x.WhileWhiteSpace(ref pos))
                          && x.Optional(x.WhileChar(':', ref pos))
                          && x.Optional(x.WhileWhiteSpace(ref pos))
                          && x.PhoneNumber(ref pos)
                          && x.Optional(x.WhileWhiteSpace(ref pos))
                          && x.End(pos));

        /// <summary>
        /// Include only lines that match postal codes.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IEnumerable<Input> PostalCodes(this IEnumerable<Input> lines) =>
            lines.Where(x => x.Begin(out var pos)
                          && x.PostalOrZipCode(ref pos)
                          && x.Optional(x.WhileWhiteSpace(ref pos))
                          && x.End(pos));

        /// <summary>
        /// Include only lines that match "city, province, country".
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IEnumerable<Input> CitiesProvincesCountries(this IEnumerable<Input> lines, Dictionary<string, string> provinces, Dictionary<string, string> countries) =>
            lines.Where(x => x.Begin(out var pos)
                          && x.CityProvinceCountry(out var city, provinces, out var prov, countries, out var c, ref pos)
                          && x.Optional(x.WhileWhiteSpace(ref pos))
                          && x.End(pos));

        /// <summary>
        /// Include only lines that match known provinces.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IEnumerable<Input> CitiesProvinces(this IEnumerable<Input> lines, Dictionary<string, string> provinces) =>
            lines.Where(x => x.Begin(out var pos)
                          && x.CityProvince(out var city, provinces, out var prov, ref pos)
                          && x.Optional(x.WhileWhiteSpace(ref pos))
                          && x.End(pos));

        /// <summary>
        /// Include only lines that match known countries.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IEnumerable<Input> Countries(this IEnumerable<Input> lines, Dictionary<string, string> countries) =>
            lines.Where(x => x.Begin(out var pos)
                          && x.Country(countries, out var c, ref pos)
                          && x.Optional(x.WhileWhiteSpace(ref pos))
                          && x.End(pos));

        /// <summary>
        /// Include only lines that match postal codes.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IEnumerable<Input> CareOf(this IEnumerable<Input> lines) =>
            lines.Where(x => x.Begin(out var pos)
                          && x.Optional(x.WhileWhiteSpace(ref pos))
                          && x.CareOf(ref pos)
                          && x.Optional(x.WhileWhiteSpace(ref pos))
                          && x.End(pos));

        /// <summary>
        /// Include only lines that match "attn:" labels.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IEnumerable<Input> AttentionLines(this IEnumerable<Input> lines) =>
            lines.Where(x => x.Begin(out var pos)
                          && x.Attention(ref pos));
        #endregion
    }
}
