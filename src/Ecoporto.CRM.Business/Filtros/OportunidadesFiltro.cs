using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.Filtros
{
    public class OportunidadesFiltro
    {
        public int? Identificacao { get; set; }

        public string Descricao { get; set; }

        public int? ContaId { get; set; }

        public int? ModeloId { get; set; }

        public int? TabelaId { get; set; }

        public string AdendoId { get; set; }

        public string FichaId { get; set; }

        public string PremioId { get; set; }

        public StatusOportunidade? StatusOportunidade { get; set; }

        public SucessoNegociacao? SucessoNegociacao { get; set; }

        public int? CriadoPor { get; set; }

        public string DataInicio { get; set; }

        public string DataTermino { get; set; }

        public TipoServico? TipoServico { get; set; }
    }
}
