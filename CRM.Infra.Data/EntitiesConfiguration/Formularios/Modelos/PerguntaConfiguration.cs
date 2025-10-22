using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Modelos.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infra.Data.EntitiesConfiguration.Formularios.Modelos;

public class PerguntaConfiguration : IEntityTypeConfiguration<Pergunta>
{
    public void Configure(EntityTypeBuilder<Pergunta> builder)
    {
        builder.HasKey(pergunta => pergunta.Id);

        builder.Property(pergunta => pergunta.Enunciado).IsRequired().HasMaxLength(250);
        builder.Property(pergunta => pergunta.Descricao).HasMaxLength(1000);
        builder.Property(pergunta => pergunta.Obrigatorio).IsRequired();
        builder.Property(pergunta => pergunta.Ordem).IsRequired();
        builder.Property(pergunta => pergunta.SecaoId).IsRequired();

        builder.HasDiscriminator<string>("Tipo")
            .HasValue<TextoCurto>("TextoCurto")
            .HasValue<TextoLongo>("TextoLongo")
            .HasValue<MultiplaEscolha>("MultiplaEscolha")
            .HasValue<CaixaSelecao>("CaixaSelecao")
            .HasValue<ListaSuspensa>("ListaSuspensa");

        builder.HasOne(p => p.Secao)
            .WithMany(f => f.Perguntas)
            .HasForeignKey(p => p.SecaoId);

        builder.HasMany(pergunta => pergunta.Respostas)
               .WithOne(resposta => resposta.Pergunta)
               .OnDelete(DeleteBehavior.Cascade);


        builder.ToTable("perguntas", "formularios");

    }
}
