using AutoMapper;
using CleanArchitecture.Application.UseCases.Users.DeleteUser;
using CleanArchitecture.Core.Tests.Application.common;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Tests.Application
{
    public class DeleteUserHandlerTests : UserHandlerTestBase
    {
        private readonly DeleteUserHandler _handler;

        public DeleteUserHandlerTests()
        {
            _handler = new DeleteUserHandler(UowMock.Object, RepoMock.Object, Mapper);
        }

        [Fact(DisplayName = "Should delete user when it exists")]
        public async Task Should_Delete_User()
        {
            // Arrange
            var user = CreateValidUser();
            RepoMock.Setup(x => x.Get(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Act
            await _handler.Handle(new DeleteUserRequest(user.Id), default);

            // Assert
            RepoMock.Verify(x => x.Delete(user), Times.Once);
            UowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
