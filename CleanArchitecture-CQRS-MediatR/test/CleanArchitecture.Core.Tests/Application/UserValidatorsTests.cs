using CleanArchitecture.Application.UseCases.Users.CreateUser;
using CleanArchitecture.Application.UseCases.Users.DeleteUser;
using CleanArchitecture.Application.UseCases.Users.UpdateUser;
using CleanArchitecture.Application.Validator.User;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Tests.Application
{
    public class UserValidatorsTests
    {
        private readonly CreateUserValidator _validator = new();

        [Fact(DisplayName = "DeleteUser ID cannot be empty")]
        public void Delete_Validator_Should_Fail_When_Id_Empty()
        {
            var validator = new DeleteUserValidator();
            var result = validator.Validate(new DeleteUserRequest(Guid.Empty));
            result.IsValid.Should().BeFalse();
        }

        [Theory(DisplayName = "UpdateUser should validate email and name rules")]
        [InlineData("", "Nome", false)] // Email vazio
        [InlineData("email@valido.com", "Ab", false)] // Nome curto
        [InlineData("email@valido.com", "Nome Valido", true)] // Tudo OK
        public void Update_Validator_Should_Follow_Rules(string email, string name, bool expected)
        {
            var validator = new UpdateUserValidator();
            var result = validator.Validate(new UpdateUserRequest(Guid.NewGuid(), email, name));
            result.IsValid.Should().Be(expected);
        }

        [Theory(DisplayName = "CreateUser validation should fail for invalid data")]
        [InlineData("", "Thiago", "Email is required")]
        [InlineData("thiago@email.com", "Th", "Name is too short")]
        [InlineData("email-invalido", "Thiago Silva", "Invalid email format")]
        public void Should_Fail_When_Data_Is_Invalid(string email, string name, string reason)
        {
            // Arrange
            var request = new CreateUserRequest(email, name);

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse(reason);
        }
    }
}
