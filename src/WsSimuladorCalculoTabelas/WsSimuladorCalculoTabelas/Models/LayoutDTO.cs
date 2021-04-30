using Ecoporto.CRM.Business.Enums;
using System.Collections.Generic;
using WsSimuladorCalculoTabelas.Enums;

namespace WsSimuladorCalculoTabelas.Models
{
    public class LayoutDTO
    {              
        public int Id { get; set; }

        public int ModeloId { get; set; }

        public int ServicoId { get; set; }

        public int Linha { get; set; }

        public int LinhaReferencia { get; set; }

        public bool Ocultar { get; set; }

        public string Descricao { get; set; }

        public TipoRegistro TipoRegistro { get; set; }

        public BaseCalculo BaseCalculo { get; set; }

        public int Moeda { get; set; }

        public TipoCarga TipoCarga { get; set; }

        public int TipoTrabalho { get; set; }

        public int Margem { get; set; }

        public string DescricaoValor { get; set; }

        public int Periodo { get; set; }

        public string DescricaoPeriodo { get; set; }

        public int QtdeDias { get; set; }

        public decimal Valor20 { get; set; }

        public decimal Valor40 { get; set; }

        public decimal Valor { get; set; }

        public decimal CifMinimo { get; set; }

        public decimal CifMaximo { get; set; }

        public string DescricaoCif { get; set; }

        public int Reembolso { get; set; }

        public decimal AdicionalArmazenagem { get; set; }

        public decimal AdicionalGRC { get; set; }

        public decimal MinimoGRC { get; set; }

        public decimal AdicionalIMO { get; set; }

        public decimal AdicionalIMOGRC { get; set; }

        public decimal ValorANVISA { get; set; }

        public decimal ANVISAGRC { get; set; }

        public decimal ValorMinimo { get; set; }

        public decimal ValorMinimo20 { get; set; }

        public decimal ValorMinimo40 { get; set; }

        public decimal ValorMargemDireita { get; set; }

        public decimal ValorMargemEsquerda { get; set; }

        public decimal ValorEntreMargens { get; set; }

        public decimal PesoMaximo { get; set; }

        public decimal AdicionalPeso { get; set; }

        public decimal ValorMinimoMargemDireita { get; set; }

        public decimal ValorMinimoMargemEsquerda { get; set; }

        public decimal ValorMinimoEntreMargens { get; set; }

        public string CondicoesGerais { get; set; }

        public string CondicoesIniciais { get; set; }

        public int Origem { get; set; }

        public int Destino { get; set; }

        public int OportunidadeId { get; set; }

        public int TipoDocumentoId { get; set; }

        public BaseCalculo BaseExcesso { get; set; }

        public decimal ValorExcesso { get; set; }

        public decimal PesoLimite { get; set; }

        public bool ProRata { get; set; }

        public int FormaPagamentoNVOCC { get; set; }

        public int GrupoAtracacaoId { get; set; }

        public int LimiteBls { get; set; }

        public decimal ValorCif { get; set; }
        public decimal Exercito { get; set; }
    }
}
