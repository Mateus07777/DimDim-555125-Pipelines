namespace DimDim.Api.Services;
// Regra demonstrativa para evidenciar uma alteracao funcional na PR.
// Nao representa oferta ou previsao de rentabilidade financeira.
public sealed class RendimentoService
{
    public const decimal TaxaMensal = 0.01m;
    public decimal Calcular(decimal saldo)
    {
        if (saldo < 0) throw new ArgumentOutOfRangeException(nameof(saldo));
        return Math.Round(saldo * TaxaMensal, 2, MidpointRounding.AwayFromZero);
    }
}
