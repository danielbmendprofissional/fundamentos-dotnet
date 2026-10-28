using DesignPatterns.Flyweight.Good.GOF.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Flyweight.Good.GOF
{
    // [FlyweightFactory] no diagrama GoF.
    public class ItemFactory
    {
        // O dicionário garante a unicidade do Estado Intrínseco.
        private readonly Dictionary<string, IItemFlyweight> _flyweights = new();

        // A 'key' identifica o grupo de compartilhamento (ex: "Espada_Nivel_1").
        // Todo objeto do tipo Espada_Nivel_1 não precisa de criar outro, reutiliza o existente.
        public IItemFlyweight GetFlyweight(string key, byte[] icon, byte[] model)
        {
            if (!_flyweights.ContainsKey(key))
            {
                // Se a chave não existe, o objeto pesado é criado e guardado.
                _flyweights[key] = new ItemType(icon, model);
            }

            // Se já existe, retorna a referência do que já está na RAM.
            return _flyweights[key];
        }
    }
}
