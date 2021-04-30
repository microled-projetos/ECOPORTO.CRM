namespace Ecoporto.CRM.Business.Models
{
    public class Parametros
    {
        public double Mora { get; set; }

        public double Multa { get; set; }

        public int DiaUtilCancelamentoSAP { get; set; }

        public bool GerarPdfProposta { get; set; }

        public bool ValidaConcomitancia { get; set; }

        public bool CriarAdendoExclusaoCliente { get; set; }

        public bool IntegraChronos { get; set; }

        public bool AnexarSimulador { get; set; }

        public decimal DividaSpc { get; set; }
    }
}
