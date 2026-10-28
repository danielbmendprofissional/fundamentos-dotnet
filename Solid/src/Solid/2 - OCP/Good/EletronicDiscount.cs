using Solid.OCP.Good.@base;

namespace Solid.OCP.Good
{
    public class EletronicDiscount : Discount
    {
        public override decimal Calculate(decimal valor) => valor * 0.2M;
    }
}
