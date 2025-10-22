using System;
using CRM.Domain.Common.Extensions;
using CRM.Domain.Validation;

namespace CRM.Domain.Entities.Formularios.ValueObjects;

public class DateRange
{
    public DateTime DataInicio { get; private set; }
    public DateTime? DataTermino { get; private set; }

    public DateRange(DateTime? dataInicio, DateTime? dataTermino)
    {
        DataInicio = dataInicio?.BrazilTimeToUtc() ?? DateTime.UtcNow;
        DataTermino = dataTermino?.BrazilTimeToUtc() ?? null;
        Validate();
    }

    private void Validate()
    {
        DomainValidation.IsStartDateAfterEndDate(DataInicio, DataTermino);

        DomainValidation.IsStartDateBeforeToday(DataInicio);
    }

    private DateRange() { }
}
