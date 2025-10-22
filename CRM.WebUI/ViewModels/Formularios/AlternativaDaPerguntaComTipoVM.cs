using System;

namespace CRM.WebUI.ViewModels.Formularios;

public class AlternativaDaPerguntaComTipoVM
{
    public Guid SecaoIndex { get; set; }
    public Guid PerguntaIndex { get; set; }
    public string Tipo { get; set; }
}
