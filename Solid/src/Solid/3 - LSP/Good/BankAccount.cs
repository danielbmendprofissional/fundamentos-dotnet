using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solid.LSP.Good
{

    //agora eu ja deixei o set protected para só poder ser alteradoo para quem derivou.
    //agora o pai é abstrato e ele obriga o filho a decidir o comportamento dele.
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
