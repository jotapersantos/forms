using System.Collections.Generic;
using CRM.Domain.Entities.Formularios.Modelos;
using CRM.Domain.Entities.Formularios.Respostas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infra.Data.EntitiesConfiguration.Formularios.Respostas;
public class RespostaObjetivaConfiguration : IEntityTypeConfiguration<RespostaObjetiva>
{
    public void Configure(EntityTypeBuilder<RespostaObjetiva> builder)
    {
        builder.HasBaseType<Resposta>();

        builder.HasMany(p => p.AlternativasSelecionadas)
               .WithMany(p => p.PerguntasRespondidas)
               .UsingEntity<Dictionary<string, object>>("respostas-alternativas",
                    p => p.HasOne<Alternativa>().WithMany().HasForeignKey("AlternativaId"),
                    p => p.HasOne<RespostaObjetiva>().WithMany().HasForeignKey("RespostaId")
               );
    }
}
