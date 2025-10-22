using System;
using CRM.Application.DTOs.Formularios.Modelos;
using CRM.Application.DTOs.Formularios.Respostas;

namespace CRM.WebUI.ViewModels.Formularios;

public record FormularioGabaritoComModeloVM
(
    DateTime RespondidoEm,
    ModeloDTO Modelo,
    FormularioGabaritoDTO GabaritoComRespostas
);
