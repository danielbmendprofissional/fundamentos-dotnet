using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Composite.SecondExample
{
    //Composite node representing a shipping container that can hold multiple logistics items
    public class ShippingContainerComposite : ILogisticsItem
    {
        private readonly string _identifier;
        private readonly List<ILogisticsItem> _items = new();

        public ShippingContainerComposite(string identifier) => _identifier = identifier;

        // Management methods for child items, only available in the composite
        // Is not a perfect implementation, but to respect ISP we avoid to add these methods to the ILogisticsItem interface

        public void Add(ILogisticsItem item) => _items.Add(item);
        public void Remove(ILogisticsItem item) => _items.Remove(item);

        public string GetDescription() => $"Container {_identifier} containing {_items.Count} items";

        // Recursive...
        public double GetWeight() => _items.Sum(item => item.GetWeight());
    }
}
