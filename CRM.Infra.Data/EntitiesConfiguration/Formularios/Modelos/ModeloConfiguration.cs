using CRM.Domain.Entities.Formularios.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infra.Data.EntitiesConfiguration;

public class ModeloConfiguration : IEntityTypeConfiguration<Modelo>
{
    public void Configure(EntityTypeBuilder<Modelo> builder)
    {
        builder.HasKey(modelo => modelo.Id);

        builder.Property(modelo => modelo.Titulo).HasMaxLength(250).IsRequired();
        builder.Property(modelo => modelo.CriadoEmUtc).IsRequired();
        builder.Property(modelo => modelo.Ativo).IsRequired();

        builder.HasMany(f => f.Secoes)
            .WithOne(p => p.Modelo);

        builder.ToTable("modelos", "formularios");
    }
}
