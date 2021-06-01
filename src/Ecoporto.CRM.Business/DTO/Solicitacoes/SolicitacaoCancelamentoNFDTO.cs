using Ecoporto.CRM.Business.Enums;
using System;

namespace Ecoporto.CRM.Business.DTO
{
    public class SolicitacaoCancelamentoNFDTO
    {   
        public int Id { get; set; }

        public int SolicitacaoId { get; set; }

        public StatusSolicitacao StatusSolicitacao { get; set; }

        public string TipoPesquisaNumero { get; set; }

        public int TipoOperacao { get; set; }

        public int Lote { get; set; }

        public int NotaFiscalId { get; set; }

        public int NFE { get; set; }

        public int ContaId { get; set; }

        public decimal ValorNF { get; set; }

        public string RazaoSocial { get; set; }

        public string Documento { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        public decimal Desconto { get; set; }

        public decimal ValorNovaNF { get; set; }

        public DateTime DataProrrogacao { get; set; }

        public DateTime? DataEmissao { get; set; }

        public DateTime DataCadastro { get; set; }

        public string CriadoPor { get; set; }

        public int CriadoPorId { get; set; }

        public decimal ValorCIF { get; set; }

        public string Indicador { get; set; }

        public string IndicadorDocumento { get; set; }
    }
}
