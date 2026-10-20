namespace DimDim.Api.Models;
public sealed class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
}
public sealed class Conta
{
    public int Id { get; set; }
    public string Numero { get; set; } = "";
    public decimal Saldo { get; set; }
    public int ClienteId { get; set; }
}
