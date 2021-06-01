using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.DTO
{
    public class AuditoriaDTO
    {
        public int Id { get; set; }

        public string Data { get; set; }

        public string Usuario { get; set; }

        public string Chave { get; set; }

        public string ChavePai { get; set; }

        public string Controller { get; set; }

        public string Mensagem { get; set; }

        public string Objeto { get; set; }

        public TipoLogAuditoria Acao { get; set; }

        public string Maquina { get; set; }
    }
}
