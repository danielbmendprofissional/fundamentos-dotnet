using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solid.LSP.Bad
{
    //o filho (savings) está sobrescrevendo o pai alterando o seu comportamento

    public class BankAccount
    {
        //virtual = pode ou não sobrescrever o método, abstract sempre precisa do override
        public virtual void Withdraw(decimal amount)
        {
            Console.WriteLine("Comportamento pai");
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
            Console.WriteLine("Comportamento alterado");
        }
    }

        
    //Ele herda a classe pai e altera seu comportamento, agora essa conta simplesmente não pode sacar (o pai espera que sim).
    //É aqui que devemos pensar se precisamos alterar as "regras" do pai ou se a filha realmente é filha desse pai e deve  herdar.
    public class AnotherClass : BankAccount
    {
        public decimal Balance { get; set; }

        public override void Withdraw(decimal amount)
        {
            throw new InvalidOperationException("Conta não pode salvar");
        }
    }
}
