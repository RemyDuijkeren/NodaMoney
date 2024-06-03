using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.CurrencyUnitSpec
{
    public class CreateCurrencyUnit
    {
        [Theory]
        [InlineData("EUR")]
        [InlineData("MYR")]
        [InlineData("USD")]
        public void CurrencyIsCreated_GivenCodeIsThreeCapitalLetters(string code)
        {
            var currency = new CurrencyUnit(code, 0);

            currency.Code.Should().Be(code);
            //currency.Flag.Should().BeFalse();
        }
        
        [Theory]
        [InlineData("E")]
        [InlineData("EU")]
        [InlineData("EURO")]
        [InlineData("eur")]
        [InlineData("EU1")]
        public void ThrowArgumentException_GivenCodeIsNotThreeCapitalLetters(string code)
        {
            Action act = () => new CurrencyUnit(code, 0);
            
            act.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void ThrowArgumentNullException_GivenCodeIsNull()
        {
            Action act = () => new CurrencyUnit(null, 0);
            
            act.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void CurrencyIsXXX_GivenDefaultCurrency()
        {
            // Arrange / Act
            var noCurrency = new CurrencyUnit("XXX", 0);
            CurrencyUnit defaultCurrency = default;

            // Assert
            noCurrency.Should().NotBeNull();
            noCurrency.Should().Be(default(CurrencyUnit));

            // Assert with XUnit methods, because https://stackoverflow.com/questions/61556309/fluent-assertions-be-vs-equals
            Assert.Equal(default, noCurrency);
            Assert.Equal(default(CurrencyUnit), (object)noCurrency);
            Assert.True(noCurrency == default);
            Assert.True(noCurrency == default(CurrencyUnit));
            Assert.True(noCurrency.Equals(default));
            Assert.True(noCurrency.Equals((object)default(CurrencyUnit)));
            Assert.True(object.Equals(noCurrency, default(CurrencyUnit)));
        }
        
        [Fact]
        public void SizeIs2Bytes_GivenCurrencyType()
        {
            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Currency));

            size.Should().Be(64);
        }
        
        [Theory]
        [InlineData("EUR")]
        [InlineData("MYR")]
        [InlineData("USD")]
        [InlineData("XXX")]
        [InlineData("AAA")]
        [InlineData("ZZZ")]
        public void WhenTryToFitCodeIn2Bytes_ThenItShouldBePossible(string currencyCode)
        {
            bool isIso4217 = true;
            
            // EUR = 69, 85, 82 => 5, 21, 18
            byte[] A_InputBytes = currencyCode.ToCharArray().Select(c => (byte)(c - 'A' + 1)).ToArray();
            var A_InputArray = A_InputBytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).ToArray();
            
            // store in ushort = 2bytes (15bits needed, 1bit left)
            ushort B_Storage = (ushort)(A_InputBytes[0] << 10 | A_InputBytes[1] << 5| A_InputBytes[2]);
            if(isIso4217) B_Storage |= 1 << 15;
            var B_StorageString = Convert.ToString(B_Storage, 2).PadLeft(16, '0');
            string[] B_StorageArray = {B_StorageString.Substring(0, 8), B_StorageString.Substring(8, 8)};

            // shift into bytes again with clearing left 3 bits (by using & 0b_0001_1111 = 0x1F = 31)
            var C_OutputBytes = new[] { (byte)(B_Storage >> 10 & 0x1F), (byte)(B_Storage >> 5 & 0x1F), (byte)(B_Storage & 0x1F) };
            var C_OutputArray = C_OutputBytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).ToArray();
            
            // check if the code is ISO 4217
            bool C_OutputIsIso4217 = Convert.ToBoolean(B_Storage >> 15);
            
            var outputCode = new string(C_OutputBytes.Select(b => (char)(b + 'A' - 1)).ToArray());

            outputCode.Should().Be(currencyCode);
            C_OutputIsIso4217.Should().Be(isIso4217);
            
            // ushort for storing code (2bytes) = 15bits needed, 1bit left => use 1bit to mark if ISO? ISO=0, 1=other?
            // byte for storing namespace (4bits=15 or 3bits=7) and minor unit (4bits=15 or 5bit=31)? or use CurrencyInfo to retrieve?
        }
    }
}
