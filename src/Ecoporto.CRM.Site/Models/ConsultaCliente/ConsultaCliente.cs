namespace Ecoporto.CRM.Site.Models
{
    public class ConsultaCliente
    {
        public string Resposta { get; set; }

        public ConsultaClienteArquivo[] Arquivos { get; set; }
    }

    public class ConsultaClienteArquivo
    {
        public string Nome { get; set; }

        public string Extensao { get; set; }

        public byte[] dataArray { get; set; }
    }
}