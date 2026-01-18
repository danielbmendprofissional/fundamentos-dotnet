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
    public class EmailTests
    {
        [Theory(DisplayName = "Should create valid and normalized email")]
        [InlineData("TESTE@GMAIL.COM", "teste@gmail.com")]
        [InlineData("user.name@valuto.com", "user.name@valuto.com")]
        public void Should_Create_Valid_Email(string input, string expected)
        {
            var email = Email.Create(input);
            email.Adress.Should().Be(expected);
        }

        [Theory(DisplayName = "Should throw exception for invalid email format")]
        [InlineData("invalid-email")]
        [InlineData("user@com")]
        [InlineData("@gmail.com")]
        public void Should_Not_Create_Invalid_Email(string input)
        {
            Action act = () => Email.Create(input);
            act.Should().Throw<DomainException>().WithMessage("invalid e-mail");
        }
    }
}
