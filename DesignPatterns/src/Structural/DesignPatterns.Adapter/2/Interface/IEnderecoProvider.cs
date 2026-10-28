using DesignPatterns.Adapter._2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Adapter._2.Interface
{
    // Interface that my system expects to get address data
    // My system don't know the external API directly
    public interface IEnderecoProvider
    {
        Task<Endereco> Buscar(string cep);
    }
}
