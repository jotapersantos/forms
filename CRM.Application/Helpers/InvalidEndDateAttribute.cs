using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CRM.Application.Helpers;

public sealed class InvalidEndDateAttribute : ValidationAttribute, IClientModelValidator
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var startDate = (DateTime)validationContext.ObjectInstance.GetType().GetProperty("DataInicio").GetValue(validationContext.ObjectInstance);
        
        DateTime.TryParse(value.ToString(), out DateTime endDate);
        
        if (endDate < startDate)
        {
            return new ValidationResult(ErrorMessage = "O evento não pode terminar antes da data de início");
        }
        
        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Attributes.Add("data-val", "true");
        context.Attributes.Add("data-val-invalid-end-date", "O evento não pode terminar antes da data de início");
    }
}
