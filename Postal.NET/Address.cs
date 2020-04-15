using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Postal
{
    /// <summary>
    /// Recognize parts of a given address.
    /// </summary>
    public static class Address
    {
        public static IEnumerable<string> PhoneNos(string[] lines) =>
            lines.Select(x => new Input(x)).PhoneNos().Select(x => x.Value);

        public static IEnumerable<Input> PhoneNos(this IEnumerable<Input> lines) =>
            lines.Where(x => x.Begin(out var pos) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             (x.CaseInsensitive("Fax", ref pos) || x.CaseInsensitive("Phone", ref pos)) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             x.Optional(x.Char(':', ref pos)) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             x.PhoneNumber(ref pos) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             x.End(pos));

        public static IEnumerable<Input> PostalCodes(this IEnumerable<Input> lines) =>
            lines.Where(x => x.Begin(out var pos) &&
                             x.PostalOrZipCode(ref pos) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             x.End(pos));

        public static IEnumerable<Input> AttentionLines(this IEnumerable<Input> lines) =>
            lines.Where(x => x.Begin(out var pos) &&
                             x.Attention(ref pos));

    }
}
