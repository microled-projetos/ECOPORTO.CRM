namespace Ecoporto.CRM.Site.Models
{
    public class ConsultaClienteViewModel
    {
        public string Solicitante { get; set; }

        public string SolicitanteCNPJ { get; set; }

        public string Importador { get; set; }

        public string ImportadorCNPJ { get; set; }

        public string Mensagem { get; set; }

        public int Acao { get; set; } 
    }
}