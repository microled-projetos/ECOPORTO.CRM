using System;

namespace Ecoporto.CRM.Site.Services
{
    public class CalculoDescontoMinutaService
    {
        public CalculoDescontoMinutaService(int minuta, int servico, decimal valorDesconto, int solicitacaoId)
        {
            Minuta = minuta;
            Servico = servico;
            ValorDesconto = valorDesconto;          
            SolicitacaoId = solicitacaoId;
        }

        public int Minuta { get; set; }

        public int Servico { get; set; }

        public decimal ValorDesconto { get; set; }

        public int SolicitacaoId { get; set; }

        public ResultadoCalculo CalcularDesconto()
        {
            if (Minuta == 0)
                throw new Exception("Minuta não informada");

            if (Servico == 0)
                throw new Exception("Serviço não informado");

            if (ValorDesconto == 0)
                throw new Exception("Valor desconto não informado ou igual a zero");     

            if (SolicitacaoId == 0)
                throw new Exception("ID da Solicitação Comercial não informada ou igual a zero");

            using (var ws = new WsCalculo.Calcular())
            {
                var resultado = ws.Calcula_Impostos_Minuta_Desconto(
                    this.Minuta,
                    this.Servico,
                    (double)this.ValorDesconto,
                    this.SolicitacaoId);

                return new ResultadoCalculo(resultado);
            }
        }
    }    
}