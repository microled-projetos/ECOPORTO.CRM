using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsSimuladorCalculoTabelas.DTO
{
    public class ExcecaoImpostosCRM
    {
        public int Tabela { get; set; }

        public int ServicoId { get; set; }

        public int ImpostoId { get; set; }

        public int Isento { get; set; }

        public decimal Percentual { get; set; }
    }
}