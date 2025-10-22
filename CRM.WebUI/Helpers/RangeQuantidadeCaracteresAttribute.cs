using System;
using System.ComponentModel.DataAnnotations;
using CRM.Domain.Entities.Formularios.Modelos;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CRM.WebUI.Helpers;

internal class RangeQuantidadeCaracteresAttribute : ValidationAttribute
{
    private readonly string _propertyTipoPergunta;
    private readonly string _property;

    public RangeQuantidadeCaracteresAttribute(string propertyTipoPergunta, string property)
    {
        _propertyTipoPergunta = propertyTipoPergunta;
        _property = property;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        string tipoPergunta = (string)validationContext.ObjectType.GetProperty(_propertyTipoPergunta).GetValue(validationContext.ObjectInstance);
        
        
        if (value == null || tipoPergunta != "TextoCurto" || tipoPergunta != "TextoLongo") { 
            return ValidationResult.Success;
        }

        int.TryParse(value?.ToString(), out int quantidadeCaracteres);

        (int minimo, int maximo) limitesPorTipo = tipoPergunta switch
        {
            "TextoCurto" => (minimo: 1,  maximo: 250),
            "TextoLongo" => (minimo: 1, maximo: 5000),
            _ => (minimo: 0, maximo: 0)
        };

        if (quantidadeCaracteres < limitesPorTipo.minimo || quantidadeCaracteres > limitesPorTipo.maximo)
        {
            return new ValidationResult($"O campo {_property} deve ter entre {limitesPorTipo.minimo} e {limitesPorTipo.maximo}");
        }
        
        return ValidationResult.Success;
    }
}
