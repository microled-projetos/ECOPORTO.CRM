using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum StatusFichaFaturamento
    {
        [Display(Name = "Em Andamento")]
        EM_ANDAMENTO = 1,
        [Display(Name = "Em Aprovação")]
        EM_APROVACAO,
        [Display(Name = "Aprovado")]
        APROVADO,
        [Display(Name = "Rejeitado")]
        REJEITADO,
        [Display(Name = "Cancelado")]
        CANCELADO,
        [Display(Name = "Revisada")]
        REVISADA
    }
}
