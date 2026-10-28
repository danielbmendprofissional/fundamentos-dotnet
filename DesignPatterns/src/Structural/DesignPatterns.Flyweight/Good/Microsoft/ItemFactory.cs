using DesignPatterns.Flyweight.Good.Microsoft.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Flyweight.Good.Microsoft
{
    public class ItemFactory
    {
        // Dicionário para garantir que não existam dois objetos pesados iguais na RAM.
        private readonly Dictionary<string, IItemFlyweight> _flyweights = new();

        public IItemFlyweight GetFlyweight(string categoryKey, byte[] icon, byte[] model)
        {
            // Se já temos esse "modelo" de item carregado, retornamos a referência existente.
            if (!_flyweights.ContainsKey(categoryKey))
            {
                _flyweights[categoryKey] = new ItemType(icon, model);
            }
            return _flyweights[categoryKey];
        }
    }
}
