using CRM.Domain.Entities.Formularios.Respostas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infra.Data.EntitiesConfiguration.Formularios.gabaritos;

public class FormularioGabaritoConfiguration : IEntityTypeConfiguration<FormularioGabarito>
{
    public void Configure(EntityTypeBuilder<FormularioGabarito> builder)
    {
        builder.HasBaseType<Gabarito>();

        builder.HasOne(gabarito => gabarito.Formulario)
               .WithMany(formulario => formulario.Gabaritos)
               .HasForeignKey(gabarito => gabarito.FormularioId);
    }
}
