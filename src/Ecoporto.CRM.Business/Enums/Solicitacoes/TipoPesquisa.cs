using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoPesquisa
    {
        [Display(Name = "NF")]
        NF = 1,
        [Display(Name = "Lote")]
        LOTE,
        [Display(Name = "GR")]
        GR,
        [Display(Name = "Minuta")]
        MINUTA,
        [Display(Name = "Fatura")]
        FATURA,
        [Display(Name = "Booking")]
        BOOKING,
        [Display(Name = "BL")]
        BL
    }
}
