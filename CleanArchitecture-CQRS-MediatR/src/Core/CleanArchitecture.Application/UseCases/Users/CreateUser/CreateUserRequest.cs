using MediatR;

namespace CleanArchitecture.Application.UseCases.Users.CreateUser
{
    /*
        Record que representa o comando para criar um novo usuário e contém os dados necessários
        para realiza-lo. Implementa a interface IRequest<> indicando que é um comando MediatR. 
    */
    public sealed record CreateUserRequest(string Email, string Name) : IRequest<CreateUserResponse>;
}
