namespace WsSimuladorCalculoTabelas.Models
{
    public class ServicoFixoVariavel
    {        
        public int ServicoFixoVariavelId { get; set; }

        public int ServicoId { get; set; }

        public bool FlagDesova { get; set; }

        public string Descricao { get; set; }

        public string Margem { get; set; }

        public bool FlagFCL { get; set; }

        public int Lista { get; set; }

        public decimal PrecoMaximo { get; set; }

        public decimal PrecoMinimo { get; set; }

        public decimal AdicionalGRC { get; set; }

        public decimal MinimoGRC { get; set; }

        public decimal MinimoAnvisaGRC { get; set; }

        public decimal PrecoUnitario { get; set; }

        public string BaseCalculo { get; set; }

        public decimal ValorMinimo { get; set; }

        public decimal ValorFinal{ get; set; }

        public decimal Percentual { get; set; }

        public int OportunidadeId { get; set; }

        public int Linha { get; set; }

        public int LinhaReferencia { get; set; }

        public string TipoCarga { get; set; }

        public string TipoOperacao { get; set; }

        public string VarianteLocal { get; set; }

        public string BaseExcesso { get; set; }

        public decimal ValorExcesso { get; set; }

        public int TipoDocumentoId { get; set; }

        public int Moeda { get; set; }

        public decimal ValorAcrescimo { get; set; }

        public int Periodo { get; set; }

        public int QtdeDias { get; set; }

        public int? GrupoAtracacaoId { get; set; }

        public int LocalAtracacaoId { get; set; }

        public bool ProRata { get; set; }

        public decimal ValorAcrescimoPeso { get; set; }

        public decimal PesoLimite { get; set; }

        public int LimiteBls { get; set; }

        public decimal ValorAnvisa { get; set; }

        public bool CobrarNVOCC { get; set; }

        public int FormaPagamentoNVOCC { get; set; }

        public decimal AdicionalIMO { get; set; }

        public decimal PesoMaximo { get; set; }

        public decimal AdicionalPeso { get; set; }

        public int Origem { get; set; }

        public int Destino { get; set; }

        public decimal PrecoMinimoMDir { get; set; }

        public decimal PrecoMinimoMEsq { get; set; }

        public decimal ValorAnvisaGRC { get; set; }

        public int TabelaSincronismoId { get; set; }

        public string TipoServico { get; set; }

        public decimal ValorCif { get; set; }

        public decimal Exercito { get; set; }
    }
}