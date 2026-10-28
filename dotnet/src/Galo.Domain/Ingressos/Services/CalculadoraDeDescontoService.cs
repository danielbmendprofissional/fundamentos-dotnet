using Galo.Domain.Torcida.Enums;

namespace Galo.Domain.Ingressos.Services
{
    /// <summary>
    /// DOMAIN SERVICE: regra de negócio que não pertence a uma entidade só
    /// (envolve o plano do Torcedor e o desconto da Compra) e não tem estado.
    /// </summary>
    public interface ICalculadoraDeDescontoService
    {
        decimal PercentualParaPlano(PlanoSocio plano);
    }

    public sealed class CalculadoraDeDescontoService : ICalculadoraDeDescontoService
    {
        public decimal PercentualParaPlano(PlanoSocio plano) => plano switch
        {
            PlanoSocio.Bronze => 5m,
            PlanoSocio.Prata => 10m,
            PlanoSocio.Ouro => 20m,
            PlanoSocio.Diamante => 30m,
            _ => 0m
        };
    }
}
