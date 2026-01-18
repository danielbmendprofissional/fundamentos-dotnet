using AutoMapper;
using CleanArchitecture.Application.UseCases.Users.GetAllUser;
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
    public class GetAllUserHandlerTests : UserHandlerTestBase
    {
        private readonly GetAllUserHandler _handler;

        public GetAllUserHandlerTests()
        {
            _handler = new GetAllUserHandler(RepoMock.Object, Mapper);
        }

        [Fact(DisplayName = "Should return a list of users")]
        public async Task Should_Return_User_List()
        {
            // Arrange
            var users = new List<User> { CreateValidUser(), CreateValidUser() };
            RepoMock.Setup(x => x.GetAll(It.IsAny<CancellationToken>())).ReturnsAsync(users);

            // Act
            var result = await _handler.Handle(new GetAllUserRequest(), default);

            // Assert
            result.Should().HaveCount(2);
            RepoMock.Verify(x => x.GetAll(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
