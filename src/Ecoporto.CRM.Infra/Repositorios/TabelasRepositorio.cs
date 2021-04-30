using Dapper;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models.Oportunidades;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class TabelasRepositorio : ITabelasRepositorio
    {
        public OportunidadeTabelaConcomitante ObterTabelaChronosPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "TabelaId", value: id, direction: ParameterDirection.Input);

                return con.Query<OportunidadeTabelaConcomitante>(@"SELECT AUTONUM As Id, IMPORTADOR As ImportadorId, DESPACHANTE As DespachanteId, COLOADER As ColoaderId, COCOLOADER As CoColoaderId, COCOLOADER2 As CoColoader2Id FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :TabelaId", parametros).FirstOrDefault();
            }
        }

        public bool ExisteParceiroNoGrupo(int tabelaId, int parceiroId, string tipo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                parametros.Add(name: "ParceiroId", value: parceiroId, direction: ParameterDirection.Input);
                parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                var total = con.Query<int>(@"
                        SELECT COUNT(1) FROM SGIPA.TB_TP_GRUPOS WHERE 
                            TIPO = :Tipo AND AUTONUMLISTA = :TabelaId", parametros).Single();

                if (total < 2)
                    return false;

                return true;
            }
        }

        public bool ExisteParceiroNaProposta(int oportunidadeId, SegmentoSubCliente segmentoSubCliente)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "SegmentoSubCliente", value: segmentoSubCliente, direction: ParameterDirection.Input);                

                var total = con.Query<int>(@"
                        SELECT COUNT(1) FROM CRM.TB_CRM_OPORTUNIDADE_CLIENTES WHERE 
                            OportunidadeId = :OportunidadeId AND Segmento = :SegmentoSubCliente", parametros).Single();

                if (total < 2)
                    return false;

                return true;
            }
        }
    }
}
