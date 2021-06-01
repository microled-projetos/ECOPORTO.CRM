using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum AmbienteOracle
    {
        [Display(Name = "Desenvolvimento")]
        DEV = 1,
        [Display(Name = "Produção")]
        PROD
    }
}
