using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum StatusOportunidade
    {
        [Display(Name = "Ativa")]
        ATIVA = 1,
        [Display(Name = "Cancelada")]
        CANCELADA,
        [Display(Name = "Revisada")]
        REVISADA,
        [Display(Name = "Recusado")]
        RECUSADO,
        [Display(Name = "Vencido")]
        VENCIDO,
        [Display(Name = "Enviado para Aprovação")]
        ENVIADO_PARA_APROVACAO,
        [Display(Name = "Rascunho")]
        RASCUNHO      
    }
}
