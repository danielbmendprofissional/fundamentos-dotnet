namespace Solid.ISP.Good
{
    internal class Good { }

    public interface IDogActions
    {
        void Walk();
        void Bark();
    }
    public interface IPigeonActions
    {
        void Fly();
        void Peck();
    }
    public interface IFishActions
    {
        void Swim();
        void BiteHook();
    }

    public class Dog : IDogActions
    {
        public void Walk() => Console.WriteLine("Dog walking");
        public void Bark() => Console.WriteLine("Dog barking");
    }

    public class Pigeon : IPigeonActions
    {
        public void Fly() => Console.WriteLine("Pigeon flying");
        public void Peck() => Console.WriteLine("Pigeon pecking");
    }
}