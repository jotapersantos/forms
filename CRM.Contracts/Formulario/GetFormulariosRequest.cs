namespace CRM.Contracts.Formulario;

public record GetFormulariosRequest
(
    int draw,
    int start,
    int length,
    Search search
);

public record Search(string value);
