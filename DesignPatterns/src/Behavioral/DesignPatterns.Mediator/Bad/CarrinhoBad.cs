using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Bad
{
    public class CarrinhoBad
    {
        // OBRIGATORIEDADE: O Carrinho conhece TODOS os outros serviços.
        // Isso é a "Teia de Aranha" ou "Spaghetti Code".
        private readonly EstoqueBad _estoque = new();
        private readonly FinanceiroBad _financeiro = new();

        public void FinalizarCompra(string produto, decimal valor)
        {
            Console.WriteLine("--- [BAD] Iniciando Checkout ---");

            if (_estoque.Validar(produto))
            {
                _financeiro.Cobrar(valor);
                Console.WriteLine("[Carrinho] Compra finalizada com sucesso.");
            }
        }
    }
}
