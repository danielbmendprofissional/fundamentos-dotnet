namespace CleanArchitecture.Application.UseCases.Users.GetAllUser;

public sealed record GetAllUserResponse
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
}
