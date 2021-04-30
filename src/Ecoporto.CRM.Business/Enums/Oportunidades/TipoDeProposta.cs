using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoDeProposta
    {
        [Display(Name = "Geral")]
        GERAL = 1,
        [Display(Name = "Específica")]
        ESPECIFICA,
        [Display(Name = "Reduzida")]
        REDUZIDA,
        [Display(Name = "Acordo")]
        ACORDO
    }
}
