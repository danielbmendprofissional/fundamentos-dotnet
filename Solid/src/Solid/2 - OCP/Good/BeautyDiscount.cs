using Solid.OCP.Good.@base;

namespace Solid.OCP.Good
{
    public class BeautyDiscount : Discount
    {
        public override decimal Calculate(decimal valor) => valor * 0.4M;
    }
}
