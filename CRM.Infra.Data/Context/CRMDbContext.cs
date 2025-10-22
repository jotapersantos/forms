using CRM.Domain.Entities.Formularios;
using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using CRM.Domain.Entities.Formularios.Respostas;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infra.Data.Context;

public class CRMDbContext : DbContext
{
    public CRMDbContext(DbContextOptions<CRMDbContext> options) : base(options)
    { }

    public DbSet<Modelo> Modelos { get; set; }
    public DbSet<Secao> Secoes { get; set; }
    public DbSet<Pergunta> Perguntas { get; set; }
    public DbSet<Alternativa> Alternativas { get; set; }

    public DbSet<Formulario> Formularios {  get; set; }
    public DbSet<FormularioGabarito> FormularioGabaritos { get; set; }

    public DbSet<Resposta> Respostas { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("public");

        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(CRMDbContext).Assembly);
    }
}
