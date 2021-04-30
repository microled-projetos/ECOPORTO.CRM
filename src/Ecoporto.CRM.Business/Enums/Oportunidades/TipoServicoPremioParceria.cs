using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoServicoPremioParceria
    {
        [Display(Name = "Importação")]
        IMPORTACAO = 1,
        [Display(Name = "Exportação")]
        EXPORTACAO,
        [Display(Name = "LTL Exportação")]
        LTL_EXPORTACAO
    }
}
