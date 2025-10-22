using System;
using CRM.Domain.Validation;

namespace CRM.Domain.Entities.Formularios.Respostas;
public sealed class RespostaDiscursiva : Resposta
{
    public string Texto { get; private set; }

    public static RespostaDiscursiva Build(Guid perguntaId, string texto)
        => new(perguntaId, texto);

    private RespostaDiscursiva(Guid perguntaId, string texto)
        : base(perguntaId, "Discursiva")
    {
        Id = Guid.Empty;
        Texto = texto;

        Validate();
    }

    private void Validate()
    {
        DomainValidation.MaxLength(Texto, 5000, "Texto da resposta");
    }

    private RespostaDiscursiva() : base () { }
}
