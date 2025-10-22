using System;

namespace CRM.Domain.Dtos.Formularios;
public class ModeloIndexDTO
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public bool Ativo { get; set; }

    public bool ModeloUtilizado { get; set; }
}
