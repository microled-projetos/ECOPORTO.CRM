using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class GrupoAtracacaoRepositorio : IGrupoAtracacaoRepositorio
    {      
        public IEnumerable<Terminal> ObterGruposAtracacao()
        {
            MemoryCache cache = MemoryCache.Default;

            var gruposAtracacao = cache["GrupoAtracacao.ObterGruposAtracacao"] as IEnumerable<Terminal>;

            if (gruposAtracacao == null)
            {
                using (OracleConnection con = new OracleConnection(Config.StringConexao()))
                {
                    gruposAtracacao = con.Query<Terminal>(@"SELECT 0 AS Id, '' AS Descricao FROM DUAL UNION SELECT Id, Descricao FROM SGIPA.TB_GRUPOS_ATRACACAO ORDER BY DESCRICAO NULLS FIRST");
                }

                cache["GrupoAtracacao.ObterGruposAtracacao"] = gruposAtracacao;
            }

            return gruposAtracacao;
        }
    }
}
