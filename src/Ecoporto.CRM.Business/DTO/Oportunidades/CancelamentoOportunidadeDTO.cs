using System;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.DTO
{
    public class CancelamentoOportunidadeDTO
    {
        public CancelamentoOportunidadeDTO()
        {
            ClientesGrupo = new List<ClientePropostaDTO>();
        }

        public string ContaDescricao { get; set; }
        public string ContaDocumento { get; set; }
        public string ContaLogradouro { get; set; }
        public int ContaNumero { get; set; }
        public string ContaBairro { get; set; }
        public string ContaCidade { get; set; }
        public string ContaEstado { get; set; }
        public string ContaCEP { get; set; }
        public string IdentificacaoOportunidade { get; set; }

        public DateTime? DataCancelamento { get; set; }
        public IEnumerable<ClientePropostaDTO> ClientesGrupo { get; set; }
    }
}
