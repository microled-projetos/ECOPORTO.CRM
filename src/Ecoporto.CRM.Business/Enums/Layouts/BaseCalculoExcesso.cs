using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum BaseCalculoExcesso
    {
        [Display(Name = "Valor")]
        VALOR = 1,
        [Display(Name = "Percentual")]
        PERCENTUAL,        
        [Display(Name = "Metragem")]
        METRAGEM
    }
}