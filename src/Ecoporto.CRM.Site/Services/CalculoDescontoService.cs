using System;

namespace Ecoporto.CRM.Site.Services
{
    public class CalculoDescontoService
    {
        public CalculoDescontoService(int lote, int servico, decimal valorDesconto, long tabelaId, int solicitacaoId, long seqGr = 0)
        {
            Lote = lote;
            Servico = servico;
            ValorDesconto = valorDesconto;
            SeqGr = seqGr;
            TabelaId = tabelaId;
            SolicitacaoId = solicitacaoId;
        }

        public int Lote { get; set; }

        public int Servico { get; set; }

        public decimal ValorDesconto { get; set; }

        public long SeqGr { get; set; }

        public long TabelaId { get; set; }

        public int SolicitacaoId { get; set; }

        public ResultadoCalculo CalcularDesconto()
        { 
            if (Lote == 0)
                throw new Exception("Lote não informado");

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
                var resultado = ws.Calcula_Impostos_Desconto(
                    this.Lote,
                    this.Servico,
                    (double)this.ValorDesconto,
                    this.SeqGr,
                    this.TabelaId,
                    this.SolicitacaoId);

                return new ResultadoCalculo(resultado);
                
             

        }
        }
    }    
}