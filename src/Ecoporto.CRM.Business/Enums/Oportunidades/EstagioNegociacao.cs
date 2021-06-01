using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum EstagioNegociacao
    {
        [Display(Name = "Início do Processo")]
        INICIO_DO_PROCESSO = 1,
        [Display(Name = "Envio da Proposta")]
        ENVIO_DA_PROPOSTA,
        [Display(Name = "Aceito pelo Cliente")]
        ACEITO_PELO_CLIENTE,
        [Display(Name = "Ganho")]
        GANHO,
        [Display(Name = "Perdido")]
        PERDIDO
    }
}
