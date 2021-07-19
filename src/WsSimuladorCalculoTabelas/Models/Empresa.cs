using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsSimuladorCalculoTabelas.Models
{
    public class Empresa
    {
       public int ID { get; set; }
       public string NOME_EMPRESA { get; set; }
       public int COD_EMPRESA { get; set; }
       public string JUROS { get; set; }
       public int QTD_DIAS_ALERTA { get; set; }
       public int DIA_FAT { get; set; }
       public string BASE_SAPIENS { get; set; }
       public string DIR_LOG { get; set; }
       public string WS_NFE { get; set; }

       public string WS_BOLETO_SAP { get; set; }

       public string LETRA_NOTA_MINUTA { get; set; }

       public string LETRA_NOTA_REDEX { get; set; }
       public string SERIE_IPA { get; set; }

       public string SERIE_OPE { get; set; }
       
       public string SERIE_REDEX { get; set; }

       public string SERVIDOR_SID { get; set; }
       
       public string LOGIN_SID { get; set; }
       public string SENHA_SID { get; set; }
       public string MSG_EMAIL_NFE { get; set; }
       public string EMAIL_COPIA { get; set; }
       public string EMAIL_RECUSA_IMG { get; set; }
       public string END_EMAIL_ENVIO { get; set; }
    }
}