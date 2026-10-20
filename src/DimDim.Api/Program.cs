using DimDim.Api.Data;
using DimDim.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<RendimentoService>();
builder.Services.AddDbContext<DimDimContext>(options =>
{
    if (builder.Configuration["DatabaseProvider"] == "Sqlite")
        options.UseSqlite(builder.Configuration.GetConnectionString("DimDim") ?? "Data Source=dimdim.local.db");
    else
        options.UseSqlServer(builder.Configuration.GetConnectionString("DimDim")
            ?? throw new InvalidOperationException("Configure ConnectionStrings__DimDim antes de iniciar."),
            sql => sql.EnableRetryOnFailure());
});
var app = builder.Build();
// Projeto academico sem migrations: schema criado no primeiro inicio de um banco vazio.
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<DimDimContext>().Database.EnsureCreated();
app.UseExceptionHandler(handler => handler.Run(async context =>
{
    context.Response.StatusCode = 500;
    await context.Response.WriteAsJsonAsync(new { erro = "Falha interna. Consulte os logs da aplicacao." });
}));
app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapGet("/health", async (DimDimContext db) =>
    await db.Database.CanConnectAsync()
        ? Results.Ok(new { status = "Healthy", banco = "Conectado" })
        : Results.Problem("Banco indisponivel", statusCode: 503));
app.Run();
public partial class Program { }
