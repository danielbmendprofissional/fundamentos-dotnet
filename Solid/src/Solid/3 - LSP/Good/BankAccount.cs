namespace Solid.LSP.Good
{
    // now I set the property as protected so it can only be changed by derived classes.
    // the parent is abstract and forces the child to decide its own behavior.
    public abstract class BankAccount
    {
        public decimal Balance { get; protected set; }

        public abstract void WithDraw(decimal amount);
    }

    public class CheckingAccount : BankAccount
    {
        public override void WithDraw(decimal amount)
        {
            Balance -= amount;
        }
    }

    public class SavingsAccount : BankAccount
    {
        public override void WithDraw(decimal amount)
        {
            Balance -= amount;
        }
    }
}