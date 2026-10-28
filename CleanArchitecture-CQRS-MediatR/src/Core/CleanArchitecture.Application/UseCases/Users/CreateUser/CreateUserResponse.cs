namespace CleanArchitecture.Application.UseCases.Users.CreateUser
{
    /*
        Record que representa a resposta após a criação com as informações relevantes
        que desejo retornar.
    */
    public sealed record CreateUserResponse
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
}
