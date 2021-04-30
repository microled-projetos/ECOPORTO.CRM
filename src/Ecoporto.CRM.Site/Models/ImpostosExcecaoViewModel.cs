using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class ImpostosExcecaoViewModel
    {
        [Required]
        public int ModeloId { get; set; }

        [Required]
        public int OportunidadeId { get; set; }

        public StatusOportunidade StatusOportunidade { get; set; }

        public IEnumerable<ImpostosExcecaoDTO> Servicos { get; set; }

        [Required(ErrorMessage = "Selecione um Tipo de Exceção")]
        public TiposExcecoesImpostos Tipo { get; set; }

        public int[] ServicosSelecionados { get; set; }

        public bool ISS { get; set; }

        public bool PIS { get; set; }

        public bool COFINS { get; set; }

        [Display(Name = "Valor ISS")]
        public string ValorISS { get; set; }

        [Display(Name = "Valor PIS")]
        public string ValorPIS { get; set; }

        [Display(Name = "Valor COFINS")]
        public string ValorCOFINS { get; set; }
    }
}