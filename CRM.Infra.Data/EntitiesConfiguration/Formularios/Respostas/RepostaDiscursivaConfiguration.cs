using System;
using CRM.Domain.Entities.Formularios.Respostas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infra.Data.EntitiesConfiguration.Formularios.Respostas;
public class RepostaDiscursivaConfiguration : IEntityTypeConfiguration<RespostaDiscursiva>
{
    public void Configure(EntityTypeBuilder<RespostaDiscursiva> builder)
    {
        builder.HasBaseType<Resposta>();

        builder.Property(p => p.Texto).HasMaxLength(5000);
    }
}
