using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoValidade
    {
        [Display(Name = "Dias")]
        DIAS = 1,
        [Display(Name = "Meses")]
        MESES,
        [Display(Name = "Anos")]
        ANOS       
    }
}
