namespace Vendas.Domain.Common.Base
{
    public abstract class AggragateRoot : Entity
    {
        protected AggragateRoot() : base()
        {
        }

        protected AggragateRoot(Guid id) : base(id)
        {
        }
    }
}
