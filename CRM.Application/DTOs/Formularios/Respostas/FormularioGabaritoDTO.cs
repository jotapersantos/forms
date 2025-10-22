using System;
using System.Collections.Generic;

namespace CRM.Application.DTOs.Formularios.Respostas;

public class FormularioGabaritoDTO
{
    public Guid Id { get; set; }
    public DateTime RespondidoEm { get; set; }   

    public Guid ModeloId { get; set; }

    public Guid FormularioId { get; set; }
    public FormularioDTO Formulario { get; set; }

    public IEnumerable<RespostaDTO> Respostas { get; set; }
}
