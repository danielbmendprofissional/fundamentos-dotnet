using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Composite.SecondExample
{
    //Common interface for both composite and leaf nodes
    public interface ILogisticsItem
    {
        string GetDescription();
        double GetWeight();
    }
}
