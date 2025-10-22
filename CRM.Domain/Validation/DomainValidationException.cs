using System;

namespace CRM.Domain.Validation;

public class DomainValidationException : Exception
{
    public DomainValidationException(string error) : base(error)
    { }

    public static void When(bool hasError, string error)
    {
        if (!hasError)
        {
            return;
        }
        throw new DomainValidationException(error);
    }
}
