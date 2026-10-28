using Solid.OCP.Good.@base;

namespace Solid.OCP.Good
{
    public class FashionDiscount : Discount
    {
        public override decimal Calculate(decimal valor) => valor * 0.5M;
    }
}
