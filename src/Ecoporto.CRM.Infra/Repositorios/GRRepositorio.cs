using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class GRRepositorio : IGRRepositorio
    {
        public IEnumerable<GR> ObterGRsPorLote(int lote)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Lote", value: lote, direction: ParameterDirection.Input);

                return con.Query<GR>(@"
                    SELECT
                        A.AUTONUM As Id,
                        A.SEQ_GR As SeqGR,
                        A.STATUS_GR As StatusGR,
                        C.CGC As ImportadorCnpj,
                        D.CGC As IndicadorCnpj
                    FROM
                        SGIPA.TB_GR_BL A
                    INNER JOIN
                        SGIPA.TB_BL B ON A.BL = B.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS C ON B.IMPORTADOR = C.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS D ON B.CAPTADOR = D.AUTONUM    
                    WHERE
                        A.BL = :Lote AND (A.STATUS_GR = 'GE')  
                    UNION ALL
                    SELECT
                        DISTINCT
                            0 As Id,
                            0 As SeqGR,
                            'GE' As StatusGR,
                            C.CGC As ImportadorCnpj,
                            D.CGC As IndicadorCnpj
                        FROM
                            SGIPA.TB_SERVICOS_FATURADOS A
                        INNER JOIN
                            SGIPA.TB_BL B ON A.BL = B.AUTONUM
                        LEFT JOIN
                            SGIPA.TB_CAD_PARCEIROS C ON B.IMPORTADOR = C.AUTONUM
                        LEFT JOIN
                            SGIPA.TB_CAD_PARCEIROS D ON B.CAPTADOR = D.AUTONUM 
                        WHERE
                            A.BL = :Lote AND NVL(A.SEQ_GR,0) = 0 ORDER BY 2", parametros);
            }
        }

        public IEnumerable<GR> ObterGRsFaturadasPorLote(int lote)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Lote", value: lote, direction: ParameterDirection.Input);

                return con.Query<GR>(@"
                    SELECT 
	                    DISTINCT 
                            GR.AUTONUM As Id,
                            GR.SEQ_GR As SeqGR,
                            GR.STATUS_GR As StatusGR
                    FROM 
	                    SGIPA.TB_GR_BL GR
                    WHERE 
	                    GR.STATUS_GR = 'GE'
                    AND 
	                    GR.FORMA_PAGAMENTO = 2
                    AND 
	                    GR.BL = :Lote

                    UNION

                    SELECT 
	                    DISTINCT 
		                    0 As Id,
                            0 As SeqGR,
                            'GE' As StatusGR
                    FROM 
	                    SGIPA.TB_SERVICOS_FATURADOS SFAT
                    INNER JOIN 
	                    SGIPA.TB_GR_PRE_CALCULO PC ON PC.BL = SFAT.BL
                    WHERE 
	                    SFAT.SEQ_GR IS NULL 
                    AND 
	                    PC.FORMAPAGAMENTO = 2
                    AND 
	                    SFAT.BL = :Lote", parametros);
            }            
        }

        public IEnumerable<GR> ObterGRsPorBL(string bl)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Numero", value: bl, direction: ParameterDirection.Input);

                return con.Query<GR>(@"
                    SELECT
                        A.AUTONUM As Id,
                        A.SEQ_GR As SeqGR,
                        A.STATUS_GR As StatusGR,
                        C.CGC As ImportadorCnpj,
                        D.CGC As IndicadorCnpj
                    FROM
                        SGIPA.TB_GR_BL A
                    INNER JOIN
                        SGIPA.TB_BL B ON A.BL = B.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS C ON B.IMPORTADOR = C.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS D ON B.CAPTADOR = D.AUTONUM    
                    WHERE
                        B.NUMERO = :Numero AND (A.STATUS_GR = 'GE')  
                    UNION ALL
                    SELECT
                        DISTINCT
                            0 As Id,
                            0 As SeqGR,
                            'GE' As StatusGR,
                            C.CGC As ImportadorCnpj,
                            D.CGC As IndicadorCnpj
                        FROM
                            SGIPA.TB_SERVICOS_FATURADOS A
                        INNER JOIN
                            SGIPA.TB_BL B ON A.BL = B.AUTONUM
                        LEFT JOIN
                            SGIPA.TB_CAD_PARCEIROS C ON B.IMPORTADOR = C.AUTONUM
                        LEFT JOIN
                            SGIPA.TB_CAD_PARCEIROS D ON B.CAPTADOR = D.AUTONUM 
                        WHERE
                            B.NUMERO = :Numero AND NVL(A.SEQ_GR,0) = 0 ORDER BY 2", parametros);
            }
        }

        public IEnumerable<GR> ObterGRsRedexPorReserva(string reserva)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Reserva", value: reserva, direction: ParameterDirection.Input);

                return con.Query<GR>(@"
                    SELECT
	                    A.AUTONUM As Id,
	                    A.SEQ_GR As SeqGR,
	                    A.STATUS_GR As StatusGR,
	                    C.Fantasia As ClienteDescricao,
                        C.Autonum as ClienteId,
                        C.CGC As ExportadorCnpj
                    FROM
	                    REDEX.TB_GR_BOOKING A
                    INNER JOIN
	                    REDEX.TB_BOOKING B ON A.Booking = B.AUTONUM_BOO
                    LEFT JOIN
	                    REDEX.TB_CAD_PARCEIROS C ON A.CLIENTE_FATURA = C.AUTONUM
                    WHERE
	                    B.REFERENCE = :Reserva AND STATUS_GR = 'GE'
                    UNION ALL
                    SELECT
	                    DISTINCT
		                    0 As Id,
		                    0 As SeqGR,
		                    'GE' As StatusGR,
                            C.Fantasia As ClienteDescricao,
		                    C.autonum As ClienteId
	                    FROM
		                    REDEX.TB_SERVICOS_FATURADOS A
	                    INNER JOIN
		                    REDEX.TB_BOOKING B ON A.Booking = B.AUTONUM_BOO        
                        LEFT JOIN
	                       REDEX.TB_CAD_PARCEIROS C ON A.CLIENTE_FATURA = C.AUTONUM
	                    WHERE
		                    B.REFERENCE = :Reserva AND NVL(A.SEQ_GR,0) = 0 
                    GROUP BY 
                        C.Autonum,
                        C.Fantasia 
                    ORDER BY 2", parametros);
            }
        }

        public GR ObterDetalhesGR(int seqGR)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "SeqGR", value: seqGR, direction: ParameterDirection.Input);
                
                return con.Query<GR>(@"
                    SELECT
                        B.AUTONUM As Id,
                        A.SEQ_GR As SeqGR,
                        A.STATUS_GR As StatusGR,
                        A.VALOR_GR As Valor,
                        C.AUTONUM As ClienteId,
                        C.CGC As ImportadorCnpj,
                        C.RAZAO As ClienteDescricao,
                        D.AUTONUM As IndicadorId,
                        D.RAZAO As IndicadorDescricao,
                        D.CGC As IndicadorCnpj,
                        A.BL As Lote,
                        A.TABELA_GR As Proposta,
                        A.DT_FIM_PERIODO As Vencimento,
                        A.VALIDADE_GR AS FreeTime,
                        A.PERIODOS As Periodos,
                        DECODE(A.FORMA_PAGAMENTO, 2, 1, 3, 2) As FormaPagamento
                    FROM
                        SGIPA.TB_GR_BL A 
                    INNER JOIN
                        SGIPA.TB_BL B ON A.BL = B.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS C ON B.IMPORTADOR = C.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS D ON B.CAPTADOR = D.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_LISTAS_PRECOS E ON A.TABELA_GR = E.AUTONUM
                    WHERE
                        A.SEQ_GR = :SeqGR", parametros).FirstOrDefault();
            }
        }

        public GR ObterDetalhesGRRedex(long booking, int seqGR, int clienteId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Booking", value: booking, direction: ParameterDirection.Input);
                parametros.Add(name: "Gr", value: seqGR, direction: ParameterDirection.Input);
                parametros.Add(name: "ClienteId", value: clienteId, direction: ParameterDirection.Input);

                return con.Query<GR>(@"                          
                    SELECT                        
	                    A.AUTONUM_BOO As Id,
	                    0 As SeqGR, 
	                    F.Valor,
	                    '' As StatusGR,             
	                    G.AUTONUM As ClienteId,
	                    G.RAZAO As ClienteDescricao,
	                    A.REFERENCE As Reserva,
	                    NVL(A.AUTONUM_LISTA, 0) As Proposta,
	                    '' As Vencimento,
	                    '' AS FreeTime,
	                    '' As Periodos
                    FROM
	                    REDEX.TB_BOOKING A
                    LEFT JOIN
	                    REDEX.TB_LISTAS_PRECOS E ON A.AUTONUM_LISTA = E.AUTONUM                    
                    LEFT JOIN
	                    (
		                    SELECT
			                    BOOKING,
			                    SUM(((VALOR + ADICIONAL) - DESCONTO) + NVL(
				                    B.VALORIMP,
				                    0
			                    )) VALOR,
			                    NVL(SEQ_GR,0) AS SEQ_GR,
			                    CLIENTE_FATURA
		                    FROM
			                    REDEX.TB_SERVICOS_FATURADOS A
			                    LEFT JOIN (
				                    SELECT
					                    A.AUTONUM_SERVICO_FATURADO AUTONUM,
					                    SUM(VALOR_IMPOSTO) VALORIMP
				                    FROM
					                    REDEX.TB_SERVICOS_FATURADOS_IMPOSTOS A
				                        GROUP BY
					                    A.AUTONUM_SERVICO_FATURADO
			                    ) B ON A.AUTONUM = B.AUTONUM
		                    WHERE                                
			                    A.CLIENTE_FATURA = :ClienteId AND A.Booking = :Booking AND NVL(A.SEQ_GR, 0) = :Gr
		                    GROUP BY
			                    NVL(SEQ_GR,0),
			                    CLIENTE_FATURA,
			                    BOOKING
	                    ) F ON A.AUTONUM_BOO = F.BOOKING
                    LEFT JOIN
	                    REDEX.TB_CAD_PARCEIROS G ON F.CLIENTE_FATURA = G.AUTONUM
                    WHERE
	                    A.AUTONUM_BOO = :Booking", parametros).FirstOrDefault();
            }
        }

        public GR ObterDetalhesGRRedexPorReserva(long booking)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Booking", value: booking, direction: ParameterDirection.Input);

                return con.Query<GR>(@"                          
                    SELECT                        
                        A.AUTONUM_BOO As Id,
                        0 As SeqGR, 
                        F.Valor,
                        '' As StatusGR,             
                        G.AUTONUM As ClienteId,
                        G.RAZAO As ClienteDescricao,
                        A.REFERENCE As Reserva,
                        NVL(A.AUTONUM_LISTA, 0) As Proposta,
                        '' As Vencimento,
                        '' AS FreeTime,
                        '' As Periodos
                    FROM
                        REDEX.TB_BOOKING A
                    LEFT JOIN
                        REDEX.TB_LISTAS_PRECOS E ON A.AUTONUM_LISTA = E.AUTONUM                    
                    LEFT JOIN
                        (
                            SELECT
                                BOOKING,
                                SUM(((VALOR + ADICIONAL) - DESCONTO) + NVL(
                                    B.VALORIMP,
                                    0
                                )) VALOR,
                                NVL(SEQ_GR,0) AS SEQ_GR,
                                CLIENTE_FATURA
                            FROM
                                REDEX.TB_SERVICOS_FATURADOS A
                                LEFT JOIN (
                                    SELECT
                                        A.AUTONUM_SERVICO_FATURADO AUTONUM,
                                        SUM(VALOR_IMPOSTO) VALORIMP
                                    FROM
                                        REDEX.TB_SERVICOS_FATURADOS_IMPOSTOS A
                                    GROUP BY
                                        A.AUTONUM_SERVICO_FATURADO
                                ) B ON A.AUTONUM = B.AUTONUM
                            WHERE                                
                                A.Booking = :Booking AND NVL(A.SEQ_GR, 0) = 0
                            GROUP BY
                                NVL(SEQ_GR,0),
                                CLIENTE_FATURA,
                                BOOKING
                        ) F ON A.AUTONUM_BOO = F.BOOKING
                    LEFT JOIN
                        REDEX.TB_CAD_PARCEIROS G ON F.CLIENTE_FATURA = G.AUTONUM
                    WHERE
                        A.AUTONUM_BOO = :Booking ", parametros).FirstOrDefault();
            }
        }

        public GR ObterDetalhesGRPorLote(int lote, int seq_gr)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Lote", value: lote, direction: ParameterDirection.Input);
                parametros.Add(name: "Gr", value: seq_gr, direction: ParameterDirection.Input);

                return con.Query<GR>(@"
                    SELECT
                        B.AUTONUM As Id,
                        A.SEQ_GR As SeqGR,
                        A.STATUS_GR As StatusGR,
                        SUM(A.VALOR_GR) As Valor,
                        C.AUTONUM As ClienteId,
                        C.RAZAO As ClienteDescricao,
                        D.AUTONUM As IndicadorId,
                        D.RAZAO As IndicadorDescricao,
                        A.BL As Lote,
                        A.TABELA_GR As Proposta,
                        A.DT_FIM_PERIODO As Vencimento,
                        A.VALIDADE_GR AS FreeTime,
                        A.PERIODOS As Periodos,
                        DECODE(A.FORMA_PAGAMENTO, 2, 1, 3, 2) As FormaPagamento
                    FROM
                        SGIPA.TB_GR_BL A 
                    INNER JOIN
                        SGIPA.TB_BL B ON A.BL = B.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS C ON B.IMPORTADOR = C.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS D ON B.CAPTADOR = D.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_LISTAS_PRECOS E ON A.TABELA_GR = E.AUTONUM
                    WHERE
                        A.BL = :Lote AND A.Status_Gr = 'GE' AND A.SEQ_GR = :Gr
                    GROUP BY
                        B.AUTONUM,
                        A.SEQ_GR,
                        A.STATUS_GR,
                        C.AUTONUM,
                        C.RAZAO,
                        D.AUTONUM,
                        D.RAZAO,
                        A.BL,
                        A.TABELA_GR,
                        A.DT_FIM_PERIODO,
                        A.VALIDADE_GR,
                        A.PERIODOS,
                        A.FORMA_PAGAMENTO", parametros).FirstOrDefault();
            }
        }

        public GR ObterDetalhesGRPorBL(string bl, string seq_gr)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "BL", value: bl, direction: ParameterDirection.Input);
                parametros.Add(name: "Gr", value: seq_gr, direction: ParameterDirection.Input);

                return con.Query<GR>(@"
                    SELECT
                        B.AUTONUM As Id,
                        A.SEQ_GR As SeqGR,
                        A.STATUS_GR As StatusGR,
                        SUM(A.VALOR_GR) As Valor,
                        C.AUTONUM As ClienteId,
                        C.RAZAO As ClienteDescricao,
                        D.AUTONUM As IndicadorId,
                        D.RAZAO As IndicadorDescricao,
                        A.BL As Lote,
                        A.TABELA_GR As Proposta,
                        A.DT_FIM_PERIODO As Vencimento,
                        A.VALIDADE_GR AS FreeTime,
                        A.PERIODOS As Periodos,
                        DECODE(A.FORMA_PAGAMENTO, 2, 1, 3, 2) As FormaPagamento
                    FROM
                        SGIPA.TB_GR_BL A 
                    INNER JOIN
                        SGIPA.TB_BL B ON A.BL = B.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS C ON B.IMPORTADOR = C.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS D ON B.CAPTADOR = D.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_LISTAS_PRECOS E ON A.TABELA_GR = E.AUTONUM
                    WHERE
                        B.NUMERO = :BL AND A.Status_Gr = 'GE' AND A.SEQ_GR = :Gr
                    GROUP BY
                        B.AUTONUM,
                        A.SEQ_GR,
                        A.STATUS_GR,
                        C.AUTONUM,
                        C.RAZAO,
                        D.AUTONUM,
                        D.RAZAO,
                        A.BL,
                        A.TABELA_GR,
                        A.DT_FIM_PERIODO,
                        A.VALIDADE_GR,
                        A.PERIODOS,
                        A.FORMA_PAGAMENTO", parametros).FirstOrDefault();
            }
        }

        public GR ObterDetalhesPreCalculoLote(int lote)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Lote", value: lote, direction: ParameterDirection.Input);

                return con.Query<GR>(@"
                    SELECT
                        B.AUTONUM AS Id,
                        (
                            SELECT
                                SUM(VALOR + ADICIONAL + DESCONTO + NVL(
                                    B.VALORIMP,
                                    0
                                )) VALOR
                            FROM
                                SGIPA.TB_SERVICOS_FATURADOS A
                                LEFT JOIN (
                                    SELECT
                                        A.AUTONUM_SERVICO_FATURADO AUTONUM,
                                        SUM(VALOR_IMPOSTO) VALORIMP
                                    FROM
                                        SGIPA.TB_SERVICOS_FATURADOS_IMPOSTOS A
                                    GROUP BY
                                        A.AUTONUM_SERVICO_FATURADO
                                ) B ON A.AUTONUM = B.AUTONUM
                            WHERE
                                    SEQ_GR IS NULL
                                AND
                                    BL = :Lote AND SEQ_GR IS NULL                           
                        ) AS Valor,
                        C.AUTONUM AS ClienteId,
                        C.RAZAO AS ClienteDescricao,
                        D.AUTONUM AS IndicadorId,
                        D.RAZAO AS IndicadorDescricao,
                        A.LISTA AS Proposta,
                        A.DATA_FINAL AS Vencimento,
                        A.VALIDADE_GR AS FreeTime,
                        A.PERIODOS AS Periodos,
                        A.BL AS Lote,
                        DECODE(A.FORMAPAGAMENTO, 2, 1, 3, 2) As FormaPagamento
                    FROM
                        SGIPA.TB_GR_PRE_CALCULO A
                    LEFT JOIN 
                        SGIPA.TB_BL B ON A.BL = B.AUTONUM 
                    LEFT JOIN 
                        SGIPA.TB_CAD_PARCEIROS C ON B.IMPORTADOR = C.AUTONUM 
                    LEFT JOIN 
                        SGIPA.TB_CAD_PARCEIROS D ON B.CAPTADOR = D.AUTONUM
                    INNER JOIN 
                        (
                            SELECT BL FROM SGIPA.TB_SERVICOS_FATURADOS WHERE BL = :Lote AND SEQ_GR IS NULL GROUP BY BL
                        ) SVF ON A.BL = SVF.BL                    
                    WHERE
                        A.BL = :Lote", parametros).FirstOrDefault();
            }
        }

        public GR ObterDetalhesPreCalculoPorBL(string numero)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Numero", value: numero, direction: ParameterDirection.Input);

                return con.Query<GR>(@"
                    SELECT
                        B.AUTONUM AS Id,
                        (
                            SELECT
                                SUM(VALOR + ADICIONAL + DESCONTO + NVL(
                                    B.VALORIMP,
                                    0
                                )) VALOR
                            FROM
                                SGIPA.TB_SERVICOS_FATURADOS A
                                LEFT JOIN (
                                    SELECT
                                        A.AUTONUM_SERVICO_FATURADO AUTONUM,
                                        SUM(VALOR_IMPOSTO) VALORIMP
                                    FROM
                                        SGIPA.TB_SERVICOS_FATURADOS_IMPOSTOS A
                                    GROUP BY
                                        A.AUTONUM_SERVICO_FATURADO
                                ) B ON A.AUTONUM = B.AUTONUM
                                LEFT JOIN
                                    SGIPA.TB_BL C ON A.BL = C.AUTONUM
                            WHERE
                                    A.SEQ_GR IS NULL
                                AND
                                    C.NUMERO = :Numero AND A.SEQ_GR IS NULL                           
                        ) AS Valor,
                        C.AUTONUM AS ClienteId,
                        C.RAZAO AS ClienteDescricao,
                        D.AUTONUM AS IndicadorId,
                        D.RAZAO AS IndicadorDescricao,
                        A.LISTA AS Proposta,
                        A.DATA_FINAL AS Vencimento,
                        A.VALIDADE_GR AS FreeTime,
                        A.PERIODOS AS Periodos,
                        A.BL AS Lote,
                        DECODE(A.FORMAPAGAMENTO, 2, 1, 3, 2) As FormaPagamento
                    FROM
                        SGIPA.TB_GR_PRE_CALCULO A
                    LEFT JOIN 
                        SGIPA.TB_BL B ON A.BL = B.AUTONUM 
                    LEFT JOIN 
                        SGIPA.TB_CAD_PARCEIROS C ON B.IMPORTADOR = C.AUTONUM 
                    LEFT JOIN 
                        SGIPA.TB_CAD_PARCEIROS D ON B.CAPTADOR = D.AUTONUM
                    INNER JOIN 
                        (
                            SELECT BL FROM SGIPA.TB_SERVICOS_FATURADOS FT INNER JOIN SGIPA.TB_BL BL ON FT.BL = BL.AUTONUM WHERE BL.NUMERO = :Numero AND FT.SEQ_GR IS NULL GROUP BY FT.BL
                        ) SVF ON A.BL = SVF.BL                    
                    WHERE
                        B.NUMERO = :Numero", parametros).FirstOrDefault();
            }
        }
    }
}
