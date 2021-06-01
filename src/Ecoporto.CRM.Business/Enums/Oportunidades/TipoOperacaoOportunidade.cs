using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoOperacaoOportunidade
    {
        [Display(Name = "Regular")]
        REGULAR = 1,
        [Display(Name = "Spot")]
        SPOT
    }
}
