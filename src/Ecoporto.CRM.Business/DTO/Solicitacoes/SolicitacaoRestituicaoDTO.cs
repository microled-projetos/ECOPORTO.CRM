using Ecoporto.CRM.Business.Enums;
using System;

namespace Ecoporto.CRM.Business.DTO
{
    public class SolicitacaoRestituicaoDTO
    {
        public int Id { get; set; }

        public int SolicitacaoId { get; set; }

        public TipoPesquisa TipoPesquisa { get; set; }

        public StatusSolicitacao StatusSolicitacao { get; set; }

        public int TipoOperacao { get; set; }

        public string TipoPesquisaNumero { get; set; }

        public int NotaFiscalId { get; set; }

        public int NFE { get; set; }

        public decimal ValorNF { get; set; }

        public int RPS { get; set; }

        public int Lote { get; set; }

        public string Documento { get; set; }

        public int FavorecidoId { get; set; }

        public string FavorecidoDescricao { get; set; }

        public string FavorecidoDocumento { get; set; }

        public int BancoId { get; set; }

        public string  BancoDescricao { get; set; }

        public string Agencia { get; set; }

        public string ContaCorrente { get; set; }

        public string FornecedorSAP { get; set; }

        public decimal ValorAPagar { get; set; }

        public string Observacoes { get; set; }

        public DateTime DataVencimento { get; set; }

        public DateTime DataEmissao { get; set; }

        public DateTime DataCadastro { get; set; }

        public string CriadoPor { get; set; }

        public int CriadoPorId { get; set; }
    }
}
