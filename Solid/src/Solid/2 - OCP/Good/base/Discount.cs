namespace Solid.OCP.Good.@base
{
    public abstract class Discount
    {
        // -- marking a method as abstract tells the code that another class will implement this method.
        public abstract decimal Calculate(decimal valor);
    }
}
