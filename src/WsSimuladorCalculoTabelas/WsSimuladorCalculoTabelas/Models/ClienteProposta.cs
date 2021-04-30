using Ecoporto.CRM.Business.Enums;
using WsSimuladorCalculoTabelas.Enums;

namespace WsSimuladorCalculoTabelas.Models
{
    public class ClienteProposta
    {
        public ClienteProposta()
        {

        }

        public ClienteProposta(string documento, SegmentoSubCliente segmento, string descricao, string nomeFantasia)
        {
            Documento = documento;
            Segmento = segmento;
            Descricao = descricao;
            NomeFantasia = nomeFantasia;
        }

        public int Id { get; set; }
        public int AdendoId { get; set; }
        public int ContaId { get; set; }
        public int SubClienteId { get; set; }
        public int GrupoCNPJId { get; set; }
        public int OportunidadeId { get; set; }
        public string Descricao { get; set; }
        public string NomeFantasia { get; set; }
        public string Documento { get; set; }
        public SegmentoSubCliente Segmento { get; set; }
        public Segmento SegmentoConta { get; set; }
        public string Tipo { get; set; }
        public string CriadoPor { get; set; }
        public string OportunidadeDescricao { get; set; }
        public string DataCriacao { get; set; }       
        public Segmento SegmentoOportunidade { get; set; }
        public bool Checado { get; set; }
    }
}