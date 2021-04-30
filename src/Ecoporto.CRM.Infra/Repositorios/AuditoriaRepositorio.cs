using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Filtros;
using Ecoporto.CRM.Business.Helpers;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class AuditoriaRepositorio : IAuditoriaRepositorio
    {
        public IEnumerable<AuditoriaDTO> ObterHistorico(string controller, int chave, int chavePai = 0)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Controller", value: controller, direction: ParameterDirection.Input);
                parametros.Add(name: "Chave", value: chave, direction: ParameterDirection.Input);

                if (chavePai > 0)
                {
                    parametros.Add(name: "ChavePai", value: chavePai, direction: ParameterDirection.Input);
                }

                return con.Query<AuditoriaDTO>(@"
                    SELECT 
                        DISTINCT
                            Data,
                            Mensagem, 
                            Usuario, 
                            Chave, 
                            ChavePai, 
                            Controller, 
                            Acao, 
                            Maquina, 
                            Objeto
                    FROM 
                        CRM.TB_CRM_AUDITORIA 
                    WHERE 
                        Controller = :Controller
                    AND 
                        CHAVE = :Chave                    
                    ORDER BY 
                        Data", parametros);
            }
        }

        public IEnumerable<AuditoriaDTO> ObterLogsOportunidade(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<AuditoriaDTO>(@"
                    SELECT 
                        Id,
                        TO_CHAR(MIN(TO_TIMESTAMP(Data,'YYYY-MM-DD HH24:MI:SS.FF')),'DD/MM/YYYY HH24:MI') Data,
                        Mensagem, 
                        Usuario, 
                        Chave, 
                        Controller, 
                        Acao, 
                        Maquina, 
                        Objeto
                    FROM 
                        CRM.TB_CRM_AUDITORIA
                    WHERE 
                        (Action = 'ClonarOportunidade' OR Action = 'AtualizarInformacoesIniciais')
                    AND 
                        Chave = :OportunidadeId
                    GROUP BY
                        Id,
                        Data,
                        Mensagem, 
                        Usuario, 
                        Chave, 
                        Controller, 
                        Acao, 
                        Maquina, 
                        Objeto 
                    ORDER BY 
                        Id", parametros);
            }
        }

        public IEnumerable<AuditoriaDTO> ObterLogsFichasFaturamento(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<AuditoriaDTO>(@"
                    SELECT 
                        Id,
                        TO_CHAR(MIN(TO_TIMESTAMP(Data,'YYYY-MM-DD HH24:MI:SS.FF')),'DD/MM/YYYY HH24:MI') Data,
                        Mensagem, 
                        Usuario, 
                        Chave, 
                        Controller, 
                        Acao, 
                        Maquina, 
                        Objeto
                    FROM 
                        CRM.TB_CRM_AUDITORIA
                    WHERE 
                        (Action = 'CadastrarFichaFaturamento' OR Action = 'ExcluirFichaFaturamento')
                    AND 
                        ChavePai = :OportunidadeId
                    GROUP BY
                        Id,
                        Data,
                        Mensagem, 
                        Usuario, 
                        Chave, 
                        Controller, 
                        Acao, 
                        Maquina, 
                        Objeto 
                    ORDER BY 
                        Id", parametros);
            }
        }

        public IEnumerable<AuditoriaDTO> ObterLogsPremiosParceria(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<AuditoriaDTO>(@"
                    SELECT 
                        Id,
                        TO_CHAR(MIN(TO_TIMESTAMP(Data,'YYYY-MM-DD HH24:MI:SS.FF')),'DD/MM/YYYY HH24:MI') Data,
                        Mensagem, 
                        Usuario, 
                        Chave, 
                        Controller, 
                        Acao, 
                        Maquina, 
                        Objeto
                    FROM 
                        CRM.TB_CRM_AUDITORIA
                    WHERE 
                        (Action = 'CadastrarPremiosParceria' OR Action = 'ExcluirPremioParceria')
                    AND 
                        ChavePai = :OportunidadeId
                    GROUP BY
                        Id,
                        Data,
                        Mensagem, 
                        Usuario, 
                        Chave, 
                        Controller, 
                        Acao, 
                        Maquina, 
                        Objeto 
                    ORDER BY 
                        Id", parametros);
            }
        }

        public IEnumerable<AuditoriaDTO> ObterLogsProposta(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<AuditoriaDTO>(@"
                    SELECT
                        Data,
                        Mensagem,
                        Usuario,
                        Chave,
                        Controller,
                        Acao,
                        Maquina,
                        Objeto
                    FROM
                        (
                            SELECT 
                                TO_CHAR(MIN(TO_TIMESTAMP(Data,'YYYY-MM-DD HH24:MI:SS.FF')),'DD/MM/YYYY HH24:MI') Data,
                                CASE WHEN Action <> 'GerarProposta' THEN Mensagem ELSE 'Proposta Gerada' END Mensagem, 
                                Usuario, 
                                Chave, 
                                Controller, 
                                Acao, 
                                Maquina, 
                                Objeto
                            FROM 
                                CRM.TB_CRM_AUDITORIA
                            WHERE 
                                (Action = 'AtualizarInformacoesProposta' OR Action = 'GerarProposta' OR Action = 'Atualizar')
                            AND 
                                ChavePai = :OportunidadeId
                            GROUP BY
                                Action,
                                TO_CHAR(TO_TIMESTAMP(Data,'YYYY-MM-DD HH24:MI:SS.FF'),'YYYY-MM-DD HH24:MI'),
                                Mensagem, 
                                Usuario, 
                                Chave, 
                                Controller, 
                                Acao, 
                                Maquina, 
                                Objeto 
              
                            UNION ALL
        
                            SELECT
                                TO_CHAR(MIN(A.Data),'DD/MM/YYYY HH24:MI') Data,
                                'Valores Atualizados' As Mensaqem,
                                B.Login Usuario,
                                A.Id As Chave,
                                'AtualizacaoValoresProposta' As Controller,
                                2 As Acao,
                                '' As Maquina,
                                '' As Objeto
                            FROM
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT_LOG A
                            INNER JOIN
                                CRM.TB_CRM_USUARIOS B ON A.UsuarioId = B.Id
                            WHERE
                                A.OportunidadeId = :OportunidadeId AND ROWNUM < 2
                            GROUP BY
                                TO_CHAR(A.Data,'YYYY-MM-DD HH24:MI'),
                                B.Login,
                                A.Id
                    )
                    ORDER BY 
                        TO_CHAR(TO_TIMESTAMP(Data,'YYYY-MM-DD HH24:MI:SS.FF'),'YYYY-MM-DD HH24:MI:SS')", parametros);
            }
        }

        public IEnumerable<AuditoriaDTO> ObterLogsAnexos(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<AuditoriaDTO>(@"
                    SELECT 
                        Id,
                        TO_CHAR(MIN(TO_TIMESTAMP(Data,'YYYY-MM-DD HH24:MI:SS.FF')),'DD/MM/YYYY HH24:MI') Data,
                        Mensagem, 
                        Usuario, 
                        Chave, 
                        Controller, 
                        Acao, 
                        Maquina, 
                        Objeto
                    FROM 
                        CRM.TB_CRM_AUDITORIA
                    WHERE 
                        Action = 'ExcluirAnexo' 
                    AND 
                        ChavePai = :OportunidadeId
                    GROUP BY
                        Id,
                        Data,
                        Mensagem, 
                        Usuario, 
                        Chave, 
                        Controller, 
                        Acao, 
                        Maquina, 
                        Objeto 
                    ORDER BY 
                        Id", parametros);
            }
        }

        public IEnumerable<AuditoriaDTO> ObterLogsAdendos(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<AuditoriaDTO>(@"
                    SELECT 
                        Id,
                        TO_CHAR(MIN(TO_TIMESTAMP(Data,'YYYY-MM-DD HH24:MI:SS.FF')),'DD/MM/YYYY HH24:MI') Data,
                        Mensagem, 
                        Usuario, 
                        Chave, 
                        Controller, 
                        Acao, 
                        Maquina, 
                        Objeto
                    FROM 
                        CRM.TB_CRM_AUDITORIA
                    WHERE 
                        (Action = 'CadastrarAdendos' OR Action = 'AtualizarAdendo')
                    AND 
                        ChavePai = :OportunidadeId
                    GROUP BY
                        Id,
                        Data,
                        Mensagem, 
                        Usuario, 
                        Chave, 
                        Controller, 
                        Acao, 
                        Maquina, 
                        Objeto 
                    ORDER BY 
                        Id", parametros);
            }
        }

        public IEnumerable<AuditoriaAcessosDTO> ObterLogsAcesso(int pagina, int registrosPorPagina, AuditoriaAcessoFiltro filtro, string orderBy, out int totalFiltro)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var filtroSQL = new StringBuilder();
                var parametros = new DynamicParameters();

                if (!string.IsNullOrEmpty(filtro.Login))
                {                    
                    parametros.Add(name: "Login", value: "%" + filtro.Login.ToLower() + "%", direction: ParameterDirection.Input);
                    filtroSQL.Append(" AND LOWER(B.Login) LIKE :Login");
                }

                if (!string.IsNullOrEmpty(filtro.IP))
                {
                    parametros.Add(name: "IP", value: filtro.IP, direction: ParameterDirection.Input);
                    filtroSQL.Append(" AND A.IP = :IP ");
                }

                if (DateTimeHelpers.IsDate(filtro.De))
                {
                    var dataInicio = Convert.ToDateTime(filtro.De);
                    parametros.Add(name: "De", value: new DateTime(dataInicio.Year, dataInicio.Month, dataInicio.Day, 0, 0, 0), direction: ParameterDirection.Input);
                    filtroSQL.Append(" AND A.DataHora >= :De");
                }

                if (DateTimeHelpers.IsDate(filtro.Ate))
                {
                    var dataTermino = Convert.ToDateTime(filtro.Ate);
                    parametros.Add(name: "Ate", value: new DateTime(dataTermino.Year, dataTermino.Month, dataTermino.Day, 23, 59, 59), direction: ParameterDirection.Input);
                    filtroSQL.Append(" AND A.DataHora <= :Ate");
                }

                if (filtro.Externo.HasValue)
                {
                    if (filtro.Externo.Value != 2)
                    {
                        parametros.Add(name: "Externo", value: filtro.Externo, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.Externo = :Externo ");
                    }
                }

                if (filtro.Sucesso.HasValue)
                {
                    if (filtro.Sucesso.Value != 2)
                    {
                        parametros.Add(name: "Sucesso", value: filtro.Sucesso, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.Sucesso = :Sucesso ");
                    }
                }
               
                var sql = $@"
                    SELECT * FROM (
                        SELECT LogsAcesso.*, ROWNUM row_num
                            FROM (
                                SELECT 
                                    DISTINCT
                                        A.Id,
                                        DECODE(B.Externo, 1, B.LoginExterno, B.Login) Login,
                                        A.IP,
                                        A.DataHora,
                                        A.Externo,
                                        A.Sucesso,
                                        A.Mensagem,
                                        count(*) over() TotalLinhas
                                    FROM
                                        TB_CRM_LOG_ACESSO A 
                                    INNER JOIN
                                        TB_CRM_USUARIOS B ON A.UsuarioId = B.Id 
                                    WHERE
                                        A.Id > 0 {filtroSQL.ToString()} {orderBy}) LogsAcesso
                            WHERE ROWNUM < (({pagina} * {registrosPorPagina}) + 1)) 
                        WHERE row_num >= ((({pagina} - 1) * {registrosPorPagina}) + 1)";


                var query = con.Query<AuditoriaAcessosDTO>(sql, parametros);

                totalFiltro = query.Select(c => c.TotalLinhas).FirstOrDefault();

                return query;
            }
        }

        public int ObterTotalLogsAcessos()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<int>($@"SELECT COUNT(1) FROM TB_CRM_LOG_ACESSO").Single();
            }
        }
    }
}
