using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Documento;
using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Documento.USA;
using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento;
using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento.USA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory
{
    public class UsaCheckout : IInternacionalCheckout
    {
        // 1 para N: Precisa de lógica (Dictionary) pois há escolha
        // Ela utiliza a sua fábrica especialista de pagamentos
        private readonly PagamentoFactory _pagamentoFactory = new USAPagamentoFactory();
        private readonly DocumentoFactory _documentoFactory = new UsaDocumentoFactory();

        // Implementação 1 para N: Delega para a USAPagamentoFactory
        public IPagamento CriarPagamento(string tipo)
        {
            return _pagamentoFactory.Criar(tipo);
        }

        // Implementação 1 para 1: Resolve aqui direto
        public IDocumento CriarDocumento()
        {
            return _documentoFactory.Criar();
        }
    }
}
