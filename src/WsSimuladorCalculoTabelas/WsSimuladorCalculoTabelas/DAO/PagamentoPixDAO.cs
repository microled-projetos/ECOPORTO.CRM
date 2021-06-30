using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WsSimuladorCalculoTabelas.Configuracao;
using WsSimuladorCalculoTabelas.Models;
using System.Text;

namespace WsSimuladorCalculoTabelas.DAO
{
    public class PagamentoPixDAO
    {
        public IEnumerable<IntegracaoBaixa> GetListaBL(long numeroTitulo)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT AUTONUM LOTE, SEQ_GR, AUTONUM FROM SGIPA.TB_BL WHERE NUM_TITULO_PIX =  " + numeroTitulo);

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).AsEnumerable();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool atualizaImpressaoGR(string status, int lote, int seqGR)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT IMPORTADOR FROM SGIPA.TB_BL WHERE FATURADO = 0 AND BL = " + lote + " AND SEQ_GR = " + seqGR);

                    int usuario_imp = db.Query<int>(sb.ToString()).FirstOrDefault();


                    sb.AppendLine(" UPDATE SGIPA..TB_GR_BL SET  ");
                    sb.AppendLine(" STATUS_GR = 'IM', ");
                    sb.AppendLine(" FLAG_GR_PAGA = 1, ");

                    if (status == "IM")
                    {
                        sb.AppendLine(" DT_IMPRESSAO = sysdate ");
                    }

                    sb.AppendLine(" USUARIO_IMP = " + usuario_imp);
                    sb.AppendLine(" WHERE FATURADO = 0 AND BL = " + lote + " AND SEQ_GR = " + seqGR);


                    return db.Query<bool>(sb.ToString()).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool atualizaDescontoCom(long seq_gr)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("  UPDATE SGIPA.TB_VAL_COMERCIAL SET FLAG_APROVADO = 4 ");
                    sb.AppendLine(" WHERE FLAG_APROVADO = 0 ");
                    sb.AppendLine(" AND GR = " + seq_gr);

                    

