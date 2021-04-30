using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TiposExcecoesImpostos
    {
        [Display(Name = "Impostos a Isentar")]
        ImpostoAIsentar = 1,
        [Display(Name = "% Imposto Diferenciado")]
        ImpostoDiferenciado
    }
}
