using DesignPatterns.Command;
using DesignPatterns.Command.Bad;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Bad = DesignPatterns.Command.Bad;
using Gof = DesignPatterns.Command.Good.GOF;
using Micro = DesignPatterns.Command.Good.Microsoft;

// =============================================== BAD ===============================================

var vendaServiceBad = new Bad.VendaProcessor();
var dto = new PedidoDto
{
    Produto = "Bad",
    Quantidade = 1,
    PrecoUnitario = 10
};
vendaServiceBad.ExecutarVenda(dto);

// =============================================== GOOD ===============================================

// ============================= GOF

var estoqueGof = new Bad.EstoqueService();
var invoker = new Gof.ProcessadorInvoker();
invoker.Processar(new Gof.BaixarEstoqueCommand(estoqueGof, "Notebook Pro", 1));

// ============================= MICROSOFT

// OBRIGATORIEDADE: Setup do Container de DI Profissional.
var serviceProvider = new ServiceCollection()
    .AddCommandServices()
    .BuildServiceProvider();

// OBRIGATORIEDADE: O "Invoker" no mundo Microsoft é a interface IMediator.
var mediator = serviceProvider.GetRequiredService<IMediator>();

// OBRIGATORIEDADE: Criamos o comando (Data Only).
var command = new Micro.Command.BaixarEstoqueCommand("Placa de Vídeo RTX", 1);

Console.WriteLine("\n--- [GOOD - MSFT] MediatR (Command Dispatcher) ---");

// O cliente (Program) não sabe QUEM processa o comando, apenas o envia.
await mediator.Send(command);

// ===================================================================================================
// CONCLUSÃO: 
// A implementação Microsoft (MediatR) leva o Command ao nível máximo de desacoplamento.
// 1. O invocador não conhece o comando concreto (apenas envia via interface).
// 2. O comando não conhece a lógica (apenas carrega dados).
// 3. O Handler isola a execução, permitindo adicionar Behaviors (Log, Validação) de forma transversal.
// ===================================================================================================