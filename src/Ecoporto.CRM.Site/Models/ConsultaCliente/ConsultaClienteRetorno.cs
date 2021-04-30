using System.Collections.Generic;

namespace Ecoporto.CRM.Site.Models
{
    public class ConsultaClienteRetorno
    {
        public bool Sucesso { get; set; }

        public string Mensagem { get; set; }

        public List<ConsultaClienteRetornoArquivo> Arquivos { get; set; }
            = new List<ConsultaClienteRetornoArquivo>();
    }

    public class ConsultaClienteRetornoArquivo
    {
        public ConsultaClienteRetornoArquivo(string arquivo, string extensao, string base64)
        {
            Arquivo = arquivo;
            Extensao = extensao;
            Base64 = base64;
        }

        public string Arquivo { get; set; }

        public string Extensao { get; set; }

        public string Base64 { get; set; }
    }
}