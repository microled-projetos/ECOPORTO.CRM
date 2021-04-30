using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoTrabalho
    {
        [Display(Name = "Mecanizada")]
        MECANIZADA = 1,
        [Display(Name = "Manual")]
        MANUAL
    }
}
