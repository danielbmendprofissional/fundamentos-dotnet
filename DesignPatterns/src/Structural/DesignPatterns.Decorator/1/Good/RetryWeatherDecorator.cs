using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Decorator.Good
{

    //On that example of Decorator pattern we are adding retry logic to an existing Weather Service
    //without modifying the original, but wrapping it inside a Decorator class...
    //So now the client continue to use the same Interface, but receives a new object.
    //Is like a power ranger transforming into a Megazord, maybe...
    //The power ranger is still there, but now with more power!
    public class RetryWeatherDecorator : IWeatherService
    {
        private readonly IWeatherService _inner;
        private readonly int _maxRetries;

        public RetryWeatherDecorator(IWeatherService inner, RetryConfig config)
        {
            _inner = inner;
            _maxRetries = config.MaxRetries;
        }

        public string GetWeather()
        {
            for (int i = 1; i <= _maxRetries; i++)
            {
                try
                {
                    return _inner.GetWeather(); // Call the original
                }
                catch (Exception)
                {
                    if (i == _maxRetries) throw;
                    Console.WriteLine($"[Decorator] Retrying in 1s... ({i}/{_maxRetries})");
                    Thread.Sleep(1000); // Wait before next try
                }
            }
            return null;
        }
    }
}
