using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.Models
{
    public class Parceiro
    {
        public Parceiro()
        {

        }

        public Parceiro(int id, string nomeFantasia)
        {
            Id = id;
            NomeFantasia = nomeFantasia;
        }

        public int Id { get; set; }

        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

        public string InscricaoEstadual { get; set; }

        public string Documento { get; set; }

        public string Logradouro { get; set; }

        public string Numero { get; set; }

        public string Bairro { get; set; }

        public string CEP { get; set; }        

        public string Complemento { get; set; }

        public Estado Estado { get; set; }

        public string Cidade { get; set; }

        public string TipoCliente { get; set; }
    }
}
