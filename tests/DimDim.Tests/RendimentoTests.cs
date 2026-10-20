using DimDim.Api.Services;
using Xunit;
namespace DimDim.Tests;
public sealed class RendimentoTests
{
    [Theory]
    [InlineData(1000, 10)]
    [InlineData(2500, 25)]
    [InlineData(0, 0)]
    [InlineData(123.45, 1.23)]
    public void Calcula_rendimento_demonstrativo(decimal saldo, decimal esperado) =>
        Assert.Equal(esperado, new RendimentoService().Calcular(saldo));
    [Fact]
    public void Rejeita_saldo_negativo() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new RendimentoService().Calcular(-1));
}
