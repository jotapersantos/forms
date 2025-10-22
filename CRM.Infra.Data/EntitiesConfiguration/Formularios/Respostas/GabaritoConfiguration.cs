using CRM.Domain.Entities.Formularios.Respostas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infra.Data.EntitiesConfiguration.Formularios.Respostas;

public class GabaritoConfiguration : IEntityTypeConfiguration<Gabarito>
{
    public void Configure(EntityTypeBuilder<Gabarito> builder)
    {
        builder.HasKey(gabarito => gabarito.Id);

        builder.Property(gabarito => gabarito.RespondidoEm);

        builder.HasDiscriminator<string>("Tipo")
       .HasValue<FormularioGabarito>("Formulario");

        builder.ToTable("gabaritos", "formularios");
    }
}
