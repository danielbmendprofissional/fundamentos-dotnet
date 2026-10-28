using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Proxy.Remote.Bad
{
    // This class represents a bad response from an external weather service API.
    // On that Scenario, we gonna use Adapter too.
    public class ExternalWeatherResponse
    {
        public double temp_f { get; set; }
        public string loc_name { get; set; }
        public string status_code { get; set; }
    }
}
