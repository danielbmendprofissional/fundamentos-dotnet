using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Proxy.Remote
{
    // Domain Interface. Application only knows this.
    public interface IWeatherService
    {
        decimal GetTemperatureCelsius(string cityName);
    }
}
