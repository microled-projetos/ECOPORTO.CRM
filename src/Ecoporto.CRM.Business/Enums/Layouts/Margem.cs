using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum Margem
    {
        [Display(Name = "Direita")]
        DIREITA = 1,
        [Display(Name = "Esquerda")]
        ESQUERDA,
        [Display(Name = "Entre Margens")]
        ENTRE
    }
}
