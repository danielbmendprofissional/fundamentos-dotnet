using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.ValueObjects;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Tests.Domain.ValueObjects
{
    public class PhoneTests
    {
        [Theory(DisplayName = "Should create and format phone correctly")]
        [InlineData("11988887777", "11988887777", "(11) 98888-7777")]
        [InlineData("(11) 4444-5555", "1144445555", "(11) 4444-5555")]
        public void Should_Create_Valid_Phone(string input, string expectedDigits, string expectedFormat)
        {
            var phone = Phone.Create(input);
            phone.Number.Should().Be(expectedDigits);
            phone.ToString().Should().Be(expectedFormat);
        }

        [Theory(DisplayName = "Should throw for invalid digit count")]
        [InlineData("123456789")] // 9 digits
        [InlineData("123456789012")] // 12 digits
        public void Should_Not_Create_Invalid_Phone(string input)
        {
            Action act = () => Phone.Create(input);
            act.Should().Throw<DomainException>()
               .WithMessage("phone number needs to contain 10 or 11 digits");
        }
    }
}
