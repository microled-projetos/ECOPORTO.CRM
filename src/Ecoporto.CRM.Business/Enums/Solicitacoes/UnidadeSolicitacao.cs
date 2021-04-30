using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum _UnidadeSolicitacao
    {
        [Display(Name = "Ecoporto Santos")]
        ECOPORTO_SANTOS = 1,
        [Display(Name = "Ecoporto Alfandegado")]
        ECOPORTO_ALFANDEGADO,        
        [Display(Name = "Ecopatio")]
        ECOPATIO
    }
}