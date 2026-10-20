using System.Net;
using System.Net.Http.Json;
using DimDim.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;
namespace DimDim.Tests;
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string file = Path.Combine(Path.GetTempPath(), $"dimdim-{Guid.NewGuid()}.db");
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?> {
            ["DatabaseProvider"] = "Sqlite", ["ConnectionStrings:DimDim"] = $"Data Source={file};Pooling=False" }));
    protected override void Dispose(bool disposing) { base.Dispose(disposing); if (disposing && File.Exists(file)) File.Delete(file); }
}
public sealed class ApiTests
{
    [Fact]
    public async Task CRUD_das_duas_tabelas_preserva_relacionamento()
    {
        using var app = new ApiFactory(); using var http = app.CreateClient();
        var post = await http.PostAsJsonAsync("/api/clientes", new { nome = "Mateus", email = "mateus@example.com" });
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);
        var cliente = (await post.Content.ReadFromJsonAsync<Cliente>())!;
        Assert.Equal(HttpStatusCode.OK, (await http.GetAsync($"/api/clientes/{cliente.Id}")).StatusCode);
        var contaPost = await http.PostAsJsonAsync("/api/contas", new { numero = "555125-1", saldo = 1000, clienteId = cliente.Id });
        Assert.Equal(HttpStatusCode.Created, contaPost.StatusCode);
        var conta = (await contaPost.Content.ReadFromJsonAsync<Conta>())!;
        Assert.Equal(HttpStatusCode.Conflict, (await http.DeleteAsync($"/api/clientes/{cliente.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await http.PutAsJsonAsync($"/api/clientes/{cliente.Id}", new { nome = "Mateus Pierro", email = "pierro@example.com" })).StatusCode);
        Assert.Equal("Mateus Pierro", (await http.GetFromJsonAsync<Cliente>($"/api/clientes/{cliente.Id}"))!.Nome);
        Assert.Equal(HttpStatusCode.NoContent, (await http.PutAsJsonAsync($"/api/contas/{conta.Id}", new { numero = "555125-2", saldo = 2500, clienteId = cliente.Id })).StatusCode);
        Assert.Equal(2500m, (await http.GetFromJsonAsync<Conta>($"/api/contas/{conta.Id}"))!.Saldo);
        Assert.Equal(HttpStatusCode.OK, (await http.GetAsync($"/api/contas/{conta.Id}/rendimento")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await http.DeleteAsync($"/api/contas/{conta.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await http.GetAsync($"/api/contas/{conta.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await http.DeleteAsync($"/api/clientes/{cliente.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await http.GetAsync($"/api/clientes/{cliente.Id}")).StatusCode);
    }
    [Fact]
    public async Task Nao_permite_conta_sem_cliente()
    {
        using var app = new ApiFactory(); using var http = app.CreateClient();
        Assert.Equal(HttpStatusCode.BadRequest, (await http.PostAsJsonAsync("/api/contas", new { numero = "invalida", saldo = 10, clienteId = 999 })).StatusCode);
    }
    [Fact]
    public async Task Nao_permite_email_duplicado()
    {
        using var app = new ApiFactory(); using var http = app.CreateClient();
        var input = new { nome = "Cliente Teste", email = "teste@example.com" };
        Assert.Equal(HttpStatusCode.Created, (await http.PostAsJsonAsync("/api/clientes", input)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await http.PostAsJsonAsync("/api/clientes", input)).StatusCode);
    }
    [Fact]
    public async Task Valida_email_e_saldo()
    {
        using var app = new ApiFactory(); using var http = app.CreateClient();
        Assert.Equal(HttpStatusCode.BadRequest, (await http.PostAsJsonAsync("/api/clientes", new { nome = "Mateus", email = "invalido" })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await http.PostAsJsonAsync("/api/contas", new { numero = "123", saldo = -1, clienteId = 1 })).StatusCode);
    }
    [Fact]
    public async Task Banco_esta_acessivel_no_health()
    {
        using var app = new ApiFactory(); using var http = app.CreateClient();
        Assert.Equal(HttpStatusCode.OK, (await http.GetAsync("/health")).StatusCode);
    }
}
