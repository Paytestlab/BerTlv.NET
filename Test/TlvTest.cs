﻿using System;
using System.Collections.Generic;
using System.Linq;
using BerTlv;
using Xunit;

namespace Test
{
    public class TlvTest
    {
        // Verify: http://www.emvlab.org/tlvutils/?data=6F1A840E315041592E5359532E4444463031A5088801025F2D02656E
        private readonly string _validAsciiHexString = "6F1A840E315041592E5359532E4444463031A5088801025F2D02656E";

        // Note: emvlab does not correctly parse inline and trailing padding (0x00, 0xFF), but the specification 
        // allows for it. The TLV string below should be parsed in the same way as the one above
        private readonly string _validAsciiHexStringWithPadding = "006F1B840E315041592E5359532E444446303100A5088801025F2D02656E0000";

        [Fact]
        [Trait("Build", "Run")]
        public void ShouldThrowExceptionOnEmptyString()
        {
            Assert.Throws<ArgumentException>(() => { Tlv.ParseTlv(""); });
        }

        [Fact]
        [Trait("Build", "Run")]
        public void ShouldThrowExceptionOnEmptyHexArray()
        {
            Assert.Throws<ArgumentException>(() => {  Tlv.ParseTlv(new byte[] { }); });
        }

        [Fact]
        [Trait("Build", "Run")]
        public void ShouldParseValidAsciiHexString()
        {
            var tlvs = Tlv.ParseTlv(_validAsciiHexString);

            AssertTlv(tlvs);
        }

        [Fact]
        [Trait("Build", "Run")]
        public void ShouldParseValidHexArray()
        {
            var validHexArray = Enumerable
                .Range(0, _validAsciiHexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(_validAsciiHexString.Substring(x, 2), 16))
                .ToArray();

            var tlvs = Tlv.ParseTlv(validHexArray);

            AssertTlv(tlvs);
        }

        [Fact]
        [Trait("Build", "Run")]
        public void ShouldParseValidAsciiHexStringWithPadding()
        {
            var tlvs = Tlv.ParseTlv(_validAsciiHexStringWithPadding);

            AssertTlv(tlvs);
        }

        [Fact]
        [Trait("Build", "Run")]
        public void ShouldParseValidHexArrayWithPadding()
        {
            var validHexArrayWithPadding = Enumerable
                .Range(0, _validAsciiHexStringWithPadding.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(_validAsciiHexStringWithPadding.Substring(x, 2), 16))
                .ToArray();

            var tlvs = Tlv.ParseTlv(validHexArrayWithPadding);

            AssertTlv(tlvs);
        }

        private static void AssertTlv(ICollection<Tlv> tlvs)
        {
            Assert.NotNull(tlvs);
            Assert.Single(tlvs);

            var _6F = tlvs.Single(t => t.HexTag == "6F");
            Assert.Equal(2, _6F.Children.Count);

            var _6F84 = _6F.Children.Single(t => t.HexTag == "84");
            Assert.Empty(_6F84.Children);
            Assert.Equal("315041592E5359532E4444463031", _6F84.HexValue);

            var _6FA5 = _6F.Children.Single(t => t.HexTag == "A5");
            Assert.Equal(2, _6FA5.Children.Count);

            var _6FA588 = _6FA5.Children.Single(t => t.HexTag == "88");
            Assert.Equal("02", _6FA588.HexValue);

            var _6FA55F2D = _6FA5.Children.Single(t => t.HexTag == "5F2D");
            Assert.Equal("656E", _6FA55F2D.HexValue);
        }

    }
}
