using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Composite.SecondExample.Factory
{
    public class LogisticsFactory : ILogisticsFactory
    {
        //It´s just a example, but you can create different methods to create different types of shipments

        public ILogisticsItem CreateDefaultInboundShipment()
        {
            var mainContainer = new ShippingContainerComposite("ROOT-HUB-01");

            // Simulate adding various packages and sub-containers
            var electronics = new ShippingContainerComposite("ELEC-SEC");
            electronics.Add(new IndividualPackageLeaf("Laptop", 2.5));
            electronics.Add(new IndividualPackageLeaf("Monitor", 4.0));

            mainContainer.Add(electronics);
            mainContainer.Add(new IndividualPackageLeaf("Desk Chair", 15.0));

            return mainContainer;
        }
    }
}
