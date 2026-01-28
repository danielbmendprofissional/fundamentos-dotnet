using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Decorator.Good
{
    public class RetryConfig
    {
        public int MaxRetries { get; set; } = 3;
    }
}
