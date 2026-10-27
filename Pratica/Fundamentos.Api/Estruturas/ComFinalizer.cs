public class ComFinalizer : IDisposable
{
    public static int Esquecidos;                 // quantos foram parar no finalizer
    private bool _liberado;

    public void Dispose()
    {
        _liberado = true;
        GC.SuppressFinalize(this);                // já limpei: o finalizer não precisa rodar
    }

    ~ComFinalizer()
    {
        if (!_liberado)
            Interlocked.Increment(ref Esquecidos);   // chegou aqui = alguém esqueceu o Dispose
    }
}
