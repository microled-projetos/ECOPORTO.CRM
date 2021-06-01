using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.Models.Oportunidades
{
    public class OportunidadeTabelaConcomitante
    {
        public int Id { get; set; }

        public int TabelaId { get; set; }

        public int Identificacao { get; set; }

        public int ImportadorId { get; set; }

        public int DespachanteId { get; set; }

        public int ColoaderId { get; set; }

        public int CoColoaderId { get; set; }

        public int CoColoader2Id { get; set; }

        public string CnpjImportador { get; set; }

        public string CnpjDespachante { get; set; }

        public string CnpjColoader { get; set; }

        public string CnpjCoColoader { get; set; }

        public string CnpjCoColoader2 { get; set; }

        public SegmentoSubCliente Segmento { get; set; }
    }
}
