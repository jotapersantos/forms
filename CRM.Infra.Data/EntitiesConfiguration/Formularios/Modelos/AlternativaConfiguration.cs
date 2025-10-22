using CRM.Domain.Entities.Formularios.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infra.Data.EntitiesConfiguration.Formularios.Modelos;
public class AlternativaConfiguration : IEntityTypeConfiguration<Alternativa>
{
    public void Configure(EntityTypeBuilder<Alternativa> builder)
    {
        builder.HasKey(alternativa => alternativa.Id);

        builder.Property(alternativa => alternativa.Texto).HasMaxLength(250).IsRequired();
        builder.Property(alternativa => alternativa.Ordem);

        builder.HasOne(alternativa => alternativa.Pergunta)
            .WithMany()
            .HasForeignKey(o => o.PerguntaId)
            .IsRequired();

        builder.ToTable("alternativas", "formularios");

    }
}
