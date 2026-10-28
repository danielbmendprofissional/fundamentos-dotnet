using Microsoft.Extensions.DependencyInjection;
using Bad = DesignPatterns.Memento.Bad;
using Gof = DesignPatterns.Memento.Good.GOF;
using Micro = DesignPatterns.Memento.Good.Microsoft;

// =============================================== BAD ===============================================

Console.WriteLine("--- [BAD] Memento - Histórico Acoplado e Sem Encapsulamento ---");

// Criamos o objeto "Anêmico"
var perfil = new Bad.PerfilUsuarioBad
{
    Nome = "Felipe",
    Bio = "Arquiteto de Software",
    Idade = 30,
    TemaHex = "#000000"
};

// O serviço que "rouba" a responsabilidade de saber como salvar
var historico = new Bad.ServicoHistoricoBad();

// --- OPERAÇÃO 1 ---
Console.WriteLine($"[Estado Atual] {perfil.Nome} - {perfil.Bio}");
historico.SalvarEstado(perfil); // O serviço lê as propriedades uma a uma internamente

// --- OPERAÇÃO 2 (Alteração) ---
perfil.Nome = "Felipe Alterado";
perfil.Bio = "Bio que não deveria existir";
perfil.TemaHex = "AZUL_CLARO"; // VIOLAÇÃO: Setamos um valor inválido porque o campo é publico

Console.WriteLine($"[Estado Alterado] {perfil.Nome} - {perfil.Bio} - Cor: {perfil.TemaHex}");

// --- DESFAZER (UNDO) ---
Console.WriteLine("\nRealizando Undo manual...");
historico.Desfazer(perfil);

// OBRIGATORIEDADE: Verificação do resultado
Console.WriteLine($"[Após Undo] {perfil.Nome} - {perfil.Bio} - Cor: {perfil.TemaHex}");

// =============================================== GOOD ===============================================

// ============================= GOF

Console.WriteLine("--- [GOOD - GOF] Memento Profissional (Snapshots) ---");

var perfil1 = new Gof.PerfilUsuarioOriginator();
// OBRIGATORIEDADE: Caretaker que gerencia a coleção de Mementos.
var zelador = new List<Gof.Interfaces.IMemento>();

// Ação 1
perfil1.Atualizar("Felipe", "Dev C#", 30);
zelador.Add(perfil1.CriarSnapshot());

// Ação 2 (Erro do usuário)
perfil1.Atualizar("Felipe Errado", "Lixo", 99);
Console.WriteLine("Estado atual está corrompido...");

// OBRIGATORIEDADE: Restauração sem o Caretaker saber os detalhes.
perfil1.Restaurar(zelador.Last());

// ============================= MICROSOFT

Console.WriteLine("--- [GOOD - MICROSOFT] Memento com Snapshots Imutáveis & DI ---");

// 1. Setup do Container Microsoft
var services = new ServiceCollection();
services.AddTransient<Micro.PerfilUsuario>();
// Registro do Caretaker como Singleton para manter o histórico na sessão
services.AddSingleton<Micro.UndoRedoService<Micro.PerfilState>>();

var provider = services.BuildServiceProvider();

// 2. Execução
var perfil2 = provider.GetRequiredService<Micro.PerfilUsuario>();
var undoService2 = provider.GetRequiredService<Micro.UndoRedoService<Micro.PerfilState>>();

// Passo 1: Estado Inicial
perfil2.SetData("Carlos", "Engenheiro Microsoft", 45);
undoService2.Save(perfil2.TakeSnapshot());

// Passo 2: Alteração indesejada
perfil2.SetData("Carlos Errado", "Bio Corrompida", 0);
Console.WriteLine($"Estado Atual Corrompido: {perfil2}");

// Passo 3: Undo Profissional
var previousState2 = undoService2.Undo();
perfil2.Restore(previousState2);

Console.WriteLine($"Após Restauração: {perfil2}");

// ===================================================================================================