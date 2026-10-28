public class No
{
    public int Valor { get; }
    public No? Esquerda { get; set; }
    public No? Direita { get; set; }

    public No(int valor) => Valor = valor;
}

public class ArvoreBinariaBusca
{
    public No? Raiz { get; private set; }

    public bool Inserir(int valor)
    {
        if (Raiz is null) { Raiz = new No(valor); return true; }

        var atual = Raiz;
        while (true)
        {
            if (valor == atual.Valor) return false;                  // sem duplicados

            if (valor < atual.Valor)
            {
                if (atual.Esquerda is null) { atual.Esquerda = new No(valor); return true; }
                atual = atual.Esquerda;
            }
            else
            {
                if (atual.Direita is null) { atual.Direita = new No(valor); return true; }
                atual = atual.Direita;
            }
        }
    }

    public bool Buscar(int valor, out int passos)
    {
        passos = 0;
        var atual = Raiz;
        while (atual is not null)
        {
            passos++;
            if (valor == atual.Valor) return true;
            atual = valor < atual.Valor ? atual.Esquerda : atual.Direita;
        }
        return false;
    }

    public List<int> EmOrdem()
    {
        var resultado = new List<int>();
        VisitarEmOrdem(Raiz, resultado);
        return resultado;
    }

    public List<int> PreOrdem()
    {
        var resultado = new List<int>();
        VisitarPreOrdem(Raiz, resultado);
        return resultado;
    }

    public List<int> PosOrdem()
    {
        var resultado = new List<int>();
        VisitarPosOrdem(Raiz, resultado);
        return resultado;
    }

    public List<int> EmLargura()
    {
        var resultado = new List<int>();
        if (Raiz is null) return resultado;

        var fila = new Queue<No>();
        fila.Enqueue(Raiz);
        while (fila.Count > 0)
        {
            var no = fila.Dequeue();
            resultado.Add(no.Valor);
            if (no.Esquerda is not null) fila.Enqueue(no.Esquerda);
            if (no.Direita is not null) fila.Enqueue(no.Direita);
        }
        return resultado;
    }

    public int Altura() => CalcularAltura(Raiz);

    private static void VisitarEmOrdem(No? no, List<int> r)
    {
        if (no is null) return;
        VisitarEmOrdem(no.Esquerda, r);     // 1º esquerda
        r.Add(no.Valor);                    // 2º o próprio nó
        VisitarEmOrdem(no.Direita, r);      // 3º direita
    }

    private static void VisitarPreOrdem(No? no, List<int> r)
    {
        if (no is null) return;
        r.Add(no.Valor);                    // 1º o próprio nó
        VisitarPreOrdem(no.Esquerda, r);
        VisitarPreOrdem(no.Direita, r);
    }

    private static void VisitarPosOrdem(No? no, List<int> r)
    {
        if (no is null) return;
        VisitarPosOrdem(no.Esquerda, r);
        VisitarPosOrdem(no.Direita, r);
        r.Add(no.Valor);                    // por último o próprio nó
    }

    private static int CalcularAltura(No? no) =>
        no is null ? 0 : 1 + Math.Max(CalcularAltura(no.Esquerda), CalcularAltura(no.Direita));
}
