using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recognizers
{
    /// <summary>
    /// Recognize parts of a given address.
    /// </summary>
    public static class Address
    {
        //public static IEnumerable<char[]> PhoneNos(string[] lines) =>
        //    lines.Select(x => new Input(x)).PhoneNos().Select(x => x.Value);

        public static IEnumerable<Input> PhoneNos(this IEnumerable<Input> lines) =>
            lines.Where(x => x.Begin(out var pos) &&
                             x.Optional(x.WhiteSpaces(ref pos)) &&
                             (x.LiteralIgnoreCase("Fax", ref pos) || x.LiteralIgnoreCase("Phone", ref pos)) &&
                             x.Optional(x.WhiteSpaces(ref pos)) &&
                             x.Optional(x.Chars(':', ref pos)) &&
                             x.Optional(x.WhiteSpaces(ref pos)) &&
                             x.PhoneNumber(ref pos) &&
                             x.Optional(x.WhiteSpaces(ref pos)) &&
                             x.End(pos));

        public static IEnumerable<Input> PostalCodes(this IEnumerable<Input> lines) =>
            lines.Where(x => x.Begin(out var pos) &&
                             x.PostalOrZipCode(ref pos) &&
                             x.Optional(x.WhiteSpaces(ref pos)) &&
                             x.End(pos));

        public static IEnumerable<Input> AttentionLines(this IEnumerable<Input> lines) =>
            lines.Where(x => x.Begin(out var pos) &&
                             x.Attention(ref pos));

    }
}
