using Ecoporto.CRM.Business.Enums;
using System;

namespace Ecoporto.CRM.Business.DTO
{
    public class SimuladorDTO
    {
        public int Id { get; set; }

        public int TipoRegistro { get; set; }

        public int Classe { get; set; }

        public string Proposta { get; set; }

        public string Descricao { get; set; }

        public string Importador { get; set; }

        public string Despachante { get; set; }

        public string Coloader { get; set; }

        public string CoColoader { get; set; }

        public string CoColoader2 { get; set; }

        public string OportunidadeId { get; set; }

        public string Linha { get; set; }

        public int LinhaReferencia { get; set; }

        public string ServicoFaturamentoId { get; set; }

        public string Periodo { get; set; }

        public string QtdeDias { get; set; }

        public string TipoCarga { get; set; }

        public string BaseCalculo { get; set; }

        public decimal PrecoUnitario { get; set; }

        public decimal PrecoMinimo { get; set; }

        public string VarianteLocal { get; set; }

        public string Moeda { get; set; }

        public string FormaPagamento { get; set; }

        public string TipoOperacao { get; set; }

        public int  Origem { get; set; }

        public decimal Valor { get; set; }

        public decimal ValorMinimo { get; set; }

        public decimal Valor20 { get; set; }

        public decimal Valor40 { get; set; }

        public decimal ValorMinimo20 { get; set; }

        public decimal ValorMinimo40 { get; set; }

        public decimal ValorMargemDireita { get; set; }

        public decimal ValorMargemEsquerda { get; set; }

        public decimal ValorEntreMargens { get; set; }

        public decimal ValorMinimoMargemDireita { get; set; }

        public decimal ValorMinimoMargemEsquerda { get; set; }

        public decimal ValorMinimoEntreMargens { get; set; }

        public string Margem { get; set; }

        public DateTime DataHora { get; set; }

        public string Usuario { get; set; }

        public Regime Regime { get; set; }

        public int TipoDocumentoId { get; set; }

        public string TipoDocumento { get; set; }

        public string Armador { get; set; }

        public int LocalAtracacaoId { get; set; }

        public string LocalAtracacao { get; set; }

        public int GrupoAtracacaoId { get; set; }

        public string GrupoAtracacao { get; set; }

        public int ArmadorId { get; set; }

        public string ArmadorDescricao { get; set; }

        public string ArmadorDocumento { get; set; }

        public decimal CifConteiner { get; set; }

        public decimal CifCargaSolta { get; set; }

        public decimal VolumeM3 { get; set; }

        public int NumeroLotes { get; set; }

        public int Periodos { get; set; }
    }
}
