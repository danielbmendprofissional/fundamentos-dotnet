using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Observer.Bad
{
    public class MarketingService
    {
        public void NotificarCampanha(string produto)
        {
            Console.WriteLine($"MARKETING: Gerando anúncios para {produto}.");
        }
    }
}
