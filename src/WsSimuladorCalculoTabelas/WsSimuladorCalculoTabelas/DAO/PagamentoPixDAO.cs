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

                    sb.AppendLine(" SELECT AUTONUM LOTE, SEQ_GR, AUTONUM , CASE WHEN PATIO=5 THEN 2 ELSE 1 END PATIO  FROM SGIPA.TB_BL WHERE NUM_TITULO_PIX =  " + numeroTitulo);

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

        public int Verificacalculo(long lote)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("  select count(1)  from SGIPA.tb_servicos_faturados ");
                    sb.AppendLine(" WHERE NVL(SEQ_GR,0)=0 ");
                    sb.AppendLine(" AND bl = " + lote);
                    return db.Query<int>(sb.ToString()).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int Verificaformapagamento(long lote)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("  select nvl(max(formapagamento),0) from SGIPA.tb_gr_pre_calculo ");
                    sb.AppendLine(" WHERE ");
                    sb.AppendLine(" bl = " + lote);

                    return db.Query<int>(sb.ToString()).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int VerificaReefer(long lote)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Select count(1) from SGIPA.TB_CNTR_BL a inner join SGIPA.DTE_TB_TIPOS_CONTEINER b on a.tipo=b.code inner join SGIPA.TB_AMR_CNTR_BL c on c.cntr=a.autonum  where  b.flag_temperatura=1 and a.flag_terminal=1 and nvl(a.flag_desligado,0)=0 ");
                    sb.AppendLine(" AND c.bl = " + lote);
                    return db.Query<int>(sb.ToString()).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int Verificaliberado(long lote)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Select NVL(MAX(FLAG_PRE_CALCULO),0) from SGIPA.TB_BL ");
                    sb.AppendLine(" WHERE AUTONUM  = " + lote);
                    return db.Query<int>(sb.ToString()).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool inseregr_bl(long seq_gr, long lote, string inicio, string fim,long NumeroTitulo )
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("INSERT INTO SGIPA.TB_GR_BL ");
                    sb.AppendLine("(AUTONUM, ");
                    sb.AppendLine("BL, ");
                    sb.AppendLine("SEQ_GR, ");
                    sb.AppendLine("DT_GR, ");
                    sb.AppendLine("VALOR_GR, ");
                    sb.AppendLine("STATUS_GR, ");
                    sb.AppendLine("FLAG_GR_PAGA, ");
                    sb.AppendLine("VALIDADE_GR, ");
                    sb.AppendLine("DT_BASE_CALCULO, ");
                    sb.AppendLine("Dt_Base_Calculo_Reefer,");
                    sb.AppendLine("GRAU_TABELA_GR, ");
                    sb.AppendLine("TABELA_GR, ");
                    sb.AppendLine("DT_REGISTRO, ");
                    sb.AppendLine("IMPORTADOR_GR, ");
                    sb.AppendLine("DT_FIM_PERIODO, ");
                    sb.AppendLine("FORMA_PAGAMENTO, ");
                    sb.AppendLine("dt_inicio_periodo, ");
                    sb.AppendLine("usuario, ");
                    sb.AppendLine("periodos, ");
                    sb.AppendLine("f_qtd_cntr, ");
                    sb.AppendLine("f_qtd_volumes, ");
                    sb.AppendLine("f_peso_cs, ");
                    sb.AppendLine("DT_INICIO_CALCULO, ");
                    sb.AppendLine("DT_LIBERACAO , NUM_TITULO_PIX ,DT_IMPRESSAO ,USUARIO_IMP,NUM_CHEQUE,MEIODEPAGAMENTO,BANCO,NUMCONTA");
                    sb.AppendLine(") ");
                    sb.AppendLine("select SGIPA.SEQ_GR_BL.NEXTVAL, ");
                    sb.AppendLine("A.BL, ");
                    sb.AppendLine(seq_gr + ", ");
                    sb.AppendLine("sysdate,");
                    sb.AppendLine("V.VALORGR,");
                    sb.AppendLine("'IM', ");
                    sb.AppendLine("1, ");
                    sb.AppendLine("A.VALIDADE_GR,");
                    sb.AppendLine("A.Data_base, ");
                    sb.AppendLine("A.Data_Reefer,");
                    sb.AppendLine("0, ");
                    sb.AppendLine("A.LISTA,");
                    sb.AppendLine("SYSDATE, ");
                    sb.AppendLine("B.IMPORTADOR,");
                    sb.AppendLine("to_DATE('" + fim + "', 'DD/MM/YYYY'), ");
                    sb.AppendLine("A.FORMAPAGAMENTO,");
                    sb.AppendLine("to_DATE('" + inicio + "', 'DD/MM/YYYY'), ");
                    sb.AppendLine("90, ");
                    sb.AppendLine("A.periodos,");
                    sb.AppendLine("0, ");
                    sb.AppendLine("0, ");
                    sb.AppendLine("0, ");
                    sb.AppendLine("A.DT_INICIO_CALCULO, ");
                    sb.AppendLine("B.DT_LIBERACAO ,");
                    sb.AppendLine(NumeroTitulo + " , sysdate, 90,'PIX','GRPX','1','29.136-4'");
                    sb.AppendLine("FROM  SGIPA.TB_GR_PRE_CALCULO  A INNER JOIN SGIPA.TB_BL B ON A.BL=B.AUTONUM ");
                    sb.AppendLine("INNER JOIN (");
                    sb.AppendLine("select bl, sum(valor+desconto+adicional)+sum(nvl(b.valori,0))  valorgr from SGIPA.tb_servicos_faturados a  ");
                    sb.AppendLine("left join (select sum(valor_imposto) valori , autonum_servico_faturado from SGIPA.tb_servicos_faturados_impostos ");
                    sb.AppendLine("group by autonum_servico_faturado) b on a.autonum=b.autonum_servico_faturado");
                    sb.AppendLine("where nvl(seq_gr,0) ="+ seq_gr +"  group by bl) V ON A.BL=V.BL WHERE A.BL=" + lote );
 
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
        public int? UsuarioEmProcessoDeCalculo(int Lote)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT count(1)  ");
                    sb.AppendLine(" FROM ");
                    sb.AppendLine(" SGIPA.TB_BL  ");
                    sb.AppendLine(" WHERE REG_GR>0 AND AUTONUM =  " + Lote);

                    return con.Query<int>(sb.ToString()).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return 1;
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
                    sb.AppendLine(" max(DATA_FINAL) As Data_Final, ");
                    sb.AppendLine(" max(Lista) As Lista ");
                    sb.AppendLine(" FROM ");
                    sb.AppendLine(" SGIPA.tb_gr_pre_calculo ");
                    sb.AppendLine(" WHERE  ");
                    sb.AppendLine("  bl  =  " + lote);

                    var query = db.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int verificaBLSemSaida(int lote)
        {
            try
            {
                using (OracleConnection db = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" count(1)  ");
                    sb.AppendLine(" FROM   ");
                    sb.AppendLine(" SGIPA.tb_bl ");
                    sb.AppendLine(" WHERE  ");
                    sb.AppendLine(" ULTIMA_SAIDA is null ");
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" AUTONUM =  " + lote);

                    return db.Query<int>(sb.ToString()).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return 1;
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
                return 0;
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
                    sb.AppendLine(" USUARIO =  " + usuario + "  ");
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
                    sb.AppendLine(" SGIPA.TB_AMR_CNTR_BL B ");
                    sb.AppendLine(" WHERE ");
                    sb.AppendLine(" B.CNTR = A.AUTONUM ");
                    sb.AppendLine(" AND A.FLAG_TERMINAL = 1 ");
                    sb.AppendLine(" AND BL = " + lote);
                    sb.AppendLine(" AND nvl(seq_gr,0) =0) ");

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

                    sb.AppendLine(" UPDATE SGIPA.TB_Carga_solta SET ");
                    sb.AppendLine(" SEQ_GR =  " + seqGr);
                    sb.AppendLine(" WHERE  ");
                    sb.AppendLine(" FLAG_TERMINAL = 1 ");
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" BL = " + lote);
                    sb.AppendLine(" AND  ");
                    sb.AppendLine(" nvl(SEQ_GR,0) = 0 ");

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
                return 0;
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
                    sb.AppendLine(" SGIPA.TB_CNTR_BL A, ");
                    sb.AppendLine(" SGIPA.TB_AMR_CNTR_BL B ");
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
                    sb.AppendLine(" where nvl(c.seq_gr,0)=0 AND d.seq_gr= " + seqGR);
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

                    sb.AppendLine(" SELECT nvl(MAX(ID),0) FROM FATURA FATURANOTA WHERE CANCELADA = 0 AND TIPO = 'GR' AND GR = '" + GR + "'  AND STATUSNFE NOT IN (4,5,0) ");

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

                    sb.AppendLine("UPDATE SGIPA.TB_GR_BL SET  ");
                    sb.AppendLine(" DATAFATURADO = TO_DATE('" + dataEmi + "', 'dd/MM/yyyy'), ");
                    sb.AppendLine(" DATAPAGAMENTO = TO_DATE('" + dataEmi + "', 'dd/MM/yyyy'), ");
                    sb.AppendLine(" DATADEPOSITO = TO_DATE('" + dataEmi + "', 'dd/MM/yyyy'), ");
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

                    sb.AppendLine(" AND DATA_INI <= TO_DATE('" + DateTime.Now.Date.ToString("dd/MM/yyyy") + "','DD/MM/YYYY') AND  DATA_FIM >= TO_DATE('" + DateTime.Now.Date.ToString("dd/MM/yyyy") + "' ,'DD/MM/YYYY')"); 


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

                   
                    sb.AppendLine("select pt.cod_empresa  From sgipa.tb_bl bl inner join operador.tb_patios pt on bl.patio = pt.autonum  where bl.autonum =" + lote);

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
                        sb.AppendLine(" SGIPA.TB_CAD_PARCEIROS ");
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

                    sb.AppendLine(" select nvl(max(id),0) from FATURA.FATURANOTA WHERE CANCELADA = 0 AND TIPO = 'GR' ");
                    sb.AppendLine(" AND ID IN(SELECT FATID FROM FATURA.FAT_GR WHERE SEQ_GR in(" + seq_gr + ") OR GR = '"+ seq_gr +"')   ");

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

                    sb.AppendLine(" SELECT MAX(RPSNUM) FROM FATURA.rpsfat WHERE  fatseq =  " + idNota);

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
                string tipo, int gr, string Embarque,
                string dtEmissao, string dtVencimento, string obs1, 
                string obs2, string natOper, string codOper, 
                string numDoc, string tipoDoc, int autonumViagemn, 
                string Dolar, int patio, int autonumMin, SapCliente sapCli,
                int notaAC, int autonumCli, int Lote, int Parceiro, 
                string NFESubstituida, string ClienteSAPEntrega, int fonteOP, 
                int fonteIpa, int fonteGRP, int fonteParc, int fonteGR, 
                int codManual, int fonteRedex, int fonteLTL, int CodCli, string valor, 
                int numero, int codEmpresa, int usuario, string serieNF, string servico, 
                int idNotaSub)
        {
            
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    int Tipo_Feito = 1;
                    double Desconto = 0;
                    string endereco = "";
                    string cidade = "";
                    string cgccli = "";
                    int tipoPessoa = 0;
                    string razao_representante = "";
                    string cgccpf = "";
                    string TipCli = "";
                    string CepCli = "";
                    string CGCRep = "";
                    int posicao = 0;
                    string corpo_nota = "";

                   decimal valor_Nota = Valor_Nota(gr.ToString());

                    if(sapCli.CIDCOB.ToUpper() == "SANTOS" || sapCli.CIDCOB.ToUpper() == "" && sapCli.CIDCLI.ToUpper() =="SANTOS" )
                    {
                        Desconto = Valor_Desconto(gr.ToString());
                    }

                    sb.AppendLine(" Select nvl(a.razao_representante_sap,' ') as Razao, nvl(b.cgc,'000.000.000-00') as cgccpf, ");
                    sb.AppendLine(" CASE WHEN LENGTH(Replace(Replace(Replace(Replace(b.CGC, '.', ''), '-', ''), '/', ''), '_', '')) > 11 THEN 'J' ELSE 'F' END AS TIPCLI from  SGIPA.tb_cad_parceiros a left join ");
                    sb.AppendLine(" SGIPA.TB_CAD_PARCEIROS b on a.razao_representante_SAP = b.RAZAO where a.autonum = " + autonumCli);

                    var dataCli = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    razao_representante = dataCli.Razao;
                    cgccpf = dataCli.CGCCPF.ToString();
                    TipCli = dataCli.TIPCLI;

                    sb.Clear();

                    sb.AppendLine("SELECT FATURA.SEQ_FATURANOTA.NEXTVAL AS numero  from DUAL");

                    int idFat = con.Query<int>(sb.ToString()).FirstOrDefault();

                    sb.Clear();


                    sb.AppendLine(" SELECT CODE FROM SGIPA.TB_TIPOS_DOCUMENTOS WHERE DESCR = '" + tipoDoc + "' ");

                    string code = con.Query<string>(sb.ToString()).FirstOrDefault();

                    sb.Clear();

                    #region inserir FATURANOTA

                    sb.AppendLine(" INSERT INTO  FATURA.FATURANOTA ");
                    sb.AppendLine(" ( ");
                    sb.AppendLine("     ID,  ");
                    sb.AppendLine("     CLIENTE, ");
                    sb.AppendLine("     DT_EMISSAO,  ");
                    sb.AppendLine("     DT_VENCIMENTO, ");
                    sb.AppendLine("     NUMERO, ");
                    sb.AppendLine("     VALOR,     ");
                    sb.AppendLine("     EXTENSO,    ");
                    sb.AppendLine("     OBS,   ");
                    sb.AppendLine("     OBS2,   ");
                    sb.AppendLine("     NAT_OPER,   ");
                    sb.AppendLine("     COD_OPER,   ");
                    sb.AppendLine("     IMPRESSA,   ");
                    sb.AppendLine("     TIPO,  ");
                    sb.AppendLine("     GR,   ");
                    sb.AppendLine("     DOCUMENTO_ORIGEM,   ");
                    sb.AppendLine("     TIPO_DOCUMENTO, ");
                    sb.AppendLine("     TIPO_FEITO,  ");
                    sb.AppendLine("     VIAGEM, ");
                    sb.AppendLine("     DOLAR, ");
                    sb.AppendLine("     DESCONTO,  ");
                    sb.AppendLine("     FILLAL, ");
                    sb.AppendLine("     INTEGRADA, ");
                    sb.AppendLine("     IDDOCUMENTO,   ");
                    sb.AppendLine("     USUARIO, ");
                    sb.AppendLine("     DATA_INCLUSAO ,");
                    sb.AppendLine("     AUTONUM_MINUTA,  ");
                    sb.AppendLine("     COD_EMPRESA, ");
                    sb.AppendLine("     VERSAO,  ");
                    sb.AppendLine("     CLIENTE_SAP,  ");
                    sb.AppendLine("     NOMCLI,  ");
                    sb.AppendLine("     ENDCOB, ");
                    sb.AppendLine("     CIDCOB, ");
                    sb.AppendLine("     CGCCPF, ");
                    sb.AppendLine("     BAICOB, ");
                    sb.AppendLine("     ESTCOB, ");
                    sb.AppendLine("     CEPCOB, ");
                    sb.AppendLine("     INSEST, ");
                    sb.AppendLine("     NOTA_AC, ");
                    sb.AppendLine("     RAZAO_REPRESENTANTE, ");

                    sb.AppendLine("     CODIBGE,  ");
                    sb.AppendLine("     SERIE, ");
                    sb.AppendLine("     CODCPG, ");
                    sb.AppendLine("     LOTE, ");
                    sb.AppendLine("     PARCEIRO, ");
                    sb.AppendLine("     EMBARQUE, ");
                    sb.AppendLine("     ID_FATURA_SB, ");
                    sb.AppendLine("     COND_MANUAL, ");
                    sb.AppendLine("    FPLTL ");

                    sb.AppendLine(" ) VALUES ( ");

                    sb.AppendLine(" " + idFat + ",");
                    sb.AppendLine(" '" + CodCli + "',   ");
                    sb.AppendLine("  TO_DATE('" + dtEmissao + "', 'DD/MM/YYYY'), ");
                    sb.AppendLine("  TO_DATE('" + dtVencimento + "', 'DD/MM/YYYY'), ");
                    sb.AppendLine(" " + idFat.ToString("00000000") + ", ");
                    //valor nota
                    sb.AppendLine(" '" + valor_Nota + "', ");
                    //valor nota extenso
                    sb.AppendLine(" '" + valor_Nota.ToString().Replace(",", ".") + "', ");

                    sb.AppendLine(" NULL,  ");
                    sb.AppendLine(" NULL,  ");
                    sb.AppendLine(" '" + natOper + "', ");
                    sb.AppendLine(" '" + codOper + "', ");
                    sb.AppendLine(" 0, ");
                    sb.AppendLine(" 'GR', ");
                    sb.AppendLine(" " + gr + ", ");
                    sb.AppendLine(" '" + numDoc + "', ");
                    sb.AppendLine(" '" + tipoDoc + "', ");

                    //Tipo Feitos
                    sb.AppendLine(" " + Tipo_Feito + ", ");

                    sb.AppendLine(" NULL,  ");
                    sb.AppendLine(" NULL,  ");
                    //Desconto 
                    sb.AppendLine("  " + Desconto + ",  ");
                    sb.AppendLine(" TO_CHAR('00' || " + patio + "),  ");
                    sb.AppendLine(" 1,  ");
                    sb.AppendLine(" 0,  ");
                    sb.AppendLine(" " + usuario + ",  ");
                    sb.AppendLine(" TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'DD/MM/YYYY') , ");
                    sb.AppendLine(" NULL, ");
                    sb.AppendLine(" " + codEmpresa + ", ");

                    //Versao - Serie NF
                    sb.AppendLine(" " + serieNF.Substring(0, 1) + ", ");
                    //CLIENTE_SAP, NOMCLI, ENDCOB, CIDCOB, CGCCPF

                    if (sapCli.CODCLI.ToString() == "")
                    {
                        sb.AppendLine(" '000', ");
                    }
                    else
                    {
                        sb.AppendLine(" '" + sapCli.CODCLI.ToString() + "', ");
                    }

                    sb.AppendLine("'" + sapCli.NOMCLI.Replace("'", "") + "', ");


                    if (sapCli.ENDCOB == "")
                    {
                        endereco = sapCli.USU_TIPLOGR + sapCli.ENDCLI + ", " + sapCli.NENCLI + ", " + sapCli.CPLEND;
                    }
                    else
                    {
                        endereco = sapCli.ENDCOB + ", " + sapCli.NENCOB + ", " + sapCli.CPLCOB;
                    }


                    sb.AppendLine("'" + endereco.Replace("'", "") + "', ");

                    if (sapCli.CIDCOB == "")
                    {
                        cidade = sapCli.CIDCLI;
                    }
                    else
                    {
                        cidade = sapCli.CIDCOB;
                    }
                    sb.AppendLine(" '" + cidade.Replace("'","") + "', ");

                    if (sapCli.TIPCLI.ToUpper() == "J")
                    {
                        tipoPessoa = 1;
                        cgccli = Formata_CGCCPF(  sapCli.CGCCPF,1);
                    }
                    else
                    {
                        tipoPessoa = 2;

                        cgccli = Formata_CGCCPF(  sapCli.CGCCPF,2);
                    }


                    sb.AppendLine("'" + cgccli + "' ,");

                    //BAICOB, ESTCOB, CEPCOB, INSEST,


                    if (sapCli.BAICOB == "")
                    {
                        sb.AppendLine(" '" + sapCli.BAICLI.Replace("'", "") + "', ");
                    }
                    else
                    {
                        sb.AppendLine(" '" + sapCli.BAICOB.Replace("'", "") + "', ");
                    }


                    if (sapCli.EST_COB == "")
                    {
                        sb.AppendLine(" '" + sapCli.SIGUFS + "', ");
                    }
                    else 
                    {
                        sb.AppendLine("  '" + sapCli.EST_COB + "', ");
                    }
                    
                    
                    if (sapCli.CEPCOB == "")
                    {
                        CepCli = sapCli.CEPCLI;
                    }
                    else 
                    {
                        CepCli = sapCli.CEPCOB;
                    }
                    sb.AppendLine(" '" + CepCli + "', ");
                    sb.AppendLine(" '" +  sapCli.INSEST + "', ");
                    sb.AppendLine(" 0, ");

                    //Razao_Representante 

                    if (TipCli == "J")
                    {
                        CGCRep = Formata_CGCCPF(cgccpf, 1);
                        CGCRep = razao_representante + "  - C.G.C.: " + CGCRep;
                    }
                    else 
                    {
                        CGCRep = Formata_CGCCPF(cgccpf, 2);
                        CGCRep = razao_representante + "  - C.P.F.: " + CGCRep;
                    }
                    

                    sb.AppendLine(" '"+ CGCRep +"',  "); ;

                    //CODIBGE, SERIE, CODCPG
                    sb.AppendLine(" " + sapCli.IBGE  + ", ");
                    sb.AppendLine(" '"+ serieNF +"' ,");
                    sb.AppendLine(" 'Pix', ");

                    //Lote e parceiro
                    sb.AppendLine(" " + Lote + ", ");
                    sb.AppendLine(" " + Parceiro + ", ");
                    //Embarque
                    sb.AppendLine(" '" + Embarque + "',  ");
                    //ID_FATURA_SB
                    sb.AppendLine(" 0,  ");

                    sb.AppendLine(" " + codManual + ",  ");

                    sb.AppendLine(" " + fonteLTL + " ");

                    sb.AppendLine(" ) ");

                    con.Query<string>(sb.ToString()).FirstOrDefault();

                    #endregion

                    sb.Clear();

                    posicao = Obtem_Id_Nota(gr, numDoc);


                    var monta_Itens = Monta_Itens_Nota(gr, servico);

                    int i = 0;
                    int j = 2; 

                    foreach (var monta_item in monta_Itens)
                    {
                        i = i + 1; 

                        corpo_nota = corpo_nota + MontaCorpoNota(i, monta_item.DESCR_SERVICO, monta_item.TOTAL);

                        Monta_Insert_Fatura_Item(posicao, i, monta_item.DESCR_SERVICO, monta_item.TOTAL, monta_item.SERVICO,  gr.ToString(), 0, 0, 0, "");
                    }

                    var monta_Itens_IMP = Monta_Itens_Nota_Imp(gr);

                    foreach (var monta_item_imp in monta_Itens_IMP)
                    {
                        i = i + 1;

                        Monta_Insert_Fatura_Item(posicao, i, monta_item_imp.DESCR_SERVICO, monta_item_imp.VALOR_IMPOSTO, 0, gr.ToString(), 1, 0, 0, "");

                        corpo_nota = corpo_nota + MontaCorpoNota(i, servico, valor_Nota);
                    }

                    string query = "";

                    return query;                    

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool carrega(int seq_gr)
        {
            try
            {
                var rsServicos = GetServicoIDs(seq_gr);
                string servico = "";
                int contaItens = 0;
                int contaFatura = 0;
                string wIdServicos = "";
                string corpoNota = "";
                int ICount = 0;
                decimal total = 0;
                decimal Impostos = 0;
                decimal totalIMP = 0;
                decimal valor = 0;

                contaItens = contaItens + 1;

                for (int i = 0; i < rsServicos.Count(); i++)
                {
                    var montaItensNotaIMP = Monta_Itens_Nota_Imp(seq_gr);

                    foreach (var itens in montaItensNotaIMP)
                    {
                        servico = itens.DESCR_SERVICO;
                        valor = itens.VALOR_IMPOSTO;

                        corpoNota = corpoNota + MontaCorpoNota(ICount, servico, valor);

                        ICount = ICount + 1;

                    }

                    var montaItensNota = Monta_Itens_Nota(seq_gr, servico);


                    foreach (var itens in montaItensNotaIMP)
                    {
                        servico = itens.DESCR_SERVICO;
                        valor = itens.VALOR_IMPOSTO;
                        total = itens.TOTAL;
                        Impostos = itens.IMPOSTOS;
                        totalIMP = total + Impostos;

                        if (totalIMP > 0)
                        {
                            corpoNota = corpoNota + MontaCorpoNota(ICount, servico, valor);
                            ICount = ICount + 1;
                        }
                    }

                    var ret = corpoNota.Length;

                    if (ret + rsServicos.Count() > 2000)
                    {
                        var autonumFat = getAutonumServicosFaturados(seq_gr);

                        for (int j = 0; j < autonumFat.Count(); j++)
                        {
                            if (wIdServicos != "")
                            {
                                wIdServicos = wIdServicos + ",";
                            }
                            wIdServicos = wIdServicos + rsServicos.ToList().ToString();
                        }

                        var notaAtu = new NotasGerar();
                        notaAtu.idsServFaturados = wIdServicos;


                    }
                    else
                    {

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string geraIntegracao(string servico, int nfeSubst, string dtEmissao, int patioGR, 
            string cbMeio, bool check, string tituloSap, string dtMov, string conta, string valor, string condicao, 
            SapCliente sapcliente, int id_nota,  int cod_empresa, string serie, string corpoNota, int RPSSubstituida, int gr)
        {
            try
            {

                string estadoCli = sapcliente.EST_COB;
                string cidadeCli = sapcliente.CIDCOB;
                string CGC = sapcliente.CGCCPF.ToString();
                string codOper = "";
                string Tipo_Emp = sapcliente.TIPMER;
                string[] CMD_XML = new string[555];
                string[] CMD_SID = new string[555];
                int filial = 0;
                int I = 0;
                int QTD_I = 0;
                bool substNF = false;
                string GeraRPSFAT = "1";
                string monta_SID_Fecha_Nota = "";
                string monta_SID_Itens = "";
                string Retorno = "";                
                string meioPagamento = "Pix";
                bool nfeSubstituida = false;



                monta_SID_Fecha_Nota = Monta_Sid_FechaNota(serie, "NFE", true, tituloSap, dtEmissao, conta, valor.ToString(), condicao, nfeSubst.ToString());


                var itens = Monta_Itens_Nota(gr, servico);

                int i = 2;
                int j = 0;


                foreach (var itemNF in itens)
                {
                    i = i + 1;

                    if (itemNF.TOTAL + itemNF.IMPOSTOS > 0)
                    {
                        j = j + 1;

                        monta_SID_Itens = Monta_Sid_Itens("GR", cod_empresa, i, serie, itemNF.SERVICO.ToString(), itemNF.DESCR_SERVICO, Convert.ToDouble(itemNF.TOTAL + itemNF.IMPOSTOS), itemNF.CODSER.ToString());

                        if (monta_SID_Itens == "ERRO")
                        {
                            excluiFaturaEItem(id_nota);
                        }
                        else
                        {
                            CMD_XML[j] = monta_SID_Itens;
                        }
                    }
                }

                for (int z =i+1; z < 550; z++)
                {
                    CMD_XML[z] = "";
                }              


                if (Tipo_Emp == "E")
                {
                    codOper = "7949A";
                }
                else
                {
                    if (estadoCli == "SP")
                    {
                        if (patioGR != 0 || patioGR.ToString() != "")
                        {
                            if (cidadeCli.ToUpper() == "SANTOS")
                            {
                                codOper = "5949E";
                            }
                            else
                            {
                                codOper = "5949D";
                            }
                        }
                        else
                        {
                            if (cidadeCli.ToUpper() == "SANTOS")
                            {
                                codOper = "5949B";
                            }
                            else
                            {
                                codOper = "5949A";
                            }
                        }
                    }
                    else
                    {
                        if (patioGR.ToString() != "" || patioGR != 0)
                        {
                            if (patioGR == 4 || patioGR == 6)
                            {
                                codOper = "6949D";
                            }
                            else;
                            {
                                codOper = "6949A";
                            }
                        }
                    }
                }

                string strInstruction = "";
                string strInstructionSAP = "";

                strInstruction = "<sidxml><param retorno='XML'/><sid acao='SID.SRV.ALTEMPFIL'>";
                strInstruction = strInstruction + "<param nome='CodEmp' valor='" + cod_empresa + "'/>";
                strInstruction = strInstruction + "<param nome='CodFil' valor='" + filial + "'/></sid>";

                CMD_XML[550] = strInstruction;



                strInstruction = "<sid acao='SID.NFV.GRAVAR'>";

                strInstruction = strInstruction + "<param nome='CodSnf' valor='" + serie + "'/>";
                strInstruction = strInstruction + "<param nome='CodCli' valor='" + sapcliente.CODCLI + "'/>";
                strInstruction = strInstruction + "<param nome='TNSSER' valor='" + codOper + "'/>";

                strInstruction = strInstruction + "<param nome='CODREP' valor='1'/>";
                strInstruction = strInstruction + "<param nome='USU_GR' valor='90'/>";

                if (dtEmissao == "")
                {
                    strInstruction = strInstruction + "<param nome='DATEMI' valor='" + dtEmissao + "'/>";
                }
                else
                {
                    strInstruction = strInstruction + "<param nome='DATEMI' valor='" + DateTime.Now.ToString("dd/MM/yyyy") + "'/>";
                }

                strInstruction = strInstruction + "<param nome='CodCpg' valor='" + meioPagamento + "'/>";

                strInstruction = strInstruction + "<param nome='NumNfv' valor='@numnfe'/>";

                strInstruction = strInstruction + "</sid>";


                CMD_XML[549] = strInstruction;


                strInstruction = "<sid acao='SID.Nfv.RecalcularParcelas'>";

                strInstruction = strInstruction + "<param nome='CodSnf' valor='" + serie + "'/>";

                strInstruction = strInstruction + "<param nome='NumNfv' valor='@numnfe'/>";


                strInstruction = strInstruction + "</sid>";
                CMD_XML[0] = strInstruction;

                strInstruction = "";
                strInstruction = CMD_XML[550];

                strInstructionSAP = strInstruction + CMD_XML[549];

                strInstruction = strInstruction + CMD_XML[0];




                for (I = 3; I < 548; I++)
                {
                    if (CMD_XML[I] != "")
                    {
                        strInstructionSAP = strInstructionSAP + CMD_XML[I];
                    }
                    else
                    {
                        QTD_I = I - 3;
                        I = 549;
                    }
                }

                strInstruction = strInstruction + CMD_XML[1] + CMD_XML[2];


                int rsSer = getServicoByIDFatura(id_nota);

                if (rsSer != QTD_I)
                {
                    return "Divergencia entre os Itens da Nota Fiscal, Contate o Suporte";
                }
                else
                {
                        if (GeraRPSFAT == "1")
                        {
                            if (!insereRPSFATProc(id_nota, "", strInstruction, strInstructionSAP, RPSSubstituida))
                            {
                                return "Erro ao inserir o registro do RPS";
                            }
                        }
                        else
                        {
                            var par = GetDadosParametro(cod_empresa);
                            var corpoRequest = new WSUnico.GeraNotaFiscalDeVendaSAPRequestBody(id_nota, strInstruction, corpoNota, strInstructionSAP);
                            var requisicao = new WSUnico.GeraNotaFiscalDeVendaSAPRequest(corpoRequest);
                            WSUnico.ServiceSoap meuServico;

                            if (par.WS_NFE != "")
                            {
                                meuServico = new WSUnico.ServiceSoapClient("ServiceSoap3", par.WS_NFE);
                            }
                            else
                            {
                                meuServico = new WSUnico.ServiceSoapClient();
                            }

                            Retorno = meuServico.GeraNotaFiscalDeVendaSAP(requisicao).Body.GeraNotaFiscalDeVendaSAPResult.ToString();
                        }
                    }

                for (I = 0; I < 550; I++)
                {
                    CMD_XML[I] = "";
                    CMD_SID[I] = "";
                }

                

                return "";

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool UpdateCodTns(string codOper, int PosFat)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" Update FATURA.faturanota set codtns ='" + codOper + "' where id =" + PosFat);

                    bool statusFat = con.Query<bool>(sb.ToString()).FirstOrDefault();

                    return statusFat;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public int getServicoByIDFatura(int pos)
        {
            int servico = 0;

            try
            {                
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" Select count(1) from fatura.fatura_item where imposto = 0 and nvl(valor,0) > 0 and idfatura = " + pos);

                    servico = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return servico;
                }
            }
            catch (Exception ex)
            {
                return servico;
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
                    sb.AppendLine(" NVL(fpgr,0) as fpgr  , ");
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
                    sb.AppendLine(" TIPO_DOCUMENTO TIPODOC_DESCRICAO, ");
                    sb.AppendLine(" PATIO ,");
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
        public bool insereRPSFATProc(int id, string str, string corpo, string strSAP, int subst)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    {
                        using (OracleCommand cmd = new OracleCommand("FATURA.PROC_CHRONOS_RPS", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new OracleParameter
                            {
                                OracleDbType = OracleDbType.Long,
                                Direction = ParameterDirection.Input,
                                Value = id
                            });
                            cmd.Parameters.Add(new OracleParameter
                            {
                                OracleDbType = OracleDbType.Clob,
                                Direction = ParameterDirection.Input,
                                Value = str
                            });
                            cmd.Parameters.Add(new OracleParameter
                            {
                                OracleDbType = OracleDbType.Clob,
                                Direction = ParameterDirection.Input,
                                Value = corpo
                            });
                            cmd.Parameters.Add(new OracleParameter
                            {
                                OracleDbType = OracleDbType.Varchar2,
                                Direction = ParameterDirection.Input,
                                Value = strSAP
                            });
                            cmd.Parameters.Add(new OracleParameter
                            {
                                OracleDbType = OracleDbType.Int32,
                                Direction = ParameterDirection.Input,
                                Value = subst
                            });

                            cmd.Parameters.Add(new OracleParameter
                            {
                                OracleDbType = OracleDbType.Varchar2,
                                Direction = ParameterDirection.Output
                            });

                            con.Open();

                            OracleDataReader retorno = cmd.ExecuteReader();


                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool UpdateFatura(int id)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" UPDATE FATURA.FATURANOTA SET STATUSNFE = 1 WHERE ID = " + id);

                    bool update_Fat = con.Query<bool>(sb.ToString()).FirstOrDefault();

                    return update_Fat;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public Empresa GetDadosParametro(int cod_empresa)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append(" SELECT ID, NOME_EMPRESA, COD_EMPRESA, JUROS, QTD_DIAS_ALERTA, DIA_FAT, BASE_SAPIENS, DIR_LOG, LETRA_NOTA_GR, LETRA_NOTA_MINUTA, LETRA_NOTA_REDEX, SERIE_IPA, SERIE_OPE, SERIE_REDEX, SERVIDOR_SID, LOGIN_SID, SENHA_SID, WS_NFE, WS_BOLETO_SAP, MSG_EMAIL_NFE, EMAIL_COPIA, EMAIL_RECUSA_IMG, END_EMAIL_ENVIO FROM  SGIPA.PARAMETRO WHERE COD_EMPRESA= " + cod_empresa);

                    var query = con.Query<Empresa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch(Exception ex)
            {
                return null;
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
        public IEnumerable<IntegracaoBaixa> GetServicoIDs(int seq_gr)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("  SELECT DISTINCT SERVICO FROM SGIPA.TB_SERVICOS_FATURADOS WHERE SEQ_GR IN(" + seq_gr + ")  ");
                    sb.AppendLine("  AND (NVL(VALOR,0) + NVL(ADICIONAL,0) + NVL(DESCONTO,0) ) > 0 ");

                    var servico = con.Query<IntegracaoBaixa>(sb.ToString()).AsEnumerable();

                    return servico; 
                }
            }
            catch (Exception Ex)
            {
                return null;
            }
        }
        public IEnumerable<IntegracaoBaixa> Monta_Itens_Nota_Imp(int gr )
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(" SELECT SUM(A.VALOR_IMPOSTO) AS VALOR_IMPOSTO, A.AUTONUM_IMPOSTO , C.DESCRICAO AS DESCR_SERVICO  ");
                    sb.AppendLine(" FROM SGIPA.TB_SERVICOS_FATURADOS_IMPOSTOS A INNER JOIN ");
                    sb.AppendLine(" (SELECT AUTONUM, SERVICO, SEQ_GR FROM SGIPA.TB_SERVICOS_FATURADOS) B  ON A.AUTONUM_SERVICO_FATURADO = B.AUTONUM ");
                    sb.AppendLine(" JOIN SGIPA.TB_CAD_IMPOSTOS C ON A.AUTONUM_IMPOSTO = C.AUTONUM ");
                    sb.AppendLine(" WHERE B.SEQ_GR IN(" + gr + ") ");
                    sb.AppendLine(" GROUP BY A.AUTONUM_IMPOSTO, C.DESCRICAO ");
                    sb.AppendLine(" ORDER BY C.DESCRICAO, A.AUTONUM_IMPOSTO ");


                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).AsEnumerable();

                    return query;                     
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IEnumerable<IntegracaoBaixa> Monta_Itens_Nota(int gr, string servicos)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT SUM(A.VALOR + A.ADICIONAL + A.DESCONTO) AS TOTAL, SUM(C.IMPOSTO) AS IMPOSTOS, ");
                    sb.AppendLine("  A.SERVICO, replace(B.DESCR,'''','') AS DESCR_SERVICO, B.CODSER_SAP ");
                    sb.AppendLine(" FROM SGIPA.TB_SERVICOS_FATURADOS A JOIN  ");
                    sb.AppendLine(" SGIPA.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM LEFT JOIN ");
                    sb.AppendLine(" (SELECT AUTONUM_SERVICO_FATURADO, SUM(VALOR_IMPOSTO) AS IMPOSTO " );
                    sb.AppendLine(" FROM SGIPA.TB_SERVICOS_FATURADOS_IMPOSTOS ");
                    sb.AppendLine(" GROUP BY AUTONUM_SERVICO_FATURADO) C   ");
                    sb.AppendLine(" ON A.AUTONUM = C.AUTONUM_SERVICO_FATURADO ");
                    sb.AppendLine(" WHERE  a.valor>0 and A.SEQ_GR IN(" + gr + ") ");

                    if (servicos != "")
                    {
                        sb.AppendLine(" AND A.SERVICO IN(" + servicos + ") ");
                    }

                    sb.AppendLine(" GROUP BY A.SERVICO, B.DESCR, B.CODSER_SAP ");
                
                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).AsEnumerable();

                    return query;
                }
            }
            catch (Exception ex)
            { 
                return null;
            }
        }
        public IEnumerable<IntegracaoBaixa> getAutonumServicosFaturados(int gr )
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT AUTONUM FROM SGIPA.TB_SERVICOS_FATURADOS WHERE SEQ_GR IN(" + gr + ") ");
                  
                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).AsEnumerable();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string obtemTituloSapiens(int codEmpresa)
        {
            try
            {
                {
                    using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.AppendLine(" SELECT TIT_SAPIENS FROM SGIPA.TB_SERIE WHERE Cod_Empresa =  " + codEmpresa);
                        sb.AppendLine("  AND TIPO_GR = 1 AND DATA_INI <= TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "', 'DD/MM/YYYY') ");
                        sb.AppendLine("  AND  DATA_FIM >= TO_DATE('" + DateTime.Now.ToString("dd/MM/yyyy") + "','DD/MM/YYYY')  ");

                        string titSapiens = con.Query<string>(sb.ToString()).FirstOrDefault();

                        return titSapiens;
                    }   
                }
            }
            catch (Exception ex)
            {
                return null;            
            }
        }
        public int GetParceiroGR(int seq_Gr)
        {
            int query = 0;

            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT  ");
                    sb.AppendLine(" PARCEIRO,   ");                    
                    sb.AppendLine(" WHERE  ");
                    sb.AppendLine(" SEQ_GR = " + seq_Gr);

                    query = con.Query<int>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch (Exception ex)
            {
                return query;
            }
        }
        public string MontaCorpoNota(int item, string servico, decimal valor)
        {
            try
            {
                string strS = "";

                strS = "| Item:" + item + " - ";
                strS = strS + servico.ToUpper() + " - ";
                strS = strS + "R$ " + valor.ToString().Replace(",", ".");


                return strS;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public IntegracaoBaixa ObtemStatusNota(int id)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT A.NFE, A.STATUSNFE, C.CRITICA, C.RPSNUM ");
                    sb.AppendLine(" FROM FATURA.FATURANOTA A LEFT JOIN ");
                    sb.AppendLine(" (SELECT MAX(RPSNUM) AS RPSNUM, FATSEQ ");
                    sb.AppendLine(" FROM FATURA.RPSFAT GROUP BY FATSEQ) B ON A.ID = B.FATSEQ LEFT JOIN ");
                    sb.AppendLine(" FATURA.RPSFAT C ON C.RPSNUM = B.RPSNUM AND C.FATSEQ = B.FATSEQ ");
                    sb.AppendLine(" WHERE  A.ID =" + id);

                    var query = con.Query<IntegracaoBaixa>(sb.ToString()).FirstOrDefault();

                    return query;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        #endregion
        #region monta_Sid
        public string Monta_Sid_FechaNota(string Serie, string Tipo,  bool baixaTitulo, string tituloSap, string dtNow,
            string conta, string valor, string condicao, string nfeSubstituida)
        {
            string strS = "";

            try
            {
                strS = "<sid acao='SID.Nfv.Fechar'>";
                strS = strS + "<param nome='CodSnf' valor='" + Serie + "'/>";
                strS = strS + "<param nome='NumNfv' valor='@numnfe'/></sid>";

                if (baixaTitulo)
                {
                    strS = strS + "<sid acao='SID.Tcr.Baixar'>";
                    strS = strS + "<param nome='NumTit' valor='@NUMNFE" + tituloSap + "'/>";
                    strS = strS + "<param nome='CodTpt' valor='DUP'/>";
                    strS = strS + "<param nome='CodTns' valor='90356'/>";
                    strS = strS + "<param nome='DatMov' valor='" + dtNow + "'/>";
                    strS = strS + "<param nome='DatPgt' valor='" + dtNow + "'/>";
                    strS = strS + "<param nome='NumCco' valor='" + conta + "'/>";
                    strS = strS + "<param nome='VlrMov' valor='" + valor.Replace(".", "").Replace("R$", "").ToUpper() + "'/>";
                    strS = strS + "<param nome='NumDoc' valor='@NUMNFE'/>";
                    strS = strS + "<param nome='CodFpg' valor='" + condicao + "'/>";
                    strS = strS + "<param nome='TnsBxa' valor='90624'/>";
                    strS = strS + "</sid>";
                }

                
                strS = strS + "</sidxml>";

                return strS;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string Monta_Sid_Itens(string Tipo, int Empresa, int Item, string Serie, string codServico, string Servico, double preco, string CodigoSer)
        {
            string strS = "";
            string SisFin = "SAP";
            string Monta_Sid_Itens = "";

            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    strS = "<sid acao='SID.Nfv.GravarServico'>";
                    strS = strS + "<param nome='CodSnf' valor='" + Serie + "'/>";
                    strS = strS + "<param nome='SeqIsv' valor='" + Item + "'/>";

                    
                        if (CodigoSer != "")
                        {

                            strS = strS + "<param nome='CodSer' valor='" + CodigoSer + "'/>";
                        }
                        else
                        {
                            Monta_Sid_Itens = "ERRO: O Serviço " + Servico.ToUpper() + " - [" + codServico + "] não possui codigo de Serviço relacionado ao SAP - Operação Cancelada";
                            return Monta_Sid_Itens;    
                        }
                   
                    strS = strS + "<param nome='PreUni' valor='" + preco + "'/>";
                    strS = strS + "<param nome='QtdFat' valor='1'/>";
                    strS = strS + "<param nome='NumNfv' valor='@numnfe'/></sid>";

                    return strS;
                }

                
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region consistencias GR

        public string consistenciaGR(int seq_gr, int parceiro)
        {
            int grRPS = 0;
            int grDoc = 0;
            int grZerada = 0;
            int grEmBranco = 0;            
            string embarque = "";
            int docs = 0;
            string mensagem = "";
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();


                    sb.AppendLine(" SELECT COUNT(1) FROM SGIPA.TB_GR_BL WHERE SEQ_GR IN (" + seq_gr + ") AND NVL(RPS, 0) = 1 ");

                    grRPS = con.Query<int>(sb.ToString()).FirstOrDefault();

                    if (grRPS > 0)
                    {
                        mensagem = "GR já possui RPS/Faturamento gerado";
                    }

                    sb.Clear();

                    sb.AppendLine(" SELECT COUNT(1) FROM SGIPA.TB_BL WHERE AUTONUM IN (");
                    sb.AppendLine(" select BL FROM SGIPA.TB_GR_BL WHERE SEQ_GR IN(" + seq_gr + ")");
                    sb.AppendLine("  AND NVL(RPS, 0) = 1) ");
                    sb.AppendLine(" And (NVL(NUM_DOCUMENTO,' ') = ' ' OR NUM_DOCUMENTO = '') ");

                    grDoc = con.Query<int>(sb.ToString()).FirstOrDefault();

                    if(grDoc > 0)
                    {
                        mensagem = "O Documento da GR [" + seq_gr + "] está em branco"; 
                    }

                    sb.Clear();

                    sb.AppendLine(" SELECT COUNT(1) FROM SGIPA.TB_GR_BL WHERE SEQ_GR IN (" + seq_gr + ") AND NVL(VALOR_GR,0) = 0 ");

                    grZerada = con.Query<int>(sb.ToString()).FirstOrDefault();


                    if (grZerada > 0)
                    {
                        mensagem = "Uma ou mais GRs com o valor zerado";
                    }


                    sb.Clear();

                    sb.AppendLine(" SELECT COUNT(1) FROM SGIPA.TB_BL BL ");
                    sb.AppendLine(" INNER JOIN SGIPA.TB_TIPOS_DOCUMENTOS TP ON BL.TIPO_DOCUMENTO = TP.CODE ");
                    sb.AppendLine(" WHERE BL.AUTONUM IN(SELECT BL FROM SGIPA.TB_GR_BL WHERE SEQ_GR IN(" + seq_gr + "))  ");
                    sb.AppendLine(" And (NVL(TP.DESCR,' ') = ' ' OR TP.DESCR = '') ");

                    grEmBranco = con.Query<int>(sb.ToString()).FirstOrDefault();

                    if (grEmBranco > 0)
                    {
                        mensagem = "O Tipo Do Documento da Gr[" + seq_gr + "] esta em branco";
                    }

                    sb.Clear();

                    sb.AppendLine(" SELECT  NVL(DIA_FAT,0) As DIA_FAT FROM SGIPA.TB_CAD_PARCEIROS WHERE AUTONUM =  " + parceiro);

                    int dia_fat = con.Query<int>(sb.ToString()).FirstOrDefault();

                    sb.Clear();

                    if (dia_fat > 0)
                    {
                        IntegracaoBaixa.validaEmbarque = true;
                        sb.AppendLine(" SELECT Embarque as EMBARQUE FROM SGIPA.TB_BOSCH WHERE AUTONUM_BL IN(SELECT DISTINCT BL FROM SGIPA.TB_GR_BL WHERE SEQ_GR In(" + seq_gr + ") ");

                        embarque = con.Query<string>(sb.ToString()).FirstOrDefault();
                    }
                    else
                    {
                        IntegracaoBaixa.validaEmbarque = false;
                    }

                    if (IntegracaoBaixa.validaEmbarque == true && embarque == "")
                    {
                        mensagem = "Não é possível gerar a nota fiscal, BL sem referência Do cliente cadastrado.";
                    }

                    sb.Clear();

                    sb.AppendLine(" Select COUNT(DISTINCT NUM_DOCUMENTO) FROM SGIPA.TB_BL ");
                    sb.AppendLine(" WHERE AUTONUM IN (Select BL FROM SGIPA.TB_GR_BL WHERE SEQ_GR In(" + seq_gr + ")) ");


                    docs = con.Query<int>(sb.ToString()).FirstOrDefault();

                    if (docs > 1)
                    {
                        mensagem = "Para agrupar GRs é necessário ser o mesmo documento de origem";
                    }


                    return mensagem;
                }
            }
            catch (Exception ex)
            {
                return "Erro ao verificar consistenciasGR";
            }
        }
        #endregion
        public string GetEmbarque(int seq_gr)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" SELECT nvl(max(Embarque),' ') as EMBARQUE FROM SGIPA.TB_BOSCH WHERE AUTONUM_BL IN(SELECT DISTINCT BL FROM SGIPA.TB_GR_BL WHERE SEQ_GR In(" + seq_gr + ")) ");

                    string embarque = con.Query<string>(sb.ToString()).FirstOrDefault();

                    return embarque;
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
        
        public decimal Valor_Nota(string seq_gr)
        {
            decimal Valor = 0;
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("SELECT sum(A.VALOR+A.DESCONTO+A.ADICIONAL) + nvl(sum(B.TOTAL),0) ");
                    sb.AppendLine(" FROM SGIPA.TB_SERVICOS_FATURADOS A LEFT JOIN ");
                    sb.AppendLine(" (SELECT SUM(VALOR_IMPOSTO) TOTAL, AUTONUM_SERVICO_FATURADO  ");
                    sb.AppendLine("       FROM SGIPA.TB_SERVICOS_FATURADOS_IMPOSTOS GROUP BY AUTONUM_SERVICO_FATURADO ) B  ");
                    sb.AppendLine("   ON A.AUTONUM = B.AUTONUM_SERVICO_FATURADO  ");
                    sb.AppendLine(" WHERE A.SEQ_GR in(" + seq_gr  + ")  ");

                    Valor = con.Query<decimal>(sb.ToString()).FirstOrDefault();

                    return Valor;
                }
            }
            catch (Exception ex)
            {
                return Valor;
            }
        }
        public double Valor_Desconto(string seq_gr )
        {
            double valor = 0;
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("  Select A.autonum , A.bl , A.seq_gr , nvl(round(((A.valor + A.desconto + A.adicional) * (NL(b.taxa, 0) / 100)), 2), 0) ISS from sgipa.tb_servicos_faturados  A");
                    sb.AppendLine(" Inner Join ");
                    sb.AppendLine(" (select seq_gr, nvl(max(case when NVL(B.FLAG_ISENTO_IMPOSTO,0)=1 then 0 else taxa end),0) taxa ");
                    sb.AppendLine(" From sgipa.tb_GR_BL A  inner Join sgipa.tb_cad_parceiros b on a.importador_gr=b.autonum ");
                    sb.AppendLine(" inner  Join  sgipa.tb_cad_impostos c on 1=1  where SEQ_GR IN(" + seq_gr + ") And c.iss=1 GROUP BY SEQ_GR ) B ");
                    sb.AppendLine(" On A.SEQ_GR=B.SEQ_GR ");
                    sb.AppendLine(" WHERE A.SEQ_GR IN(" + seq_gr + ") ");
                   

                    valor = con.Query<double>(sb.ToString()).FirstOrDefault();

                    return valor;

                }
            }
            catch (Exception ex)
            {
                return valor;  
            }
        }
        public bool Monta_Insert_Fatura_Item(int posicao, int countI, string descricao_servico, decimal total, int servico,  string doc, int imposto, double valor, int quantidade, string SD)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" INSERT INTO FATURA.FATURA_ITEM ");
                    sb.AppendLine(" ( ");
                    sb.AppendLine(" ID, IDFATURA,ITEM,DESCRICAO,QTDE,VALOR,SERVICO,GRS, IMPOSTO, RATE, SD ");
                    sb.AppendLine(" ) VALUES ( ");
                    sb.AppendLine(" fatura.SEQ_FATURA_ITEM.NEXTVAL, ");
                    sb.AppendLine(" " + posicao + ", ");
                    sb.AppendLine(" " + countI + ", ");
                    sb.AppendLine(" '" + descricao_servico + "', ");
                    if (quantidade == 0)
                    {
                        sb.AppendLine(" 0, ");
                    }
                    else
                    {
                        sb.AppendLine(" " + quantidade + ", ");
                    }

                    sb.AppendLine(" " + total.ToString().Replace(",", ".") + ", ");
                    sb.AppendLine(" " + servico + ", ");
                    sb.AppendLine(" " + doc + ", ");
                    sb.AppendLine(" " + imposto + ", ");
                    sb.AppendLine(" " + valor.ToString().Replace(",", ".") + ", ");
                    sb.AppendLine(" '" + SD + " ' ");
                    sb.AppendLine(" ) ");


                    bool ins = con.Query<bool>(sb.ToString()).FirstOrDefault();

                    return ins;

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool excluiFaturaEItem(int pos)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(" DELETE FROM FATURA.FATURANOTA WHERE id = " + pos);

                    con.Query<bool>(sb.ToString()).FirstOrDefault();

                    sb.Clear();

                    sb.AppendLine(" DELETE FROM FATURA.FATURA_ITEM WHERE IDFATURA = " + pos);

                    con.Query<bool>(sb.ToString()).FirstOrDefault();

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string Formata_CGCCPF(string doc, int? tipo)
        {
            try
            {
                string Formata = doc;
                if (doc == null)
                {
                    doc = "1";
                }
                if (tipo == null)
                {
                    tipo = 0;
                }

                if (tipo == 1)
                {
                    if (doc.Length == 14)
                    {
                     //   Formata = Formata.Substring(doc.Length - 1);
                        Formata = doc;
                        Formata = Formata.Substring(0, 2) + "." + Formata.Substring(2, 3) + "." + Formata.Substring(5, 3) + "/" + Formata.Substring(8, 4) + "-" + Formata.Substring(12, 2);
                    }

                    return Formata;
                }
                else if (tipo == 2)
                {
                    if (doc.Length == 11)
                    {
                        Formata = Formata.Substring(doc.Length - 1);
                        Formata = Formata.Substring(0, 3) + "." + Formata.Substring(3, 3) + "." + Formata.Substring(6, 3) + "-" + Formata.Substring(10, 2);
                    }

                    return Formata;
                }

                return Formata;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //Update na tb_bl quando for feito uma cancelamento
        public string CancelaTituloPix(long numero_titulo)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    StringBuilder sb = new StringBuilder();
                    {
                        StringBuilder sbu = new StringBuilder();

                        int count = 0;

                        sb.AppendLine(" SELECT count(1) FROM  SGIPA.TB_GR_BL  WHERE NUM_TITULO_PIX  =   " + numero_titulo);

                        count = con.Query<int>(sb.ToString()).FirstOrDefault();

                        if (count == 0)
                        {

                            sbu.AppendLine(" UPDATE SGIPA.TB_BL SET  NUM_PAGAMENTO_PIX = 0, NUM_TITULO_PIX = 0 WHERE NUM_TITULO_PIX  =   " + numero_titulo);
                            con.Query<string>(sbu.ToString()).FirstOrDefault();
                            return "000-Titulo cancelado com sucesso";
                        }
                        else
                        {
                            return "001-O título tem GR impressa";

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "002-O título não pode ser cancelado";
            }
        }
       
    }
}