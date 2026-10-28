using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.ValueObjects;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Tests.Domain.Entities
{
    public class UserTests
    {
        // Helpers para gerar objetos válidos
        private static FullName ValidName() => FullName.Create("Daniel Pessoal");
        private static Email ValidEmail() => Email.Create("daniel@valuto.com");
        private static Phone ValidPhone() => Phone.Create("11999998888");

        [Fact(DisplayName = "Should create a valid user with default orientation")]
        public void Should_Create_Valid_User()
        {
            // Act
            var user = User.Create(ValidName(), ValidEmail(), ValidPhone());

            // Assert
            user.Should().NotBeNull();
            user.Orientation.Should().Be(Orientation.NoInformed);
            user.Name.Name.Should().Be("Daniel");
        }

        [Fact(DisplayName = "Should create user with specific orientation")]
        public void Should_Create_User_With_Orientation()
        {
            var user = User.Create(ValidName(), ValidEmail(), ValidPhone(), Orientation.Male);
            user.Orientation.Should().Be(Orientation.Male);
        }

        [Fact(DisplayName = "Should not create user with null components")]
        public void Should_Not_Create_User_With_Nulls()
        {
            Action act = () => User.Create(null!, ValidEmail(), ValidPhone());

            // Dependendo de como seu Guard.AgainstNull está implementado (ArgumentNullException ou DomainException)
            act.Should().Throw<Exception>();
        }

        [Fact(DisplayName = "Should throw when orientation enum is invalid")]
        public void Should_Not_Create_User_With_Invalid_Enum()
        {
            // Forçando um valor que não existe no Enum Orientation
            Action act = () => User.Create(ValidName(), ValidEmail(), ValidPhone(), (Orientation)99);

            act.Should().Throw<Exception>();
        }

        [Fact(DisplayName = "Should ensure two different users with same data have different IDs")]
        public void Should_Ensure_Unique_Ids_For_Different_Instances()
        {
            // Arrange & Act
            var user1 = User.Create(ValidName(), ValidEmail(), ValidPhone());
            var user2 = User.Create(ValidName(), ValidEmail(), ValidPhone());

            // Assert
            user1.Id.Should().NotBeEmpty();
            user2.Id.Should().NotBeEmpty();
            user1.Id.Should().NotBe(user2.Id); // Mesmo com dados iguais, IDs devem ser únicos
        }

        [Fact(DisplayName = "Should be able to change properties only through private setters (Reflections Check)")]
        public void Should_Have_Private_Setters()
        {
            // Esse teste garante que ninguém mude o estado do User sem passar pelo Create/Validate
            var user = typeof(User);

            user.GetProperty(nameof(User.Name))?.GetSetMethod(true).IsPrivate.Should().BeTrue();
            user.GetProperty(nameof(User.Email))?.GetSetMethod(true).IsPrivate.Should().BeTrue();
            user.GetProperty(nameof(User.Phone))?.GetSetMethod(true).IsPrivate.Should().BeTrue();
        }

        [Theory(DisplayName = "Should correctly assign all valid orientations")]
        [InlineData(Orientation.Male)]
        [InlineData(Orientation.Female)]
        [InlineData(Orientation.Other)]
        [InlineData(Orientation.NoInformed)]
        public void Should_Assign_All_Valid_Orientations(Orientation orientation)
        {
            // Act
            var user = User.Create(ValidName(), ValidEmail(), ValidPhone(), orientation);

            // Assert
            user.Orientation.Should().Be(orientation);
        }

        [Fact(DisplayName = "Should throw exception when mandatory ValueObjects are missing")]
        public void Should_Not_Allow_Null_Mandatory_ValueObjects()
        {
            // Testando a regra do Guard.AgainstNull dentro do User.Create
            Action actName = () => User.Create(null!, ValidEmail(), ValidPhone());
            Action actEmail = () => User.Create(ValidName(), null!, ValidPhone());
            Action actPhone = () => User.Create(ValidName(), ValidEmail(), null!);

            // Assert
            actName.Should().Throw<Exception>();
            actEmail.Should().Throw<Exception>();
            actPhone.Should().Throw<Exception>();
        }
    }
}
