using System;
using System.Collections;
using System.Collections.Generic;

namespace CRM.WebUI.ViewModels.Formularios;

public record RespostasDetalhadasFormularioVM
(
    Guid FormularioId,
    string FormularioNome,
    Guid ModeloId,
    IEnumerable<Guid> GabaritoIds
);
