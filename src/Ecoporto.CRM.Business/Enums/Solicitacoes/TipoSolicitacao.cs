using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoSolicitacao
    {
        [Display(Name = "Cancelamento NF")]
        CANCELAMENTO_NF = 1,
        [Display(Name = "Desconto")]
        DESCONTO,
        [Display(Name = "Prorrogação Boleto")]
        PRORROGACAO_BOLETO,
        [Display(Name = "Restituição")]
        RESTITUICAO,
        [Display(Name = "Outros")]
        OUTROS
    }
}
