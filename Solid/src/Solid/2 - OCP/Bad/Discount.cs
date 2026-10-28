namespace Solid.OCP.Bad
{

    public enum EProductType
    {
        Eletronics = 1,
        Health = 2,
        Beauty = 3,
        Fashion = 4
    }

    public class Discount
    {
        public decimal Calculate(EProductType productType, decimal price)
        {
            if(productType == EProductType.Eletronics)
                return price * 0.2M;
            if (productType == EProductType.Health)
                return price * 0.3M;
            if (productType == EProductType.Beauty)
                return price * 0.4M;
            if (productType == EProductType.Fashion)
                return price * 0.5M;

            return price;
        }
    }
}
