using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum SegmentoSubCliente
    {
        [Display(Name = "Importador")]
        IMPORTADOR = 1,
        [Display(Name = "Despachante")]
        DESPACHANTE,        
        [Display(Name = "Coloader")]
        COLOADER,
        [Display(Name = "Co-Coloader1")]
        CO_COLOADER1,
        [Display(Name = "Co-Coloader2")]
        CO_COLOADER2,
    }
}