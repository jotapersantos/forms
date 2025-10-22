using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CRM.Domain.SeedWork;
using CRM.Domain.Validation;

namespace CRM.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string Endereco { get; private set; }

    public Email(string email)
    {
        Validate(email);

        Endereco = email;
    }

    private void Validate(string email)
    {
        DomainValidation.NotNullOrEmpty(email, "Email");
        DomainValidation.NotNullOrWhiteSpace(email, "Email");

        DomainValidation.MaxLength(email, 100, "Email");
        DomainValidation.MinLength(email, 3, "Email");

        if (!RegexValidation(email))
        {
            throw new DomainValidationException("Email regex inválido.");
        }

        if (!MailAddress.TryCreate(email, out _))
        {
            throw new DomainValidationException("Email inválido.");
        }
    }

    private bool RegexValidation(string email)
    {
        const string pattern =
                        @"^([0-9a-zA-Z]" +
                        @"([\+\-_\.][0-9a-zA-Z]+)*" +
                        @")+" +
                        @"@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$";

        bool isValidEmail = Regex.IsMatch(email,
                                          pattern,
                                          RegexOptions.IgnoreCase,
                                          TimeSpan.FromMilliseconds(250));

        return isValidEmail;
    }

    private Email() { }
}
