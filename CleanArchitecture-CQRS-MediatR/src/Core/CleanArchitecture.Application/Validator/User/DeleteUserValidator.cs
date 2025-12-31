using CleanArchitecture.Application.UseCases.Users.DeleteUser;
using FluentValidation;

namespace CleanArchitecture.Application.Validator.User
{
    public class DeleteUserValidator :
    AbstractValidator<DeleteUserRequest>
    {
        public DeleteUserValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
