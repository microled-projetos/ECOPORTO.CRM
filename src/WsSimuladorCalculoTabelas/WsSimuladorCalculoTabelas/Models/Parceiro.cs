namespace WsSimuladorCalculoTabelas.Models
{
    public class Parceiro
    {
        public int Id { get; set; }

        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

        public string CNPJ { get; set; }

        public bool Vendedor { get; set; }

        public bool Importador { get; set; }

        public bool Despachante { get; set; }

        public bool Indicador { get; set; }

        public bool Coloaders { get; set; }        
    }
}