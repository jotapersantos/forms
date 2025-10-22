using System;
using System.Collections.Generic;

namespace CRM.Application.DTOs.Formularios.Modelos;

public class ModeloDTO
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public DateTime CriadoEm { get; set; }
    public bool Ativo {  get; set; }
    public IEnumerable<SecaoDTO> Secoes { get; set; }
}
