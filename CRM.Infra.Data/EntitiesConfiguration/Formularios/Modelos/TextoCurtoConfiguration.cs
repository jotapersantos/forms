using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using CRM.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infra.Data.EntitiesConfiguration.Formularios.Modelos;

public class TextoCurtoConfiguration : IEntityTypeConfiguration<TextoCurto>
{
    public void Configure(EntityTypeBuilder<TextoCurto> builder)
    {
        builder.HasBaseType<Pergunta>();

        builder.Property(pergunta => pergunta.TipoTexto)
               .HasConversion(
                    p => p.Value,
                    p => ETipoTextoCurto.FromValue(p)
               );
    }
}
