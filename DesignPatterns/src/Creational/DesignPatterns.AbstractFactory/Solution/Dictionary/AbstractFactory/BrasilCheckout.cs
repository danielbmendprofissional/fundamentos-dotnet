using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Documento;
using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Documento.Brasil;
using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento;
using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento.Brasil;
using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento.Brasil.Products;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory
{
    public class BrasilCheckout : IInternacionalCheckout
    {
        // 1 para N: Precisa de lógica (Dictionary) pois há escolha
        // Ela utiliza a sua fábrica especialista de pagamentos
        private readonly PagamentoFactory _pagamentoFactory = new BrasilPagamentoFactory();
        private readonly DocumentoFactory _documentoFactory = new BrasilDocumentoFactory();

        // Implementação 1 para N: Delega para a sua BrasilPagamentoFactory
        public IPagamento CriarPagamento(string tipo)
        {
            return _pagamentoFactory.Criar(tipo);
        }

        // Implementação 1 para 1: Resolve aqui direto (Igual ao vídeo)
        public IDocumento CriarDocumento()
        {
           return _documentoFactory.Criar();
        }
    }
}
