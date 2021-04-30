using WsSimuladorCalculoTabelas.Models;

namespace WsSimuladorCalculoTabelas.Responses
{
    public class Response
    {
        public bool Sucesso { get; set; }

        public string Mensagem { get; set; }

        public Tabela[] Lista { get; set; }

        public int TabelaId { get; set; }

        public int ArquivoId { get; set; }

        public string Hash { get; set; }

        public string NomeArquivo { get; set; }

        public int TamanhoArquivo { get; set; }

        public string Base64 { get; set; }

        public int SimuladorId { get; set; }
    }
}