using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class EmpresaRepositorio : IEmpresaRepositorio
    {      
        public IEnumerable<Empresa> ObterEmpresas()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Empresa>(@"SELECT * FROM CRM.TB_CRM_EMPRESAS ORDER BY Id");
            }
        }
    }
}
