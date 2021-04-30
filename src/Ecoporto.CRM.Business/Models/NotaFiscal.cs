using Ecoporto.CRM.Business.Enums;
using System;

namespace Ecoporto.CRM.Business.Models
{
    public class NotaFiscal : Entidade<NotaFiscal>
    {
        public string NFE { get; set; }

        public decimal Valor { get; set; }

        public string Cliente { get; set; }

        public string DocumentoCliente { get; set; }

        public string RPS { get; set; }

        public string Reserva { get; set; }

        public string Documento { get; set; }

        public int StatusNFE { get; set; }

        public string Cidade { get; set; }

        public DateTime DataEmissao { get; set; }

        public DateTime DataVencimento { get; set; }

        public int Lote { get; set; }

        public string GR { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        public string CnpjImportador { get; set; }

        public string CnpjCaptador { get; set; }

        public override void Validar()
        {
            ValidationResult = Validate(this);
        }
    }
}
