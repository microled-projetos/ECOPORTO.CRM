using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum Status
    {
        [Display(Name = "Inativo")]
        INATIVO,
        [Display(Name = "Ativo")]
        ATIVO
    }
}
