using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum FormaPagamento
    {
        [Display(Name = "À Vista")]
        AVISTA = 1,
        [Display(Name = "Faturado")]
        FATURADO
    }
}
