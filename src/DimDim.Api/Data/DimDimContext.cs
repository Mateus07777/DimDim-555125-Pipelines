using DimDim.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace DimDim.Api.Data;
public sealed class DimDimContext(DbContextOptions<DimDimContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Conta> Contas => Set<Conta>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Cliente>(e => {
            e.ToTable("Clientes"); e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(150).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
        });
        b.Entity<Conta>(e => {
            e.ToTable("Contas"); e.HasKey(x => x.Id);
            e.Property(x => x.Numero).HasMaxLength(20).IsRequired();
            e.Property(x => x.Saldo).HasPrecision(18, 2);
            e.HasIndex(x => x.Numero).IsUnique();
            e.HasOne<Cliente>().WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
