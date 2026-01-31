using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Observer.Good.GOF.Interface
{
    // [Subject] no diagrama GoF.
    // Interface única, posso ter varios Observables, mas todos herdam essa única interface
    public interface IObservable
    {
        // Permite que um observador entre na lista de notificações.
        void Subscribe(IObserver observer);

        // Permite que um observador saia da lista.
        void Unsubscribe(IObserver observer);

        // Percorre a lista de observadores disparando o método Update.
        void Notify();
    }
}
