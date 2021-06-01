using Ecoporto.CRM.Business.Extensions;

namespace Ecoporto.CRM.Site.Services
{
    public class ResultadoCalculo
    {
        public decimal ValorFinal { get; set; }

        public decimal Imposto { get; set; }

        public decimal ValorAPagar { get; set; }

        public ResultadoCalculo(string retornoWebService)
        {
            var valores = retornoWebService.Split('|');

            if (valores.Length > 0)
            {
                ValorFinal = valores[0].ToDecimal();
                Imposto = valores[1].ToDecimal();
            }
        }
    }
}