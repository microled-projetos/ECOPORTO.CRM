using System;

namespace WsSimuladorCalculoTabelas.Models
{
    public class Anexo
    {
        public int Id { get; set; }

        public int IdProcesso { get; set; }

        public string Arquivo { get; set; }

        public DateTime DataCadastro { get; set; }

        public int CriadoPor { get; set; }

        public int TipoAnexo { get; set; }

        public int Versao { get; set; }

        public string IdArquivo { get; set; }

        public int TipoDoc { get; set; }
    }
}