using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class SimuladorPropostaRepositorio : ISimuladorPropostaRepositorio
    {
        public int CadastrarParametrosSimulador(SimuladorPropostaParametros simulador)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: simulador.OportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "ModeloSimuladorId", value: simulador.ModeloSimuladorId, direction: ParameterDirection.Input);
                parametros.Add(name: "Regime", value: simulador.Regime, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: simulador.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "GrupoAtracacaoId", value: simulador.GrupoAtracacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: simulador.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodos", value: simulador.Periodos, direction: ParameterDirection.Input);
                parametros.Add(name: "VolumeM3", value: simulador.VolumeM3, direction: ParameterDirection.Input);
                parametros.Add(name: "Peso", value: simulador.Peso, direction: ParameterDirection.Input);
                parametros.Add(name: "NumeroLotes", value: simulador.NumeroLotes, direction: ParameterDirection.Input);
                parametros.Add(name: "Usuario", value: simulador.CriadoPor, direction: ParameterDirection.Input);
                parametros.Add(name: "Qtde20", value: simulador.Qtde20, direction: ParameterDirection.Input);
                parametros.Add(name: "Qtde40", value: simulador.Qtde40, direction: ParameterDirection.Input);
                parametros.Add(name: "Observacoes", value: simulador.Observacoes, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorCif", value: simulador.ValorCif, direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"
                        INSERT INTO CRM.TB_CRM_PARAMETROS_SIMULADOR 
                            ( 
                                Id,
                                OportunidadeId,
                                ModeloSimuladorId,
                                Regime,
                                DocumentoId,
                                GrupoAtracacaoId,
                                Margem,
                                Periodos,                                
                                VolumeM3,
                                Peso,
                                NumeroLotes,
                                UsuarioId,
                                Qtde20,
                                Qtde40,
                                Observacoes,
                                ValorCif
                            ) VALUES ( 
                                CRM.SEQ_CRM_PARAMETROS_SIMULADOR.NEXTVAL, 
                                :OportunidadeId,
                                :ModeloSimuladorId,
                                :Regime,
                                :TipoDocumentoId,
                                :GrupoAtracacaoId,
                                :Margem,
                                :Periodos,                                
                                :VolumeM3,
                                :Peso,
                                1,
                                :Usuario,
                                :Qtde20,
                                :Qtde40,
                                :Observacoes,
                                :ValorCif
                            ) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void AtualizarParametrosSimulador(SimuladorPropostaParametros simulador)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloSimuladorId", value: simulador.ModeloSimuladorId, direction: ParameterDirection.Input);
                parametros.Add(name: "Regime", value: simulador.Regime, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: simulador.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "GrupoAtracacaoId", value: simulador.GrupoAtracacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: simulador.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodos", value: simulador.Periodos, direction: ParameterDirection.Input);
                parametros.Add(name: "VolumeM3", value: simulador.VolumeM3, direction: ParameterDirection.Input);
                parametros.Add(name: "Peso", value: simulador.Peso, direction: ParameterDirection.Input);
                parametros.Add(name: "NumeroLotes", value: simulador.NumeroLotes, direction: ParameterDirection.Input);
                parametros.Add(name: "Qtde20", value: simulador.Qtde20, direction: ParameterDirection.Input);
                parametros.Add(name: "Qtde40", value: simulador.Qtde40, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorCif", value: simulador.ValorCif, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: simulador.Id, direction: ParameterDirection.Input);

                con.Execute(@"
                        UPDATE CRM.TB_CRM_PARAMETROS_SIMULADOR 
                            SET
                                ModeloSimuladorId = :ModeloSimuladorId,
                                Regime = :Regime,
                                DocumentoId = :TipoDocumentoId,
                                GrupoAtracacaoId = :GrupoAtracacaoId,
                                Margem = :Margem,
                                Periodos = :Periodos,
                                VolumeM3 = :VolumeM3,
                                Peso = :Peso,
                                NumeroLotes = :NumeroLotes,
                                Qtde20 = :Qtde20,
                                Qtde40 = :Qtde40,
                                ValorCif = :ValorCif
                            WHERE
                                Id = :Id", parametros);
            }
        }

        public IEnumerable<OportunidadeParametrosSimuladorDTO> ObterParametrosSimulador(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<OportunidadeParametrosSimuladorDTO>(@"
                    SELECT
                        max(A.ID) AS ID,
                        E.Id As OportunidadeId,
                        A.ModeloSimuladorId As ModeloId,
                        C.Descricao As ModeloDescricao,
                        A.Regime,
                        B.DESCR As TipoDocumento,
                        A.DocumentoId,
                        A.GrupoAtracacaoId,
                        D.DESCRICAO As GrupoAtracacao, 
                        A.Margem,
                        A.Periodos,
                        A.VolumeM3,
                        A.Peso,
                        A.NumeroLotes,
                        A.Qtde20,
                        A.Qtde40,
                        A.ValorCif As CifOportunidade,
                        A.Observacoes                        
                    FROM
                        CRM.TB_CRM_PARAMETROS_SIMULADOR A
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADES O ON A.OportunidadeId = O.Id
                    LEFT JOIN
                        SGIPA.TB_TIPOS_DOCUMENTOS B ON A.DocumentoId = B.CODE
                    LEFT JOIN
                        CRM.TB_CRM_SIMULADOR_MODELO C ON A.ModeloSimuladorId = C.Id
                    LEFT JOIN
                        SGIPA.TB_GRUPOS_ATRACACAO D ON A.GrupoAtracacaoId = D.Id
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADES E ON A.OportunidadeId = E.Id
                    WHERE
                        A.OportunidadeId = :OportunidadeId 
                        GROUP BY
                        E.Id  ,
                        A.ModeloSimuladorId,  
                        C.Descricao,  
                        A.Regime,
                        B.DESCR  ,
                        A.DocumentoId,
                        A.GrupoAtracacaoId,
                        D.DESCRICAO  ,
                        A.Margem,
                        A.Periodos,
                        A.VolumeM3,
                        A.Peso,
                        A.NumeroLotes,
                        A.Qtde20,
                        A.Qtde40,
                        A.ValorCif  ,
                        A.Observacoes             ", parametros);
            }
        }

        public OportunidadeParametrosSimuladorDTO ObterParametroSimuladorPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<OportunidadeParametrosSimuladorDTO>(@"
                    SELECT
                        A.Id,
                        E.Id As OportunidadeId,
                        A.ModeloSimuladorId As ModeloId,
                        C.Descricao As ModeloDescricao,
                        A.Regime,
                        B.DESCR As TipoDocumento,
                        A.DocumentoId,
                        A.GrupoAtracacaoId,
                        D.DESCRICAO As GrupoAtracacao, 
                        A.Margem,
                        A.Periodos,
                        A.VolumeM3,
                        A.Peso,
                        A.NumeroLotes,
                        A.Qtde20,
                        A.Qtde40,
                        A.ValorCif As CifOportunidade,
                        A.Observacoes,
                        A.ValorCif
                    FROM
                        CRM.TB_CRM_PARAMETROS_SIMULADOR A
                    LEFT JOIN
                        SGIPA.TB_TIPOS_DOCUMENTOS B ON A.DocumentoId = B.CODE
                    LEFT JOIN
                        CRM.TB_CRM_SIMULADOR_MODELO C ON A.ModeloSimuladorId = C.Id
                    LEFT JOIN
                        SGIPA.TB_GRUPOS_ATRACACAO D ON A.GrupoAtracacaoId = D.Id
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADES E ON A.OportunidadeId = E.Id
                    WHERE
                        A.Id = :Id", parametros).FirstOrDefault();
            }
        }

        public OportunidadeParametrosSimuladorDTO ExisteParametrosSimulador(int oportunidadeId, int modeloSimuladorId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "ModeloSimuladorId", value: modeloSimuladorId, direction: ParameterDirection.Input);

                return con.Query<OportunidadeParametrosSimuladorDTO>(@"
                    SELECT
                        A.Id,
                        E.Id As OportunidadeId,
                        A.ModeloSimuladorId As ModeloId,
                        C.Descricao As ModeloDescricao,
                        A.Regime,
                        B.DESCR As TipoDocumento,
                        A.DocumentoId,
                        A.GrupoAtracacaoId,
                        D.DESCRICAO As GrupoAtracacao, 
                        A.Margem,
                        A.Periodos,
                        A.VolumeM3,
                        A.Peso,
                        A.NumeroLotes,
                        A.Qtde20,
                        A.Qtde40,
                        A.ValorCif As CifOportunidade,
                        A.Observacoes,
                        A.ValorCif
                    FROM
                        CRM.TB_CRM_PARAMETROS_SIMULADOR A
                    LEFT JOIN
                        SGIPA.TB_TIPOS_DOCUMENTOS B ON A.DocumentoId = B.CODE
                    LEFT JOIN
                        CRM.TB_CRM_SIMULADOR_MODELO C ON A.ModeloSimuladorId = C.Id
                    LEFT JOIN
                        SGIPA.TB_GRUPOS_ATRACACAO D ON A.GrupoAtracacaoId = D.Id
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADES E ON A.OportunidadeId = E.Id
                    WHERE
                        A.OportunidadeId = :OportunidadeId
                    AND
                        A.ModeloSimuladorId = :ModeloSimuladorId", parametros).FirstOrDefault();
            }
        }

        public void ExcluirParametroSimulador(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_PARAMETROS_SIMULADOR WHERE Id = :Id", parametros);
            }
        }
    }
}
