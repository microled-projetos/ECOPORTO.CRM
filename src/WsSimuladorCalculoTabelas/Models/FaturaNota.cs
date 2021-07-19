using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsSimuladorCalculoTabelas.Models
{
    public class FaturaNota
    {
        public int ID { get; set; }
        public string CLIENTE { get; set; }
        public DateTime DT_EMISSAO { get; set; }
        public DateTime DT_VENCIMENTO { get; set; }
        public string NUMERO { get; set; }
        public string EXTENSO { get; set; }
        public string OBS { get; set; }
        public string NAT_OPER { get; set; }
        public string COD_OPER { get; set; }
        public int VALOR { get; set; }
        public int IMPRESSA { get; set; }
        public string TIPO { get; set; }
        public int CANCELADA { get; set; }
        public DateTime DT_CANCELADA { get; set; }
        public string GR { get; set; }
        public string DOCUMENTO_ORIGEM { get; set; }
        public string TIPO_DOCUMENTO { get; set; }
        public string BOLETO { get; set; }
        public int BOLETO_IMPRESSAO { get; set; }
        public int TIPO_FEITO { get; set; }
        public DateTime ENTRADA { get; set; }
        public string MINUTA { get; set; }
        public int VIAGEM { get; set; }
        public int DOLAR { get; set; }
        public int VERSAO { get; set; }
        public string OBS2 { get; set; }
        public int DESCONTO2 { get; set; }
        public string DEPTO { get; set; }
        public string FILIAL { get; set; }
        public string CENTROCUSTO { get; set; }
        public string CONTA { get; set; }
        public int INTEGRADA { get; set; }        
        public int COD_CLI { get; set; }
        public string BAICOB { get; set; }
        public string CEPCOB { get; set; }
        public string CIDCOB { get; set; }
        public string ENDCOB { get; set; }
        public string ESTCOB { get; set; }
        public string NOMCLI { get; set; }
        public string CGCCPF { get; set; }
        public string INSEST { get; set; }
        public string RAZAO_REPRESENTANTE { get; set; }
    }
}