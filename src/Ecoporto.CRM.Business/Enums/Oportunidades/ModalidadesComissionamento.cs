using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum ModalidadesComissionamento
    {
        [Display(Name = "GR Paga")]
        GR_PAGA = 1,
        [Display(Name = "BL com Entrada")]
        BL_COM_ENTRADA,
        [Display(Name = "BL Pago")]
        BL_PAGO,
        [Display(Name = "Contêiner com Entrada")]
        CONTEINER_COM_ENTRADA,
        [Display(Name = "Contêiner Pago")]
        CONTEINER_PAGO,
        [Display(Name = "Valor Fixo")]
        VALOR_FIXO,
        [Display(Name = "Pacote")]
        PACOTE,
        [Display(Name = "Contêiner")]
        CONTEINER
    }
}
