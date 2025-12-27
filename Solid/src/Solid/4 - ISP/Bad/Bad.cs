using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solid.ISP.Bad
{
    public class Bad {}

    public interface IAcoesAnimal
    {
        void Andar();
        void Voar();
        void Nadar();
    }

    public class Cachorro : IAcoesAnimal
    {
        public void Andar() => Console.WriteLine("Cachorro comendo");

        //Problema
        public void Voar() => throw new NotImplementedException(); //Cachoro não voa
        public void Nadar() => throw new NotImplementedException(); //Cachoro não nada
    }
}
