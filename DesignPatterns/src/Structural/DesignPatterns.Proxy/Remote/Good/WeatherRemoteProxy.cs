using DesignPatterns.Proxy.Remote.Bad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;

namespace DesignPatterns.Proxy.Remote.Good
{
    // This class acts as a REMOTE PROXY because it hides the network from client (HTTP).
    // It acts as an ADAPTER because it converts the bad Response to a Response that client needs.
    // Together, it forms your ANTICORRUPTION LAYER (ACL).
    public class WeatherRemoteProxy : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "SECRET_123";

        public WeatherRemoteProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public decimal GetTemperatureCelsius(string cityName)
        {
            // --- REMOTE PROXY PART: Hiding HTTP complexity ---
            // The caller doesn't know we are crossing the ocean via REST LOL
            var response = _httpClient.GetFromJsonAsync<ExternalWeatherResponse>(
                $"https://legacy-api.com/v1/weather?q={cityName}&key={_apiKey}"
            ).Result;

            if (response == null || response.status_code != "OK")
                throw new Exception("Remote service unreachable");

            // --- ADAPTER PART: Transforming data formats ---
            // Translating "temp_f" (External) to "Celsius" (Internal)
            decimal celsius = (decimal)(response.temp_f - 32) * 5 / 9;

            // Returning the clean result expected by our Domain
            // Domain dont know about how we made it, just get the Celsius temperature
            return Math.Round(celsius, 2);
        }
    }
}
