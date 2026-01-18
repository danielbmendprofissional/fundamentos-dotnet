using AutoMapper;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces.Repository;
using CleanArchitecture.Domain.Interfaces.Repository.Base;
using MediatR;

namespace CleanArchitecture.Application.UseCases.Users.CreateUser
{
    /*
        Handler que vai operar o command(request) -> mapear os dados, operar o negócio e retornar o response.
        Sealed para não ser herdada/alterada por ninguém.
    */

    public sealed class CreateUserHandler : IRequestHandler<CreateUserRequest, CreateUserResponse>
    {
        private readonly IUnitOfWork _unityOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public CreateUserHandler(IUnitOfWork unityOfWork, IEmailService emailService, IUserRepository userRepository, IMapper mapper)
        {
            _unityOfWork = unityOfWork;
            _emailService = emailService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(request);

            _userRepository.Create(user);

            await _unityOfWork.Commit(cancellationToken);

            await _emailService.EnviarEmail(
                request.Email, "Created User", "congratulations, you are now part of the team!", cancellationToken);

            return _mapper.Map<CreateUserResponse>(user);
        }
    }
}
