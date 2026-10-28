namespace Galo.Application.Common.Models
{
    public sealed record PagedResult<T>(IReadOnlyList<T> Itens, int Pagina, int TamanhoPagina, int Total)
    {
        public int TotalPaginas => TamanhoPagina <= 0 ? 0 : (int)Math.Ceiling(Total / (double)TamanhoPagina);
    }

    /// <summary>Normaliza paginação vinda da API (evita pedir 1 milhão de linhas).</summary>
    public static class Paginacao
    {
        public const int TamanhoMaximo = 200;

        public static (int Pagina, int Tamanho) Normalizar(int pagina, int tamanho)
            => (Math.Max(1, pagina), Math.Clamp(tamanho, 1, TamanhoMaximo));
    }
}
