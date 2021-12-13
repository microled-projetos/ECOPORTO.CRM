using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class VendedorRepositorio : IVendedorRepositorio
    {
        public IEnumerable<Vendedor> ObterVendedores()
        {
            MemoryCache cache = MemoryCache.Default;

           
                using (OracleConnection con = new OracleConnection(Config.StringConexao()))
                {
              var vendedores = con.Query<Vendedor>(@"SELECT A.Id, A.Nome, A.Login FROM CRM.TB_CRM_USUARIOS A INNER JOIN CRM.TB_CRM_USUARIOS_CARGOS B ON A.CargoId = B.Id WHERE B.Vendedor = 1 AND A.Ativo = 1 ORDER BY A.Nome");
                return vendedores;
            }

                
          
        }

        public Vendedor ObterVendedorPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<Vendedor>(@"SELECT Id, Nome FROM CRM.TB_CRM_USUARIOS WHERE Id = :Id", parametros).FirstOrDefault();
            }
        }
    }
}
