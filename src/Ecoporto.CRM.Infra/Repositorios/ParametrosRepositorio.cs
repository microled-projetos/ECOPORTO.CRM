using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Linq;
using System.Runtime.Caching;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class ParametrosRepositorio : IParametrosRepositorio
    {
        public Parametros ObterParametros()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Parametros>(@"SELECT Mora, Multa, GerarPdfProposta, ValidaConcomitancia, CriarAdendoExclusaoCliente, IntegraChronos, AnexarSimulador, DividaSpc FROM CRM.TB_CRM_PARAMETROS").FirstOrDefault();
            }
        }

        public ParametrosFatura ObterParametrosFatura(int empresaId)
        {
            MemoryCache cache = MemoryCache.Default;

            var parametrosFatura = cache["Parametros.ObterParametrosFatura"] as ParametrosFatura;

            if (parametrosFatura == null)
            {
                using (OracleConnection con = new OracleConnection(Config.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "EmpresaId", value: empresaId, direction: ParameterDirection.Input);

                    parametrosFatura = con.Query<ParametrosFatura>(@"SELECT DIA_UTIL_CANC_SAP As DiaUtilCancelamentoSAP FROM FATURA.PARAMETRO WHERE COD_EMPRESA = :EmpresaId", parametros).FirstOrDefault();
                }

                cache["Parametros.ObterParametrosFatura"] = parametrosFatura;
            }

            return parametrosFatura;
        }
    }
}
