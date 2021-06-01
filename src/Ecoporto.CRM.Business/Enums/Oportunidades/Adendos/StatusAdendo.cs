using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum StatusAdendo
    {
        [Display(Name = "Aberto")]
        ABERTO = 1,
        [Display(Name = "Enviado")]
        ENVIADO,
        [Display(Name = "Rejeitado")]
        REJEITADO,
        [Display(Name = "Aprovado")]
        APROVADO       
    }
}