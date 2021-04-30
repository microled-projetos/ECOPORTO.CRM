using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class PaisRepositorio : IPaisRepositorio
    {
        public IEnumerable<Pais> ObterPaises()
        {
            MemoryCache cache = MemoryCache.Default;

            var paises = cache["Pais.ObterPaises"] as IEnumerable<Pais>;

            if (paises == null)
            {
                using (OracleConnection con = new OracleConnection(Config.StringConexao()))
                {
                    paises = con.Query<Pais>(@"SELECT Id, Descricao, Sigla FROM CRM.TB_CRM_PAISES ORDER BY Descricao");
                }

                cache["Pais.ObterPaises"] = paises;
            }

            return paises;
        }        
    }
}
