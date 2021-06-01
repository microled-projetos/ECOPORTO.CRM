using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoNegocio
    {
        [Display(Name = "Novo")]
        NOVO = 1,
        //[Display(Name = "Verticalização")]
        //VERTICALIZACAO,
        [Display(Name = "Revisão")]
        REVISAO_AJUSTE = 3
    }
}
