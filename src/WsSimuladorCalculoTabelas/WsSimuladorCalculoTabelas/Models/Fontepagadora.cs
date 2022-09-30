using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsSimuladorCalculoTabelas.Models
{
    public class FontePagadora
    {

        public string condPag { get; set; }
        public long  idClienteNota { get; set; }
        public long idCliNFFM0 { get; set; }
        public long idCliNFFM1 { get; set; }
        public long idClienteEntrega { get; set; }
        public string codcliEntrega { get; set; }
        public long autonum { get; set; }
        public int ultimoDiaSemana { get; set; }
        public int ultimoDiaMes { get; set; }
        public int ultimoDiaMesCorte { get; set; }
        public int ultimoDiaMesVcto { get; set; }

        public DataTable rsDiasSemana { get; set; }

        public DataTable  rsDias { get; set; }
    }
}