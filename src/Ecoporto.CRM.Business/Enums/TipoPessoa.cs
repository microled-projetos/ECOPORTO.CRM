using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoPessoa
    {
        [Display(Name = "PF")]
        PessoaFisica = 1,
        [Display(Name = "PJ")]
        PessoaJuridica
    }
}
