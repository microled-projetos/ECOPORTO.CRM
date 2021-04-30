using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum SucessoNegociacao
    {
        //[Display(Name = "Prospect")]
        //PROSPECT = 1,
        [Display(Name = "Em Negociação")]
        EM_NEGOCIACAO = 2,
        [Display(Name = "Aceito pelo Cliente")]
        ACEITO_PELO_CLIENTE = 3,
        [Display(Name = "Ganho")]
        GANHO = 4,
        [Display(Name = "Perdido")]
        PERDIDO = 5
    }
}
