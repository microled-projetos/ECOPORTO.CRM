using Ecoporto.CRM.Business.Enums;
using WsSimuladorCalculoTabelas.Enums;

namespace WsSimuladorCalculoTabelas.Models
{
    public class SubCliente
    {
        public SubCliente(string documento, SegmentoSubCliente segmento)
        {
            Documento = documento;
            Segmento = segmento;
        }

        public string Documento { get; set; }

        public SegmentoSubCliente Segmento { get; set; }
    }
}