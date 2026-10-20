using DimDim.Api.Data;
using DimDim.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DimDim.Api.Controllers;
[ApiController, Route("api/clientes")]
public sealed class ClientesController(DimDimContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Cliente>>> Get() => await db.Clientes.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cliente>> GetById(int id) =>
        await db.Clientes.FindAsync(id) is Cliente c ? Ok(c) : NotFound();
    [HttpPost]
    public async Task<ActionResult<Cliente>> Post(ClienteRequest input)
    {
        var email = input.Email.Trim().ToLowerInvariant();
        if (await db.Clientes.AnyAsync(x => x.Email == email)) return Conflict(new { erro = "Email ja cadastrado." });
        var c = new Cliente { Nome = input.Nome.Trim(), Email = email };
        db.Clientes.Add(c); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = c.Id }, c);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, ClienteRequest input)
    {
        var c = await db.Clientes.FindAsync(id);
        if (c is null) return NotFound();
        var email = input.Email.Trim().ToLowerInvariant();
        if (await db.Clientes.AnyAsync(x => x.Email == email && x.Id != id)) return Conflict(new { erro = "Email ja cadastrado." });
        c.Nome = input.Nome.Trim(); c.Email = email;
        await db.SaveChangesAsync(); return NoContent();
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await db.Clientes.FindAsync(id);
        if (c is null) return NotFound();
        if (await db.Contas.AnyAsync(x => x.ClienteId == id)) return Conflict(new { erro = "Exclua as contas deste cliente primeiro." });
        db.Clientes.Remove(c); await db.SaveChangesAsync(); return NoContent();
    }
}
