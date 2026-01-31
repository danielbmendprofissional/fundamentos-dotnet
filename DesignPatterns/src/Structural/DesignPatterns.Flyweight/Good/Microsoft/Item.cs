using DesignPatterns.Flyweight.Good.Microsoft.Interface;

namespace DesignPatterns.Flyweight.Good.Microsoft
{
    public class Item
    {
        // ESTADO EXTRÍNSECO: Dados únicos que variam para cada instância.
        public Guid Id { get; }
        public string Owner { get; }
        public string Name { get; }
        public string Description { get; }

        // REFERÊNCIA AO FLYWEIGHT: A ligação com a parte pesada.
        private readonly IItemFlyweight _type;

        public Item(Guid id, string owner, string name, string description, IItemFlyweight type)
        {
            Id = id;
            Owner = owner;
            Name = name;
            Description = description;
            _type = type;
        }

        // PROPRIEDADES DE ACESSO DIRETO (Proxy):
        // Para quem usa a classe, parece que IconData e Model3D pertencem ao Item.
        // Na verdade, estamos apenas lendo do objeto compartilhado via referência.
        public byte[] IconData => _type.IconData;
        public byte[] Model3D => _type.Model3D;

        public void Display()
        {
            // O objeto consegue se "auto-exibir" usando seus dados únicos + os compartilhados.
            Console.WriteLine($"[ID: {Id}] {Name} | Owner: {Owner} | Model Size: {Model3D.Length} bytes");
        }
    }
}
