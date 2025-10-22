using System;
using CRM.Application.DTOs.Formularios.Modelos;

namespace CRM.WebUI.ViewModels.Formularios;

public record ResponderFormularioViewModel
{
    public Guid FormularioId { get; init; }
    public string FormularioNome { get; init; }
    public ModeloDTO FormularioModelo { get; init; }
}
