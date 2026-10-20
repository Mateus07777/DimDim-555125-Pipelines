using DimDim.Api.Data;
using DimDim.Api.Models;
using DimDim.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DimDim.Api.Controllers;
[ApiController, Route("api/contas")]
public sealed class ContasController(DimDimContext db, RendimentoService rendimento) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Conta>>> Get() => await db.Contas.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Conta>> GetById(int id) =>
        await db.Contas.FindAsync(id) is Conta c ? Ok(c) : NotFound();
    [HttpPost]
    public async Task<ActionResult<Conta>> Post(ContaRequest input)
    {
        if (!await db.Clientes.AnyAsync(x => x.Id == input.ClienteId)) return BadRequest(new { erro = "Cliente inexistente." });
        var numero = input.Numero.Trim();
        if (await db.Contas.AnyAsync(x => x.Numero == numero)) return Conflict(new { erro = "Numero ja cadastrado." });
        var c = new Conta { Numero = numero, Saldo = input.Saldo, ClienteId = input.ClienteId };
        db.Contas.Add(c); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = c.Id }, c);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, ContaRequest input)
    {
        var c = await db.Contas.FindAsync(id);
        if (c is null) return NotFound();
        if (!await db.Clientes.AnyAsync(x => x.Id == input.ClienteId)) return BadRequest(new { erro = "Cliente inexistente." });
        var numero = input.Numero.Trim();
        if (await db.Contas.AnyAsync(x => x.Numero == numero && x.Id != id)) return Conflict(new { erro = "Numero ja cadastrado." });
        c.Numero = numero; c.Saldo = input.Saldo; c.ClienteId = input.ClienteId;
        await db.SaveChangesAsync(); return NoContent();
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await db.Contas.FindAsync(id);
        if (c is null) return NotFound();
        db.Contas.Remove(c); await db.SaveChangesAsync(); return NoContent();
    }
    [HttpGet("{id:int}/rendimento")]
    public async Task<IActionResult> Rendimento(int id)
    {
        var c = await db.Contas.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        return c is null ? NotFound() : Ok(new {
            contaId = c.Id, saldo = c.Saldo, taxaMensal = RendimentoService.TaxaMensal,
            rendimento = rendimento.Calcular(c.Saldo), demonstrativo = true });
    }
}
