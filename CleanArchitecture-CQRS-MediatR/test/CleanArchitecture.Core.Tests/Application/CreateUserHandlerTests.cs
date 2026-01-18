using AutoMapper;
using CleanArchitecture.Application.UseCases.Users.CreateUser;
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
    public class CreateUserHandlerTests : UserHandlerTestBase
    {
        private readonly CreateUserHandler _handler;

        public CreateUserHandlerTests()
        {
            _handler = new CreateUserHandler(UowMock.Object, EmailMock.Object, RepoMock.Object, Mapper);
        }

        [Fact(DisplayName = "Should create user and send welcome email when request is valid")]
        public async Task Should_Create_User_And_Send_Email()
        {
            // Arrange
            var request = new CreateUserRequest("thiago@valuto.com", "Thiago Silva");

            RepoMock.Setup(x => x.Create(It.IsAny<User>()));
            UowMock.Setup(x => x.Commit(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            EmailMock.Setup(x => x.EnviarEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(request.Email);

            RepoMock.Verify(x => x.Create(It.IsAny<User>()), Times.Once);
            UowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
            EmailMock.Verify(x => x.EnviarEmail(request.Email, "Created User", It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
