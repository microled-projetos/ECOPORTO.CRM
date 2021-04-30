using Dapper;
using Ecoporto.CRM.Business.Interfaces.Servicos;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Linq;

namespace Ecoporto.CRM.Infra.Services
{
    public class RelogioService : IRelogioService
    {
        public DateTime ObterDataHora()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<DateTime>(@"SELECT SYSDATE FROM DUAL").FirstOrDefault();
            }
        }
    }
}
