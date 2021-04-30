using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum StatusPremioParceria
    {
        [Display(Name = "Em Andamento")]
        EM_ANDAMENTO = 1,
        [Display(Name = "Em Aprovação")]
        EM_APROVACAO,
        [Display(Name = "Cadastrado")]
        CADASTRADO,
        [Display(Name = "Revisado")]
        REVISADO,
        [Display(Name = "Rejeitado")]
        REJEITADO,
        [Display(Name = "Cancelado")]
        CANCELADO
    }
}
