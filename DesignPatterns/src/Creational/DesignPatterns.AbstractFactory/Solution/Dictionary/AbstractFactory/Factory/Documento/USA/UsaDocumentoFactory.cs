using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Documento.USA
{
    public class UsaDocumentoFactory : DocumentoFactory
    {
        public override IDocumento Criar()
        {
            return new UsaInvoice();
        }
    }
}
