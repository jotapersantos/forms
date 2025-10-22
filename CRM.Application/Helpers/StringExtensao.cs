using System.Text.RegularExpressions;

namespace CRM.Application.Helpers;

public static class StringExtensao
{
    public static bool IsValidEmail(this string str)
    {
        // Define a expressão regular para validar o email
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

        // Cria um objeto Regex com o padrão especificado
        var regex = new Regex(pattern);

        // Verifica se a string corresponde ao padrão
        return regex.IsMatch(str);
    }

    public static string NormalizeString(this string str)
    {
        return str.Trim().ToUpperInvariant();
    }
}
