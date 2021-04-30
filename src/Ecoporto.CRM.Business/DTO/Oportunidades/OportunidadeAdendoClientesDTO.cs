using Ecoporto.CRM.Business.Enums;
using System;

namespace Ecoporto.CRM.Business.DTO
{
    public class OportunidadeAdendoClientesDTO
    {
        public int Id { get; set; }

        public int OportunidadeId { get; set; }

        public string OportunidadeIdentificacao { get; set; }

        public SegmentoSubCliente Segmento { get; set; }

        public StatusAdendo StatusAdendo { get; set; }

        public TipoAdendo TipoAdendo { get; set; }

        public string OportunidadeDescricao { get; set; }

        public StatusOportunidade StatusOportunidade { get; set; }

        public int AdendoId { get; set; }

        public int SubClienteId { get; set; }

        public int GrupoCNPJId { get; set; }

        public string DescricaoCliente { get; set; }

        public string Documento { get; set; }

        public AdendoAcao Acao { get; set; }

        public DateTime DataCadastro { get; set; }
    }
}
