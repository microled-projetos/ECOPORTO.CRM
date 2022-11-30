using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsSimuladorCalculoTabelas.Models
{
    public class IntegracaoBaixa
    {
        public int AUTONUM_BL { get; set; }
        public DateTime Data_Final { get; set; }
        public DateTime Data_Doc { get; set; }
        public DateTime VALIDADE_GR { get; set; }
        public DateTime DATA_REFER { get; set; }
        public int PERIODOS { get; set; }
        public DateTime Data_Base { get; set; }
        public int Lista { get; set; }
        public DateTime DT_INICIO_CALCULO { get; set; }
        public DateTime dt_liberacao { get; set; }
        public DateTime INICIO { get; set; }
        public DateTime FINAL { get; set; }
        public int SERVICO { get; set; }
        public int AUTONUM_FERIADO { get; set; }
        public DateTime DATAFERIADO { get; set; }
        public int Nro_Conteiner { get; set; }
        public string NOME { get; set; }
        public int REG_GR { get; set; }
        public int AUTONUM { get; set; }
        public DateTime Data { get; set; }
        public int QTDE { get; set; }
        public decimal PESO_BRUTO { get; set; }
        public decimal PESO { get; set; }
        public int id_user { get; set; }
        public string iusnome { get; set; }
        public string iusemail { get; set; }
        public string EMAIL_RECUSA_IMAGEM { get; set; }
        public int PATIO { get; set; }
        public int num_documento { get; set; }
        public string NUM_DOC { get; set; }
        public string descr { get; set; }
        public int IDNum { get; set; }
        public int RPSNUM { get; set; }
        public string SERIE { get; set; }
        public string TIT_SAPIENS { get; set; }
        public DateTime HORA_INI { get; set; }
        public int USUARIO { get; set; }
        public string LOTE { get; set; }
        public string SEQ_GR { get; set; }
        public int FPPARC { get; set; }
        public int FPGRP { get; set; }
        public int FPIPA { get; set; }
        public int PARCEIRO { get; set; }
        public int fpgr { get; set; }
        public int flag_hubport { get; set; }
        public int Codcli { get; set; }
        public string Razao { get; set; }
        public string CGC { get; set; }
        public string Cidade_Cli { get; set; }
        public int autonum_cli { get; set; }
        public int ind_codcli { get; set; }
        public string Ind_Razao { get; set; }
        public string Ind_CGC { get; set; }
        public string Ind_Cidade_cli { get; set; }
        public int Ind_autonum { get; set; }
        public int DIA_FAT { get; set; }
        public string EMBARQUE { get; set; }
        public string dir_email { get; set; }
        public int DESPACHANTE { get; set; }
        public int NFE { get; set; }
        public int cliente_sap { get; set; }
        public string TIPO { get; set; }
        public int AUTONUM_REGRA { get; set; }
        public int AUTONUM_SEM_REGRA { get; set; }
        public int OPTENTREGA_REGRA { get; set; }
        public int OPTENTREGA_SEM_REGRA { get; set; }
        public string ENDERECO_SEM_REGRA { get; set; }
        public int FORCA_REGRA { get; set; }
        public int FORCA_SEM_REGRA { get; set; }
        public string USU_TIPLOGR { get; set; }
        public string ENDCLI { get; set; }
        public string NENCLI { get; set; }
        public string CPLEND { get; set; }
        public string CIDCLI { get; set; }
        public string BAICLI { get; set; }
        public string SIGUFS { get; set; }
        public string CEPCLI { get; set; }
        public string ENDCOB { get; set; }
        public string CPLCOB { get; set; }
        public string CIDCOB { get; set; }
        public string BAICOB { get; set; }
        public string ESTCOB { get; set; }
        public string CEPCOB { get; set; }
        public string NENCOB { get; set; }
        public string NOMCLI { get; set; }
        public int CODCLI { get; set; }
        public string TIPMER { get; set; }
        public string TIPCLI { get; set; }
        public string CGCCPF { get; set; }
        public string INSEST { get; set; }
        public string IBGE { get; set; }
        public int IMPORTADOR { get; set; }
        public DateTime Data1 { get; set; }
        public DateTime Data2 { get; set; }
        public int EMPRESA { get; set; }
        public int ITEM { get; set; }
        public int IMPOSTO { get; set; }
        public int AUTONUM_END { get; set; }
        public string TIPODOC_DESCRICAO { get; set; }
        public decimal VALOR_IMPOSTO { get; set; }
        public int AUTONUM_IMPOSTO { get; set; }
        public string DESCR_SERVICO { get; set; }
        public static bool validaEmbarque { get; set; }
        public static bool notaAgrupada { get; set; }       
        public static bool substituicao { get; set; }
        public static List<object> listaFatura { get; set; }     
        public decimal TOTAL { get; set; }
        public decimal  IMPOSTOS { get; set; }
        public int STATUSNFE { get; set; }
        public int CODSER { get; set; }
        public string CRITICA { get; set; }
        public int FPLTL { get; set; }

    }
}   