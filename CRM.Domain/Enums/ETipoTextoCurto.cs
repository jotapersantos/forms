using Ardalis.SmartEnum;

namespace CRM.Domain.Enums;

public class ETipoTextoCurto : SmartEnum<ETipoTextoCurto>
{
    public static readonly ETipoTextoCurto Email = new ETipoTextoCurto("E-mail", 0);
    public static readonly ETipoTextoCurto Url = new ETipoTextoCurto("URL", 1);

    public ETipoTextoCurto(string name, int value) : base(name, value)
    {

    }
}
