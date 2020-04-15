using System;
using System.Linq;
using Xunit;
using Postal;

namespace Postal.Tests
{
    public class Tests
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
        }
    }
}
