using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.IntegraChronosService.Enums
{
    public enum Status
    {
        [Display(Name = "Demanda na fila de processamento")]
        Pendente = 1,
        [Display(Name = "Demanda executada")]
        Executada,
        [Display(Name = "Erro na execução")]
        Erro
    }
}
