using AutoMapper;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Mapper;
using CleanArchitecture.Application.UseCases.Users.CreateUser;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces.Repository;
using CleanArchitecture.Domain.Interfaces.Repository.Base;
using CleanArchitecture.Domain.ValueObjects;
using Moq;

namespace CleanArchitecture.Core.Tests.Application.common
{
    public abstract class UserHandlerTestBase
    {
        protected readonly IMapper Mapper;
        protected readonly Mock<IUnitOfWork> UowMock = new();
        protected readonly Mock<IUserRepository> RepoMock = new();
        protected readonly Mock<IEmailService> EmailMock = new();

        protected UserHandlerTestBase()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<UserProfiles>());
            Mapper = config.CreateMapper();
        }

        // Helper para criar um User válido, já que os VOs (Email, Name) têm validação
        protected User CreateValidUser()
        {
            return User.Create(
                FullName.Create("Daniel Pessoal"),
                Email.Create("daniel@valuto.com"),
                Phone.Create("11999998888")
            );
        }
    }
}
