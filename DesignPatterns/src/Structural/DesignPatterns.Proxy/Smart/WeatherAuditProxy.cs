using DesignPatterns.Proxy.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Proxy.Smart
{
    // THE SMART PROXY: Adds intelligence (Logging/Auditing)
    // I don´t go to create a new big example for that one, because to be honest,
    // every proxy is a smart one, because it adds some logic, but just for an example
    // we can create a simple auditing proxy that logs requests and responses.
    // A proxy who adds loggs to an existing service.

    public class WeatherAuditProxy : IWeatherService
    {
        private readonly IWeatherService _realService;

        public WeatherAuditProxy(IWeatherService realService)
        {
            _realService = realService;
        }

        public decimal GetTemperatureCelsius(string cityName)
        {
            // 1. SMART LOGIC: Audit logging
            Console.WriteLine($"[AUDIT] Request for '{cityName}' at {DateTime.Now}");

            // 2. Forwarding the call
            var result = _realService.GetTemperatureCelsius(cityName);

            // 3. SMART LOGIC: Post-processing audit
            Console.WriteLine($"[AUDIT] Success. Value: {result}C");

            return result;
        }
    }
}
