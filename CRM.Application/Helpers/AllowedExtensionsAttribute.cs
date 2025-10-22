using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace CRM.Application.Helpers;

public sealed class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;

    public AllowedExtensionsAttribute(string[] extensions)
    {
        _extensions = extensions;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value != null)
        {
            var file = value as IFormFile;

            string extension = Path.GetExtension(file.FileName);

            if (!_extensions.Contains(extension.ToLowerInvariant()))
            {
                return new ValidationResult(GetErrorMessage(_extensions));
            }

            return ValidationResult.Success;
        }

        return ValidationResult.Success;
    }

    private string GetErrorMessage(string[] extensions)
    {
        StringBuilder erros = new();

        erros.Append("Tipo de arquivo inválido. Extensões permitidas: ");

        foreach (string extensao in extensions)
        {
            erros.Append(extensao);
        }

        return erros.ToString();
    }
}
