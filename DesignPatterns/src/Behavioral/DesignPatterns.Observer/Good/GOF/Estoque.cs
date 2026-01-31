using DesignPatterns.Observer.Good.GOF.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Observer.Good.GOF
{
    // [ConcreteSubject] no diagrama GoF.
    public class Estoque : IObservable
    {
        // Lista interna que armazena referências genéricas de IObserver.
        private readonly List<IObserver> _observers = new();

        public string ProdutoNome { get; private set; } = string.Empty;
        public int Quantidade { get; private set; }

        public void AtualizarEstoque(string nome, int qtd)
        {
            ProdutoNome = nome;
            Quantidade = qtd;

            Console.WriteLine($"ESTOQUE (GOF): {nome} atualizado para {qtd} unidades.");

            // A lógica de notificação é disparada aqui, após a mudança do estado.
            Notify();
        }

        public void Subscribe(IObserver observer) => _observers.Add(observer);

        public void Unsubscribe(IObserver observer) => _observers.Remove(observer);

        public void Notify()
        {
            // Notifica cada observador da lista sem saber o que eles farão com a informação.
            foreach (var observer in _observers)
            {
                observer.Update(this);
            }
        }
    }
}
