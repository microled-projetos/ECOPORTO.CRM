using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum Instrucao
    {
        [Display(Name = "Geral")]
        GERAL = 1,
        [Display(Name = "Anterior")]
        ANTERIOR,        
        [Display(Name = "Nova")]
        NOVA
    }
}