using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class ImpostoRepositorio : IImpostoRepositorio
    {      
        public IEnumerable<Imposto> ObterImpostos()
        {
            MemoryCache cache = MemoryCache.Default;

            var impostos = cache["Imposto.ObterImpostos"] as IEnumerable<Imposto>;

            if (impostos == null)
            {
                using (OracleConnection con = new OracleConnection(Config.StringConexao()))
                {
                    impostos = con.Query<Imposto>(@"SELECT Id, Descricao FROM CRM.TB_CRM_IMPOSTOS  ORDER BY Id");
                }

                cache["Imposto.ObterImpostos"] = impostos;
            }

            return impostos;
        }
    }
}