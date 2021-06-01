using Ecoporto.CRM.Business.Enums;
using System;

namespace Ecoporto.CRM.Business.DTO
{
    public class SolicitacaoProrrogacaoDTO
    {
        public int Id { get; set; }

        public int SolicitacaoId { get; set; }

        public StatusSolicitacao StatusSolicitacao { get; set; }

        public int NFE { get; set; }

        public int NotaFiscalId { get; set; }

        public int ContaId { get; set; }

        public string RazaoSocial { get; set; }

        public string Documento { get; set; }

        public Boleano IsentarJuros { get; set; }

        public decimal ValorNF { get; set; }

        public decimal ValorJuros { get; set; }

        public DateTime VencimentoOriginal { get; set; }

        public DateTime DataProrrogacao { get; set; }

        public int NumeroProrrogacao { get; set; }

        public decimal ValorTotalComJuros { get; set; }

        public string Observacoes { get; set; }

        public DateTime DataCadastro { get; set; }

        public string CriadoPor { get; set; }

        public int CriadoPorId { get; set; }
    }
}
