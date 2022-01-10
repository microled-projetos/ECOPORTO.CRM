using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum BaseCalculo
    {
        [Display(Name = "UNID")]
        UNID = 1,
        [Display(Name = "TON")]
        TON,        
        [Display(Name = "CIF")]
        CIF,
        [Display(Name = "CIFM")]
        CIFM,
        [Display(Name = "CIF0")]
        CIF0,
        [Display(Name = "BL")]
        BL,
        [Display(Name = "VOLP")]
        VOLP,
        [Display(Name = "VOL")]
        VOL
    }
}