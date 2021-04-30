using System.ComponentModel.DataAnnotations;

namespace WsConsultaSPC
{
    public enum StatusLimiteCreditoResponse
    {
        [Display(Name = "Em Andamento")]
        EM_ANDAMENTO = 1,
        [Display(Name = "Em Aprovação")]
        EM_APROVACAO,
        [Display(Name = "Aprovado")]
        APROVADO,
        [Display(Name = "Rejeitado")]
        REJEITADO
    }
}
