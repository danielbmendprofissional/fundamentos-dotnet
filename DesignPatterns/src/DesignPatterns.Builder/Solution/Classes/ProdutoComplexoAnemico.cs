using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Builder.Solution.Classes
{
    internal sealed class ProdutoComplexoAnemico
    {
        internal ProdutoComplexoAnemico() { }

        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public double Peso { get; set; }
        public bool Ativo { get; set; }
        public List<ItemPedidoAnemico> ItensRelacionados { get; set; } = new();
        public FornecedorAnemico Fornecedor { get; set; } = new();
    }
}
