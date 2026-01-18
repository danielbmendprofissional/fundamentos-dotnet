using AutoMapper;
using CleanArchitecture.Application.UseCases.Users.CreateUser;
using CleanArchitecture.Application.UseCases.Users.DeleteUser;
using CleanArchitecture.Application.UseCases.Users.GetAllUser;
using CleanArchitecture.Application.UseCases.Users.UpdateUser;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.Mapper
{
    public sealed class UserProfiles : Profile
    {
        public UserProfiles()
        {
            CreateMap<CreateUserRequest, User>()
                .ConstructUsing(src => User.Create(
                    FullName.Create(src.Name),
                    Email.Create(src.Email),
                    Phone.Create("00000000000"),
                    Orientation.NoInformed 
                ));

            CreateMap<User, CreateUserResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Adress));

            CreateMap<UpdateUserRequest, User>()
                .ConstructUsing(src => User.Create(
                    FullName.Create(src.Name),
                    Email.Create(src.Email),
                    Phone.Create("00000000000"),
                    Orientation.NoInformed
                ));

            CreateMap<User, UpdateUserResponse>();
            CreateMap<User, DeleteUserResponse>();
            CreateMap<User, GetAllUserResponse>();
        }
    }
}
