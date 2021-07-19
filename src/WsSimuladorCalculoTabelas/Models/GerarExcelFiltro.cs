namespace WsSimuladorCalculoTabelas.Models
{
    public class GerarExcelFiltro
	{
        public int SimuladorId { get; set; }

        public int ParametroSimuladorId { get; set; }

        public int ModeloSimuladorId { get; set; }

        public int OportunidadeId { get; set; }

        public bool SomenteEstimativa { get; set; }

        public bool ServicosComplementares { get; set; }

        public bool ComAnaliseDeDados { get; set; }

        public string DataPgtoInicial { get; set; }

        public string DataPgtoFinal { get; set; }

        public bool DadosDoCliente { get; set; }

        public string FiltroCliente { get; set; }

        public int TabelaId { get; set; }

        public bool CRM { get; set; }

        public bool AnexarArquivoSimulador { get; set; }

        public int UsuarioSimuladorId { get; set; }
    }
}