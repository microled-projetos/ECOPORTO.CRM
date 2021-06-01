using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ecoporto.CRM.Site.Models
{
    public class AuditoriaViewModel
    {
        public string Data { get; set; }

        public string Usuario { get; set; }        

        public int Chave { get; set; }

        public string Controller { get; set; }

        public string Mensagem { get; set; }

        public string Objeto { get; set; }

        public TipoLogAuditoria Acao { get; set; }

        public string Maquina { get; set; }

        public IEnumerable<AuditoriaDTO> Historico { get; set; }

        public IEnumerable<AuditoriaDTO> Atualizacoes { get; set; }
    }
}