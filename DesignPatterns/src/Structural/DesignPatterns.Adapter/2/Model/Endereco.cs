using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Adapter._2.Model
{
    // Model from my system that needs to be populated with data from external API
    public record Endereco(string Cep, string Rua);
}
