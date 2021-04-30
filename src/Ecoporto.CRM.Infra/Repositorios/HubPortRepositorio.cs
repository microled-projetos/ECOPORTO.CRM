using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class HubPortRepositorio : IHubPortRepositorio
    {
        public IEnumerable<ClienteHubPort> ObterClientesHubPort()
        {
            MemoryCache cache = MemoryCache.Default;

            var clientesHP = cache["ClienteHubPort.ObterClientesHubPort"] as IEnumerable<ClienteHubPort>;

            if (clientesHP == null)
            {
                using (OracleConnection con = new OracleConnection(Config.StringConexao()))
                {
                    clientesHP = con.Query<ClienteHubPort>(@"SELECT AUTONUM AS ID, FANTASIA AS DESCRICAO FROM SGIPA.TB_CAD_PARCEIROS WHERE FLAG_EADI > 0");
                }

                cache["ClienteHubPort.ObterClientesHubPort"] = clientesHP;
            }

            return clientesHP;            
        }
    }
}
