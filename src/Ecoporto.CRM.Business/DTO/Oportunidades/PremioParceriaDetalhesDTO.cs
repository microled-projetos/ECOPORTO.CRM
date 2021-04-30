using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.DTO
{
    public class PremioParceriaDetalhesDTO
    {
        public int Id { get; set; }

        public int OportunidadeId { get; set; }

        public string OportunidadeDescricao { get; set; }

        public string ContatoId { get; set; }

        public string DescricaoContato { get; set; }

        public string PremioReferenciaDescricao { get; set; }

        public StatusPremioParceria StatusPremioParceria { get; set; }

        public string DescricaoStatusPremioParceria { get; set; }

        public TipoServicoPremioParceria TipoServicoPremioParceria { get; set; }

        public StatusOportunidade StatusOportunidade { get; set; }

        public string DescricaoServicoPremioParceria { get; set; }

        public string Favorecido1 { get; set; }

        public string DescricaoFavorecido1 { get; set; }

        public string DocumentoFavorecido1 { get; set; }

        public string Favorecido2 { get; set; }

        public string DescricaoFavorecido2 { get; set; }

        public string DocumentoFavorecido2 { get; set; }

        public string Favorecido3 { get; set; }

        public string DescricaoFavorecido3 { get; set; }

        public string DocumentoFavorecido3 { get; set; }

        public string EmailFavorecido1 { get; set; }

        public string EmailFavorecido2 { get; set; }

        public string EmailFavorecido3 { get; set; }

        public string PremioReferenciaId { get; set; }

        public string AnexoId { get; set; }

        public string Anexo { get; set; }

        public Instrucao Instrucao { get; set; }

        public string DescricaoInstrucao { get; set; }

        public string DataCadastro { get; set; }

        public int CriadoPor { get; set; }       

        public string Observacoes { get; set; }

        public string UrlPremio { get; set; }

        public string DataUrlPremio { get; set; }

        public TipoOperacao TipoOperacao { get; set; }

        public string DescricaoTipoOperacao { get; set; }
    }
}
