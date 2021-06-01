using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum Moeda
    {
        [Display(Name = "R$")]
        REAL = 1,
        [Display(Name = "US$")]
        DOLAR 
    }
}
