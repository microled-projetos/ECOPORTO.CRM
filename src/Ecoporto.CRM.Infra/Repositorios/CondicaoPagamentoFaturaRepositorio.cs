using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class CondicaoPagamentoFaturaRepositorio : ICondicaoPagamentoFaturaRepositorio
    {
        public IEnumerable<CondicaoPagamentoFatura> ObterCondicoesPagamento()
        {
            MemoryCache cache = MemoryCache.Default;

            var condicoesPgto = cache["CondicaoPagamentoFatura.ObterCondicoesPagamento"] as IEnumerable<CondicaoPagamentoFatura>;

            if (condicoesPgto == null)
            {
                using (OracleConnection con = new OracleConnection(Config.StringConexao()))
                {
                    condicoesPgto = con.Query<CondicaoPagamentoFatura>(@"SELECT CODCPG AS Id, DESCPG As Descricao FROM FATURA.TB_COND_PGTO ORDER BY CODCPG");
                }

                cache["CondicaoPagamentoFatura.ObterCondicoesPagamento"] = condicoesPgto;
            }

            return condicoesPgto;
        }

        public CondicaoPagamentoFatura ObterCondicoPagamentoPorId(string id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<CondicaoPagamentoFatura>(@"SELECT CODCPG AS Id, DESCPG As Descricao FROM FATURA.TB_COND_PGTO WHERE CODCPG = :id", new { id }).FirstOrDefault();
            }
        }
    }
}
