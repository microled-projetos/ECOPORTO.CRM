using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class PremioParceriaRepositorio : IPremioParceriaRepositorio
    {
        public int Cadastrar(OportunidadePremioParceria premioParceria)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "OportunidadeId", value: premioParceria.OportunidadeId, direction: ParameterDirection.Input);
                    parametros.Add(name: "StatusPremioParceria", value: premioParceria.StatusPremioParceria, direction: ParameterDirection.Input);
                    parametros.Add(name: "Favorecido1", value: premioParceria.Favorecido1, direction: ParameterDirection.Input);
                    parametros.Add(name: "Favorecido2", value: premioParceria.Favorecido2, direction: ParameterDirection.Input);
                    parametros.Add(name: "Favorecido3", value: premioParceria.Favorecido3, direction: ParameterDirection.Input);
                    parametros.Add(name: "Instrucao", value: premioParceria.Instrucao, direction: ParameterDirection.Input);
                    parametros.Add(name: "ContatoId", value: premioParceria.ContatoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "PremioReferenciaId", value: premioParceria.PremioReferenciaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoServicoPremioParceria", value: premioParceria.TipoServicoPremioParceria, direction: ParameterDirection.Input);
                    parametros.Add(name: "Observacoes", value: premioParceria.Observacoes, direction: ParameterDirection.Input);
                    parametros.Add(name: "AnexoId", value: premioParceria.AnexoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "PremioRevisaoId", value: premioParceria.PremioRevisaoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "UrlPremio", value: premioParceria.UrlPremio, direction: ParameterDirection.Input);
                    parametros.Add(name: "DataUrlPremio", value: premioParceria.DataUrlPremio, direction: ParameterDirection.Input);
                    parametros.Add(name: "EmailFavorecido1", value: premioParceria.EmailFavorecido1, direction: ParameterDirection.Input);
                    parametros.Add(name: "EmailFavorecido2", value: premioParceria.EmailFavorecido2, direction: ParameterDirection.Input);
                    parametros.Add(name: "EmailFavorecido3", value: premioParceria.EmailFavorecido3, direction: ParameterDirection.Input);
                    parametros.Add(name: "CriadoPor", value: premioParceria.CriadoPor, direction: ParameterDirection.Input);
                    parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    con.Execute(@"
                        INSERT INTO 
                            CRM.TB_CRM_OPORTUNIDADE_PREMIOS 
                                ( 
                                    Id, 
                                    OportunidadeId, 
                                    StatusPremioParceria, 
                                    Favorecido1,                 
                                    Favorecido2, 
                                    Favorecido3, 
                                    Instrucao, 
                                    ContatoId, 
                                    PremioReferenciaId,
                                    TipoServicoPremioParceria,
                                    Observacoes,
                                    AnexoId,
                                    PremioRevisaoId,
                                    UrlPremio, 
                                    DataUrlPremio, 
                                    EmailFavorecido1,
                                    EmailFavorecido2,
                                    EmailFavorecido3,
                                    CriadoPor,
                                    DataCadastro
                                ) VALUES ( 
                                    CRM.SEQ_CRM_OPORTUNIDADE_PREMIOS.NEXTVAL, 
                                    :OportunidadeId, 
                                    :StatusPremioParceria, 
                                    :Favorecido1,                 
                                    :Favorecido2, 
                                    :Favorecido3, 
                                    :Instrucao, 
                                    :ContatoId, 
                                    :PremioReferenciaId,
                                    :TipoServicoPremioParceria,
                                    :Observacoes,
                                    :AnexoId, 
                                    :PremioRevisaoId,
                                    :UrlPremio, 
                                    :DataUrlPremio, 
                                    :EmailFavorecido1,
                                    :EmailFavorecido2,
                                    :EmailFavorecido3,
                                    :CriadoPor,
                                    SYSDATE
                                ) RETURNING Id INTO :Id", parametros);

                    var id = parametros.Get<int>("Id");

                    foreach (var modalidade in premioParceria.Modalidades)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "OportunidadeId", value: premioParceria.OportunidadeId, direction: ParameterDirection.Input);
                        parametros.Add(name: "OportunidadePremioId", value: id, direction: ParameterDirection.Input);
                        parametros.Add(name: "Modalidade", value: modalidade, direction: ParameterDirection.Input);

                        con.Execute(@"INSERT INTO CRM.TB_CRM_PREMIOS_MODALIDADES (Id, OportunidadeId, OportunidadePremioId, Modalidade) VALUES (CRM.SEQ_CRM_PREMIOS_MODALIDADES.NEXTVAL, :OportunidadeId, :OportunidadePremioId, :Modalidade)", parametros);
                    }

                    transaction.Commit();

                    return id;
                }
            }
        }

        public void Atualizar(OportunidadePremioParceria premioParceria)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "OportunidadeId", value: premioParceria.OportunidadeId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Favorecido1", value: premioParceria.Favorecido1, direction: ParameterDirection.Input);
                    parametros.Add(name: "Favorecido2", value: premioParceria.Favorecido2, direction: ParameterDirection.Input);
                    parametros.Add(name: "Favorecido3", value: premioParceria.Favorecido3, direction: ParameterDirection.Input);
                    parametros.Add(name: "Instrucao", value: premioParceria.Instrucao, direction: ParameterDirection.Input);
                    parametros.Add(name: "ContatoId", value: premioParceria.ContatoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "PremioReferenciaId", value: premioParceria.PremioReferenciaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoServicoPremioParceria", value: premioParceria.TipoServicoPremioParceria, direction: ParameterDirection.Input);
                    parametros.Add(name: "Observacoes", value: premioParceria.Observacoes, direction: ParameterDirection.Input);
                    parametros.Add(name: "AnexoId", value: premioParceria.AnexoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "UrlPremio", value: premioParceria.UrlPremio, direction: ParameterDirection.Input);
                    parametros.Add(name: "DataUrlPremio", value: premioParceria.DataUrlPremio, direction: ParameterDirection.Input);
                    parametros.Add(name: "EmailFavorecido1", value: premioParceria.EmailFavorecido1, direction: ParameterDirection.Input);
                    parametros.Add(name: "EmailFavorecido2", value: premioParceria.EmailFavorecido2, direction: ParameterDirection.Input);
                    parametros.Add(name: "EmailFavorecido3", value: premioParceria.EmailFavorecido3, direction: ParameterDirection.Input);
                    parametros.Add(name: "Id", value: premioParceria.Id, direction: ParameterDirection.Input);

                    con.Execute(@"
                            UPDATE CRM.TB_CRM_OPORTUNIDADE_PREMIOS 
                                SET                            
                                    Favorecido1 = :Favorecido1,                 
                                    Favorecido2 = :Favorecido2, 
                                    Favorecido3 = :Favorecido3, 
                                    Instrucao = :Instrucao, 
                                    ContatoId = :ContatoId, 
                                    PremioReferenciaId = :PremioReferenciaId,
                                    TipoServicoPremioParceria = :TipoServicoPremioParceria,
                                    Observacoes = :Observacoes,
                                    AnexoId = :AnexoId, 
                                    UrlPremio = :UrlPremio, 
                                    DataUrlPremio = :DataUrlPremio, 
                                    EmailFavorecido1 = :EmailFavorecido1,
                                    EmailFavorecido2 = :EmailFavorecido2,
                                    EmailFavorecido3 = :EmailFavorecido3
                                WHERE Id = :Id", parametros);

                    con.Execute(@"DELETE FROM CRM.TB_CRM_PREMIOS_MODALIDADES WHERE OportunidadePremioId = :premioParceriaId AND OportunidadeId = :oportunidadeId", new { premioParceriaId = premioParceria.Id, oportunidadeId = premioParceria.OportunidadeId });

                    foreach (var modalidade in premioParceria.Modalidades)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "OportunidadeId", value: premioParceria.OportunidadeId, direction: ParameterDirection.Input);
                        parametros.Add(name: "OportunidadePremioId", value: premioParceria.Id, direction: ParameterDirection.Input);
                        parametros.Add(name: "Modalidade", value: modalidade, direction: ParameterDirection.Input);

                        con.Execute(@"INSERT INTO CRM.TB_CRM_PREMIOS_MODALIDADES (Id, OportunidadeId, OportunidadePremioId, Modalidade) VALUES (CRM.SEQ_CRM_PREMIOS_MODALIDADES.NEXTVAL, :OportunidadeId, :OportunidadePremioId, :Modalidade)", parametros);
                    }

                    transaction.Commit();
                }
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                    con.Execute(@"DELETE FROM CRM.TB_CRM_ANEXOS WHERE Id IN (SELECT AnexoId FROM TB_CRM_OPORTUNIDADE_PREMIOS WHERE Id = :Id)", parametros);
                    con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_PREMIOS WHERE Id = :Id", parametros);

                    transaction.Commit();
                }
            }
        }

        public void CancelarPremioParceria(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADE_PREMIOS SET StatusPremioParceria = 6, Cancelado = 1 WHERE Id = :id", new { id });
            }
        }

        public IEnumerable<PremioParceriaDTO> ObterPremiosParceriaPorOportunidade(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<PremioParceriaDTO>(@"
                        SELECT 
                            A.Id,
                            F.Login As CriadoPor,
                            A.StatusPremioParceria,
                            C.NomeFantasia as Favorecido1,
                            D.NomeFantasia as Favorecido2,
                            E.NomeFantasia as Favorecido3,
                            A.Instrucao,
                            A.DataCadastro,
                            RAWTOHEX(G.IdFile) IdFile,
                            A.PremioReferenciaId,
                            A.OportunidadeId,
                            B.StatusOportunidade,
                            'P-' || A.PremioReferenciaId As PremioReferenciaDescricao,
                            (SELECT OportunidadeId FROM CRM.TB_CRM_OPORTUNIDADE_PREMIOS WHERE Id = A.PremioReferenciaId) OportunidadePremioReferencia
                        FROM
                            CRM.TB_CRM_OPORTUNIDADE_PREMIOS A
                        INNER JOIN
                            CRM.TB_CRM_OPORTUNIDADES B ON A.OportunidadeId = B.Id
                        LEFT JOIN
                            CRM.TB_CRM_CONTAS C ON A.Favorecido1 = C.Id
                        LEFT JOIN
                            CRM.TB_CRM_CONTAS D ON A.Favorecido2 = D.Id
                        LEFT JOIN
                            CRM.TB_CRM_CONTAS E ON A.Favorecido3 = E.Id
                        LEFT JOIN
                            CRM.TB_CRM_USUARIOS F ON A.CriadoPor = F.Id
                        LEFT JOIN
                            CRM.TB_CRM_ANEXOS G ON A.AnexoId = G.Id
                        WHERE
                            A.OportunidadeId = :oportunidadeId
                        ORDER BY
                            A.DataCadastro DESC", new { oportunidadeId });
            }
        }

        public OportunidadePremioParceria ObterPremioParceriaPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<OportunidadePremioParceria>(@"SELECT * FROM CRM.TB_CRM_OPORTUNIDADE_PREMIOS WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }

        public IEnumerable<OportunidadePremioParceria> ObterPremiosParceriaPorStatus(StatusPremioParceria statusPremioParceria)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "StatusPremioParceria", value: statusPremioParceria, direction: ParameterDirection.Input);

                return con.Query<OportunidadePremioParceria>($@"SELECT * FROM CRM.TB_CRM_OPORTUNIDADE_PREMIOS WHERE StatusPremioParceria = :StatusPremioParceria", parametros);
            }
        }

        public IEnumerable<OportunidadePremioParceriaModalidade> ObterModalidades(int premioParceriaId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<OportunidadePremioParceriaModalidade>(@"
                        SELECT 
                            OportunidadeId,
                            OportunidadePremioId,
                            Modalidade
                        FROM
                            CRM.TB_CRM_PREMIOS_MODALIDADES
                        WHERE
                            OportunidadePremioId = :premioParceriaId", new { premioParceriaId });
            }
        }

        public void AtualizarStatusPremioParceria(StatusPremioParceria status, int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "StatusPremioParceria", value: status, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADE_PREMIOS SET StatusPremioParceria = :StatusPremioParceria WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<PremioParceriaDTO> ObterPremiosParceriaPorDescricao(string descricao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var criterio = $"{descricao}%";

                return con.Query<PremioParceriaDTO>(@"SELECT Id FROM TB_CRM_OPORTUNIDADE_PREMIOS WHERE Id LIKE :criterio AND ROWNUM < 15", new { criterio });
            }
        }

        public PremioParceriaDetalhesDTO ObterDetalhesPremioParceria(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<PremioParceriaDetalhesDTO>(@"SELECT * FROM CRM.VW_CRM_PREMIOS_PARCERIA WHERE Id = :Id", parametros).FirstOrDefault();
            }
        }

        public bool ExistePremioParceria(OportunidadePremioParceria premioParceria)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: premioParceria.OportunidadeId, direction: ParameterDirection.Input);                                

                return con.Query<bool>(@"SELECT * FROM TB_CRM_OPORTUNIDADE_PREMIOS WHERE StatusPremioParceria = 2 AND OportunidadeId = :OportunidadeId", parametros).Any();
            }
        }

        public void AtualizarCancelamento(bool cancelado)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Cancelado", value: StatusOportunidade.CANCELADA, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADE_PREMIOS SET Cancelado = :Cancelado WHERE Id = :Id", parametros);
            }
        }
    }
}
