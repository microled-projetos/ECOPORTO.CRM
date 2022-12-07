using Ecoporto.CRM.Business.Enums;
using System.Collections.Generic;

namespace WsSimuladorCalculoTabelas.Models
{
    public class OportunidadeFicha
    {
        public int Id { get; set; }

        public int TabelaId { get; set; }

        public StatusFichaFaturamento StatusFichaFaturamento { get; set; }

        public int OportunidadeId { get; set; }

        public int ContaId { get; set; }

        public int FaturadoContraId { get; set; }

        public string FaturadoContraDescricao { get; set; }

        public string FaturadoContraFantasia { get; set; }

        public string FaturadoContraDocumento { get; set; }

        public int FontePagadoraId { get; set; }

        public string FontePagadoraDescricao { get; set; }

        public string FontePagadoraFantasia { get; set; }

        public string FontePagadoraDocumento { get; set; }

        public string ContaDescricao { get; set; }

        public string ContaFantasia { get; set; }

        public string ContaDocumento { get; set; }

        public string DiasSemana { get; set; }

        public string DiasFaturamento { get; set; }

        public string CondicaoPgtoDia { get; set; }

        public string CondicaoPgtoDiaSemana { get; set; }

        public int DataCorte { get; set; }

        public string CondicaoPagamentoFaturamentoId { get; set; }

        public string EmailFaturamento { get; set; }

        public string ObservacoesFaturamento { get; set; }

        public int AnexoFaturamentoId { get; set; }

        public string AnexoFaturamento { get; set; }

        public bool EntregaEletronica { get; set; }

        public int RevisaoId { get; set; }

        public bool EntregaManual { get; set; }

        public bool CorrreiosComum { get; set; }

        public bool CorreiosSedex { get; set; }

        public bool DiaUtil { get; set; }

        public bool UltimoDiaDoMes { get; set; }

        public bool FichaGeral { get; set; }

        public int AgruparDoctos { get; set; }

        public SegmentoSubCliente SegmentoSubCliente { get; set; }

        public List<ClienteProposta> SubClientes { get; set; } = new List<ClienteProposta>();
    }
}