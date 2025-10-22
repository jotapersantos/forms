using System;
using CRM.Application.DTOs.Formularios.Modelos;

namespace CRM.Application.DTOs.Formularios;

public class FormularioDTO
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string MensagemConfirmacao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataTermino { get; set; }

    public ModeloDTO Modelo { get; set; }
    public Guid ModeloId { get; set; }
}
