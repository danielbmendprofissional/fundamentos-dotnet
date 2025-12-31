using CleanArchitecture.Domain.Entities.Base;

namespace CleanArchitecture.Domain.Entities
{
    /*
        Non-anemic domain entity, must include validations and methods
    */
    public class User : BaseEntity
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
}
