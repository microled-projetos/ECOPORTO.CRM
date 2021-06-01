namespace Ecoporto.CRM.Site.Models
{
    public class SimuladorDownloadRelatorioViewModel
    {
        public SimuladorDownloadRelatorioViewModel()
        {

        }

        public SimuladorDownloadRelatorioViewModel(string mensagem)
        {
            Mensagem = mensagem;
        }

        public int SimuladorId { get; set; }

        public string NomeArquivo { get; set; }

        public string TamanhoArquivo { get; set; }

        public string ArquivoId { get; set; }

        public string Hash { get; set; }
        public string Mensagem { get; set; }
    }
}