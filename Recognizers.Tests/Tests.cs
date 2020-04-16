using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Recognizers.Tests
{
    public static class Tests
    {
        // examples:
        // http://stdcxx.apache.org/doc/stdlibug/26-1.html

        [Theory]
        [InlineData(1, 1, @"Some address somewhere
3000 Some street
Englewood, CO
55555
Attn:  Some Person
Phone: 555-555-5555")]
        [InlineData(1, 0, @"Some address somewhere
3000 Some street
Englewood, CO
55555
Phone: +1 (555)-555-5555")]
        [InlineData(0, 1, @"Some address somewhere
3000 Some street
Englewood, CO
55555
Attn:  Some Person")]
        [InlineData(0, 1, @"Some address somewhere
3000 Some street
Englewood, CO
55555
Att:  Some Person")]
        [InlineData(0, 1, @"Some address somewhere
3000 Some street
Englewood, CO
55555
Attention:  Some Person")]
        [InlineData(1, 0, @"Some address somewhere
3000 Some street
Englewood, CO
55555
Fax:   +1 555-555-555 ")]
        [InlineData(2, 0, @"Some address somewhere
3000 Some street
Englewood, CO
55555
Fax:   (555) 555-555 
Phone:   555 555 555 ")]
        public static void CheckData(int phoneCount, int attnCount, string input)
        {
            var lines = input.Split('\n').Select(x => new Input(x));
            var phones = Address.PhoneNos(lines);
            Assert.Equal(phoneCount, phones.Count());
         
            var attn = Address.AttentionLines(lines);
            Assert.Equal(attnCount, attn.Count());

            var code = Address.PostalCodes(lines);
            Assert.Single(code);
            Assert.Equal("55555\r", code.Single().Value);
        }

        [Theory]
        [InlineData(true, '=', "key=\"value\"")]
        [InlineData(true, ':', "key:\"value\"")]
        [InlineData(false, '=', "key=value")]
        [InlineData(false, '=', "key \"value\"")]
        public static void KeyValuePairs(bool isValid, char eq, string input)
        {
            var source = new Input(input);
            var pos = new Position();
            Assert.Equal(isValid, source.KeyValuePair(ref pos, eq));
            Assert.Equal(isValid, source.End(pos));

            if (isValid)
            {
                var pos2 = new Position();
                Assert.True(source.KeyValuePair(ref pos2, out var key, out var value, eq));
                Assert.Equal("key", key.ToString());
                Assert.Equal("value", value.ToString());
            }
        }

        [Theory]
        [InlineData(true, "555-555")]
        [InlineData(true, "(555) 555-555")]
        [InlineData(true, "555 555-555")]
        [InlineData(true, "555-555-555")]
        [InlineData(true, "555-555 555")]
        [InlineData(true, "+1 555-555 555")]
        [InlineData(true, "+1 (555) 555 555")]
        [InlineData(true, "(089) / 636-48018")]
        [InlineData(true, "+49-89-636-48018")]
        [InlineData(true, "19-49-89-636-48018")]
        [InlineData(true, "191 541 754 3010")]
        [InlineData(false, "5a5-555 555")]
        [InlineData(false, "a555-555 555")]
        public static void PhoneNumbers(bool isPhoneNo, string input)
        {
            var source = new Input(input);
            var pos = new Position();
            Assert.Equal(isPhoneNo, source.PhoneNumber(ref pos));
            Assert.Equal(isPhoneNo, source.End(pos));

            // check string capture
            var pos2 = new Position();
            Assert.Equal(isPhoneNo, source.PhoneNumber(ref pos2, out ReadOnlySpan<char> capture));
            if (isPhoneNo)
            {
                Assert.Equal(input, capture.ToString());

                // check digits capture
                var pos3 = new Position();
                Assert.Equal(isPhoneNo, source.PhoneNumber(ref pos3, out List<string> digits));
                var parts = input.Split(new[] { ' ', '-', '(', ')', '+', '/' }, StringSplitOptions.RemoveEmptyEntries);
                Assert.Equal(parts, digits);
            }
            
            // compare to regex
            var rx = new System.Text.RegularExpressions.Regex("[\x020]*[+]?[\x020]*[0-9]*[\x020]*([(][0-9]+[)])?[\x020]*[/]?([0-9-\x020])+");
            var match = rx.Match(input);
            Assert.Equal(isPhoneNo, match.Success && match.Length == input.Length);
        }
    }
}
