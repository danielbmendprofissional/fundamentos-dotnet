using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Observer.Good.GOF.Interface
{
    // [Observer] no diagrama GoF.
    // Interface única, posso ter varios Observers, mas todos herdam essa única interface
    public interface IObserver
    {
        // Método chamado pelo Subject para notificar uma mudança.
        // Passamos a interface IObservable para que o observador saiba quem o notificou.
        void Update(IObservable subject);
    }
}
