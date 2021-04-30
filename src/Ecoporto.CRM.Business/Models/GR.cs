using Ecoporto.CRM.Business.Enums;
using System;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Models
{
    public class GR : Entidade<GR>
    {
        public GR()
        {
            ServicosFaturados = new List<ServicoFaturamento>();
        }
        
        public int SeqGR { get; set; }

        public decimal Valor { get; set; }

        public int ClienteId { get; set; }

        public string ClienteDescricao { get; set; }

        public int IndicadorId { get; set; }

        public string IndicadorDescricao { get; set; }

        public string IndicadorCnpj { get; set; }

        public int ExportadorId { get; set; }

        public string ExportadorDescricao { get; set; }

        public string ExportadorCnpj { get; set; }

        public int DespachanteId { get; set; }

        public string DespachanteDescricao { get; set; }

        public string Proposta { get; set; }

        public string Reserva { get; set; }

        public string ImportadorCnpj { get; set; }

        public DateTime Vencimento { get; set; }

        public DateTime FreeTime { get; set; }

        public int Periodos { get; set; }

        public int Lote { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        public string StatusGR { get; set; }

        public List<ServicoFaturamento> ServicosFaturados { get; set; }

        public override void Validar()
        {            
            ValidationResult = Validate(this);
        }
    }
}