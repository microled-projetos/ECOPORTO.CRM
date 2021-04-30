using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum SituacaoCadastral
    {
        [Display(Name = "Ativo")]
        ATIVO = 1,
        [Display(Name = "Baixado")]
        BAIXADO
    }
}
