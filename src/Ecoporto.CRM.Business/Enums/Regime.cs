using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum Regime
    {
        [Display(Name = "FCL")]
        FCL,
        [Display(Name = "LCL")]
        LCL
    }
}
