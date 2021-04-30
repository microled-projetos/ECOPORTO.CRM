using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.DTO
{
    public class LayoutDTO
    {      
        public LayoutDTO()
        {
            ClientesHubPort = new List<ClienteHubPort>();
        }

        public int Id { get; set; }

        public int ModeloId { get; set; }

        public int ServicoId { get; set; }

        public Servico Servico { get; set; }

        public int Linha { get; set; }

        public bool Ocultar { get; set; }

        public string Descricao { get; set; }

        public int LimiteBls { get; set; }

        public TipoRegistro TipoRegistro { get; set; }

        public BaseCalculo BaseCalculo { get; set; }

        public Moeda Moeda { get; set; }

        public TipoCarga TipoCarga { get; set; }

        public TipoTrabalho TipoTrabalho { get; set; }

        public Margem Margem { get; set; }

        public int LinhaReferencia { get; set; }

        public string DescricaoValor { get; set; }

        public int Periodo { get; set; }

        public string DescricaoPeriodo { get; set; }

        public int QtdeDias { get; set; }

        public decimal ValorCIF { get; set; }

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

        public decimal Exercito { get; set; }

        public decimal AdicionalIMOGRC { get; set; }

        public decimal ValorANVISA { get; set; }

        public decimal AnvisaGRC { get; set; }

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

        public BaseCalculoExcesso BaseExcesso { get; set; }

        public decimal ValorExcesso { get; set; }

        public decimal PesoLimite { get; set; }

        public bool ProRata { get; set; }

        public FormaPagamento FormaPagamentoNVOCC { get; set; }

        public int GrupoAtracacaoId { get; set; }

        public IEnumerable<FaixaBL> FaixasBL { get; set; }

        public IEnumerable<FaixaCIF> FaixasCIF { get; set; }

        public IEnumerable<FaixaPeso> FaixasPeso { get; set; }

        public IEnumerable<ClienteHubPort> ClientesHubPort { get; set; }
    }
}
