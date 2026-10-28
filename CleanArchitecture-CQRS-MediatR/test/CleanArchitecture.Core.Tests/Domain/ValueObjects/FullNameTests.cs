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
    public class FullNameTests
    {
        [Theory(DisplayName = "Should create a valid FullName and split correctly")]
        [InlineData("Daniel Pessoal", "Daniel", "Pessoal")]
        [InlineData("Daniel B Silva", "Daniel B", "Silva")]
        public void Should_Create_Valid_FullName(string input, string expectedName, string expectedLastName)
        {
            var fullName = FullName.Create(input);

            fullName.Name.Should().Be(expectedName);
            fullName.LastName.Should().Be(expectedLastName);
            fullName.NormalizedName.Should().Be(input);
        }

        [Fact(DisplayName = "Should not create FullName with only one name")]
        public void Should_Not_Create_FullName_With_Only_One_Name()
        {
            Action act = () => FullName.Create("Daniel");

            act.Should().Throw<DomainException>()
               .WithMessage("A complete name needs to be a name and lastname.");
        }
    }
}
