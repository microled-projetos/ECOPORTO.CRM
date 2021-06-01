using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoDesconto
    {
        [Display(Name = "R$")]
        REAIS = 1,
        [Display(Name = "%")]
        PORCENTAGEM
    }
}
