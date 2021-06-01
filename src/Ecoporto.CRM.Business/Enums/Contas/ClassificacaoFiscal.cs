using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum ClassificacaoFiscal
    {
        [Display(Name = "Pessoa Fisica")]
        PF = 1,
        [Display(Name = "Pessoa Jurídica")]
        PJ,
        [Display(Name = "Externo")]
        EXTERNO
    }
}