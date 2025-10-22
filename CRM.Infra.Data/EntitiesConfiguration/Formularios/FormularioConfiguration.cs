using CRM.Domain.Entities.Formularios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infra.Data.EntitiesConfiguration.Formularios;

public class FormularioConfiguration : IEntityTypeConfiguration<Formulario>
{
    public void Configure(EntityTypeBuilder<Formulario> builder)
    {
        builder.HasKey(formulario => formulario.Id);

        builder.Property(formulario => formulario.Nome).IsRequired().HasMaxLength(250);
        builder.Property(formulario => formulario.MensagemConfirmacao).HasMaxLength(250);

        builder.OwnsOne(formulario => formulario.Periodo, configuracao =>
        {
            configuracao.Property(e => e.DataInicio).HasColumnName("DataInicio");
            configuracao.Property(e => e.DataTermino).HasColumnName("DataTermino");
        });

        builder.HasOne(formulario => formulario.Modelo)
            .WithMany()
            .HasForeignKey(formulario => formulario.ModeloId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("formularios", "formularios");
    }
}
