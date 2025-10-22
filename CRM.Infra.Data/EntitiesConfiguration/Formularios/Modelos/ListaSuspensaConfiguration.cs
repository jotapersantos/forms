using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infra.Data.EntitiesConfiguration.Formularios.Modelos;

public class ListaSuspensaConfiguration : IEntityTypeConfiguration<ListaSuspensa>
{
    public void Configure(EntityTypeBuilder<ListaSuspensa> builder)
    {
        builder.HasBaseType<Pergunta>();

        builder.HasMany(pergunta => pergunta.Alternativas)
            .WithOne()
            .HasForeignKey(alternativa => alternativa.PerguntaId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
