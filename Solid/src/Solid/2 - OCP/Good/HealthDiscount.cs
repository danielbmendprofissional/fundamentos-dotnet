using Solid.OCP.Good.@base;

namespace Solid.OCP.Good
{
    public class HealthDiscount : Discount
    {
        public override decimal Calculate(decimal valor) => valor * 0.3M;
    }
}
