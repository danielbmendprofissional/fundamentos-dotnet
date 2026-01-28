using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Composite.FirstExample
{
    //Commom interface for both composite and leaf nodes
    public interface ISellable
    {
        string GetName();
        decimal GetPrice();
    }
}
