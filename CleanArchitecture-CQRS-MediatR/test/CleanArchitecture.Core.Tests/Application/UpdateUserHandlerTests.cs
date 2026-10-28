using AutoMapper;
using CleanArchitecture.Application.UseCases.Users.UpdateUser;
using CleanArchitecture.Core.Tests.Application.common;
using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Tests.Application
{
    public class UpdateUserHandlerTests : UserHandlerTestBase
    {
        private readonly UpdateUserHandler _handler;

        public UpdateUserHandlerTests()
        {
            _handler = new UpdateUserHandler(UowMock.Object, RepoMock.Object, Mapper);
        }

        [Fact(DisplayName = "Should update user and commit transaction")]
        public async Task Should_Update_User_When_Exists()
        {
            // Arrange
            var user = CreateValidUser();
            var request = new UpdateUserRequest(user.Id, "novo@email.com", "Novo Nome");

            RepoMock.Setup(x => x.Get(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            result.Should().NotBeNull();
            RepoMock.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            UowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
