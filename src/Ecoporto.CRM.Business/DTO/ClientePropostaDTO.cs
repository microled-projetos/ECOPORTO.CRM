using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.DTO
{
    public class ClientePropostaDTO
    {
        public int Id { get; set; }
        public int AdendoId { get; set; }
        public int ContaId { get; set; }
        public int SubClienteId { get; set; }
        public int GrupoCNPJId { get; set; }
        public int OportunidadeId { get; set; }
        public int OportunidadeIdentificacao { get; set; }
        public string Descricao { get; set; }
        public string NomeFantasia { get; set; }
        public string Documento { get; set; }
        public SegmentoSubCliente Segmento { get; set; }
        public Segmento SegmentoOportunidade { get; set; }
        public string Tipo { get; set; }
        public string CriadoPor { get; set; }
        public string OportunidadeDescricao { get; set; }
        public string DataCriacao { get; set; }
        public StatusOportunidade StatusOportunidade { get; set; }
        public bool Checado { get; set; }
        public string Display => $"{Descricao} ({Documento})";
    }
}
