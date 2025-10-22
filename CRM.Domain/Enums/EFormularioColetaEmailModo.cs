using System.ComponentModel.DataAnnotations;
using Ardalis.SmartEnum;

namespace CRM.Domain.Enums;

public sealed class EFormularioColetaEmailModo : SmartEnum<EFormularioColetaEmailModo>
{
    public static readonly EFormularioColetaEmailModo NaoColetar = new EFormularioColetaEmailModo("Não Coletar", 0);
    public static readonly EFormularioColetaEmailModo Manual = new EFormularioColetaEmailModo(nameof(Manual), 1);
    public static readonly EFormularioColetaEmailModo Automatico = new EFormularioColetaEmailModo("Automático", 2);

    public EFormularioColetaEmailModo(string name, int value) : base(name, value)
    {

    }
}
