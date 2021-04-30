using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Site.Models.Pdf
{
    public class CancelamentoOportunidade
    {
        public Conta Conta { get; set; }

        public Oportunidade Oportunidade { get; set; }

        public IEnumerable<ClientePropostaDTO> ClientesGrupoCNPJ = new List<ClientePropostaDTO>();
    }
}