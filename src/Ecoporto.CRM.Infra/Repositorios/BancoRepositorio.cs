using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class BancoRepositorio : IBancoRepositorio
    {
        public IEnumerable<Banco> ObterBancos()
        {
            MemoryCache cache = MemoryCache.Default;

            var bancos = cache["Banco.ObterBancos"] as IEnumerable<Banco>;

//            if (bancos == null)
  //          {
                using (OracleConnection con = new OracleConnection(Config.StringConexao()))
                {
                    bancos = con.Query<Banco>(@"SELECT Id, Descricao FROM CRM.TB_CRM_BANCOS ORDER BY Descricao");
                }

               // cache["Banco.ObterBancos"] = bancos;
    //        }

            return bancos;
        }      
    }
}
