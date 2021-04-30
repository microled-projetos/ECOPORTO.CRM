using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum MotivoPerda
    {
        [Display(Name = "Atendimento")]
        ATENDIMENTO = 1,
        [Display(Name = "Burocracia")]
        BUROCRACIA,
        [Display(Name = "Estrutura Operacional")]
        ESTRUTURA_OPERACIONAL,
        [Display(Name = "Preço")]
        PRECO,
        [Display(Name = "Outros")]
        OUTROS
    }
}
