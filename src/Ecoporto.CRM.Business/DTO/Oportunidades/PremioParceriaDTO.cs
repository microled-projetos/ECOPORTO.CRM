using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;

namespace Ecoporto.CRM.Business.DTO
{
    public class PremioParceriaDTO
    {
        public int Id { get; set; }

        public string OportunidadeId { get; set; }

        public string Descricao => $"P-{Id}";

        public string ContatoId { get; set; }

        public string DescricaoContato { get; set; }

        public string PremioReferenciaId { get; set; }

        public string PremioReferenciaDescricao { get; set; }

        public StatusOportunidade StatusOportunidade { get; set; }

        public StatusPremioParceria StatusPremioParceria { get; set; }

        public TipoServicoPremioParceria TipoServicoPremioParceria { get; set; }

        public string Favorecido1 { get; set; }

        public string DescricaoFavorecido1 { get; set; }

        public string Favorecido2 { get; set; }

        public string DescricaoFavorecido2 { get; set; }

        public string Favorecido3 { get; set; }

        public string DescricaoFavorecido3 { get; set; }

        public Instrucao Instrucao { get; set; }

        public string DataCadastro { get; set; }

        public string CriadoPor { get; set; }

        public string EmailFavorecido1 { get; set; }

        public string EmailFavorecido2 { get; set; }

        public string EmailFavorecido3 { get; set; }

        public string IdFile { get; set; }

        public string Observacoes { get; set; }

        public string UrlPremio { get; set; }

        public string DataUrlPremio { get; set; }

        public int OportunidadePremioReferencia { get; set; }

        public string DescricaoStatusPremioParceria
            => StatusPremioParceria.ToName();

        public string DescricaoTipoServicoPremioParceria
            => TipoServicoPremioParceria.ToName();

        public string DescricaoInstrucao
            => Instrucao.ToName();
    }
}
