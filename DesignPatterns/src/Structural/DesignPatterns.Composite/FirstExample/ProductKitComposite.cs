using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Composite.FirstExample
{
    //Composite node representing a product kit containing multiple products
    //If the client asks for his price, it will sum the prices of all the items inside it
    //for each item, it doesn't matter if it's a leaf or another composite, until it reaches all leafs
    public class ProductKitComposite : ISellable
    {
        private string _kitName;
        private List<ISellable> _items = new();

        public ProductKitComposite(string name) => _kitName = name;

        public void Add(ISellable item) => _items.Add(item);

        public string GetName() => $"{_kitName} (Kit)";

        // Aqui está a mágica: ele soma o preço de tudo que está dentro dele
        public decimal GetPrice() => _items.Sum(x => x.GetPrice());
    }
}
