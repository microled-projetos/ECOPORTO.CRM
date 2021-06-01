using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum PapelEquipe
    {
        [Display(Name = "Nenhum")]
        NENHUM,
        [Display(Name = "Assistente")]
        ASSISTENTE,
        [Display(Name = "Corporativo")]
        CORPORATIVO
    }
}
