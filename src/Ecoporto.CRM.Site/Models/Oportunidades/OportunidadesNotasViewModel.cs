using Ecoporto.CRM.Business.DTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class OportunidadesNotasViewModel
    {
        public OportunidadesNotasViewModel()
        {
            Notas = new List<AnexosNotasDTO>();
        }

        public int NotaId { get; set; }

        public int NotaOportunidadeId { get; set; }
      
        public string Nota { get; set; }

        [Display(Name = "Descrição")]
        public string NotaDescricao { get; set; }

        public List<AnexosNotasDTO> Notas { get; set; }
    }
}