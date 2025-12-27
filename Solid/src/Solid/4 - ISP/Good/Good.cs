using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solid.ISP.Good
{
    internal class Good { }

    public interface IAcoesCachorro
    {
        void Andar();
        void Latir();
    }
    public interface IAcoesPombo
    {
        void Voar();
        void Ciscar();
    }
    public interface IAcoesPeixe
    {
        void Nadar();
        void MorderIsca();
    }

    public class Cachorro : IAcoesCachorro
    {
        public void Andar() => Console.WriteLine("Cachorro comendo");
        public void Latir() => Console.WriteLine("Cachorro latindo");
    }

    public class Pombo : IAcoesPombo
    {
        public void Voar() => Console.WriteLine("Pombo voando");
        public void Ciscar() => Console.WriteLine("Pombo ciscando");
    }
}
