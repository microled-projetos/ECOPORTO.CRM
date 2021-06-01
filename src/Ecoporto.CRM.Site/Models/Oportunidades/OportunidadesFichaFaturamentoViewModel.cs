using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Workflow.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class OportunidadesFichaFaturamentoViewModel
    {
        public OportunidadesFichaFaturamentoViewModel()
        {
            CondicoesPagamentoFaturamento = new List<CondicaoPagamentoFatura>();
            FichasFaturamento = new List<FichaFaturamentoDTO>();
            ClientesProposta = new List<ClientePropostaDTO>();
        }

        public int FichaFaturamentoId { get; set; }

        public int OportunidadeId { get; set; }

        [Display(Name = "Status")]
        public StatusFichaFaturamento StatusFichaFaturamento { get; set; }

        public StatusOportunidade StatusOportunidade { get; set; }

        [Display(Name = "Faturado Contra")]
        public int? FaturadoContraId { get; set; }
        public string FaturadoContraDescricao { get; set; }

        [Display(Name = "Fonte Pagadora")]
        public int FontePagadoraId { get; set; }

        [Display(Name = "Dias Faturamento")]
        public string DiasSemana { get; set; }

        [Display(Name = "Ou")]
        public string DiasFaturamento { get; set; }

        [Display(Name = "Data Corte")]
        public int? DataCorte { get; set; }

        [Display(Name = "Condição Pagamento")]
        public string CondicaoPagamentoFaturamentoId { get; set; }

        [Display(Name = "Email")]
        public string EmailFaturamento { get; set; }

        [Display(Name = "Observações")]
        public string ObservacoesFaturamento { get; set; }

        public int AnexoFaturamentoId { get; set; }

        [Display(Name = "Anexar Ficha")]
        public string AnexoFaturamento { get; set; }

        [Display(Name = "Último dia do mês")]
        public bool UltimoDiaDoMes { get; set; }

        [Display(Name = "Dia útil")]
        public bool DiaUtil { get; set; }

        [Display(Name = "Condição Pgto p/ dia")]
        public string CondicaoPagamentoPorDia { get; set; }

        [Display(Name = "Dia da semana")]
        public string CondicaoPagamentoPorDiaSemana { get; set; }

        [Display(Name = "Entrega Eletrônica?")]
        public bool EntregaEletronica { get; set; }

        [Display(Name = "Entrega Manual?")]
        public bool EntregaManual { get; set; }

        [Display(Name = "Entrega Manual")]
        public bool EntregaManualSedex { get; set; }

        [Display(Name = "Correio Comum")]
        public bool CorreioComum { get; set; }

        [Display(Name = "Correio (Sedex)")]
        public bool CorreioSedex { get; set; }

        public int FichaRevisaoId { get; set; }

        [Display(Name = "Selecione um Cliente")]
        public int[] ClientePropostaSelecionadoId { get; set; }

        [Display(Name = "Selecione um Cliente")]
        public int ClienteSelecionadoId { get; set; }

        public List<CondicaoPagamentoFatura> CondicoesPagamentoFaturamento { get; set; }

        public List<FichaFaturamentoDTO> FichasFaturamento { get; set; }

        public List<ClientePropostaDTO> ClientesProposta { get; set; }
    }
}