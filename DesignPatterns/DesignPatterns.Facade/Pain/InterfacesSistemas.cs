using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Facade.Pain
{
    // --- EXAMPLE INTERFACES ---

    public interface IEstoque { void BaixarItem(string produto); }
    public interface IPagamento { void Processar(decimal valor); }
    public interface IFiscal { void GerarNota(string produto); }
    public interface ILogistica { void AgendarEntrega(string produto); }
}
