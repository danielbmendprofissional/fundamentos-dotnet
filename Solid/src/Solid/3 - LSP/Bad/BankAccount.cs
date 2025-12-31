namespace Solid.LSP.Bad
{
    // the child class (Savings) is overriding the parent and changing its behavior

    public class BankAccount
    {
        // virtual = may or may not override the method, abstract always requires an override
        public virtual void Withdraw(decimal amount)
        {
            Console.WriteLine("Parent behavior");
        }
    }

    public class Savings : BankAccount
    {
        public decimal Balance { get; set; }

        public override void Withdraw(decimal amount)
        {
            if (Balance < amount)
            {
                throw new Exception();
            }

            Balance -= amount;
            Console.WriteLine("Behavior changed");
        }
    }

    // It inherits from the parent class and changes its behavior, now this account simply cannot withdraw (the parent expects it can).
    // This is where we should consider whether we need to change the parent’s “rules” or if the child really belongs as a subclass.
    public class AnotherClass : BankAccount
    {
        public decimal Balance { get; set; }

        public override void Withdraw(decimal amount)
        {
            throw new InvalidOperationException("Account cannot withdraw");
        }
    }
}