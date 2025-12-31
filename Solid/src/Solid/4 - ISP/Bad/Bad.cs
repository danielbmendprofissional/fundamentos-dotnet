namespace Solid.ISP.Bad
{
    public class Bad { }

    public interface IAnimalActions
    {
        void Walk();
        void Fly();
        void Swim();
    }

    public class Dog : IAnimalActions
    {
        public void Walk() => Console.WriteLine("Dog walking");

        // Problem
        public void Fly() => throw new NotImplementedException(); // Dogs don’t fly
        public void Swim() => throw new NotImplementedException(); // Dogs don’t swim
    }
}