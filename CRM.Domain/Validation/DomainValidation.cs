using System;

namespace CRM.Domain.Validation;

public class DomainValidation
{
    public static void NotNull(object target, string fieldName)
    {
        if (target is null)
        {
            throw new DomainValidationException($"{fieldName} não pode ser nulo.");
        }
    }

    public static void NotNullOrEmpty(Guid? target, string fieldName)
    {
        if(target == null && target == Guid.Empty)
        {
            throw new DomainValidationException($"{fieldName} não pode ser nulo ou vazio.");
        }
    }

    public static void NotNullOrEmpty(string target, string fieldName)
    {
        if (string.IsNullOrEmpty(target))
        {
            throw new DomainValidationException($"{fieldName} não pode ser nulo ou vazio.");
        }
    }

    public static void NotNullOrWhiteSpace(string target, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(target))
        {
            throw new DomainValidationException($"{fieldName} não pode ser nulo ou vazio.");
        }
    }

    public static void MinLength(string target, int minLength, string fieldName)
    {
        if (string.IsNullOrEmpty(target) == false && target.Length < minLength)
        {
            throw new DomainValidationException($"{fieldName} não pode ter menos de {minLength} caracteres.");
        }
    }

    public static void MaxLength(string target, int maxLength, string fieldName)
    {
        if (string.IsNullOrEmpty(target) == false && target.Length > maxLength)
        {
            throw new DomainValidationException($"{fieldName} não pode ter mais de {maxLength} caracteres.");
        }
    }

    public static void IsStartDateAfterEndDate(DateTime start, DateTime? end) 
    { 
        if(end.HasValue == true && start > end)
        {
            throw new DomainValidationException("A data inicial não pode ser maior que a data final.");
        }
    }

    public static void IsStartDateBeforeToday(DateTime start)
    {
        if(start.Date < DateTime.UtcNow.Date)
        {
            throw new DomainValidationException("A data inicial não pode ser anterior que a data de hoje.");
        }
    }

    public static void IsLessThan(int value, int minValue, string fieldName)
    {
        if (value < minValue)
        {
            throw new DomainValidationException($"O valor de {fieldName} não pode ser menor que {minValue}.");
        }
    }

    public static void IsGreaterThan(int value, int maxValue, string fieldName) 
    { 
        if(value > maxValue)
        {
            throw new DomainValidationException($"O valor de {fieldName} não pode ser maior que {maxValue}.");
        }
    }

    public static void IsValidRange(int minValue, int maxValue, string minValueFieldName, string maxValueFieldName)
    {
        if(minValue > maxValue)
        {
            throw new DomainValidationException($"O valor de {minValueFieldName} não pode ser maior que {maxValueFieldName}.");
        }
    }

    public static void IsTrue(bool value, string fieldName)
    {
        if(value == false)
        {
            throw new DomainValidationException($"{fieldName} não pode ser falso");
        }
    }
}
