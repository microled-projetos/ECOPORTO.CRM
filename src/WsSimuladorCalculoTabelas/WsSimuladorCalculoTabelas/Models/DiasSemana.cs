using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsSimuladorCalculoTabelas.Models
{
    public class DiasSemana
    {
        public string Dia { get; set; }
        public int UltimoDiaSemana { get; set; }
        public int UltimoDiaMes { get; set; }
        public int UltimoDiaMesCorte { get; set; }
        public int UltimoDiaMesVcto { get; set; }
        public string CODCPG { get; set; }
        public string DIAS { get; set; }
    }
}