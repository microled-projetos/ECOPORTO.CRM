using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;

namespace Ecoporto.CRM.Business.DTO
{
    public class FichaFaturamentoDTO
    {
        public int Id { get; set; }

        public int OportunidadeId { get; set; }

        public int ContaId { get; set; }

        public string Cliente { get; set; }

        public string ClienteDocumento { get; set; }

        public int FaturadoContraId { get; set; }

        public string FaturadoContra { get; set; }

        public string FaturadoContraDocumento { get; set; }

        public string EmailFaturamento { get; set; }

        public string DiasSemana { get; set; }

        public string DiasSemanaLiterais { get; set; }

        public string DiasFaturamento { get; set; }

        public string DataCorte { get; set; }

        public string CondicaoPagamentoId { get; set; }

        public string CondicaoPagamento { get; set; }

        public string ObservacoesFaturamento { get; set; }

        public bool UltimoDiaDoMes { get; set; }

        public bool DiaUtil { get; set; }

        public int FontePagadoraId { get; set; }

        public string FontePagadora { get; set; }

        public string FontePagadoraDocumento { get; set; }

        public string CondicaoPagamentoPorDia { get; set; }

        public string CondicaoPagamentoPorDiaSemana { get; set; }

        public bool EntregaEletronica { get; set; }

        public bool EntregaManual { get; set; }

        public bool EntregaManualSedex { get; set; }

        public bool CorreioComum { get; set; }

        public bool CorreioSedex { get; set; }

        public string IdFile { get; set; }

        public string RevisaoId { get; set; }

        public string AgruparDoctos { get; set; }

    public bool MesmaContaOportunidade { get; set; }

        public StatusFichaFaturamento StatusFichaFaturamento { get; set; }

        public StatusOportunidade StatusOportunidade { get; set; }

        public string StatusFichaFaturamentoDescricao
            => StatusFichaFaturamento.ToName();
    }
}
