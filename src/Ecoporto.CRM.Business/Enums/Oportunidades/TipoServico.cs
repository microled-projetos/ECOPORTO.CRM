using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoServico
    {
        [Display(Name = "FCL")]
        FCL = 1,
        [Display(Name = "LCL")]
        LCL,
        [Display(Name = "FCL/LCL")]
        FCL_LCL,
        [Display(Name = "Break Bulk")]
        BREAK_BULK,
        [Display(Name = "Transporte")]
        TRANSPORTE,
        [Display(Name = "Carga Mista")]
        CARGA_MISTA,
        [Display(Name = "Veículo")]
        VEICULO
    }
}
