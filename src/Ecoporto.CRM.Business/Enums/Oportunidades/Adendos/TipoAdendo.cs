using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoAdendo
    {       
        [Display(Name = "Alteração de Vendedor")]
        ALTERACAO_VENDEDOR = 1,
        [Display(Name = "Forma Pagamento")]
        FORMA_PAGAMENTO,      
        [Display(Name = "Inclusão de Sub Cliente")]
        INCLUSAO_SUB_CLIENTE,
        [Display(Name = "Exclusão de Sub Cliente")]
        EXCLUSAO_SUB_CLIENTE,
        [Display(Name = "Inclusão de Grupo CNPJ")]
        INCLUSAO_GRUPO_CNPJ,
        [Display(Name = "Exclusão de Grupo CNPJ")]
        EXCLUSAO_GRUPO_CNPJ
    }
}
