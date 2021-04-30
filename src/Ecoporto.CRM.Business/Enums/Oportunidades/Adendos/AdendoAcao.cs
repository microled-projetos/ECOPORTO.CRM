using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum AdendoAcao
    {
        [Display(Name = "Inclusão")]
        INCLUSAO = 1,
        [Display(Name = "Exclusão")]
        EXCLUSAO
    }
}
