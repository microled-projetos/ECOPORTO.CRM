using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum AreaOcorrenciaSolicitacao
    {
        [Display(Name = "Comercial")]
        COMERCIAL = 1,
        [Display(Name = "Cadastro")]
        CADASTRO,
        [Display(Name = "Cálculo")]
        CALCULO,
        [Display(Name = "Faturamento")]
        FATURAMENTO,
        [Display(Name = "Operacional")]
        OPERACIONAL,
        [Display(Name = "TI")]
        TI,
        [Display(Name = "Documental")]
        DOCUMENTAL
    }
}
