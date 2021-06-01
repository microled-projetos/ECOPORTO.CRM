using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoAnexo
    {
        [Display(Name = "Ficha Faturamento")]
        FICHA_FATURAMENTO = 1,
        [Display(Name = "Cancelamento de Oportunidade")]
        CANCELAMENTO,
        [Display(Name = "Prêmio Parceria")]
        PREMIO_PARCERIA,
        [Display(Name = "Proposta")]
        PROPOSTA,
        [Display(Name = "Outros")]
        OUTROS,
        [Display(Name = "Solicitação")]
        SOLICITACAO,
        [Display(Name = "Simulador")]
        RELATORIO_SIMULADOR
    }
}
