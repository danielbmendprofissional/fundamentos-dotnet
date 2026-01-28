using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Composite.SecondExample
{
    //Leaf node representing an individual package
    public class IndividualPackageLeaf : ILogisticsItem
    {
        private readonly string _name;
        private readonly double _weight;

        public IndividualPackageLeaf(string name, double weight)
        {
            _name = name;
            _weight = weight;
        }

        public string GetDescription() => _name;
        public double GetWeight() => _weight;
    }
}
