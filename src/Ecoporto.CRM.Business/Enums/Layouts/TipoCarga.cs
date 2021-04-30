using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoCarga
    {
        [Display(Name = "Contêiner")]
        CONTEINER = 1,
        [Display(Name = "Carga Solta")]
        CARGA_SOLTA,
        [Display(Name = "Mudança Regime")]
        MUDANCA_REGIME
    }
}
