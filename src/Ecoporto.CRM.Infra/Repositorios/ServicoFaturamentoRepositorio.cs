using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class ServicoFaturamentoRepositorio : IServicoFaturamentoRepositorio
    {
        public IEnumerable<ServicoFaturamento> ObterServicos()
        {
            MemoryCache cache = MemoryCache.Default;

            var servicos = cache["ServicoFaturamento.ObterServicos"] as IEnumerable<ServicoFaturamento>;

            if (servicos == null)
            {
                using (OracleConnection con = new OracleConnection(Config.StringConexao()))
                {
                    servicos = con.Query<ServicoFaturamento>(@"SELECT AUTONUM AS ID, DESCR AS DESCRICAO FROM SGIPA.TB_SERVICOS_IPA ORDER BY DESCR");
                }

                cache["ServicoFaturamento.ObterServicos"] = servicos;
            }

            return servicos;
        }

        public ServicoFaturamento ObterServicoFaturamentoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<ServicoFaturamento>(@"SELECT AUTONUM AS Id, Servico, (VALOR + DESCONTO + ADICIONAL) As Valor FROM SGIPA.TB_SERVICOS_FATURADOS WHERE AUTONUM = :Id", parametros).FirstOrDefault();
            }
        }

        public ServicoFaturamento ObterServicoFaturamentoRedexPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<ServicoFaturamento>(@"SELECT AUTONUM AS Id, Servico, (((VALOR + ADICIONAL) - DESCONTO)) As Valor FROM REDEX.TB_SERVICOS_FATURADOS WHERE AUTONUM = :Id", parametros).FirstOrDefault();
            }
        }

        public IEnumerable<ServicoFaturamento> ObterServicos(int[] ids)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<ServicoFaturamento>(@"SELECT AUTONUM AS ID, DESCR AS DESCRICAO FROM SGIPA.TB_SERVICOS_IPA WHERE AUTONUM IN :sId ORDER BY DESCR", new { sId = ids });
            }
        }

        public IEnumerable<ServicoFaturamento> ObterServicosPorGR(int seqGr)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "SeqGR", value: seqGr, direction: ParameterDirection.Input);

                return con.Query<ServicoFaturamento>(@"
                    SELECT
                        DISTINCT
                            B.SERVICO As Id,
                            C.DESCR As Descricao,
                            B.Valor + B.Desconto + B.Adicional AS Valor
                    FROM
	                    SGIPA.TB_GR_BL A
                    INNER JOIN
	                    SGIPA.TB_SERVICOS_FATURADOS B ON A.SEQ_GR = B.SEQ_GR
                    INNER JOIN
	                    SGIPA.TB_SERVICOS_IPA C ON B.SERVICO = C.AUTONUM
                    WHERE
	                    A.SEQ_GR = :SeqGR
                    ORDER BY
                        C.DESCR", parametros);
            }
        }

        public IEnumerable<ServicoFaturamento> ObterServicosPorBL(int bl, int? seqGr)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "BL", value: bl, direction: ParameterDirection.Input);

                string filtroGr = string.Empty;

                if (seqGr.HasValue)
                {
                    parametros.Add(name: "SeqGr", value: seqGr.Value, direction: ParameterDirection.Input);
                    filtroGr = " AND A.Seq_GR = :SeqGr ";
                }

                return con.Query<ServicoFaturamento>($@"
                    SELECT
                        DISTINCT
                            A.AUTONUM As Id,
                            A.Servico,
                            B.DESCR As Descricao,
                            A.Valor + A.Desconto + A.Adicional AS Valor
                    FROM
	                    SGIPA.TB_SERVICOS_FATURADOS A
                    INNER JOIN
	                    SGIPA.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                    WHERE
	                    A.BL = :BL {filtroGr}
                    ORDER BY
                        B.DESCR", parametros);
            }
        }

        public IEnumerable<ServicoFaturamento> ObterServicosRedex(long booking, long clienteId, int? seqGr)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Booking", value: booking, direction: ParameterDirection.Input);
                parametros.Add(name: "ClienteId", value: clienteId, direction: ParameterDirection.Input);

                string filtroGr = string.Empty;

                if (seqGr.HasValue)
                {
                    parametros.Add(name: "SeqGr", value: seqGr.Value, direction: ParameterDirection.Input);
                    filtroGr = " AND NVL(A.Seq_GR, 0) = :SeqGr ";
                }

                return con.Query<ServicoFaturamento>($@"
                    SELECT
                        DISTINCT
                            A.AUTONUM As Id,
                            A.Servico,
                            B.DESCR As Descricao,
                            D.Razao As ClienteDescricao,
                            ((A.Valor + A.Adicional) - A.Desconto) AS Valor
                    FROM
	                    REDEX.TB_SERVICOS_FATURADOS A                    
                    INNER JOIN
	                    REDEX.TB_SERVICOS_REDEX B ON A.SERVICO = B.AUTONUM
                    INNER JOIN
                        REDEX.TB_BOOKING C ON A.Booking = C.AUTONUM_BOO
                    LEFT JOIN
                        REDEX.TB_CAD_PARCEIROS D ON A.CLIENTE_FATURA = D.AUTONUM
                    WHERE
	                    A.Booking = :Booking {filtroGr} AND A.CLIENTE_FATURA = :ClienteId
                    ORDER BY
                        B.DESCR", parametros);
            }
        }

        public IEnumerable<ServicoFaturamento> ObterServicosPreCalculoPorBL(int bl)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "BL", value: bl, direction: ParameterDirection.Input);

                return con.Query<ServicoFaturamento>(@"
                    SELECT
                        DISTINCT
                            A.AUTONUM As Id,
                            A.Servico,
                            B.DESCR As Descricao,
                            ((A.Valor + A.Adicional) - A.Desconto) AS Valor
                    FROM
	                    SGIPA.TB_SERVICOS_FATURADOS A
                    INNER JOIN
	                    SGIPA.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                    WHERE
	                    A.BL = :BL AND A.SEQ_GR IS NULL
                    ORDER BY
                        B.DESCR", parametros);
            }
        }

        public decimal ObterValorImposto(int lote, string[] grs)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Lote", value: lote, direction: ParameterDirection.Input);
                parametros.Add(name: "Grs", value: grs, direction: ParameterDirection.Input);

                return con.Query<decimal>($@"SELECT NVL(SUM(VALOR_IMPOSTO),0) AS LIQUIDO FROM SGIPA.TB_SERVICOS_FATURADOS A, SGIPA.TB_SERVICOS_FATURADOS_IMPOSTOS B WHERE B.AUTONUM_IMPOSTO = 1 AND A.AUTONUM = B.AUTONUM_SERVICO_FATURADO AND A.BL = :Lote AND NVL(A.SEQ_GR, 0) IN :Grs", parametros).Single();
            }
        }

        public decimal ObterValorImpostoRedex(int booking, string[] grs)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Booking", value: booking, direction: ParameterDirection.Input);
                parametros.Add(name: "Grs", value: grs, direction: ParameterDirection.Input);

                return con.Query<decimal>($@"SELECT NVL(SUM(VALOR_IMPOSTO),0) AS LIQUIDO FROM REDEX.TB_SERVICOS_FATURADOS A, REDEX.TB_SERVICOS_FATURADOS_IMPOSTOS B WHERE B.AUTONUM_IMPOSTO = 1 AND A.AUTONUM = B.AUTONUM_SERVICO_FATURADO AND A.Booking = :Booking AND NVL(A.SEQ_GR, 0) IN :Grs", parametros).Single();
            }
        }
    }
}