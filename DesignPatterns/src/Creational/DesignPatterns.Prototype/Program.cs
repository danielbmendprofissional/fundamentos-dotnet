using DesignPatterns.Prototype.Example;

var pedido = new PedidoPrototype
{
    Id = Guid.NewGuid(),
    Valor = 100,
    EnderecoEntrega = new Endereco { Rua = "Rua A, 123" }
};

var copia = pedido.Clone() as PedidoPrototype;

Console.WriteLine($"Pedido Original: {pedido}");