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
    public class SimuladorRepositorio : ISimuladorRepositorio
    {
        public int CadastrarSimulador(Simulador simulador)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: simulador.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Regime", value: simulador.Regime, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: simulador.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ArmadorId", value: simulador.ArmadorId, direction: ParameterDirection.Input);
                parametros.Add(name: "LocalAtracacaoId", value: simulador.LocalAtracacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "GrupoAtracacaoId", value: simulador.GrupoAtracacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: simulador.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodos", value: simulador.Periodos, direction: ParameterDirection.Input);
                parametros.Add(name: "CifConteiner", value: simulador.CifConteiner, direction: ParameterDirection.Input);
                parametros.Add(name: "CifCargaSolta", value: simulador.CifCargaSolta, direction: ParameterDirection.Input);
                parametros.Add(name: "VolumeM3", value: simulador.VolumeM3, direction: ParameterDirection.Input);
                parametros.Add(name: "NumeroLotes", value: simulador.NumeroLotes, direction: ParameterDirection.Input);
                parametros.Add(name: "Usuario", value: simulador.CriadoPor, direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"
                        INSERT INTO SGIPA.TB_SIMULADOR_CALCULO 
                            ( 
                                Autonum,
                                Descricao,
                                Regime,
                                Tipo_Documento,
                                Armador,
                                Local_Atracacao,
                                Grupo_Atracacao,
                                Margem,
                                Periodos,
                                Valor_Cif_Cntr,
                                Valor_Cif_Cs,
                                Volume_M3,
                                Numero_Lotes,
                                Usuario,
                                Flag_CRM,
                                Data_Hora
                            ) VALUES ( 
                                SGIPA.SEQ_SIMULADOR_CALCULO.NEXTVAL, 
                                :Descricao,
                                :Regime,
                                :TipoDocumentoId,
                                :ArmadorId,
                                :LocalAtracacaoId,
                                :GrupoAtracacaoId,
                                :Margem,
                                :Periodos,
                                :CifConteiner,
                                :CifCargaSolta,
                                :VolumeM3,
                                :NumeroLotes,
                                :Usuario,
                                1,
                                SYSDATE
                            ) RETURNING AUTONUM INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void AtualizarSimulador(Simulador simulador)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: simulador.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Regime", value: simulador.Regime, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: simulador.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ArmadorId", value: simulador.ArmadorId, direction: ParameterDirection.Input);
                parametros.Add(name: "LocalAtracacaoId", value: simulador.LocalAtracacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "GrupoAtracacaoId", value: simulador.GrupoAtracacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: simulador.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodos", value: simulador.Periodos, direction: ParameterDirection.Input);
                parametros.Add(name: "CifConteiner", value: simulador.CifConteiner, direction: ParameterDirection.Input);
                parametros.Add(name: "CifCargaSolta", value: simulador.CifCargaSolta, direction: ParameterDirection.Input);
                parametros.Add(name: "VolumeM3", value: simulador.VolumeM3, direction: ParameterDirection.Input);
                parametros.Add(name: "NumeroLotes", value: simulador.NumeroLotes, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: simulador.Id, direction: ParameterDirection.Input);

                con.Execute(@"
                        UPDATE SGIPA.TB_SIMULADOR_CALCULO 
                            SET
                                Descricao = :Descricao,
                                Regime = :Regime,
                                Tipo_Documento = :TipoDocumentoId,
                                Armador = :ArmadorId,
                                Local_Atracacao = :LocalAtracacaoId,
                                Grupo_Atracacao = :GrupoAtracacaoId,
                                Margem = :Margem,
                                Periodos = :Periodos,
                                Valor_Cif_Cntr = :CifConteiner,
                                Valor_Cif_Cs = :CifCargaSolta,
                                Volume_M3 = :VolumeM3,
                                Numero_Lotes = :NumeroLotes
                            WHERE 
                                Autonum = :Id", parametros);
            }
        }

        public void ExcluirSimulador(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM SGIPA.TB_SIMULADOR_CALCULO WHERE Autonum = :Id", parametros);
            }
        }

        public void IncluirCargaConteiner(SimuladorCargaConteiner simuladorCargaConteiner)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "SimuladorId", value: simuladorCargaConteiner.SimuladorId, direction: ParameterDirection.Input);
                parametros.Add(name: "Tamanho", value: simuladorCargaConteiner.Tamanho, direction: ParameterDirection.Input);
                parametros.Add(name: "Peso", value: simuladorCargaConteiner.Peso, direction: ParameterDirection.Input);
                parametros.Add(name: "Quantidade", value: simuladorCargaConteiner.Quantidade, direction: ParameterDirection.Input);
                parametros.Add(name: "UsuarioId", value: simuladorCargaConteiner.UsuarioId, direction: ParameterDirection.Input);

                con.Execute(@"
                        INSERT INTO SGIPA.TB_SIMULADOR_CARGA_CNTR 
                            ( 
                                Autonum,
                                Autonum_Calculo,
                                Tamanho,
                                Peso,
                                Qtde,
                                Usuario
                            ) VALUES ( 
                                SGIPA.SEQ_SIMULADOR_CALCULO_CNTR.NEXTVAL, 
                                :SimuladorId,
                                :Tamanho,
                                :Peso,
                                :Quantidade,
                                :UsuarioId
                            )", parametros);
            }
        }

        public void IncluirCargaSolta(SimuladorCargaSolta simuladorCargaSolta)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "SimuladorId", value: simuladorCargaSolta.SimuladorId, direction: ParameterDirection.Input);
                parametros.Add(name: "Quantidade", value: simuladorCargaSolta.Quantidade, direction: ParameterDirection.Input);
                parametros.Add(name: "VolumeM3", value: simuladorCargaSolta.VolumeM3, direction: ParameterDirection.Input);
                parametros.Add(name: "Peso", value: simuladorCargaSolta.Peso, direction: ParameterDirection.Input);
                parametros.Add(name: "UsuarioId", value: simuladorCargaSolta.UsuarioId, direction: ParameterDirection.Input);

                con.Execute(@"
                        INSERT INTO SGIPA.TB_SIMULADOR_CARGA_CS 
                            ( 
                                Autonum,
                                Autonum_Calculo,
                                Qtde,
                                Volume_M3,
                                Peso,
                                Usuario
                            ) VALUES ( 
                                SGIPA.SEQ_SIMULADOR_CALCULO_CS.NEXTVAL, 
                                :SimuladorId,
                                :Quantidade,
                                :VolumeM3,
                                :Peso,
                                :UsuarioId
                            )", parametros);
            }
        }

        public void ExcluirCargaConteiner(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM SGIPA.TB_SIMULADOR_CARGA_CNTR WHERE AUTONUM = :Id", parametros);
            }
        }

        public void ExcluirCargaSolta(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM SGIPA.TB_SIMULADOR_CARGA_CS WHERE AUTONUM = :Id", parametros);
            }
        }

        public SimuladorCargaConteiner ObterCargaConteinerPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<SimuladorCargaConteiner>(@"SELECT AUTONUM As Id, AUTONUM_CALCULO As SimuladorId, Tamanho, Peso, Qtde As Quantidade, Usuario As UsuarioId FROM SGIPA.TB_SIMULADOR_CARGA_CNTR WHERE AUTONUM = :Id", parametros).FirstOrDefault();
            }
        }

        public SimuladorCargaSolta ObterCargaSoltaPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<SimuladorCargaSolta>(@"SELECT AUTONUM As Id, AUTONUM_CALCULO As SimuladorId, Volume_M3 As VolumeM3, Peso, Qtde As Quantidade, Usuario As UsuarioId FROM SGIPA.TB_SIMULADOR_CARGA_CS WHERE AUTONUM = :Id", parametros).FirstOrDefault();
            }
        }

        public IEnumerable<SimuladorDTO> ObterSimuladores()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<SimuladorDTO>(@"
                    SELECT
                          A.AUTONUM As Id,
                          A.DESCRICAO,
                          TO_CHAR(A.DATA_HORA,'DD/MM/YYYY HH24:MI') DataHora,
                          F.Usuario,
                          DECODE(A.REGIME,0,'FCL',1,'LCL') REGIME,
                          B.DESCR AS TipoDocumento,
                          C.FANTASIA AS ARMADOR,
                          E.DESCR || ' ' || E.GRUPO AS LocalAtracacao,
                          A.VALOR_CIF_CNTR As CifConteiner,
                          A.VALOR_CIF_CS As CifCargaSolta,
                          A.PERIODOS
                    FROM
                        SGIPA.TB_SIMULADOR_CALCULO A
                    LEFT JOIN
                        SGIPA.TB_TIPOS_DOCUMENTOS B ON A.TIPO_DOCUMENTO = B.CODE
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS C ON A.ARMADOR = C.AUTONUM
                    LEFT JOIN
                        SGIPA.DTE_TB_ARMAZENS E ON A.LOCAL_ATRACACAO = E.CODE
                    LEFT JOIN
                        SGIPA.TB_CAD_USUARIOS F ON A.USUARIO = F.AUTONUM
                    WHERE
                        A.FLAG_CRM >= 0");
            }
        }

        public IEnumerable<SimuladorDTO> ObterSimuladoresPorUsuario(int usuarioId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "UsuarioId", value: usuarioId, direction: ParameterDirection.Input);

                return con.Query<SimuladorDTO>(@"
                    SELECT
                          A.AUTONUM As Id,
                          A.DESCRICAO,
                          TO_CHAR(A.DATA_HORA,'DD/MM/YYYY HH24:MI') DataHora,
                          F.Login As Usuario,
                          DECODE(A.REGIME,0,'FCL',1,'LCL') REGIME,
                          B.DESCR AS TipoDocumento,
                          C.FANTASIA AS ARMADOR,
                          E.DESCR || ' ' || E.GRUPO AS LocalAtracacao,
                          A.VALOR_CIF_CNTR As CifConteiner,
                          A.VALOR_CIF_CS As CifCargaSolta,
                          A.PERIODOS
                    FROM
                        SGIPA.TB_SIMULADOR_CALCULO A
                    LEFT JOIN
                        SGIPA.TB_TIPOS_DOCUMENTOS B ON A.TIPO_DOCUMENTO = B.CODE
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS C ON A.ARMADOR = C.AUTONUM
                    LEFT JOIN
                        SGIPA.DTE_TB_ARMAZENS E ON A.LOCAL_ATRACACAO = E.CODE
                    LEFT JOIN
                        CRM.TB_CRM_USUARIOS F ON A.USUARIO = F.Id
                    WHERE
                        A.FLAG_CRM > 0
                    AND
                        A.USUARIO = :UsuarioId", parametros);
            }
        }

        public SimuladorDTO ObterDetalhesSimuladorPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<SimuladorDTO>(@"
                    SELECT 
                        A.AUTONUM As Id, 
                        A.Descricao, 
                        A.Classe_Cliente As Classe,
                        NVL(A.LOCAL_ATRACACAO,0) LocalAtracacaoId,
                        B.DESCR As LocalAtracacaoDescricao,
                        NVL(A.GRUPO_ATRACACAO,0) GrupoAtracacaoId, 
                        C.DESCRICAO As GrupoAtracacaoDescricao,
                        NVL(A.ARMADOR,0) ArmadorId, 
                        D.RAZAO As ArmadorDescricao,
                        D.CGC As ArmadorDocumento,
                        NVL(A.PERIODOS,0) Periodos, 
                        NVL(A.REGIME,0) Regime, 
                        NVL(A.TIPO_DOCUMENTO,0) TipoDocumentoId, 
                        E.DESCR As TipoDocumentoDescricao,
                        NVL(A.VALOR_CIF_CNTR,0) CifConteiner, 
                        NVL(A.VALOR_CIF_CS,0) CifCargaSolta, 
                        NVL(A.MARGEM,'') Margem, 
                        NVL(A.VOLUME_M3,0) VolumeM3, 
                        NVL(A.NUMERO_LOTES,1) NumeroLotes
                    FROM 
                        SGIPA.TB_SIMULADOR_CALCULO A
                    LEFT JOIN
                        SGIPA.DTE_TB_ARMAZENS B ON A.LOCAL_ATRACACAO = B.CODE
                    LEFT JOIN
                        SGIPA.TB_GRUPOS_ATRACACAO C ON A.GRUPO_ATRACACAO = C.ID
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS D ON A.ARMADOR = D.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_TIPOS_DOCUMENTOS E ON A.TIPO_DOCUMENTO = E.CODE
                    WHERE
                        A.AUTONUM = :Id", parametros).FirstOrDefault();
            }
        }     

        public IEnumerable<SimuladorCargaConteiner> ObterCargaConteiner(int simuladorId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                return con.Query<SimuladorCargaConteiner>(@"SELECT A.AUTONUM As Id, A.TAMANHO, A.PESO, A.QTDE As Quantidade, A.NUMERO_LOTES As NumeroLotes FROM SGIPA.TB_SIMULADOR_CARGA_CNTR A WHERE A.AUTONUM_CALCULO = :SimuladorId ORDER BY A.AUTONUM DESC", parametros);
            }
        }

        public IEnumerable<SimuladorCargaSolta> ObterCargaSolta(int simuladorId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                return con.Query<SimuladorCargaSolta>(@"SELECT A.AUTONUM As Id, A.QTDE As Quantidade, A.VOLUME_M3 As VolumeM3, A.PESO FROM SGIPA.TB_SIMULADOR_CARGA_CS A WHERE A.AUTONUM_CALCULO = :SimuladorId ORDER BY A.AUTONUM DESC", parametros);
            }
        }

        public IEnumerable<SimuladorTabelasDTO> ObterTabelasSimulador(string cnpj)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Cnpj", value: cnpj, direction: ParameterDirection.Input);

                return con.Query<SimuladorTabelasDTO>(@"
                    SELECT 
                        DISTINCT
                          A.AUTONUM As Id,
                          A.DESCR As Descricao,
                          B.FANTASIA || ' ' || B.CGC AS Importador,
                          C.FANTASIA || ' ' || C.CGC AS Despachante,
                          D.FANTASIA || ' ' || D.CGC AS NVOCC,
                          E.FANTASIA || ' ' || E.CGC AS Coloader,
                          A.Proposta
                    FROM
                        SGIPA.TB_LISTAS_PRECOS A
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS B ON A.IMPORTADOR = B.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS C ON A.DESPACHANTE = C.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS D ON A.CAPTADOR = D.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS E ON A.COLOADER = E.AUTONUM
                    WHERE
                        B.CGC = :Cnpj OR C.CGC = :Cnpj OR D.CGC = :Cnpj OR E.CGC = :Cnpj", parametros);
            }
        }

        public void LimparServicosSimulador(string proposta)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                var parametros = new DynamicParameters();
                parametros.Add(name: "Proposta", value: proposta, direction: ParameterDirection.Input);

                using (var transaction = con.BeginTransaction())
                {
                    con.Execute($"DELETE FROM CRM.TB_LISTA_FAIXA_PESO WHERE AUTONUMSV IN (SELECT AUTONUM FROM CRM.TB_LISTA_P_S_PERIODO WHERE LISTA = :Proposta)", parametros);
                    con.Execute($"DELETE FROM CRM.TB_LISTA_FAIXA_VOLUME WHERE AUTONUMSV IN (SELECT AUTONUM FROM CRM.TB_LISTA_P_S_PERIODO WHERE LISTA = :Proposta)", parametros);
                    con.Execute($"DELETE FROM CRM.TB_LISTA_P_S_FAIXASCIF_FIX WHERE AUTONUMSV IN (SELECT AUTONUM FROM CRM.TB_LISTA_P_S_PERIODO WHERE LISTA = :Proposta)", parametros);
                    con.Execute($"DELETE FROM CRM.TB_LISTA_P_S_FAIXASCIF_PER WHERE AUTONUMSV IN (SELECT AUTONUM FROM CRM.TB_LISTA_P_S_PERIODO WHERE LISTA = :Proposta)", parametros);

                    con.Execute($"DELETE FROM CRM.TB_LISTA_P_S_PERIODO WHERE LISTA = :Proposta", parametros);

                    con.Execute($"DELETE FROM CRM.TB_LISTA_FAIXA_PESO WHERE AUTONUMSV IN (SELECT AUTONUM FROM CRM.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE LISTA = :Proposta)", parametros);
                    con.Execute($"DELETE FROM CRM.TB_LISTA_FAIXA_VOLUME WHERE AUTONUMSV IN (SELECT AUTONUM FROM CRM.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE LISTA = :Proposta)", parametros);
                    con.Execute($"DELETE FROM CRM.TB_LISTA_P_S_FAIXASCIF_FIX WHERE AUTONUMSV IN (SELECT AUTONUM FROM CRM.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE LISTA = :Proposta)", parametros);
                    con.Execute($"DELETE FROM CRM.TB_LISTA_P_S_FAIXASCIF_PER WHERE AUTONUMSV IN (SELECT AUTONUM FROM CRM.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE LISTA = :Proposta)", parametros);

                    con.Execute($"DELETE FROM CRM.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE LISTA = :Proposta", parametros);

                    transaction.Commit();
                }
            }
        }

        public SimuladorDTO ObterClientesSimulador(string proposta)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Proposta", value: proposta, direction: ParameterDirection.Input);

                return con.Query<SimuladorDTO>($@"
                    SELECT
                        (SELECT MAX(A.ContaId) As Importador FROM TB_CRM_OPORTUNIDADE_CLIENTES A INNER JOIN CRM.TB_CRM_OPORTUNIDADES B ON A.OportunidadeId = B.Id WHERE A.Segmento = 1 AND B.Identificacao = :Proposta) Importador,
                        (SELECT MAX(A.ContaId) As Despachante FROM TB_CRM_OPORTUNIDADE_CLIENTES A INNER JOIN CRM.TB_CRM_OPORTUNIDADES B ON A.OportunidadeId = B.Id  WHERE A.Segmento = 2 AND B.Identificacao = :Proposta) Despachante,
                        (SELECT MAX(A.ContaId) As Coloader FROM TB_CRM_OPORTUNIDADE_CLIENTES A INNER JOIN CRM.TB_CRM_OPORTUNIDADES B ON A.OportunidadeId = B.Id  WHERE A.Segmento = 3 AND B.Identificacao = :Proposta) Coloader,
                        (SELECT MAX(A.ContaId) As Cocoloader FROM TB_CRM_OPORTUNIDADE_CLIENTES A INNER JOIN CRM.TB_CRM_OPORTUNIDADES B ON A.OportunidadeId = B.Id  WHERE A.Segmento = 4 AND B.Identificacao = :Proposta) Cocoloader,
                        (SELECT MAX(A.ContaId) As Cocoloader2 FROM TB_CRM_OPORTUNIDADE_CLIENTES A INNER JOIN CRM.TB_CRM_OPORTUNIDADES B ON A.OportunidadeId = B.Id  WHERE A.Segmento = 5 AND B.Identificacao = :Proposta) Cocoloader2
                    FROM
                        DUAL", parametros).FirstOrDefault();
            }
        }

        public void CadastrarTabela(SimuladorDTO simulador)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Importador", value: simulador.Importador, direction: ParameterDirection.Input);
                parametros.Add(name: "Despachante", value: simulador.Despachante, direction: ParameterDirection.Input);
                parametros.Add(name: "Coloader", value: simulador.Coloader, direction: ParameterDirection.Input);
                parametros.Add(name: "CoColoader", value: simulador.CoColoader, direction: ParameterDirection.Input);
                parametros.Add(name: "CoColoader2", value: simulador.CoColoader2, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: simulador.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "DiasAposGr", value: 0, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamento", value: 0, direction: ParameterDirection.Input);
                parametros.Add(name: "Proposta", value: simulador.Proposta, direction: ParameterDirection.Input);
                parametros.Add(name: "MinimoParteLote", value: 0, direction: ParameterDirection.Input);
                parametros.Add(name: "FlagHubPort", value: 0, direction: ParameterDirection.Input);

                con.Execute($@"
                    INSERT 
                        INTO CRM.TB_LISTAS_PRECOS (
                            AUTONUM, 
                            IMPORTADOR, 
                            DESPACHANTE, 
                            COLOADER, 
                            COCOLOADER, 
                            COCOLOADER2, 
                            DESCR, 
                            DIAS_APOS_GR, 
                            FORMA_PAGAMENTO, 
                            PROPOSTA, 
                            MINIMO_PARTELOTE, 
                            FLAG_HUBPORT)
                        VALUES (
                            :Proposta,
                            :Importador,
                            :Despachante,
                            :Coloader,
                            :CoColoader,
                            :CoColoader2,
                            :Descricao,
                            :DiasAposGr,
                            :FormaPagamento,
                            :Proposta,
                            :MinimoParteLote,
                            :FlagHubPort)", parametros);
            }
        }

        public void ImportarArmazenagem(string proposta, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                var linhas = con.Query<SimuladorDTO>($@"
                    SELECT
                        A.Id,
                        A.TipoRegistro,
                        A.Linha,
                        A.LinhaReferencia,
                        A.OportunidadeId,
                        NVL(Servico.ServicoFaturamentoId, 0) ServicoFaturamentoId,
                        A.Periodo,
                        C.QtdeDias,
                        DECODE(A.TipoCarga, 1, 'SVAR', 2, 'CRGST', 3, 'CPIER') TipoCarga,
                        DECODE(A.BaseCalculo, 1, 'UNID', 2, 'TON', 3, 'CIF', 4, 'CIFM', 5, 'CIF0', 6, 'BL', 7, 'VOLP') BaseCalculo,
                        '' As VarianteLocal,
                        DECODE(1, 21, 2, 22) Moeda,
                        0 As PrecoMinimo,
                        A.Valor20,
                        A.Valor40,
                        A.ValorMinimo,
                        A.ValorMinimo20,
                        A.ValorMinimo40,
                        A.ValorAnvisa,
                        0 As FlagCobrarNVOCC,
                        DECODE(1, 2, 2, 3) FormaPagamento
                     FROM
                        CRM.TB_CRM_OPORTUNIDADE_LAYOUT A
                     LEFT JOIN
                        (
                            SELECT
                                ServicoId,
                                MAX(ServicoFaturamentoId) ServicoFaturamentoId
                            FROM TB_CRM_SERVICO_IPA GROUP BY ServicoId
                        ) Servico ON Servico.ServicoId = A.ServicoId
                     LEFT JOIN
                        CRM.TB_CRM_MODELO C ON A.ModeloId = C.Id
                     WHERE (A.TipoRegistro = 7 OR A.TipoRegistro = 8 OR A.TipoRegistro = 9)
                        AND A.OportunidadeId = :OportunidadeId
                     ORDER BY A.OportunidadeId, A.TipoRegistro, A.Linha", parametros);

                foreach (var linha in linhas)
                {
                    // Armazenagem
                    if (linha.TipoRegistro == 7)
                    {
                        var servico = new SimuladorDTO
                        {
                            Proposta = proposta,
                            Linha = linha.Linha,
                            OportunidadeId = linha.OportunidadeId,
                            BaseCalculo = linha.BaseCalculo,
                            FormaPagamento = linha.FormaPagamento,
                            Moeda = linha.Moeda,
                            Periodo = linha.Periodo,
                            QtdeDias = linha.QtdeDias,
                            ServicoFaturamentoId = linha.ServicoFaturamentoId,
                            TipoCarga = linha.TipoCarga
                        };

                        if (linha.Valor20 != linha.Valor40)
                        {
                            if (linha.Valor20 > 0 && linha.Valor40 > 0)
                            {
                                servico.PrecoUnitario = linha.Valor20;
                                GravarServicosPeriodos(servico);

                                servico.PrecoUnitario = linha.Valor40;
                                GravarServicosPeriodos(servico);
                            }
                        }
                        else
                        {
                            GravarServicosPeriodos(servico);
                        }
                    }

                    // Armazenagem Mínimo
                    if (linha.TipoRegistro == 8)
                    {
                        var servico = new SimuladorDTO
                        {
                            Proposta = proposta,
                            LinhaReferencia = linha.LinhaReferencia,
                            OportunidadeId = linha.OportunidadeId,
                            ServicoFaturamentoId = linha.ServicoFaturamentoId,
                            TipoCarga = linha.TipoCarga
                        };

                        if (linha.ValorMinimo20 == linha.ValorMinimo40)
                        {
                            servico.PrecoMinimo = linha.ValorMinimo20;
                        }
                        else
                        {
                            if (linha.ValorMinimo20 > 0)
                            {
                                servico.PrecoMinimo = linha.ValorMinimo20;
                            }

                            if (linha.ValorMinimo40 > 0)
                            {
                                servico.PrecoMinimo = linha.ValorMinimo40;
                            }
                        }

                        if (servico.LinhaReferencia > 0 && servico.PrecoMinimo > 0)
                            AtualizarServicoMinimoPeriodo(servico);
                    }

                    // Armazenagem All In
                    if (linha.TipoRegistro == 9)
                    {
                        var servico = new SimuladorDTO
                        {
                            Proposta = proposta,
                            LinhaReferencia = linha.LinhaReferencia,
                            OportunidadeId = linha.OportunidadeId,
                            ServicoFaturamentoId = linha.ServicoFaturamentoId,
                            Periodo = linha.Periodo,
                            BaseCalculo = linha.BaseCalculo,
                            PrecoMinimo = linha.ValorMinimo,
                            QtdeDias = linha.QtdeDias
                        };

                        if (linha.Valor20 != linha.Valor40)
                        {
                            if (linha.Valor20 > 0 && linha.Valor40 > 0)
                            {
                                servico.PrecoUnitario = linha.Valor20;
                                GravarServicosPeriodos(servico);

                                servico.PrecoUnitario = linha.Valor40;
                                GravarServicosPeriodos(servico);
                            }
                        }
                        else
                        {
                            GravarServicosPeriodos(servico);
                        }
                    }
                }
            }
        }

        public void ImportarServicoMargem(string proposta, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                var linhas = con.Query<SimuladorDTO>($@"
                    SELECT
                        A.Id,
                        A.TipoRegistro,
                        A.Linha,
                        A.LinhaReferencia,
                        A.OportunidadeId,
                        NVL(Servico.ServicoFaturamentoId, 0) ServicoFaturamentoId,
                        A.Periodo,
                        C.QtdeDias,
                        DECODE(A.TipoCarga, 1, 'SVAR', 2, 'CRGST', 3, 'CPIER') TipoCarga,
                        DECODE(A.BaseCalculo, 1, 'UNID', 2, 'TON', 3, 'CIF', 4, 'CIFM', 5, 'CIF0', 6, 'BL', 7, 'VOLP') BaseCalculo,
                        '' As VarianteLocal,
                        DECODE(1, 21, 2, 22) Moeda,
                        A.ValorMargemDireita,
                        A.ValorMargemEsquerda,
                        A.ValorEntreMargens,
                        A.ValorMinimoMargemDireita,
                        A.ValorMinimoMargemEsquerda,
                        A.ValorMinimoEntreMargens
                     FROM
                        CRM.TB_CRM_OPORTUNIDADE_LAYOUT A
                     LEFT JOIN
                        (
                            SELECT
                                ServicoId,
                                MAX(ServicoFaturamentoId) ServicoFaturamentoId
                            FROM TB_CRM_SERVICO_IPA GROUP BY ServicoId
                        ) Servico ON Servico.ServicoId = A.ServicoId
                     LEFT JOIN
                        CRM.TB_CRM_MODELO C ON A.ModeloId = C.Id
                     WHERE (A.TipoRegistro = 10 OR A.TipoRegistro = 11)
                        AND A.OportunidadeId = :OportunidadeId
                     ORDER BY A.OportunidadeId, A.TipoRegistro, A.Linha", parametros);

                foreach (var linha in linhas)
                {
                    // Serviço para Margem
                    if (linha.TipoRegistro == 10)
                    {
                        var servico = new SimuladorDTO
                        {
                            Proposta = proposta,
                            LinhaReferencia = linha.LinhaReferencia,
                            OportunidadeId = linha.OportunidadeId,
                            ServicoFaturamentoId = linha.ServicoFaturamentoId,
                            TipoCarga = linha.TipoCarga,
                            BaseCalculo = linha.BaseCalculo
                        };

                        if (linha.ValorMargemDireita > 0)
                        {
                            servico.VarianteLocal = "MDIR";
                            servico.PrecoUnitario = linha.ValorMargemDireita;
                            GravarServicosFixos(servico);
                        }

                        if (linha.ValorMargemEsquerda > 0)
                        {
                            servico.VarianteLocal = "MESQ";
                            servico.PrecoUnitario = linha.ValorMargemEsquerda;
                            GravarServicosFixos(servico);
                        }

                        if (linha.ValorEntreMargens > 0)
                        {
                            servico.VarianteLocal = "ENTR";
                            servico.PrecoUnitario = linha.ValorEntreMargens;
                            GravarServicosFixos(servico);
                        }
                    }

                    // Mínimo para Margem
                    if (linha.TipoRegistro == 11)
                    {
                        var servico = new SimuladorDTO
                        {
                            Proposta = proposta,
                            LinhaReferencia = linha.LinhaReferencia,
                            OportunidadeId = linha.OportunidadeId,
                            ServicoFaturamentoId = linha.ServicoFaturamentoId,
                            TipoCarga = linha.TipoCarga,
                            BaseCalculo = linha.BaseCalculo
                        };

                        if (linha.ValorMinimoMargemDireita > 0)
                        {
                            servico.PrecoMinimo = linha.ValorMinimoMargemDireita;

                            if (servico.LinhaReferencia > 0 && servico.PrecoMinimo > 0)
                                AtualizarMinimoFixo(servico);
                        }

                        if (linha.ValorMinimoMargemEsquerda > 0)
                        {
                            servico.PrecoMinimo = linha.ValorMinimoMargemEsquerda;

                            if (servico.LinhaReferencia > 0 && servico.PrecoMinimo > 0)
                                AtualizarMinimoFixo(servico);
                        }

                        if (linha.ValorMinimoEntreMargens > 0)
                        {
                            servico.PrecoMinimo = linha.ValorMinimoEntreMargens;

                            if (servico.LinhaReferencia > 0 && servico.PrecoMinimo > 0)
                                AtualizarMinimoFixo(servico);
                        }
                    }
                }
            }
        }

        public void ImportarServicoMecanicaManual(string proposta, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                var linhas = con.Query<SimuladorDTO>($@"
                    SELECT
                        A.Id,
                        A.TipoRegistro,
                        A.Linha,
                        A.LinhaReferencia,
                        A.OportunidadeId,
                        NVL(Servico.ServicoFaturamentoId, 0) ServicoFaturamentoId,
                        A.Periodo,
                        C.QtdeDias,
                        DECODE(A.TipoCarga, 1, 'SVAR', 2, 'CRGST', 3, 'CPIER') TipoCarga,
                        DECODE(A.BaseCalculo, 1, 'UNID', 2, 'TON', 3, 'CIF', 4, 'CIFM', 5, 'CIF0', 6, 'BL', 7, 'VOLP') BaseCalculo,
                        DECODE(1, 21, 2, 22) Moeda,
                        A.Valor20,
                        A.Valor40
                     FROM
                        CRM.TB_CRM_OPORTUNIDADE_LAYOUT A
                     LEFT JOIN
                        (
                            SELECT
                                ServicoId,
                                MAX(ServicoFaturamentoId) ServicoFaturamentoId
                            FROM TB_CRM_SERVICO_IPA GROUP BY ServicoId
                        ) Servico ON Servico.ServicoId = A.ServicoId
                     LEFT JOIN
                        CRM.TB_CRM_MODELO C ON A.ModeloId = C.Id
                     WHERE (A.TipoRegistro = 12 OR A.TipoRegistro = 13)
                        AND A.OportunidadeId = :OportunidadeId
                     ORDER BY A.OportunidadeId, A.TipoRegistro, A.Linha", parametros);

                foreach (var linha in linhas)
                {
                    // Serviço para Margem
                    if (linha.TipoRegistro == 12)
                    {
                        var servico = new SimuladorDTO
                        {
                            Proposta = proposta,
                            LinhaReferencia = linha.LinhaReferencia,
                            OportunidadeId = linha.OportunidadeId,
                            ServicoFaturamentoId = linha.ServicoFaturamentoId,
                            TipoCarga = linha.TipoCarga,
                            BaseCalculo = linha.BaseCalculo
                        };

                        if (linha.BaseCalculo == "1")
                        {
                            servico.TipoOperacao = "MECANIZADA";
                        }
                        else
                        {
                            servico.TipoOperacao = "MANUAL";
                        }

                        if (linha.Valor20 != linha.Valor40)
                        {
                            if (linha.Valor20 > 0 && linha.Valor40 > 0)
                            {
                                servico.PrecoUnitario = linha.Valor20;
                                GravarServicosFixos(servico);

                                servico.PrecoUnitario = linha.Valor40;
                                GravarServicosFixos(servico);
                            }
                        }
                        else
                        {
                            GravarServicosFixos(servico);
                        }
                    }

                    // Mínimo para Margem
                    if (linha.TipoRegistro == 13)
                    {
                        var servico = new SimuladorDTO
                        {
                            Proposta = proposta,
                            LinhaReferencia = linha.LinhaReferencia,
                            OportunidadeId = linha.OportunidadeId,
                            ServicoFaturamentoId = linha.ServicoFaturamentoId,
                            TipoCarga = linha.TipoCarga,
                            BaseCalculo = linha.BaseCalculo
                        };

                        if (linha.ValorMinimoMargemDireita > 0)
                        {
                            servico.PrecoMinimo = linha.ValorMinimoMargemDireita;

                            if (servico.LinhaReferencia > 0 && servico.PrecoMinimo > 0)
                                AtualizarMinimoFixo(servico);
                        }

                        if (linha.ValorMinimoMargemEsquerda > 0)
                        {
                            servico.PrecoMinimo = linha.ValorMinimoMargemEsquerda;

                            if (servico.LinhaReferencia > 0 && servico.PrecoMinimo > 0)
                                AtualizarMinimoFixo(servico);
                        }

                        if (linha.ValorMinimoEntreMargens > 0)
                        {
                            servico.PrecoMinimo = linha.ValorMinimoEntreMargens;

                            if (servico.LinhaReferencia > 0 && servico.PrecoMinimo > 0)
                                AtualizarMinimoFixo(servico);
                        }
                    }
                }
            }
        }

        public void ImportarServicoLiberacao(string proposta, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                var linhas = con.Query<SimuladorDTO>($@"
                    SELECT
                        A.Id,
                        A.TipoRegistro,
                        A.Linha,
                        A.LinhaReferencia,
                        A.OportunidadeId,
                        NVL(Servico.ServicoFaturamentoId, 0) ServicoFaturamentoId,
                        A.Periodo,
                        C.QtdeDias,
                        DECODE(A.TipoCarga, 1, 'SVAR', 2, 'CRGST', 3, 'CPIER') TipoCarga,
                        DECODE(A.BaseCalculo, 1, 'UNID', 2, 'TON', 3, 'CIF', 4, 'CIFM', 5, 'CIF0', 6, 'BL', 7, 'VOLP') BaseCalculo,
                        DECODE(1, 21, 2, 22) Moeda,
                        A.Valor20,
                        A.Valor40
                     FROM
                        CRM.TB_CRM_OPORTUNIDADE_LAYOUT A
                     LEFT JOIN
                        (
                            SELECT
                                ServicoId,
                                MAX(ServicoFaturamentoId) ServicoFaturamentoId
                            FROM TB_CRM_SERVICO_IPA GROUP BY ServicoId
                        ) Servico ON Servico.ServicoId = A.ServicoId
                     LEFT JOIN
                        CRM.TB_CRM_MODELO C ON A.ModeloId = C.Id
                     WHERE (A.TipoRegistro = 14)
                        AND A.OportunidadeId = :OportunidadeId
                     ORDER BY A.OportunidadeId, A.TipoRegistro, A.Linha", parametros);

                foreach (var linha in linhas)
                {
                    // Serviço para Margem
                    if (linha.TipoRegistro == 14)
                    {
                        var servico = new SimuladorDTO
                        {
                            Proposta = proposta,
                            LinhaReferencia = linha.LinhaReferencia,
                            OportunidadeId = linha.OportunidadeId,
                            ServicoFaturamentoId = linha.ServicoFaturamentoId,
                            TipoCarga = linha.TipoCarga,
                            BaseCalculo = linha.BaseCalculo
                        };

                        if (linha.Valor20 != linha.Valor40)
                        {
                            if (linha.Valor20 > 0 && linha.Valor40 > 0)
                            {
                                servico.PrecoUnitario = linha.Valor20;
                                GravarServicosFixos(servico);

                                servico.PrecoUnitario = linha.Valor40;
                                GravarServicosFixos(servico);
                            }
                        }
                        else
                        {
                            GravarServicosFixos(servico);
                        }
                    }
                }
            }
        }

        public void ImportarServicoHubPort(string proposta, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                var linhas = con.Query<SimuladorDTO>($@"
                    SELECT
                        A.Id,
                        A.TipoRegistro,
                        A.Linha,
                        A.LinhaReferencia,
                        A.OportunidadeId,
                        NVL(Servico.ServicoFaturamentoId, 0) ServicoFaturamentoId,
                        A.Periodo,
                        C.QtdeDias,
                        DECODE(A.TipoCarga, 1, 'SVAR', 2, 'CRGST', 3, 'CPIER') TipoCarga,
                        DECODE(A.BaseCalculo, 1, 'UNID', 2, 'TON', 3, 'CIF', 4, 'CIFM', 5, 'CIF0', 6, 'BL', 7, 'VOLP') BaseCalculo,
                        DECODE(1, 21, 2, 22) Moeda,
                        A.Valor,
                        A.Origem,
                        A.Destino
                     FROM
                        CRM.TB_CRM_OPORTUNIDADE_LAYOUT A
                     LEFT JOIN
                        (
                            SELECT
                                ServicoId,
                                MAX(ServicoFaturamentoId) ServicoFaturamentoId
                            FROM TB_CRM_SERVICO_IPA GROUP BY ServicoId
                        ) Servico ON Servico.ServicoId = A.ServicoId
                     LEFT JOIN
                        CRM.TB_CRM_MODELO C ON A.ModeloId = C.Id
                     WHERE (A.TipoRegistro = 15)
                        AND A.OportunidadeId = :OportunidadeId
                     ORDER BY A.OportunidadeId, A.TipoRegistro, A.Linha", parametros);

                foreach (var linha in linhas)
                {
                    var servico = new SimuladorDTO
                    {
                        Proposta = proposta,
                        LinhaReferencia = linha.LinhaReferencia,
                        OportunidadeId = linha.OportunidadeId,
                        ServicoFaturamentoId = linha.ServicoFaturamentoId,
                        TipoCarga = linha.TipoCarga,
                        BaseCalculo = linha.BaseCalculo,
                        Origem = linha.Origem,
                        PrecoUnitario = linha.Valor
                    };

                    GravarServicosFixos(servico);
                }
            }
        }

        public void ImportarServicosGerais(string proposta, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                var linhas = con.Query<SimuladorDTO>($@"
                    SELECT
                        A.Id,
                        A.TipoRegistro,
                        A.Linha,
                        A.LinhaReferencia,
                        A.OportunidadeId,
                        NVL(Servico.ServicoFaturamentoId, 0) ServicoFaturamentoId,
                        A.Periodo,
                        C.QtdeDias,
                        DECODE(A.TipoCarga, 1, 'SVAR', 2, 'CRGST', 3, 'CPIER') TipoCarga,
                        DECODE(A.BaseCalculo, 1, 'UNID', 2, 'TON', 3, 'CIF', 4, 'CIFM', 5, 'CIF0', 6, 'BL', 7, 'VOLP') BaseCalculo,
                        DECODE(1, 21, 2, 22) Moeda,
                        A.Valor20,
                        A.Valor40
                     FROM
                        CRM.TB_CRM_OPORTUNIDADE_LAYOUT A
                     LEFT JOIN
                        (
                            SELECT
                                ServicoId,
                                MAX(ServicoFaturamentoId) ServicoFaturamentoId
                            FROM TB_CRM_SERVICO_IPA GROUP BY ServicoId
                        ) Servico ON Servico.ServicoId = A.ServicoId
                     LEFT JOIN
                        CRM.TB_CRM_MODELO C ON A.ModeloId = C.Id
                     WHERE (A.TipoRegistro = 16 OR A.TipoRegistro = 17)
                        AND A.OportunidadeId = :OportunidadeId
                     ORDER BY A.OportunidadeId, A.TipoRegistro, A.Linha", parametros);

                foreach (var linha in linhas)
                {
                    // Serviço Geral
                    if (linha.TipoRegistro == 16)
                    {
                        var servico = new SimuladorDTO
                        {
                            Proposta = proposta,
                            LinhaReferencia = linha.LinhaReferencia,
                            OportunidadeId = linha.OportunidadeId,
                            ServicoFaturamentoId = linha.ServicoFaturamentoId,
                            TipoCarga = linha.TipoCarga,
                            BaseCalculo = linha.BaseCalculo,
                            Moeda = linha.Moeda
                        };

                        if (linha.Valor20 != linha.Valor40)
                        {
                            if (linha.Valor20 > 0 && linha.Valor40 > 0)
                            {
                                servico.PrecoUnitario = linha.Valor20;
                                GravarServicosFixos(servico);

                                servico.PrecoUnitario = linha.Valor40;
                                GravarServicosFixos(servico);
                            }
                        }
                        else
                        {
                            GravarServicosFixos(servico);
                        }
                    }

                    // Minimo Geral
                    if (linha.TipoRegistro == 17)
                    {
                        var servico = new SimuladorDTO
                        {
                            Proposta = proposta,
                            LinhaReferencia = linha.LinhaReferencia,
                            OportunidadeId = linha.OportunidadeId,
                            TipoCarga = linha.TipoCarga
                        };

                        if (linha.ValorMinimo20 == linha.ValorMinimo40)
                        {
                            servico.PrecoMinimo = linha.ValorMinimo20;
                        }
                        else
                        {
                            if (linha.ValorMinimo20 > 0)
                            {
                                servico.PrecoMinimo = linha.ValorMinimo20;
                            }

                            if (linha.ValorMinimo40 > 0)
                            {
                                servico.PrecoMinimo = linha.ValorMinimo40;
                            }
                        }

                        if (servico.LinhaReferencia > 0 && servico.PrecoMinimo > 0)
                            AtualizarMinimoFixo(servico);
                    }
                }
            }
        }

        public void GravarServicosPeriodos(SimuladorDTO servicoPeriodo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Lista", value: servicoPeriodo.Proposta, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: servicoPeriodo.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: servicoPeriodo.OportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Servico", value: servicoPeriodo.ServicoFaturamentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: servicoPeriodo.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: servicoPeriodo.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: servicoPeriodo.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: servicoPeriodo.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "PrecoUnitario", value: servicoPeriodo.PrecoUnitario, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: servicoPeriodo.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamento", value: servicoPeriodo.FormaPagamento, direction: ParameterDirection.Input);

                con.Execute($@"
                    INSERT INTO
                         CRM.TB_LISTA_P_S_PERIODO (
	                         AUTONUM,
	                         LISTA,
	                         LINHA,
	                         OPORTUNIDADEID,
	                         SERVICO,
	                         N_PERIODO,
	                         QTDE_DIAS,
	                         TIPO_CARGA,
	                         BASE_CALCULO,
	                         PRECO_UNITARIO,
	                         MOEDA,
	                         FORMA_PAGAMENTO_NVOCC
                         ) VALUES (
	                         CRM.SEQ_LISTA_P_S_PERIODO.NEXTVAL,
	                         :Lista,
	                         :Linha,
	                         :OportunidadeId,
	                         :Servico,
	                         :Periodo,
	                         :QtdeDias,
	                         :TipoCarga,
	                         :BaseCalculo,
	                         :PrecoUnitario,
	                         :Moeda,
	                         :FormaPagamento)", parametros);
            }
        }

        public void AtualizarServicoMinimoPeriodo(SimuladorDTO servicoPeriodo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: servicoPeriodo.LinhaReferencia, direction: ParameterDirection.Input);
                parametros.Add(name: "PrecoMinimo", value: servicoPeriodo.PrecoMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: servicoPeriodo.OportunidadeId, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_LISTA_P_S_PERIODO SET PRECO_MINIMO = :PrecoMinimo WHERE Linha = :Linha AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarMinimoFixo(SimuladorDTO servicoPeriodo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: servicoPeriodo.LinhaReferencia, direction: ParameterDirection.Input);
                parametros.Add(name: "PrecoMinimo", value: servicoPeriodo.PrecoMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: servicoPeriodo.OportunidadeId, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_LISTA_PRECO_SERVICOS_FIXOS SET PRECO_MINIMO = :PrecoMinimo WHERE Linha = :Linha AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void GravarServicosFixos(SimuladorDTO servicoPeriodo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Lista", value: servicoPeriodo.Proposta, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: servicoPeriodo.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: servicoPeriodo.OportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Servico", value: servicoPeriodo.ServicoFaturamentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: servicoPeriodo.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: servicoPeriodo.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "PrecoUnitario", value: servicoPeriodo.PrecoUnitario, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: servicoPeriodo.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamento", value: servicoPeriodo.FormaPagamento, direction: ParameterDirection.Input);
                parametros.Add(name: "VarianteLocal", value: servicoPeriodo.VarianteLocal, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoOperacao", value: servicoPeriodo.TipoOperacao, direction: ParameterDirection.Input);

                con.Execute($@"
                    INSERT INTO
                         CRM.TB_LISTA_PRECO_SERVICOS_FIXOS (
                             AUTONUM,
                             LISTA,
                             LINHA,
                             OPORTUNIDADEID,
                             SERVICO,                             
                             TIPO_CARGA,
                             BASE_CALCULO,
                             PRECO_UNITARIO,
                             MOEDA,
                             FORMA_PAGAMENTO_NVOCC,
                             VARIANTE_LOCAL,
                             TIPO_OPER
                         ) VALUES (
	                         CRM.SEQ_LISTA_P_S_PERIODO.NEXTVAL,
	                         :Lista,
                             :Linha,
                             :OportunidadeId,
                             :Servico,                             
                             :TipoCarga,
                             :BaseCalculo,
                             :PrecoUnitario,
                             :Moeda,
                             :FormaPagamento,
                             :VarianteLocal,
                             :TipoOperacao)", parametros);
            }
        }
    }
}
