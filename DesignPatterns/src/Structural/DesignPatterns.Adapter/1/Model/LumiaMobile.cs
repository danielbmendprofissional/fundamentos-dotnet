using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Adapter._1.Model
{
    //Just an example model representing data from external API
    public class LumiaMobile
    {
        public string IdModelo { get; set; }
        public string Altura { get; set; }
        public string Largura { get; set; }
        public string Espessura { get; set; }
        public string Peso { get; set; }
    }
}
