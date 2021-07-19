using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsSimuladorCalculoTabelas.DTO
{
    public class CIFEscalonadoDTO
    {
        public string Descricao { get; set; }

        public decimal ValorInicial { get; set; }

        public decimal ValorFinal { get; set; }

        public decimal Percentual { get; set; }

        public decimal Minimo { get; set; }

        public string Periodo { get; set; }
    }
}