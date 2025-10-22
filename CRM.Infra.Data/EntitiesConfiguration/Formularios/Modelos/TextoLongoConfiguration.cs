using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infra.Data.EntitiesConfiguration.Formularios.Modelos;

public class TextoLongoConfiguration : IEntityTypeConfiguration<TextoLongo>
{
    public void Configure(EntityTypeBuilder<TextoLongo> builder)
    {
        builder.HasBaseType<Pergunta>();
    }
}
