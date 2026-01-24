using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Documento
{
    public abstract class DocumentoFactory
    {
        public abstract IDocumento Criar();
    }
}
