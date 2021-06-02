using System.ComponentModel.DataAnnotations;

namespace WsConsultaSPC
{
    public enum StatusAnaliseDeCreditoResponse
    {
        [Display(Name = "Envio Pendente")]
        PENDENTE,
        [Display(Name = "Em Andamento")]
        EM_ANDAMENTO,
        [Display(Name = "Em Aprovação")]
        EM_APROVACAO,
        [Display(Name = "Aprovado")]
        APROVADO,
        [Display(Name = "Rejeitado")]
        REJEITADO
    }
}
