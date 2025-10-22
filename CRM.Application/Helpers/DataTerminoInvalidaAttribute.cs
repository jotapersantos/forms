using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Helpers;

public class DataTerminoInvalidaAttribute : ValidationAttribute, IClientModelValidator
{
    public string PropertyDataInicio { get; set; }

    public DataTerminoInvalidaAttribute(string propertyDataInicio)
    {
        PropertyDataInicio = propertyDataInicio;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }
        var dataInicio = (DateTime) validationContext.ObjectType.GetProperty(PropertyDataInicio).GetValue(validationContext.ObjectInstance);

        DateTime.TryParse(value.ToString(), out DateTime dataTermino);

        if (dataTermino < dataInicio)
        {
            return new ValidationResult(ErrorMessage);
        }
        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Attributes.Add("data-val", "true");
        context.Attributes.Add("data-val-dataterminoinvalida", ErrorMessage);
        context.Attributes.Add("data-val-dataterminoinvalida-datainicio", PropertyDataInicio);
    }
}
