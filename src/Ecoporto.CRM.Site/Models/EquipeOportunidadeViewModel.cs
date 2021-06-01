using Ecoporto.CRM.Business.DTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class EquipeOportunidadeViewModel
    {
        public EquipeOportunidadeViewModel()
        {
            Vinculos = new List<EquipeOportunidadeUsuarioDTO>();
        }

        public int OportunidadeId { get; set; }

        public string Proposta { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Status")]
        public string StatusOportunidade { get; set; }

        public string Vendedor { get; set; }

        public IEnumerable<EquipeOportunidadeUsuarioDTO> Vinculos { get; set; }
    }
}