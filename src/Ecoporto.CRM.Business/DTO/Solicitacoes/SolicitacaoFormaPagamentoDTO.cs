using Ecoporto.CRM.Business.Enums;
using System;

namespace Ecoporto.CRM.Business.DTO
{
    public class SolicitacaoFormaPagamentoDTO
    {
        public int Id { get; set; }

        public int SolicitacaoId { get; set; }

        public StatusSolicitacao StatusSolicitacao { get; set; }

        public int Lote { get; set; }

        public int GR { get; set; }

        public decimal Valor { get; set; }

        public string Indicador { get; set; }

        public string Cliente { get; set; }

        public DateTime DataCadastro { get; set; }

        public string FaturadoContra { get; set; }

        public string FaturadoContraDocumento { get; set; }

        public string EmailNota { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        public string CondicaoPagamentoId { get; set; }

        public string CondicaoPagamentoDescricao { get; set; }

        public string CriadoPor { get; set; }
    }
}