                    return db.Query<bool>(sb.ToString()).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public IntegracaoBaixa UsuarioEmProcessoDeCalculo(int Lote)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT B.NOME, A.REG_GR  ");
                    sb.AppendLine(" FROM ");
                    sb.AppendLine(" SGIPA.TB_BL A ");
                    sb.AppendLine(" INNER JOIN  ");
                    sb.AppendLine(" FATURA.USUARIOS B ");
                    sb.AppendLine(" ON A.REG_GR=B.ID WHERE A.AUTONUM =  " + Lote);


                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IntegracaoBaixa obtemDadosGRPre(int lote)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" Data_Final,   ");
                    sb.AppendLine(" Data_Doc,   ");
                    sb.AppendLine(" VALIDADE_GR,   ");
                    sb.AppendLine(" DATA_REEFER,  ");
                    sb.AppendLine(" Periodos,    ");
                    sb.AppendLine(" Data_Base,   ");
                    sb.AppendLine(" Lista,   ");
                    sb.AppendLine(" DT_INICIO_CALCULO,   ");
                    sb.AppendLine(" dt_liberacao  ");
                    sb.AppendLine(" FROM ");
                    sb.AppendLine(" sgipa.tb_gr_pre_calculo  ");
                    sb.AppendLine(" WHERE ");
                    sb.AppendLine(" BL =  " + lote);

                    var query = db.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IntegracaoBaixa obtemMaiorDataFinalGRPre(int lote)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" max(DATA_FINAL) As Data_Final  ");
                    sb.AppendLine(" max(Lista) As Lista ");
                    sb.AppendLine(" FROM ");
                    sb.AppendLine(" tb_gr_pre_calculo ");
                    sb.AppendLine(" WHERE  ");
                    sb.AppendLine(" Lote  =  " + lote);

                    var query = db.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IntegracaoBaixa verificaBLSemSaida(int lote)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" AUTONUM as AUTONUM_BL  ");
                    sb.AppendLine(" FROM   ");
                    sb.AppendLine(" tb_bl ");
                    sb.AppendLine(" WHERE  ");
                    sb.AppendLine(" ULTIMA_SAIDA is null ");
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" AUTONUM =  " + lote);

                    var query = db.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IntegracaoBaixa obtemDadosPeriodoGR(int lote)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" INICIO, FINAL, PERIODOS ");
                    sb.AppendLine(" FROM ");
                    sb.AppendLine(" SGIPA.TB_SERVICOS_FATURADOS ");
                    sb.AppendLine(" WHERE  ");
                    sb.AppendLine(" NVL(SEQ_GR, 0)  = 0 ");
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" PERIODOS > 0 ");
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" BL =  " + lote);
                    sb.AppendLine(" ORDER BY SERVICO ");

                    var query = db.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int obtemProximoNumGR()
        {
            int count = 0;

            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append(" SELECT SGIPA.SEQ_GR.NEXTVAL FROM DUAL ");

                    count = db.Query<int>(sb.ToString()).FirstOrDefault();

                    return count;
                }
            }
            catch (Exception ex)
            {
                return count;
            }
        }
        public IntegracaoBaixa obtemQTDCntrBL(int lote)
        {
            int count = 0;
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" count(a.id_conteiner) as Nro_Conteiner ");
                    sb.AppendLine(" FROM  ");
                    sb.AppendLine(" SGIPA.TB_CNTR_BL a, ");
                    sb.AppendLine(" SGIPA.TB_AMR_CNTR b, ");
                    sb.AppendLine(" SGIPA.TB_REGISTRO_SAIDA_CNTR c ");
                    sb.AppendLine(" SGIPA.TB_ORDEM_CARREGAMENTO d");
                    sb.AppendLine(" WHERE ");
                    sb.AppendLine(" Cntr = a.autonum AND C.CNTR(+) = A.AUTONUM AND D.AUTONUM(+) = C.ORDEM_CARREG and a.flag_terminal = 1  ");
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" b.bl = " + lote);


                    var query = db.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IntegracaoBaixa obtemQTDCargaBL(int lote)
        {
            try
            {
                using (OracleConnection db = new OracleConnection())
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" SUM(a.quantidade) As QTDE, ");
                    sb.AppendLine(" SUM(a.PESO_BRUTO) As PESO ");
                    sb.AppendLine(" FROM  ");
                    sb.AppendLine(" SGIPA.TB_CARGA_SOLTA a,  ");
                    sb.AppendLine(" SGIPA.DTE_TB_EMBALAGENS b,  ");
                    sb.AppendLine(" WHERE ");
                    sb.AppendLine(" a.embalagem=b.code(+) ");
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" a.bl =  " + lote);
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" a.quantidade <> 0");


                    var query = db.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool atualizaGREmServico(int lote, int seqGR, int usuario)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" UPDATE SGIPA.TB_SERVICOS_FATURADOS SET   ");
                    sb.AppendLine(" SEQ_GR =  " + seqGR + ", ");
                    sb.AppendLine(" USUARIO =  " + usuario + ",  ");
                    sb.AppendLine(" WHERE  ");
                    sb.AppendLine(" NVL(SEQ_GR, 0) = 0 ");
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" BL =  " + lote);

                    bool query = db.Query<bool>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool atualizaGREmDescricao(int lote, int seqGR)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" UPDATE  ");
                    sb.AppendLine(" SGIPA.TB_DESCRICAO_CALCULO SET ");
                    sb.AppendLine(" SEQ_GR =  " + seqGR);
                    sb.AppendLine(" WHERE  ");
                    sb.AppendLine(" SEQ_GR IS NULL ");
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" BL =  " + lote);


                    bool query = db.Query<bool>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool atualizaGREmCNTR(int lote, int seqGR)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" UPDATE SGIPA.TB_CNTR_BL SET ");
                    sb.AppendLine(" SEQ_GR = " + seqGR);
                    sb.AppendLine(" WHERE AUTONUM = (select a.autonum from  ");
                    sb.AppendLine(" SGIPA.TB_CNTR_BL A, ");
                    sb.AppendLine(" SGIPA.TB_AMR_CNTR_BL B, ");
                    sb.AppendLine(" WHERE ");
                    sb.AppendLine(" B.CNTR = A.AUTONUM ");
                    sb.AppendLine(" AND A.FLAG_TERMINAL = 1 ");
                    sb.AppendLine(" AND BL = " + lote);
                    sb.AppendLine(" AND seq_gr =0) ");

                    bool query = db.Query<bool>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool atualizaGREmCS(int lote, int seqGr)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" UPDATE TB_Carga_solta SET ");
                    sb.AppendLine(" SEQ_GR =  " + seqGr);
                    sb.AppendLine(" WHERE  ");
                    sb.AppendLine(" FLAG_TERMINAL = 1 ");
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" BL = " + lote);
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" SEQ_GR = 0 ");

                    bool query = db.Query<bool>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public int getAutonum(int id)
        {
            int usuario = 0;
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracao.Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT AUTONUM FROM SGIPA.TB_CAD_USUARIOS WHERE USUARIO  = " + id);

                    usuario = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return usuario;
                }
            }
            catch (Exception ex)
            {
                return usuario;
            }
        }
        public IntegracaoBaixa GetDadosAtualizaVencimento(int Lote)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" Select   Max(Validade_Gr) as Data1 ,Max(DT_BASE_CALCULO_REEFER) as Data2 ");
                    sb.AppendLine(" From sgipa.tb_gr_bl a , sgipa.tb_servicos_FATURADOS b where a.seq_gr=b.seq_gr and a.bl= " + Lote);
                    sb.AppendLine(" and NVL(A.FLAG_GR_LTL,0)=0 AND a.status_gr in('IM','GE') and (a.Forma_Pagamento=2 or (a.Forma_Pagamento=3 and (b.servico=52 or b.servico=71 or b.servico=57))) ");

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int countTBFaturados(int Lote)
        {
            int count = 0;
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" Select   count(1) From sgipa.tb_servicos_FATURADOS where seq_gr is null and (servico=52 or servico=57 or servico=71) AND bl= " + Lote);

                    count = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return count;
                }

            }
            catch (Exception ex)
            {
                return count;
            }
        }

        public int countBL_GrIMGE(int Lote)
        {
            int count = 0;
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" Select   COUNT(1)  From sgipa.tb_gr_bl a  where a.bl=" + Lote);
                    sb.AppendLine(" and NVL(A.FLAG_GR_LTL,0)=0 AND a.status_gr in('IM','GE') and (a.Forma_Pagamento=2 ) ");

                    count = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return count;
                }

            }
            catch (Exception ex)
            {
                return count;
            }
        }
        public bool GetUpdateDtFimPeriodo(string Data, int Lote)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" Update sgipa.Tb_Cntr_Bl Set Dt_Fim_Periodo= " + Data + " where Autonum in (Select Cntr From sgipa.Tb_Amr_Cntr_Bl Where Bl = " + Lote);

                    con.Query<bool>(sb.ToString()).FirstOrDefault();

                    sb.Clear();

                    sb.AppendLine(" Update sgipa.TB_CARGA_SOLTA SET Dt_Fim_Periodo= " + Data + " where BL = " + Lote);

                    con.Query<bool>(sb.ToString()).FirstOrDefault();

                    return true;

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public int obtemQtdLavagemCNTR(int Lote, int seqGR)
        {
            int count = 0;
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(" SELECT count(1) FROM  SGIPA.TB_SERVICOS_FATURADOS WHERE SEQ_GR= " + seqGR + " AND SERVICO = 112 AND BL= " + Lote);

                    count = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return count;
                }
            }
            catch (Exception ex)
            {
                return count;
            }
        }
        public bool atualizaAMRNFCNTRLavagem(int Lote, int seqGR)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" UPDATE SGIPA.Tb_Amr_Nf_Cntr SET  ");
                    sb.AppendLine(" SEQ_GR =  " + seqGR);
                    sb.AppendLine(" WHERE Nvl(Seq_Gr,0) = 0 AND AUTONUM_Cntr IN  ");
                    sb.AppendLine(" ( ");
                    sb.AppendLine(" select  ");
                    sb.AppendLine(" a.Autonum ");
                    sb.AppendLine(" from   ");
                    sb.AppendLine(" TB_CNTR_BL A, ");
                    sb.AppendLine(" TB_AMR_CNTR_BL B ");
                    sb.AppendLine(" where  ");
                    sb.AppendLine(" B.CNTR = A.AUTONUM ");
                    sb.AppendLine(" AND A.FLAG_TERMINAL = 1 ");
                    sb.AppendLine(" AND BL = " + Lote);
                    sb.AppendLine(" ) AND lote = " + Lote);

                    con.Query<bool>(sb.ToString()).FirstOrDefault();

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool atualizaServicosFixosGR(int Lote, int seqGR)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" UPDATE SGIPA.TB_SERVICOS_ADICIONAIS_BL SET SEQ_GR =  " + seqGR + " WHERE AUTONUM IN ");
                    sb.AppendLine(" ( ");
                    sb.AppendLine(" select C.AUTONUM ");
                    sb.AppendLine(" from SGIPA.TB_SERVICOS_ADICIONAIS_BL C, ");
                    sb.AppendLine(" SGIPA.TB_servicos_faturados D ");
                    sb.AppendLine(" where c.seq_gr IS NULL AND d.seq_gr= " + seqGR);
                    sb.AppendLine(" and c.servico=d.servico ");
                    sb.AppendLine(" and c.BL =  " + Lote + " AND D.BL= " + Lote);
                    sb.AppendLine(" ) ");

                    con.Query<bool>(sb.ToString()).FirstOrDefault();

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool atualizaPreCalculoGR(int Lote)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" UPDATE SGIPA.TB_GR_PRE_CALCULO set dt_inicio_calculo=null ,dt_liberacao=null ");
                    sb.AppendLine("  WHERE BL = " + Lote);

                    con.Query<bool>(sb.ToString()).FirstOrDefault();

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool atualizaBLREGGR(int Lote)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("  UPDATE SGIPA.TB_BL SET REG_GR = 0 WHERE AUTONUM = " + Lote);

                    bool query = con.Query<bool>(sb.ToString()).FirstOrDefault();
                    return query;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public IntegracaoBaixa EFeriado(string date)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT AUTONUM, DATA FROM OPERADOR.TB_FERIADOS WHERE DATA = TO_DATE('" + date + "', 'DD/MM/YYYY')");

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region metodos botão RPS
        public bool atualizaUserBLREGGR(int Lote, int usuario)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("  UPDATE SGIPA.TB_BL SET REG_GR = " + usuario + "  WHERE AUTONUM = " + Lote);

                    bool query = con.Query<bool>(sb.ToString()).FirstOrDefault();
                    return query;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public int obtemMaiorIDNotaGR(int GR)
        {
            int max = 0;
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT MAX(ID) FROM FATURA FATURANOTA WHERE CANCELADA = 0 AND TIPO = 'GR' AND GR = '" + GR + "'  AND STATUSNFE NOT IN (4,5,0) ");

                    max = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return max;
                }
            }
            catch (Exception ex)
            {
                return max;
            }
        }
        public bool Atualiza_Doc(string Tipo, string dataEmi, int Doc, bool baixa)
        {
            bool query = false;
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("UPDATE SGIPA.TB_BL_GR SET  ");
                    sb.AppendLine(" DATA_FATURADO = TO_DATE('" + dataEmi + "', 'dd/MM/yyyy'), ");
                    
                    
                    if (baixa)
                    {
                        sb.AppendLine(" DATAPAGAMENTO = TO_DATE('" + dataEmi + "', 'dd/MM/yyyy'), ");
                    }

                    sb.AppendLine(" RPS =  1, FATURADO = 1 ");
                    sb.AppendLine(" WHERE  ");
                    sb.AppendLine(" SEQ_GR IN(" + Doc + ") ");

                    query = con.Query<bool>(sb.ToString()).FirstOrDefault();
                    
                    return query;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public string Busca_Serie(string Modelo, int empresaID, string Tipo)
        {
            string DataEspecifica = "00:00:00";
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT SERIE FROM SGIPA.TB_SERIE  WHERE Cod_Empresa =  " + empresaID);

                    if (Tipo == "GR")
                    {
                        sb.AppendLine(" AND TIPO_GR = 1 ");
                    }
                    if (Tipo == "MINUTA")
                    {
                        sb.AppendLine(" AND TIPO_MINUTA = 1 ");
                    }
                    if (Tipo == "MANUAL")
                    {
                        sb.AppendLine(" AND TIPO_MANUAL = 1 ");

                    }
                    if (Tipo == "GRR")
                    {
                        sb.AppendLine(" AND TIPO_REDEX = 1 ");
                    }

                    if (DataEspecifica != "00:00:00")
                    {
                        sb.AppendLine(" AND DATA_INI <= TO_DATE('" + DataEspecifica + "', ,'DD/MM/YYYY')) AND  DATA_FIM >= TO_DATE('" + DataEspecifica + "', ,'DD/MM/YYYY')");
                    }
                    else
                    {
                        sb.AppendLine(" AND DATA_INI <= TO_DATE('" + DateTime.Now + "', ,'DD/MM/YYYY')) AND  DATA_FIM >= TO_DATE('" + DateTime.Now + "', ,'DD/MM/YYYY')");
                    }


                    string query = con.Query<string>(sb.ToString()).FirstOrDefault();

                    return query;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int getEmpresa(int lote)
        {
            int empresa = 0;
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();


                    sb.AppendLine(" select EMPRESA from VW_CALCULoweb ab where ab.LOTE = " + lote);

                    empresa = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return empresa;
                }
            }
            catch (Exception ex)
            {
                return empresa;
            }
        }
        public IntegracaoBaixa Preenche_Cliente(int codCLi)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();


                    sb.AppendLine(" SELECT TIPO_LOGRADOURO as USU_TIPLOGR, ");
                    sb.AppendLine(" LOGRADOURO as ENDCLI,");
                    sb.AppendLine(" NUM_END  as NENCLI,");
                    sb.AppendLine(" COMPLEMENTO_END as CPLEND,");
                    sb.AppendLine(" CIDADE as CIDCLI,");
                    sb.AppendLine(" BAIRRO as BAICLI,");
                    sb.AppendLine(" ESTADO as SIGUFS,");
                    sb.AppendLine(" REPLACE(REPLACE(CEP,'-',''),'_','') as CEPCLI ,");
                    sb.AppendLine(" LOGR_COB as ENDCOB,");
                    sb.AppendLine(" NUM_COB as NENCOB,");
                    sb.AppendLine(" COMPL_COB as CPLCOB,");
                    sb.AppendLine(" CIDADE_COB as CIDCOB,");
                    sb.AppendLine(" BAIRRO_COB as BAICOB,");
                    sb.AppendLine(" ESTADO_COB as ESTCOB,");
                    sb.AppendLine(" REPLACE(REPLACE(CEP_COB,'-',''),'_','') as CEPCOB ,");
                    sb.AppendLine(" CODCLI_SAP AS CODCLI, ");
                    sb.AppendLine(" RAZAO AS NOMCLI, ");
                    sb.AppendLine(" case when to_number(Replace(Replace(Replace(Replace(nvl(CGC,''), '.',''), '/', ''), '-', ''),'_','')) > 0 then 'I' ELSE 'E' END AS TIPMER, ");
                    sb.AppendLine(" case when length(Replace(Replace(Replace(Replace(nvl(CGC,''), '.',''), '/', ''), '-', ''),'_','')) > 11 then 'J' ELSE 'F' END AS TIPCLI, ");
                    sb.AppendLine(" to_number(Replace(Replace(Replace(Replace(nvl(CGC,''), '.',''), '/', ''), '-', ''),'_','')) AS CGCCPF, ");
                    sb.AppendLine(" IE AS INSEST, ");
                    sb.AppendLine(" IBGE FROM ");

                    if (codCLi > 0)
                    {
                        sb.AppendLine(" TB_CAD_PARCEIROS ");
                        sb.AppendLine(" where CODCLI_SAP = " + codCLi);
                    }
                    //else
                    //{
                    //    if (cgc > 0)
                    //    {
                    //        sb.AppendLine(" TB_CAD_PARCEIROS ");
                    //        sb.AppendLine(" where CGC = " + cgc);
                    //    }
                    //    else
                    //    {
                    //        if (cidade != "")
                    //        {
                    //            sb.AppendLine(" TB_CAD_PARCEIROS ");
                    //            sb.AppendLine(" where replace(Cidade,'''','') ='" + cgc);
                    //        }
                    //    }

                    //}
                    

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int Obtem_Id_Nota(int seq_gr, string nfe)
        {
            int id_nota = 0;
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" select max(id) from FATURA.FATURANOTA WHERE CANCELADA = 0 AND TIPO = 'GR' ");
                    sb.AppendLine(" AND ( ID IN(SELECT FATID FROM FATURA.FAT_GR WHERE SEQ_GR in(" + seq_gr + ") OR GR = "+ seq_gr +")   ");

                    id_nota = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return id_nota;
                }
            }
            catch (Exception ex)
            {
                return id_nota;
            }
        }
        public bool updateNotaById(int idNota, int idUsuario)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" UPDATE FATURA.FATURANOTA SET USUARIO_CANCELA= " + idUsuario + ", ");
                    sb.AppendLine(" DT_CANCELADA=SYSDATE, ");
                    sb.AppendLine(" motivo_cancela ='SUBSTITUIÇÃO'  WHERE ");
                    sb.AppendLine(" id =" + idNota);

                    bool query = con.Query<bool>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string valorNota(int seq_gr, string tipo, int servicos)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT sum(A.VALOR+A.DESCONTO+A.ADICIONAL) ITEM, sum(B.TOTAL) IMPOSTO ");
                    sb.AppendLine(" FROM SGIPA.TB_SERVICOS_FATURADOS A LEFT JOIN");
                    sb.AppendLine(" (SELECT SUM(VALOR_IMPOSTO) TOTAL, AUTONUM_SERVICO_FATURADO ");
                    sb.AppendLine("       FROM SGIPA.TB_SERVICOS_FATURADOS_IMPOSTOS GROUP BY AUTONUM_SERVICO_FATURADO ) B ");
                    sb.AppendLine("   ON A.AUTONUM = B.AUTONUM_SERVICO_FATURADO ");
                    sb.AppendLine(" WHERE A.SEQ_GR in(" + seq_gr + ") ");
                    sb.AppendLine(" AND A.AUTONUM IN(" + servicos + ")");


                    string valorNota = con.Query<string>(sb.ToString()).FirstOrDefault();

                    return valorNota;                    
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int obtemEmpresaPatio(int seq_gr)
        {
            int cod_empresa = 0;
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" select pt.cod_empresa ");
                    sb.AppendLine("  From SGIPA.tb_gr_bl gr ");
                    sb.AppendLine("  inner join SGIPA.tb_bl bl on gr.bl=bl.autonum");
                    sb.AppendLine("  inner join OPERADOR.tb_patios pt on bl.patio = pt.autonum ");
                    sb.AppendLine("  where gr.seq_gr in(" + seq_gr + ") ");

                    cod_empresa = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return cod_empresa;
                }
            }
            catch (Exception ex)
            {
                return cod_empresa;
            }
        }
        public int Obtem_RPSNUM(int idNota)
        {
            int idRPS = 0;
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT MAX(RPSNUM) FROM FATURA.rpsfat WHERE where fatseq =  " + idNota);

                    idRPS = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return idRPS;
                }
            }
            catch (Exception ex)
            {
                return idRPS; 
            }
        }
        public string Monta_Insert_Faturanota(
                int cliente, string dtEmissao, string dtVencimento, 
                string numero, string numDoc, string TipoDoc,
                string gr, int tipoFeito, string Desconto, 
                int patio, int usuario, int codEmpresa ,
                int lote, int cli_autonum, string valor, 
                int clienteSapEntrega, int fonteOP, int fonteIpa, 
                int fonteGrp, int fonteParc, int fonteGR, int fonteRedex, 
                int cond_manual, int fpLTL,
                int parceiro, string embarque
            )
        {
            string prestacaoServicos = "PRESTAÇÃO DE SERVIÇOS";
            string codNatureza = "20.01";

            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();


                    sb.AppendLine("SELECT SEQ_FATURANOTA.NEXTVAL AS numero  from DUAL");

                    int idFat = con.Query<int>(sb.ToString()).FirstOrDefault();


                    sb.Clear();

                    sb.AppendLine(" INSERT INTO  FATURA.FATURANOTA ");
                    sb.AppendLine(" ( ");
                    sb.AppendLine("     ID, CLIENTE, DT_EMISSAO, DT_VENCIMENTO, NUMERO, ");
                    sb.AppendLine("     VALOR, EXTENSO, OBS, OBS2, NAT_OPER, COD_OPER, ");
                    sb.AppendLine("     IMPRESSA, TIPO, GR, ");
                    sb.AppendLine("     DOCUMENTO_ORIGEM, TIPO_DOCUMENTO, TIPO_FEITO, ");
                    sb.AppendLine("     VIAGEM, DOLAR, ");
                    sb.AppendLine("     DESCONTO, FILLAL, ");
                    sb.AppendLine("     INTEGRADA, IDDOCUMENTO, USUARIO, DATA_INCLUSAO, ");
                    sb.AppendLine("     AUTONUM_MINUTA, COD_EMPRESA, VERSAO, ");
                    sb.AppendLine("     CLIENTE_SAP, NOMCLI, ENDCOB, CIDCOB, CGCCPF, ");
                    sb.AppendLine("     BAICOB, ESTCOB, CEPCOB, INSEST, ");
                    sb.AppendLine("     NOTA_AC, RAZAO_REPRESENTANTE, ");
                    sb.AppendLine("     CODIBGE, SERIE, CODCPG, LOTE, PARCEIRO, EMBARQUE, ID_FATURA_SB, ");
                    sb.AppendLine("     CLIENTE_SAP_ENTREGA, FPOP, FPIPA, FPGRP, FPPARC, FPGR, FPRED, COND_MANUAL, FPLTL  ");
                    sb.AppendLine(" ) VALUES ( ");

                    sb.AppendLine(" " + idFat + ",");
                    sb.AppendLine(" " + cliente + ",   ");
                    sb.AppendLine("  TO_DATE('" + dtEmissao + "', 'DD/MM/YYYY'), ");
                    sb.AppendLine("  TO_DATE('" + dtVencimento + "', 'DD/MM/YYYY'), ");
                    sb.AppendLine(" " + numero + ", ");
                    //valor nota
                    sb.AppendLine(" '"+ valor +"', ");
                    //valor nota extenso
                    sb.AppendLine(" '"+ valor.ToString().Replace(",", ".") +"', ");

                    sb.AppendLine(" '',  ");
                    sb.AppendLine(" '',  ");
                    sb.AppendLine(" '" + prestacaoServicos + "', ");
                    sb.AppendLine(" '" + codNatureza + "', ");
                    sb.AppendLine(" 0, ");
                    sb.AppendLine(" 'GR', ");
                    sb.AppendLine(" " + gr + ", ");
                    sb.AppendLine(" '" + numDoc + "', ");
                    sb.AppendLine(" '" + TipoDoc + "', ");
                    //Tipo Feito
                    sb.AppendLine("  ");

                    sb.AppendLine(" '',  ");
                    sb.AppendLine(" '',  ");
                    //Desconto 
                    sb.AppendLine("  ");
                    sb.AppendLine(" TO_CHAR('00' || " + patio + "),  ");
                    sb.AppendLine(" 1,  ");
                    sb.AppendLine(" 0,  ");
                    sb.AppendLine(" "+ usuario +",  ");
                    sb.AppendLine(" TO_DATE('" + DateTime.Now.ToString("dd-MM-yyyy") + "', 'DD/MM/YYYY') , ");
                    sb.AppendLine(" " + codEmpresa + ", ");

                    //Versao
                    sb.AppendLine("  ");
                    //CLIENTE_SAP, NOMCLI, ENDCOB, CIDCOB, CGCCPF
                    sb.AppendLine(" ");
                    sb.AppendLine(" ");
                    sb.AppendLine(" ");
                    sb.AppendLine(" ");
                    sb.AppendLine(" ");

                    //BAICOB, ESTCOB, CEPCOB, INSEST,

                    sb.AppendLine(" ");
                    sb.AppendLine(" ");
                    sb.AppendLine(" ");
                    sb.AppendLine(" ");


                    sb.AppendLine(" 0, ");

                    //Razao_Representante 
                    sb.AppendLine("  ");

                    //CODIBGE, SERIE, CODCPG
                    sb.AppendLine(" ");
                    sb.AppendLine(" ");
                    sb.AppendLine(" ");

                    //Lote e parceiro
                    sb.AppendLine(" " + lote + ", ");
                    sb.AppendLine(" " + parceiro + ", ");
                    //Embarque
                    sb.AppendLine("  ");
                    //Cliente SAP Entrega  
                    sb.AppendLine("  ");
                    //Campos FPOP para frente
                    sb.AppendLine(" " + fonteOP + ",  ");
                    sb.AppendLine(" " + fonteIpa + ", ");
                    sb.AppendLine(" " + fonteGrp + ", ");
                    sb.AppendLine(" " + fonteParc + ", ");
                    sb.AppendLine(" " + fonteGR + " ");
                    sb.AppendLine(" " + fonteRedex + " ");
                    sb.AppendLine(" " + cond_manual + "");
                    sb.AppendLine(" " + fpLTL + " ");


                    string query = con.Query<string>(sb.ToString()).FirstOrDefault();

                    return query;                    

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IntegracaoBaixa GetList_CriaNota(string tipo, string documentos)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT COUNT(1) FROM FATURA.TB_CRIA_NOTA ");
                    sb.AppendLine(" WHERE AUTONUM_FAT NOT IN(SELECT ID FROM  FATURA.FATURANOTA WHERE CANCELADA = 1 OR STATUSNFE = 5 ) ");
                    sb.AppendLine(" AND DOCUMENTOS =  '" + documentos + "' AND TTPO =  ");
                    sb.AppendLine(" AND STATUS NOT IN('CONSISTENCIA','ERRO INTEG','LIBERADA') ");

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IntegracaoBaixa GetHoraIni(string tipo, string documentos)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT HORA_INI, USUARIO FROM FATURA.TB_CRIA_NOTA WHERE DOCUMENTOS = '" + documentos + "' AND TIPO ='" + tipo + "' AND STATUS <> 'CONSISTENCIA' ORDER BY HORA_INI DESC ");

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool Insert_CriaNota(string tipo, string documentos, int usuarioID)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" INSERT INTO FATURA.TB_CRIA_NOTA ");
                    sb.AppendLine(" (AUTONUM, TIPO, DOCUMENTOS, HORA_INI, STATUS, USUARIO) VALUES ( ");
                    sb.AppendLine(" FATURA.SEQ_CRIA_NOTA.NEXTVAL, '" + tipo + "', '" + documentos + "', SYSDATE, 'INICIADA', " + usuarioID + " ");

                    bool query = con.Query<bool>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public IntegracaoBaixa GetDadosFaturaGr(int seq_Gr)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" NVL(FPPARC,0) as FPPARC,   ");
                    sb.AppendLine(" NVL(FPGRP, 0) as FPGRP,   ");
                    sb.AppendLine(" NVL(FPIPA, 0) as FPIPA,   ");
                    sb.AppendLine(" PARCEIRO,   ");
                    sb.AppendLine(" SEQ_GR,   ");
                    sb.AppendLine(" LOTE,   ");
                    sb.AppendLine(" NVL(fpgr,0) as fpgr   ");
                    sb.AppendLine(" flag_hubport,   ");
                    sb.AppendLine(" Codcli,  ");
                    sb.AppendLine(" Razao,   ");
                    sb.AppendLine(" CGC,   ");
                    sb.AppendLine(" Cidade_cli,   ");
                    sb.AppendLine(" autonum_cli,   ");
                    sb.AppendLine(" Ind_Codcli,  ");
                    sb.AppendLine(" Ind_Razao,  ");
                    sb.AppendLine(" Ind_CGC,  ");
                    sb.AppendLine(" Ind_Cidade_cli,  ");
                    sb.AppendLine(" num_documento as NUM_DOC,   ");
                    sb.AppendLine(" TIPODOC_DESCRICAO, ");
                    sb.AppendLine(" PATIO ");
                    sb.AppendLine(" Ind_autonum  ");
                    sb.AppendLine(" FROM  ");
                    sb.AppendLine(" FATURA.FATURA_GR ");
                    sb.AppendLine(" WHERE  ");
                    sb.AppendLine(" SEQ_GR = " + seq_Gr);

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool primeiraHub(int seq_gr)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" select nvl(max(tabela_gr),0)   from  SGIPA.TB_GR_BL WHERE SEQ_GR IN(" + seq_gr + ") ");


                    int txtIDContrato = con.Query<int>(sb.ToString()).FirstOrDefault();

                    sb.Clear();

                    sb.AppendLine(" select max(FLAG_HUBPORT)  from Sgipa.tb_listas_precos where autonum=  " + txtIDContrato);

                    int maxHubPort = con.Query<int>(sb.ToString()).FirstOrDefault();

                    sb.Clear();

                    if (maxHubPort == 1)
                    {
                        sb.AppendLine(" SELECT NVL(MAX(G.SEQ_GR),0)  FROM SGIPA.TB_GR_BL G INNER JOIN SGIPA.TB_SERVICOS_FATURADOS A ON G.BL=A.BL AND G.SEQ_GR=A.SEQ_GR INNER JOIN  ");
                        sb.AppendLine(" SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS B ON A.SERVICO=B.SERVICO WHERE G.STATUS_GR IN('GE'  ,'IM') AND B.LISTA= " + txtIDContrato);
                        sb.AppendLine(" And G.SEQ_GR In(" + seq_gr + ") And NVL(B.FLAG_COBRAR_NVOCC,0)=1 ");

                        int maxSeqGr = con.Query<int>(sb.ToString()).FirstOrDefault();

                        if (maxSeqGr > 0)
                        {
                            return true;
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public string obtemCodeTipoDoc(string tipoDoc)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("  SELECT CODE FROM SGIPA.TB_TIPOS_DOCUMENTOS WHERE DESCR = '" + tipoDoc + "' ");

                    string code = con.Query<string>(sb.ToString()).FirstOrDefault();

                    return code;                   
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
        #region consistencias GR
        public int countGRRPS(int seq_gr)
        {
            int count = 0;

            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();


                    sb.AppendLine(" SELECT COUNT(1) FROM SGIPA.TB_GR_BL WHERE SEQ_GR IN (" + seq_gr + ") AND NVL(RPS, 0) = 1 ");

                    count = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return count;
                }
            }
            catch (Exception ex)
            {
                return count;
            }
        }
        public int countGRDOC(int seq_gr)
        {
            int count = 0;

            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();


                    sb.AppendLine(" SELECT COUNT(1) FROM SGIPA.TB_BL WHERE AUTONUM IN (");
                    sb.AppendLine(" select BL FROM SGIPA.TB_GR_BL WHERE SEQ_GR IN(" + seq_gr + ")");
                    sb.AppendLine(" ) AND NVL(RPS, 0) = 1 ");
                    sb.AppendLine(" And (NVL(NUM_DOCUMENTO,' ') = ' ' OR NUM_DOCUMENTO = '') ");

                    count = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return count;
                }
            }
            catch (Exception ex)
            {
                return count;
            }
        }
        public int countGRValorZerado(int seq_gr)
        {
            int count = 0;

            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();


                    sb.AppendLine(" SELECT COUNT(1) FROM SGIPA.TB_GR_BL WHERE SEQ_GR IN (" + seq_gr + ") AND NVL(VALOR_GR,0) = 0 ");

                    count = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return count;
                }
            }
            catch (Exception ex)
            {
                return count;
            }
        }
        public int countGRDocEmBranco(int seq_gr)
        {
            int count = 0;

            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();


                    sb.AppendLine(" SELECT COUNT(1) FROM SGIPA.TB_BL BL WHERE AUTONUM IN (");
                    sb.AppendLine(" INNER JOIN SGIPA.TB_TIPOS_DOCUMENTOS TP ON BL.TIPO_DOCUMENTO = TP.CODE ");
                    sb.AppendLine(" And (NVL(TP.DESCR,' ') = ' ' OR TP.DESCR = '') ");
                    sb.AppendLine(" WHERE BL.AUTONUM IN(SELECT BL FROM SGIPA.TB_GR_BL WHERE SEQ_GR IN(" + seq_gr + "))  ");

                    count = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return count;
                }
            }
            catch (Exception ex)
            {
                return count;
            }
        }
        public string obtemEmbarque(int seq_gr, int parceiro)
        {
            try
            {
                string embarque = "";
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT  NVL(DIA_FAT,0) As DIA_FAT FROM SGIPA.TB_CAD_PARCEIROS WHERE AUTONUM =  " + parceiro);

                    int dia_fat = con.Query<int>(sb.ToString()).FirstOrDefault();

                    sb.Clear();

                    if (dia_fat > 0)
                    {
                        sb.AppendLine(" SELECT Embarque as EMBARQUE FROM SGIPA.TB_BOSCH WHERE AUTONUM_BL IN(SELECT DISTINCT BL FROM SGIPA.TB_GR_BL WHERE SEQ_GR In(" + seq_gr + ") ");

                        embarque = con.Query<string>(sb.ToString()).FirstOrDefault();


                    }
                    return embarque.Substring(0, 9);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool atualizaCODCLI(int codCli, int autonum)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" UPDATE SGIPA.tb_cad_parceiros SET codcli = " + codCli + " WHERE autonum =  " + autonum);

                    con.Query<bool>(sb.ToString()).FirstOrDefault();

                    sb.Clear();

                    sb.AppendLine(" UPDATE SGIPA.tb_cad_parceiros SET CODCLI_FATURA = " + codCli + " WHERE autonum =  " + autonum);

                    con.Query<bool>(sb.ToString()).FirstOrDefault();


                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region envio email 
        public IntegracaoBaixa GetDadosUsuarioImagem(int lote)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" select id_user, AUTONUM from SGIPA.TB_IMAGEM_PAG_GR where lote =" + lote);

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool GetUpdateTbImagem(int NumeroDocumento, int idImagem)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" UPDATE SGIPA.TB_IMAGEM_PAG_GR SET SEQ_GR = " + NumeroDocumento + " WHERE AUTONUM = " + idImagem);

                    bool query = con.Query<bool>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public IntegracaoBaixa GetDadosUserInternet(int id)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT  iusnome , iusemail from INTERNET.TB_INT_USER  WHERE iusid = " + id);

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IntegracaoBaixa GetEmailRecusa(int lote)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder Sb = new StringBuilder();

                    Sb.AppendLine(" Select DISTINCT EMAIL_RECUSA_IMAGEM, PATIO from SGIPA.TB_BL WHERE AUTONUM IN (" + lote + " ) ");

                    var query = con.Query<IntegracaoBaixa>(Sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IntegracaoBaixa obtemDirEmail()
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(" SELECT dir_email FROM SGIPA.TB_parametros_sistema  ");

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IntegracaoBaixa GetTipoDocumento(int lote)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" Select a.num_documento , t.descr ");
                    sb.AppendLine(" from SGIPA.TB_BL a  ");
                    sb.AppendLine(" left join SGIPA.TB_Tipos_Documentos b on ");
                    sb.AppendLine(" a.tipo_documento = t.code  ");
                    sb.AppendLine(" where ");
                    sb.AppendLine(" a.autonum =  " + lote);

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IEnumerable<IntegracaoBaixa> GetDadosFAT_GR(int seqGr)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT F.ID as IDNum, RPS.RPSNUM FROM  ");
                    sb.AppendLine(" FATURA.FAT_GR AMR ");
                    sb.AppendLine(" INNER JOIN FATURA.FATURANOTA F On AMR.FATID = F.ID  ");
                    sb.AppendLine(" INNER JOIN FATURA.RPSFAT RPS ON F.ID = RPS.FATSEQ  ");
                    sb.AppendLine(" WHERE AMR.SEQ_GR =  " + seqGr);
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" NVL(F.CANCELADA) = 0 ");

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).AsEnumerable();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
        #region emiteNotaGRRPS
        public IntegracaoBaixa GetDadosFaturaNota(int idFat)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" NFE, cliente_sap");
                    sb.AppendLine(" FROM ");
                    sb.AppendLine(" FATURA.FATURANOTA ");
                    sb.AppendLine(" WHERE ID IN(" + idFat + ") ");

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool DeleteTempNota(string maquina)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" DELETE FROM FATURA.TEMP_FATURANOTA WHERE MAQUINA_REDE =  '" + maquina + "' ");

                    bool query = con.Query<bool>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public IntegracaoBaixa GetDadosCodCliSAP(int cod_sap)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT RAZAO AS NOMCLI,  LOGR_COB AS ENDCOB, TIPO_LOGRADOURO AS USU_TIPLOGR, LOGRADOURO AS ENDCLI, NUM_END AS NENCLI, COMPLEMENTO_END AS CPLEND, NUM_COB AS NENCOB, COMPL_COB AS CPLCOB, ");
                    sb.AppendLine(" CIDADE AS CIDCLI, CIDADE_COB AS CIDCOB, CGC AS CGCCPF, BAIRRO AS BAICLI, BAIRRO_COB AS BAICOB, ESTADO AS SIGUFS, ESTADO_COB AS ESTCOB, CEP_COB AS CEPCOB, CEP AS CEPCLI, IM AS INSEST ");
                    sb.AppendLine(" FROM SGIPA.TB_CAD_PARCEIROS WHERE CODCLI_SAP = " + cod_sap);

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;                    
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool GetInsertTEMP_FaturaNota(string maquina)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" INSERT INTO FATURA.TEMP_FATURANOTA ");
                    sb.AppendLine(" maquina_rede, ");
                    sb.AppendLine(" ID, ");
                    sb.AppendLine(" CLIENTE, ");
                    sb.AppendLine(" DT_EMISSAO, ");
                    sb.AppendLine(" DT_VENCIMENTO, ");
                    sb.AppendLine(" NUMERO, ");
                    sb.AppendLine(" EXTENSO, ");
                    sb.AppendLine(" OBS, ");
                    sb.AppendLine(" NAT_OPER, ");
                    sb.AppendLine(" COD_OPER, ");
                    sb.AppendLine(" VALOR, ");
                    sb.AppendLine(" IMPRESSA, ");
                    sb.AppendLine(" TIPO, ");
                    sb.AppendLine(" CANCELADA, ");
                    sb.AppendLine(" DT_CANCELADA, ");
                    sb.AppendLine(" GR, ");
                    sb.AppendLine(" DOCUMENTO_ORIGEM, ");
                    sb.AppendLine(" TIPO_DOCUMENTO, ");
                    sb.AppendLine(" BOLETO, ");
                    sb.AppendLine(" BOLETO_IMPRESSO, ");
                    sb.AppendLine(" TIPO_FEITO, ");
                    sb.AppendLine(" ENTRADA, ");
                    sb.AppendLine(" MINUTA, ");
                    sb.AppendLine(" VIAGEM, ");
                    sb.AppendLine(" DOLAR, ");
                    sb.AppendLine(" OBS2, ");
                    sb.AppendLine(" DESCONTO, ");
                    sb.AppendLine(" DEPTO, ");
                    sb.AppendLine(" FILLAL, ");
                    sb.AppendLine(" CENTROCUSTO, ");
                    sb.AppendLine(" CONTA, ");
                    sb.AppendLine(" INTEGRADA, ");
                    sb.AppendLine(" VIA, ");
                    sb.AppendLine(" versao, ");
                    sb.AppendLine(" codcli, ");
                    sb.AppendLine(" baicob, ");
                    sb.AppendLine(" cepcob, ");
                    sb.AppendLine(" cidcob, ");
                    sb.AppendLine(" endcob, ");
                    sb.AppendLine(" estcob, ");
                    sb.AppendLine(" nomcli, ");
                    sb.AppendLine(" cgccpf, ");
                    sb.AppendLine(" insest, ");
                    sb.AppendLine(" RAZAO_REPRESENTANTE ");
                    sb.AppendLine(" ) ");
                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" " + maquina + " AS maquinarede  ");
                    sb.AppendLine(" ID, ");
                    sb.AppendLine(" CLIENTE, ");
                    sb.AppendLine(" DT_EMISSAO, ");
                    sb.AppendLine(" DT_VENCIMENTO, ");
                    sb.AppendLine(" NUMERO, ");
                    sb.AppendLine(" EXTENSO, ");
                    sb.AppendLine(" OBS, ");
                    sb.AppendLine(" NAT_OPER, ");
                    sb.AppendLine(" COD_OPER, ");
                    sb.AppendLine(" VALOR, ");
                    sb.AppendLine(" IMPRESSA, ");
                    sb.AppendLine(" TIPO, ");
                    sb.AppendLine(" CANCELADA, ");
                    sb.AppendLine(" DT_CANCELADA, ");
                    sb.AppendLine(" GR, ");
                    sb.AppendLine(" DOCUMENTO_ORIGEM, ");
                    sb.AppendLine(" TIPO_DOCUMENTO, ");
                    sb.AppendLine(" BOLETO, ");
                    sb.AppendLine(" BOLETO_IMPRESSO, ");
                    sb.AppendLine(" TIPO_FEITO, ");
                    sb.AppendLine(" ENTRADA, ");
                    sb.AppendLine(" MINUTA, ");
                    sb.AppendLine(" VIAGEM, ");
                    sb.AppendLine(" DOLAR, ");
                    sb.AppendLine(" OBS2, ");
                    sb.AppendLine(" DESCONTO, ");
                    sb.AppendLine(" DEPTO, ");
                    sb.AppendLine(" FILLAL, ");
                    sb.AppendLine(" CENTROCUSTO, ");
                    sb.AppendLine(" CONTA, ");
                    sb.AppendLine(" INTEGRADA, ");

                    bool query = con.Query<bool>(sb.ToString()).FirstOrDefault();

                    return query;

                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string GetDespachanteID(int id)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT DESP.AUTONUM DESPACHANTE FROM FATURA.FATURANOTA FAT ");
                    sb.AppendLine(" JOIN SGIPA.tb_gr_bl gr ");
                    sb.AppendLine(" On CASE WHEN INSTR(FAT.GR,',') > 0 THEN SUBSTR(FAT.GR,1, INSTR(FAT.GR,',') - 1) ELSE FAT.GR END = GR.SEQ_GR ");
                    sb.AppendLine(" JOIN SGIPA.tb_bl bl ON gr.bl = bl.autonum ");
                    sb.AppendLine(" LEFT JOIN SGIPA.tb_cad_parceiros desp ");
                    sb.AppendLine(" ON bl.despachante = desp.autonum ");
                    sb.AppendLine(" WHERE ID " + id);

                    string despachante = con.Query<string>(sb.ToString()).FirstOrDefault();

                    return despachante;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string obtemEnderecoEntregaDespImp(int id, string despachante)
        {
            int param = 1;
            bool validaEntrega = true;

            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" ID As AUTONUM_END ");
                    sb.AppendLine(" CLIENTE_SAP as cliente_sap ");
                    sb.AppendLine(" TIPO ");
                    sb.AppendLine(" ENDERECO ");
                    sb.AppendLine(" OPTENTREGA ");
                    sb.AppendLine(" FROM  ");
                    sb.AppendLine(" FATURA.VW_ENDERECO_ENTREGA_DESP_IMP  ");

                    if (validaEntrega)
                    {
                        sb.AppendLine(" AND OPTENTREGA = " + param);
                    }
                    sb.AppendLine(" AND ENDERECO <> '' ");

                    string endereco = con.Query<string>(sb.ToString()).FirstOrDefault();

                    return endereco;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //Começar a partir daqui a fazer o que faltou da parte dos emails pois os dois 
        public string obtemEnderecoEntregaDespachante()
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" AUTONUM_REGRA ");
                    sb.AppendLine(" AUTONUM_SEM_REGRA ");
                    sb.AppendLine(" CLIENTE_SAP ");
                    sb.AppendLine(" ENDERECO ");
                    sb.AppendLine(" ENDERECO_SEM_REGRA ");
                    sb.AppendLine(" ID as AUTONUM_END ");
                    sb.AppendLine(" OPTENTREGA_REGRA ");
                    sb.AppendLine(" OPTENTREGA_SEM_REGRA ");
                    sb.AppendLine(" TIPO ");

                    string query = con.Query<string>(sb.ToString()).FirstOrDefault();

                    return query;

                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public string obtemEnderecoEntrega(int id, int tipo)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" ENDERECO, ENDERECO_SEM_REGRA,  ");                


                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
        //Update na tb_bl quando for feito uma cancelamento
        public string CancelaTituloPix(long numero_titulo)
        {
            try
            {
                using (OracleConnection con  = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    int count = 0;
                   
                            sb.AppendLine(" SELECT count(1) FROM  SGIPA.TB_GR_BL  WHERE NUM_TITULO_PIX  =   " + numero_titulo);

                            count = con.Query<int>(sb.ToString()).FirstOrDefault();

                            if (count==0)
                        {
                            sb.AppendLine(" UPDATE SGIPA.TB_BL SET NUM_PIX_PAGAMENTO = 0, NUM_TITULO_PIX = 0 WHERE NUM_TITULO_PIX  =   " + numero_titulo);
                            con.Query<string>(sb.ToString()).FirstOrDefault();
                            return "000-Titulo cancelado com sucesso";
                         }
                        else
                        {
                            return "001-O título tem GR impressa";

                        }
                    }
            }
            catch(Exception ex)
            {
                return "002-O título não pode ser cancelado"; 
            }
        }
    }
}