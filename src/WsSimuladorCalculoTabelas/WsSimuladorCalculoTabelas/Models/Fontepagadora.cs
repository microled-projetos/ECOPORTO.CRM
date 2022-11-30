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

        public int AUTONUM { get; set; }
        public string AUTONUM_FORMA_PAGAMENTO { get; set; }
        public int AUTONUM_PAGAMENTO { get; set; }
        public int AUTONUM_CLIENTE_NOTA { get; set; }
        public int AUTONUM_CLI_NF_FM0 { get; set; }
        public int AUTONUM_CLI_NF_FM1 { get; set; }
        public int AUTONUM_CLIENTE_ENVIO_NOTA { get; set; }
        public int CODCLI_SAP { get; set; }
        public int FLAG_ULTIMO_DIA_DA_SEMANA { get; set; }
        public int FLAG_ULTIMO_DIA_DO_MES { get; set; }
        public int FLAG_ULTIMO_DIA_DO_MES_CORTE { get; set; }
        public int FLAG_VENCIMENTO_DIA_UTIL { get; set; }
        public int FLAG_ULTIMO_DIA_DO_MES_VCTO { get; set; }

        public IEnumerable<DiasSemana> rsDiasSemana { get; set; }

        public IEnumerable<DiasSemana> rsDias { get; set; }


    }
}