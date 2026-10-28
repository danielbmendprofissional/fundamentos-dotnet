namespace DesignPatterns.TemplateMethod.Good.GOF
{
    // OBRIGATORIEDADE: Classe Abstrata
    // Define o esqueleto do algoritmo.
    public abstract class ProcessadorPagamento
    {
        // OBRIGATORIEDADE: O Template Method. Ele dita a ordem das operações.
        // Em C#, não usamos 'virtual' aqui para impedir que a subclasse mude o fluxo.
        public void TemplateMethod()
        {
            Log();
            Validar();
            OperationProcessar(); // O passo que varia
            Notificar();
        }

        // Passos fixos (Comuns a todos)
        private void Log() => Console.WriteLine("[GOF] Log: Iniciando transação.");
        private void Validar() => Console.WriteLine("[GOF] Validar: Checando dados obrigatórios.");
        private void Notificar() => Console.WriteLine("[GOF] Notificar: Enviando recibo por e-mail.");

        // OBRIGATORIEDADE: Operação abstrata.
        // Deve ser implementada pelas subclasses.
        protected abstract void OperationProcessar();
    }
}
