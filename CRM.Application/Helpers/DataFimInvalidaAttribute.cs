using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CRM.Application.Helpers;

public sealed class DataFimInvalidaAttribute : ValidationAttribute, IClientModelValidator
{
    public string PropertyDataInicio { get; set; }

    public DataFimInvalidaAttribute(string propertyDataInicio)
    {
        PropertyDataInicio = propertyDataInicio;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }
        var dataInicio = (DateTime)validationContext.ObjectType.GetProperty(PropertyDataInicio).GetValue(validationContext.ObjectInstance);

        DateTime.TryParse(value.ToString(), out DateTime dataTermino);

        if (dataTermino < dataInicio)
        {
            return new ValidationResult(ErrorMessage);
        }
        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Attributes.Add("data-val", "true");
        context.Attributes.Add("data-val-datafiminvalida", ErrorMessage);
        context.Attributes.Add("data-val-datafiminvalida-datainicio", PropertyDataInicio);
    }
}
