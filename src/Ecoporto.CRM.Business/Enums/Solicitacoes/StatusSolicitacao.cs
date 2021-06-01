using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum StatusSolicitacao
    {
        [Display(Name = "Novo")]
        NOVO = 1,
        [Display(Name = "Em Aprovação")]
        EM_APROVAVAO,
        [Display(Name = "Aprovado")]
        APROVADO,
        [Display(Name = "Rejeitado")]
        REJEITADO
    }
}
