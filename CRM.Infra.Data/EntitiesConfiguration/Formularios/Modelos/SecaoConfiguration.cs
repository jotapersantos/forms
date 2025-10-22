using CRM.Domain.Entities.Formularios.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infra.Data.EntitiesConfiguration.Formularios.Modelos;

public class SecaoConfiguration : IEntityTypeConfiguration<Secao>
{
    public void Configure(EntityTypeBuilder<Secao> builder)
    {
        builder.HasKey(secao => secao.Id);

        builder.Property(secao => secao.Titulo).HasMaxLength(250);
        builder.Property(secao => secao.Ordem);

        builder.HasMany(fs => fs.Perguntas)
            .WithOne(f => f.Secao);

        builder.HasOne(fs => fs.Modelo)
            .WithMany(f => f.Secoes)
            .HasForeignKey(fs => fs.ModeloId);

        builder.ToTable("secoes", "formularios");

    }
}
