using Ecoporto.CRM.Business.DTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class OportunidadesAnexosViewModel
    {
        public OportunidadesAnexosViewModel()
        {
            Anexos = new List<AnexosDTO>();
        }

        public int AnexoOportunidadeId { get; set; }

        [Display(Name = "Anexo de Documento")]
        public string Anexo { get; set; }

        public List<AnexosDTO> Anexos { get; set; }
    }
}