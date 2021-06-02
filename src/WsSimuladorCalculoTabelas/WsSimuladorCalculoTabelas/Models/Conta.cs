using Ecoporto.CRM.Business.Enums;
using System;
using WsSimuladorCalculoTabelas.Enums;

namespace WsSimuladorCalculoTabelas.Models
{
    public class Conta 
    {
        public Conta()
        {

        }
        public int CriadoPor { get; set; }

        public string Descricao { get; set; }

        public string Documento { get; set; }

        public string NomeFantasia { get; set; }

        public string InscricaoEstadual { get; set; }

        public string Telefone { get; set; }

        public int VendedorId { get; set; }

        public string Vendedor { get; set; }

        public string Logradouro { get; set; }

        public string Bairro { get; set; }

        public int Numero { get; set; }

        public string Complemento { get; set; }

        public string CEP { get; set; }

        public int? CidadeId { get; set; }

        public int? PaisId { get; set; }
        
        public Segmento Segmento { get; set; }

        public DateTime DataCriacao { get; set; }
    }  
}
