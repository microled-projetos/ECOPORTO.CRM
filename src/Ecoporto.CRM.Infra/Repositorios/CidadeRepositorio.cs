using Dapper;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class CidadeRepositorio : ICidadeRepositorio
    {
        public IEnumerable<Cidade> ObterCidades()
        {
            MemoryCache cache = MemoryCache.Default;

            var cidades = cache["Cidade.ObterCidades"] as IEnumerable<Cidade>;

            if (cidades == null)
            {
                using (OracleConnection con = new OracleConnection(Config.StringConexao()))
                {
                    cidades = con.Query<Cidade>(@"SELECT Id, Descricao, Estado FROM CRM.TB_CRM_CIDADES ORDER BY Descricao");
                }

                cache["Cidade.ObterCidades"] = cidades;
            }

            return cidades;
        }

        public Cidade ObterCidadePorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Cidade>(@"SELECT * FROM CRM.TB_CRM_CIDADES WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }

        public IEnumerable<Cidade> ObterCidadesPorEstado(Estado estado)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Cidade>(@"SELECT Id, Descricao, Estado FROM CRM.TB_CRM_CIDADES WHERE Estado = :estado ORDER BY Descricao", new { estado });
            }
        }
    }
}
