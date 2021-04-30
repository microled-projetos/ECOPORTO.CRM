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
    public class MinutaRepositorio : IMinutaRepositorio
    {
        public Minuta ObterMinuta(int minuta)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: minuta, direction: ParameterDirection.Input);

                return con.Query<Minuta>(@"
                    SELECT     
                        B.AUTONUM As Id,
                        D.AUTONUM As ClienteId,
                        D.RAZAO As ClienteDescricao,
                        B.Status,
                        SUM((QUANTIDADE*PRECO_UNIT) + NVL(I.VALOR_IMP,0)) As Valor 
                    FROM 
                        OPERADOR.TB_MINUTA_SERVICO A 
                    INNER JOIN
                        OPERADOR.TB_MINUTA B ON A.MINUTA = B.AUTONUM
                    INNER JOIN 
                        OPERADOR.TB_SERVICOS C ON A.SERVICO = C.AUTONUM
                    LEFT  JOIN
                        OPERADOR.TB_CAD_CLIENTES D ON B.CLIENTE = D.AUTONUM
                    LEFT JOIN 
                        (
                            SELECT 
                                AUTONUM_MINUTA_SERVICO, 
                                SUM(VALOR_IMPOSTO) VALOR_IMP 
                            FROM 
                                OPERADOR.TB_MINUTA_SERVICO_IMPOSTOS 
                            GROUP BY 
                                AUTONUM_MINUTA_SERVICO
                        ) I ON A.AUTONUM = I.AUTONUM_MINUTA_SERVICO
                    WHERE 
                        B.AUTONUM = :Id  
                    GROUP BY                        
                        B.AUTONUM,
                        B.Status,
                        D.AUTONUM,
                        D.RAZAO", parametros).FirstOrDefault();
            }
        }

        public IEnumerable<ServicoFaturamento> ObterServicosPorMinuta(int minuta)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Minuta", value: minuta, direction: ParameterDirection.Input);

                return con.Query<ServicoFaturamento>(@"
                    SELECT     
                        A.AUTONUM As Id,
                        B.DESCR As Descricao,
                        (QUANTIDADE*PRECO_UNIT) + NVL(C.VALOR_IMP,0) As Valor 
                    FROM 
                        OPERADOR.TB_MINUTA_SERVICO A 
                    INNER JOIN 
                        OPERADOR.TB_SERVICOS B ON A.SERVICO = B.AUTONUM
                    LEFT JOIN 
                        (
                            SELECT 
                                AUTONUM_MINUTA_SERVICO, 
                                SUM(VALOR_IMPOSTO) VALOR_IMP 
                            FROM 
                                OPERADOR.TB_MINUTA_SERVICO_IMPOSTOS 
                            GROUP BY 
                                AUTONUM_MINUTA_SERVICO
                        ) C ON A.AUTONUM = C.AUTONUM_MINUTA_SERVICO
                    WHERE 
                        A.MINUTA = :Minuta", parametros);
            }
        }

        public ServicoFaturamento ObterServicoFaturamentoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<ServicoFaturamento>(@"
                    SELECT     
                        A.AUTONUM As Id,
                        B.AUTONUM As Servico,
                        B.DESCR As Descricao,
                        (QUANTIDADE*PRECO_UNIT) + NVL(C.VALOR_IMP,0) As Valor 
                    FROM 
                        OPERADOR.TB_MINUTA_SERVICO A 
                    INNER JOIN 
                        OPERADOR.TB_SERVICOS B ON A.SERVICO = B.AUTONUM
                    LEFT JOIN 
                        (
                            SELECT 
                                AUTONUM_MINUTA_SERVICO, 
                                SUM(VALOR_IMPOSTO) VALOR_IMP 
                            FROM 
                                OPERADOR.TB_MINUTA_SERVICO_IMPOSTOS 
                            GROUP BY 
                                AUTONUM_MINUTA_SERVICO
                        ) C ON A.AUTONUM = C.AUTONUM_MINUTA_SERVICO
                    WHERE 
                        A.AUTONUM = :Id", parametros).FirstOrDefault();
            }
        }
    }
}
