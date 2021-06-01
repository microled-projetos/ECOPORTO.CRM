using Ecoporto.CRM.Business.DTO;
using System.Collections.Generic;

namespace Ecoporto.CRM.Site.Models
{
    public class EdicaoValoresViewModel
    {
        public EdicaoValoresViewModel()
        {
            Valores = new List<EdicaoValoresPropostaDTO>();
        }

        public int OportunidadeId { get; set; }

        public string Identificacao { get; set; }

        public bool ApenasLeitura { get; set; }

        public List<EdicaoValoresPropostaDTO> Valores { get; set; }
    }
}