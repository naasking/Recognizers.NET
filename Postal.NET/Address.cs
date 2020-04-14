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
            lines.Select(x => new Cursor(x)).PhoneNos().Select(x => x.Input);

        public static IEnumerable<Cursor> PhoneNos(this IEnumerable<Cursor> lines) =>
            lines.Where(x => x.Begin(out var pos) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             (x.CaseInsensitive("Fax", ref pos) || x.CaseInsensitive("Phone", ref pos)) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             x.Optional(x.Char(':', ref pos)) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             x.PhoneNumber(ref pos) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             x.End(pos));

        public static IEnumerable<Cursor> PostalCodes(this IEnumerable<Cursor> lines) =>
            lines.Where(x => x.Begin(out var pos) &&
                             x.PostalOrZipCode(ref pos) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             x.End(pos));

        public static IEnumerable<Cursor> AttentionLines(this IEnumerable<Cursor> lines) =>
            lines.Where(x => x.Begin(out var pos) &&
                             x.Attention(ref pos));

    }
}
