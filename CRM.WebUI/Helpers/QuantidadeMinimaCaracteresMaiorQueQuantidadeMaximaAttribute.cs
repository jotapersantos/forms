using System.ComponentModel.DataAnnotations;

namespace CRM.WebUI.Helpers;

public class QuantidadeMinimaCaracteresMaiorQueQuantidadeMaximaAttribute : ValidationAttribute
{
    private readonly string _tipoPerguntaProperty;
    private readonly string _quantidadeMaximaProperty;

    public QuantidadeMinimaCaracteresMaiorQueQuantidadeMaximaAttribute(string tipoPerguntaProperty, string quantidadeMaximaProperty)
    {
        _tipoPerguntaProperty = tipoPerguntaProperty;
        _quantidadeMaximaProperty = quantidadeMaximaProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        string tipoPergunta = (string)validationContext.ObjectType.GetProperty(_tipoPerguntaProperty).GetValue(validationContext.ObjectInstance);

        if (value == null && (tipoPergunta != "TextoCurto" || tipoPergunta != "TextoLongo"))
        {
            return ValidationResult.Success;
        }

        int.TryParse(value?.ToString(), out int quantidadeMinima);

        int? quantidadeMaxima = (int?) validationContext.ObjectType.GetProperty(_quantidadeMaximaProperty).GetValue(validationContext.ObjectInstance);

        if(quantidadeMaxima.HasValue == false || quantidadeMinima > quantidadeMaxima.Value)
        {
            return new ValidationResult("O valor de Quantidade Mínima de Caracteres não pode ser maior que Quantidade Máxima de Caracteres");
        }

        return ValidationResult.Success;
    }
}
