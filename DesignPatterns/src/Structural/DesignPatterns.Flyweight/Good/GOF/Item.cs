using DesignPatterns.Flyweight.Good.GOF.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Flyweight.Good.GOF
{
    // [UnsharedConcreteFlyweight] no diagrama GoF.
    // Representa o objeto individual que o usuário manipula.
    public class Item
    {
        // extrinsicState: Dados únicos desta instância (leves).
        public Guid Id { get; }
        public string Owner { get; }
        public string Name { get; }
        public string Description { get; }

        // Referência para o "Peso Pesado" compartilhado.
        private readonly IItemFlyweight _flyweight;

        public Item(Guid id, string owner, string name, string description, IItemFlyweight flyweight)
        {
            Id = id;
            Owner = owner;
            Name = name;
            Description = description;
            _flyweight = flyweight;
        }

        // A operação no contexto apenas delega a execução para o Flyweight,
        // passando a si mesmo (seu estado único) como argumento.
        public void Display()
        {
            _flyweight.Display(Id, Owner, Name, Description);
        }
    }
}
