using DesignPatterns.Adapter._2.Interface;
using DesignPatterns.Adapter._2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Adapter._2
{
    // Adapter class that consumes an external API and maps the data to my system model
    // The bridge between external API and my system!
    public class EnderecoProvider : IEnderecoProvider
    {
        private readonly HttpClient _client;

        public async Task<Endereco> Buscar(string cep)
        {
            // Consuming an external API
            var rawData = await _client.GetFromJsonAsync<CorreiosDTO>($"/cep/{cep}");

            // ACL Mapping and providing the expected model
            return new Endereco(rawData.cepEnd, rawData.ruaEnd);
        }
    }
}
