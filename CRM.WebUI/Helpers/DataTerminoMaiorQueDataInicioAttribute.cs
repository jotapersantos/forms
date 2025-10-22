using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CRM.WebUI.Helpers;

public class DataTerminoMaiorQueDataInicioAttribute : ValidationAttribute, IClientModelValidator
{
    private readonly string _dataInicioProperty;
    private readonly string _horaInicioProperty;
    private readonly string _horaTerminoProperty;

    public DataTerminoMaiorQueDataInicioAttribute(string dataInicioProperty, string horaInicioProperty, string horaTerminoProperty)
    {
        _dataInicioProperty = dataInicioProperty;
        _horaInicioProperty = horaInicioProperty;
        _horaTerminoProperty = horaTerminoProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var dataInicio = (DateOnly?)validationContext.ObjectType.GetProperty(_dataInicioProperty).GetValue(validationContext.ObjectInstance);
        var horaInicio = (TimeOnly?) validationContext.ObjectType.GetProperty(_horaInicioProperty).GetValue(validationContext.ObjectInstance);
        var dataTermino = (DateOnly?)value;
        var horaTermino = (TimeOnly?)validationContext.ObjectType.GetProperty(_horaTerminoProperty).GetValue(validationContext.ObjectInstance);

        if (dataTermino == null || horaTermino == null)
        {
            return ValidationResult.Success;
        }

        var dateTimeInicio = dataInicio?.ToDateTime(horaInicio.Value);
        var dateTimeTermino = dataTermino?.ToDateTime(horaTermino.Value);

        if(dateTimeInicio >  dateTimeTermino)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.Add("data-val", "true");
        context.Attributes.Add("data-val-dataterminovalida", ErrorMessage);
        context.Attributes.Add("data-val-dataterminovalida-datainicio", _dataInicioProperty);
        context.Attributes.Add("data-val-dataterminovalida-horainicio", _horaInicioProperty);
        context.Attributes.Add("data-val-dataterminovalida-horatermino", _horaTerminoProperty);
    }
}
