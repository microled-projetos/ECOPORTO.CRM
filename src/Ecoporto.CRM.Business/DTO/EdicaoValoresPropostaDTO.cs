using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.DTO
{
    public class EdicaoValoresPropostaDTO
    {
        public EdicaoValoresPropostaDTO()
        {
            ClientesHubPort = new List<ClienteHubPort>();
        }

        public int Id { get; set; }

        public int ModeloId { get; set; }

        public int ServicoId { get; set; }

        public int Linha { get; set; }

        public string Descricao { get; set; }

        public string LinhaReferencia { get; set; }

        public string DescricaoValor { get; set; }

        public string Periodo { get; set; }

        public string DescricaoPeriodo { get; set; }

        public string QtdeDias { get; set; }

        public string ValorCif { get; set; }
        public string Valor20 { get; set; }

        public string Valor40 { get; set; }

        public string Valor { get; set; }

        public string CifMinimo { get; set; }

        public string CifMaximo { get; set; }

        public string DescricaoCif { get; set; }

        public string Reembolso { get; set; }

        public string AdicionalArmazenagem { get; set; }

        public string AdicionalGRC { get; set; }

        public string MinimoGRC { get; set; }

        public string AdicionalIMO { get; set; }

        public string Exercito { get; set; }

        public string ValorANVISA { get; set; }

        public string AnvisaGRC { get; set; }

        public string AdicionalIMOGRC { get; set; }

        public string ValorMinimo { get; set; }

        public string ValorMinimo20 { get; set; }

        public string ValorMinimo40 { get; set; }

        public string LimiteBls { get; set; }

        public string ValorMargemDireita { get; set; }

        public string ValorMargemEsquerda { get; set; }

        public string ValorEntreMargens { get; set; }

        public string PesoMaximo { get; set; }

        public string AdicionalPeso { get; set; }

        public string ValorMinimoMargemDireita { get; set; }

        public string ValorMinimoMargemEsquerda { get; set; }

        public string ValorMinimoEntreMargens { get; set; }

        public string CondicoesGerais { get; set; }

        public string CondicoesIniciais { get; set; }

        public string Origem { get; set; }

        public string Destino { get; set; }

        public string OportunidadeId { get; set; }

        public FormaPagamento FormaPagamentoNVOCC { get; set; }

        public BaseCalculo BaseCalculo { get; set; }

        public Moeda Moeda { get; set; }

        public TipoCarga TipoCarga { get; set; }

        public TipoTrabalho TipoTrabalho { get; set; }

        public Margem Margem { get; set; }

        public TipoRegistro TipoRegistro { get; set; }

        public IEnumerable<ClienteHubPort> ClientesHubPort { get; set; }

        public Dictionary<int, string> ListaBaseCalculo => new Dictionary<int, string>()
        {
            {0, ""},
            {1, "UNID"},
            {2, "TON"},
            {3, "CIF"},
            {4, "CIFM"},
            {5, "CIF0"},
            {6, "BL"},
            {7, "VOLP"}
        };

        public Dictionary<int, string> ListaMoeda => new Dictionary<int, string>()
        {
            {1, "R$"},
            {2, "US$"}
        };

        public Dictionary<int, string> ListaTipoCarga => new Dictionary<int, string>()
        {
            {0, ""},
            {1, "Contêiner"},
            {2, "Carga Solta"},
            {3, "Mudança Regime"}
        };

        public Dictionary<int, string> ListaTipoTrabalho => new Dictionary<int, string>()
        {
            {0, ""},
            { 1, "Mecanica"},
            {2, "Manual"},
        };

        public Dictionary<int, string> ListaMargem => new Dictionary<int, string>()
        {
            {1, "Direita"},
            {2, "Esquerda"},
            {3, "Entre Margens"}
        };

        public Dictionary<int, string> ListaFormaPagamentoHubPort => new Dictionary<int, string>()
        {
            {0, ""},
            {1, "À Vista"},
            {2, "Faturado"}
        };
    }
}