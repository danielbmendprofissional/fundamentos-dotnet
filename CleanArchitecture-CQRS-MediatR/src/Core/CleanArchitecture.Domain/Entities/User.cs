using CleanArchitecture.Domain.Entities.Base;

namespace CleanArchitecture.Domain.Entities
{
    /*
        Entidade de domínio não anêmica, precisa escrever as validações e métodos
    */
    public class User : BaseEntity
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
}
