using MediatR;

namespace CleanArchitecture.Application.UseCases.Users.DeleteUser;

public sealed record DeleteUserRequest(Guid Id)
                  : IRequest<DeleteUserResponse>;
