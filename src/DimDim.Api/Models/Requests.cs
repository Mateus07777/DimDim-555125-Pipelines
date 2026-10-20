using System.ComponentModel.DataAnnotations;
namespace DimDim.Api.Models;
public sealed record ClienteRequest(
    [param: Required, StringLength(100, MinimumLength = 2)] string Nome,
    [param: Required, EmailAddress, StringLength(150)] string Email);
public sealed record ContaRequest(
    [param: Required, StringLength(20, MinimumLength = 3)] string Numero,
    [param: Range(typeof(decimal), "0", "9999999999999999.99")] decimal Saldo,
    [param: Range(1, int.MaxValue)] int ClienteId);

