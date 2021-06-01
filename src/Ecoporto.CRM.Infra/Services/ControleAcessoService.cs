using Dapper;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Servicos;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Net.Http;

namespace Ecoporto.CRM.Infra.Services
{
    public class ControleAcessoService : IControleAcessoService
    {              
        public void LogarTentativaAcesso(int usuarioId, bool externo, bool sucesso, string mensagem, string ip)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "UsuarioId", value: usuarioId, direction: ParameterDirection.Input);
                parametros.Add(name: "IP", value: ip, direction: ParameterDirection.Input);
                parametros.Add(name: "Externo", value: externo.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Sucesso", value: sucesso.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Mensagem", value: mensagem, direction: ParameterDirection.Input);
                
                con.Execute(@"INSERT INTO CRM.TB_CRM_LOG_ACESSO (Id, UsuarioId, IP, Externo, Sucesso, Mensagem) VALUES (CRM.SEQ_LOG_ACESSO.NEXTVAL, :UsuarioId, :IP, :Externo, :Sucesso, :Mensagem)", parametros);
            }
        }
    }
}
