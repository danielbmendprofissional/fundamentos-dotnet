using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Flyweight.Bad
{
    public class Item
    {
        // Dados únicos (Extrínsecos)
        public Guid Id { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;


        // Dados pesados que se repetem (Intrínsecos)
        public byte[] IconData { get; set; }
        public byte[] Model3D { get; set; }


        public Item(string name, byte[] icon, byte[] model, Guid id, string owner)
        {
            Name = name;
            IconData = icon;
            Model3D = model;
            Id = id;
            Owner = owner;
        }

        public void Display()
        {
            Console.WriteLine($"Item {Id}: {Name} owned by {Owner} (Data Size: {IconData.Length + Model3D.Length} bytes)");
        }
    }
}
