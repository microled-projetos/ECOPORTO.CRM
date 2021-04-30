using Ecoporto.CRM.Business.DTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class SolicitacaoComercialAnexosViewModel
    {
        public SolicitacaoComercialAnexosViewModel()
        {
            Anexos = new List<AnexosDTO>();
        }

        public int AnexoSolicitacaoId { get; set; }

        [Display(Name = "Anexo de Documento")]
        public string SolicitacaoAnexo { get; set; }

        public List<AnexosDTO> Anexos { get; set; }
    }
}