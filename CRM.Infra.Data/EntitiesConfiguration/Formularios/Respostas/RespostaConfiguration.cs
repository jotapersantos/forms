using CRM.Domain.Entities.Formularios.Respostas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infra.Data.EntitiesConfiguration.Formularios;

public class RespostaConfiguration : IEntityTypeConfiguration<Resposta>
{
    public void Configure(EntityTypeBuilder<Resposta> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.Gabarito)
               .WithMany(p => p.Respostas)
               .HasForeignKey(p => p.GabaritoId);

        builder.HasOne(p => p.Pergunta)
               .WithMany(p => p.Respostas)
               .HasForeignKey(p => p.PerguntaId);

        builder.HasDiscriminator<string>("Tipo")
               .HasValue<RespostaDiscursiva>("Discursiva")
               .HasValue<RespostaObjetiva>("Objetiva");

        builder.HasIndex(e => new { e.GabaritoId, e.PerguntaId })
               .IsUnique();

        builder.ToTable("respostas", "formularios");
    }
}
