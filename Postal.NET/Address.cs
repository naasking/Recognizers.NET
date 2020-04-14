using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Postal
{
    public static class Address
    {
        public static IEnumerable<string> MaybePhoneNos(string[] lines) =>
            lines.Select(x => new Cursor(x)).MaybePhoneNos().Select(x => x.Input);

        public static IEnumerable<Cursor> MaybePhoneNos(this IEnumerable<Cursor> lines) =>
            lines.Where(x => x.Begin(out var pos) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             (x.CaseInsensitive("Fax", ref pos) || x.CaseInsensitive("Phone", ref pos)) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             x.Optional(x.Char(':', ref pos)) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             x.PhoneNumber(ref pos) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             x.End(pos));

        public static IEnumerable<Cursor> MaybePostalCode(this IEnumerable<Cursor> lines) =>
            lines.Where(x => x.Begin(out var pos) &&
                             x.PostalOrZipCode(ref pos) &&
                             x.Optional(x.Whitespace(ref pos)) &&
                             x.End(pos));

        public static IEnumerable<Cursor> MaybeAttention(this IEnumerable<Cursor> lines) =>
            lines.Where(x => x.Begin(out var pos) &&
                             x.Attention(ref pos));

    }
}
