using AutoMapper;
using CleanArchitecture.Application.UseCases.Users.CreateUser;
using CleanArchitecture.Application.UseCases.Users.DeleteUser;
using CleanArchitecture.Application.UseCases.Users.GetAllUser;
using CleanArchitecture.Application.UseCases.Users.UpdateUser;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Mapper
{
    public sealed class UserProfiles : Profile
    {
        public UserProfiles()
        {
            CreateMap<CreateUserRequest, User>();
            CreateMap<User, CreateUserResponse>();

            CreateMap<UpdateUserRequest, User>();
            CreateMap<User, UpdateUserResponse>();

            CreateMap<User, GetAllUserResponse>();

            CreateMap<DeleteUserRequest, User>();
            CreateMap<User, DeleteUserResponse>();
        }
    }
}
