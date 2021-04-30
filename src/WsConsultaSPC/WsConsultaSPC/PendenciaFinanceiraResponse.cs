using System;

namespace WsConsultaSPC
{
    public class PendenciaFinanceiraResponse
    {
        public string NotaFiscal { get; set; }

        public decimal Valor { get; set; }

        public string Cliente { get; set; }

        public DateTime Vencimento { get; set; }

        public string Lote { get; set; }

        public string Documento { get; set; }

        public string Tipo { get; set; }
    }
}
