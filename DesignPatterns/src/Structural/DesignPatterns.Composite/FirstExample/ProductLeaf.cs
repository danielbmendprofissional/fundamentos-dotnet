using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Composite.FirstExample
{
    //Leaf node representing an individual product
    public class ProductLeaf : ISellable
    {
        private string _name;
        private decimal _price;

        public ProductLeaf(string name, decimal price)
        {
            _name = name;
            _price = price;
        }

        public string GetName() => _name;
        public decimal GetPrice() => _price;
    }
}
