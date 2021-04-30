using System;

namespace Ecoporto.CRM.Site.Services
{
    public class CalculoDescontoRedexService
    {
        public CalculoDescontoRedexService(long booking, int servico, decimal valorDesconto, long tabelaId, int solicitacaoId, long seqGr, long clienteId)
        {
            Booking = booking;
            Servico = servico;
            ValorDesconto = valorDesconto;
            SeqGr = seqGr;
            ClienteId = clienteId;
            TabelaId = tabelaId;
            SolicitacaoId = solicitacaoId;
        }

        public long Booking { get; set; }

        public int Servico { get; set; }

        public decimal ValorDesconto { get; set; }

        public long SeqGr { get; set; }

        public long ClienteId { get; set; }

        public long TabelaId { get; set; }

        public int SolicitacaoId { get; set; }

        public ResultadoCalculo CalcularDescontoRedex()
        {
            if (Booking == 0)
                throw new Exception("Booking não informado");

            if (Servico == 0)
                throw new Exception("Serviço não informado");

            if (ValorDesconto == 0)
                throw new Exception("Valor desconto não informado ou igual a zero");

            if (TabelaId == 0)
                throw new Exception("Tabela não informada ou igual a zero");

            if (SolicitacaoId == 0)
                throw new Exception("ID da Solicitação Comercial não informada ou igual a zero");

            using (var ws = new WsCalculo.Calcular())
            {
                var resultado = ws.Calcula_Impostos_Desconto_Redex(
                    this.Booking,
                    this.Servico,
                    (double)this.ValorDesconto,
                    this.SeqGr,
                    this.ClienteId,
                    this.TabelaId,
                    this.SolicitacaoId);

                return new ResultadoCalculo(resultado);
            }
        }
    }    
}