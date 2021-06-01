using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum Segmento
    {
        [Display(Name = "Importador")]
        IMPORTADOR = 1,
        [Display(Name = "Exportador")]
        EXPORTADOR,
        [Display(Name = "Despachante")]
        DESPACHANTE,
        [Display(Name = "Agente de Carga")]
        AGENTE,    
        [Display(Name = "Freight Forwarder")]
        FREIGHT_FORWARDER,
        [Display(Name = "Coloader")]
        COLOADER,
        [Display(Name = "Armador")]
        ARMADOR
    }
}