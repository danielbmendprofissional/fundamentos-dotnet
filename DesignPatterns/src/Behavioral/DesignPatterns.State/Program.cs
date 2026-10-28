using Bad = DesignPatterns.State.Bad;
using Gof = DesignPatterns.State.Good.GOF;
using Micro = DesignPatterns.State.Good.Microsoft;

// =============================================== BAD ===============================================

Console.WriteLine("--- [BAD] Pedido com Switch/Case e Enums ---");
// No BAD, o programador precisa gerenciar o estado na mão e a classe Pedido é um caos de IFs.
var pedidoBad = new Bad.PedidoBad { StatusAtual = Bad.Status.Aberto };
pedidoBad.StatusAtual = Bad.Status.Pago; // Muda para Pago internamente (se o programador não esqueceu o IF)
pedidoBad.Cancelar(); // Tenta cancelar um pago

// =============================================== GOOD ===============================================

// ============================= GOF
Console.WriteLine("\n--- [GOOD - GOF] State Pattern (Classes de Estado) ---");

// OBRIGATORIEDADE: Iniciamos o contexto com um estado inicial.
var pedidoGof = new Gof.PedidoContext(new Gof.EstadoAberto());

// O comportamento do método 'Cancelar' muda completamente sozinho 
// conforme as transições ocorrem.
pedidoGof.Pagar();    // Transita para EstadoPago
pedidoGof.Cancelar(); // Executa lógica de estorno (específica do EstadoPago)
pedidoGof.Pagar();    // Erro (específico do EstadoCancelado)

// ============================= MICROSOFT

Console.WriteLine("\n--- [GOOD - MICROSOFT] State Machine (Stateless) ---");

// OBRIGATORIEDADE: Ambiente profissional com Máquina de Estados robusta.
var pedidoMs = new Micro.PedidoMicrosoft();

pedidoMs.Pagar();    // Transita Aberto -> Pago
pedidoMs.Cancelar(); // Transita Pago -> Cancelado

// Se tentássemos pagar agora, a Stateless dispararia um erro de transição inválida
// em vez de nos obrigar a escrever um método vazio "override Pagar()".

// ===================================================================================================