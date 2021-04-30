using System;

namespace Ecoporto.CRM.IntegraChronosService
{
    public class Modelo 
    {              
        public string Descricao { get; set; }

        public int DiasFreeTime { get; set; }

        public int QtdeDias { get; set; }

        public int Validade { get; set; }  

        public int? VendedorId { get; set; }

        public int ImpostoId { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataInatividade { get; set; }

        public bool Acordo { get; set; }

        public bool ParametroBL { get; set; }

        public bool ParametroLote { get; set; }

        public bool ParametroConteiner { get; set; }

        public bool ParametroIdTabela { get; set; }

        public bool HubPort { get; set; }

        public bool CobrancaEspecial { get; set; }

        public decimal DesovaParcial { get; set; }

        public decimal FatorCP { get; set; }

        public int PosicIsento { get; set; }

        public bool IntegraChronos { get; set; }
    }
}