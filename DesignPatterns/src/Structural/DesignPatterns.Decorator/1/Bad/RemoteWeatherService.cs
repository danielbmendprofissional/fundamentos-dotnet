using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Decorator.Bad
{
    //Bad class, who is not using Decorator pattern
    public class RemoteWeatherService : IWeatherService
    {
        private int _attempts = 0;

        public string GetWeather()
        {
            _attempts++;
            // Simulating a failure on the first 2 attempts
            if (_attempts <= 2)
            {
                Console.WriteLine($"[Service] Attempt {_attempts} failed (Network Error)...");
                throw new Exception("Timeout!");
            }
            return "Sunny, 25°C";
        }
    }
}
