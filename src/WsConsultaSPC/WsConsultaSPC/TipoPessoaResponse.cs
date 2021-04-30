using System.ComponentModel.DataAnnotations;

namespace WsConsultaSPC
{
    public enum TipoPessoaResponse
    {
        [Display(Name = "PF")]
        PessoaFisica = 1,
        [Display(Name = "PJ")]
        PessoaJuridica
    }
}
