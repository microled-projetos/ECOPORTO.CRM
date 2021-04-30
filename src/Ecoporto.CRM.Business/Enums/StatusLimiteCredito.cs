using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum StatusLimiteCredito
    {
        [Display(Name = "Envio Pendente")]
        PENDENTE,
        [Display(Name = "Enviada para Aprovação")]
        EM_ANDAMENTO,
        [Display(Name = "Em Aprovação")]
        EM_APROVACAO,
        [Display(Name = "Aprovado")]
        APROVADO,
        [Display(Name = "Rejeitado")]
        REJEITADO
    }
}
