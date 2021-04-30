using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class LocalAtracacaoRepositorio : ILocalAtracacaoRepositorio
    {      
        public IEnumerable<Armazem> ObterLocaisAtracacao()
        {
            MemoryCache cache = MemoryCache.Default;

            var locaisAtracacao = cache["LocalAtracacao.ObterLocaisAtracacao"] as IEnumerable<Armazem>;

            if (locaisAtracacao == null)
            {
                using (OracleConnection con = new OracleConnection(Config.StringConexao()))
                {
                    locaisAtracacao = con.Query<Armazem>(@"SELECT '0' AS Id, '' AS Descricao FROM DUAL UNION SELECT CODE AS Id, DESCR || ' ' || GRUPO AS Descricao FROM SGIPA.DTE_TB_ARMAZENS ORDER BY Descricao NULLS FIRST");
                }

                cache["LocalAtracacao.ObterLocaisAtracacao"] = locaisAtracacao;
            }

            return locaisAtracacao;
        }
    }
}
