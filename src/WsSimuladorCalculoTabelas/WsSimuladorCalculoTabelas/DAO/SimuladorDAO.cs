using Dapper;
using Ecoporto.CRM.Business.Enums;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using WsSimuladorCalculoTabelas.Configuracao;
using WsSimuladorCalculoTabelas.Models;

namespace WsSimuladorCalculoTabelas.DAO
{
    public class SimuladorDAO
    {
        private readonly bool _crm = false;
        private readonly string _schema = string.Empty;

        public SimuladorDAO(bool crm)
        {
            _crm = crm;
            _schema = crm ? "CRM" : "SGIPA";
        }

        public int CadastrarSimulador(Simulador simulador)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    con.Open();

                    using (var transaction = con.BeginTransaction())
                    {
                        var parametros = new DynamicParameters();

                        parametros.Add(name: "Descricao", value: simulador.Descricao, direction: ParameterDirection.Input);
                        parametros.Add(name: "Regime", value: simulador.Regime, direction: ParameterDirection.Input);
                        parametros.Add(name: "TipoDocumentoId", value: simulador.TipoDocumento, direction: ParameterDirection.Input);
                        parametros.Add(name: "GrupoAtracacaoId", value: simulador.GrupoAtracacao, direction: ParameterDirection.Input);
                        parametros.Add(name: "Margem", value: simulador.Margem, direction: ParameterDirection.Input);
                        parametros.Add(name: "Periodos", value: simulador.Periodos, direction: ParameterDirection.Input);
                        parametros.Add(name: "CifConteiner", value: simulador.ValorCifConteiner, direction: ParameterDirection.Input);
                        parametros.Add(name: "CifCargaSolta", value: simulador.ValorCifCargaSolta, direction: ParameterDirection.Input);
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
                                ) RETURNING AUTONUM INTO :Id", parametros, transaction);

                        var id = parametros.Get<int>("Id");

                        parametros = new DynamicParameters();

                        parametros.Add(name: "SimuladorId", value: id, direction: ParameterDirection.Input);
                        parametros.Add(name: "Peso", value: 0, direction: ParameterDirection.Input);
                        parametros.Add(name: "UsuarioId", value: simulador.CriadoPor, direction: ParameterDirection.Input);

                        if (simulador.Qtde20 > 0)
                        {
                            parametros.Add(name: "Tamanho", value: 20, direction: ParameterDirection.Input);
                            parametros.Add(name: "Quantidade", value: simulador.Qtde20, direction: ParameterDirection.Input);

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
                                    )", parametros, transaction);
                        }

                        if (simulador.Qtde40 > 0)
                        {
                            parametros.Add(name: "Tamanho", value: 40, direction: ParameterDirection.Input);
                            parametros.Add(name: "Quantidade", value: simulador.Qtde40, direction: ParameterDirection.Input);

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
                                    )", parametros, transaction);
                        }

                        transaction.Commit();

                        return id;

                    }
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    con.Open();

                    using (var transaction = con.BeginTransaction())
                    {
                        var parametros = new DynamicParameters();

                        parametros.Add(name: "Descricao", value: simulador.Descricao, direction: ParameterDirection.Input);
                        parametros.Add(name: "Regime", value: simulador.Regime, direction: ParameterDirection.Input);
                        parametros.Add(name: "TipoDocumentoId", value: simulador.TipoDocumento, direction: ParameterDirection.Input);
                        parametros.Add(name: "GrupoAtracacaoId", value: simulador.GrupoAtracacao, direction: ParameterDirection.Input);
                        parametros.Add(name: "Margem", value: simulador.Margem, direction: ParameterDirection.Input);
                        parametros.Add(name: "Periodos", value: simulador.Periodos, direction: ParameterDirection.Input);
                        parametros.Add(name: "CifConteiner", value: simulador.ValorCifConteiner, direction: ParameterDirection.Input);
                        parametros.Add(name: "CifCargaSolta", value: simulador.ValorCifCargaSolta, direction: ParameterDirection.Input);
                        parametros.Add(name: "VolumeM3", value: simulador.VolumeM3, direction: ParameterDirection.Input);
                        parametros.Add(name: "NumeroLotes", value: simulador.NumeroLotes, direction: ParameterDirection.Input);
                        parametros.Add(name: "Usuario", value: simulador.CriadoPor, direction: ParameterDirection.Input);

                        parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        var id = con.Query<int>(@"
                            INSERT INTO SGIPA..TB_SIMULADOR_CALCULO 
                                ( 
                                    Descricao,
                                    Regime,
                                    Tipo_Documento,
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
                                    @Descricao,
                                    @Regime,
                                    @TipoDocumentoId,
                                    @GrupoAtracacaoId,
                                    @Margem,
                                    @Periodos,
                                    @CifConteiner,
                                    @CifCargaSolta,
                                    @VolumeM3,
                                    @NumeroLotes,
                                    @Usuario,
                                    1,
                                    GetDate()
                                ); SELECT CAST(SCOPE_IDENTITY() AS INT)", parametros, transaction).Single();

                        parametros = new DynamicParameters();

                        parametros.Add(name: "SimuladorId", value: id, direction: ParameterDirection.Input);
                        parametros.Add(name: "Peso", value: 0, direction: ParameterDirection.Input);
                        parametros.Add(name: "UsuarioId", value: simulador.CriadoPor, direction: ParameterDirection.Input);

                        if (simulador.Qtde20 > 0)
                        {
                            parametros.Add(name: "Tamanho", value: 20, direction: ParameterDirection.Input);
                            parametros.Add(name: "Quantidade", value: simulador.Qtde20, direction: ParameterDirection.Input);

                            con.Execute(@"
                                INSERT INTO SGIPA..TB_SIMULADOR_CARGA_CNTR 
                                    ( 
                                        Autonum_Calculo,
                                        Tamanho,
                                        Peso,
                                        Qtde,
                                        Usuario
                                    ) VALUES ( 
                                        @SimuladorId,
                                        @Tamanho,
                                        @Peso,
                                        @Quantidade,
                                        @UsuarioId
                                    )", parametros, transaction);
                        }

                        if (simulador.Qtde40 > 0)
                        {
                            parametros.Add(name: "Tamanho", value: 40, direction: ParameterDirection.Input);
                            parametros.Add(name: "Quantidade", value: simulador.Qtde40, direction: ParameterDirection.Input);

                            con.Execute(@"
                                INSERT INTO SGIPA..TB_SIMULADOR_CARGA_CNTR 
                                    ( 
                                        Autonum_Calculo,
                                        Tamanho,
                                        Peso,
                                        Qtde,
                                        Usuario
                                    ) VALUES ( 
                                        @SimuladorId,
                                        @Tamanho,
                                        @Peso,
                                        @Quantidade,
                                        @UsuarioId
                                    )", parametros, transaction);
                        }

                        transaction.Commit();

                        return id;
                    }
                }
            }
        }

        public Simulador ObterDetalhesSimuladorPorId(int id)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: id, direction: ParameterDirection.Input);

                    return con.Query<Simulador>(@"
                        SELECT 
                            AUTONUM As SimuladorId,
                            NVL(NUMERO_LOTES,0) AS NumeroLotes, 
                            NVL(TIPO_DOCUMENTO,0) TipoDocumento, 
                            NVL(GRUPO_ATRACACAO,0) GrupoAtracacao, 
                            NVL(VALOR_CIF_CNTR,0) ValorCifConteiner, 
                            NVL(VALOR_CIF_CS,0) ValorCifCargaSolta, 
                            DECODE(NVL(VOLUME_M3, 0), 0, 1, VOLUME_M3) VolumeM3, 
                            NVL(ARMADOR,0) Armador, 
                            DECODE(NVL(PERIODOS,0), 0, 1, PERIODOS) Periodos, 
                            Margem,
                            Usuario As CriadoPor,
                            CASE WHEN Regime = 0 THEN 'FCL' WHEN Regime = 1 THEN 'LCL' END As Regime
                        FROM 
                            SGIPA.TB_SIMULADOR_CALCULO 
                        WHERE 
                            AUTONUM = :SimuladorId", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: id, direction: ParameterDirection.Input);

                    return con.Query<Simulador>(@"
                        SELECT 
                            AUTONUM As SimuladorId,
                            ISNULL(NUMERO_LOTES,0) AS NumeroLotes, 
                            ISNULL(TIPO_DOCUMENTO,0) TipoDocumento, 
                            ISNULL(GRUPO_ATRACACAO,0) GrupoAtracacao, 
                            ISNULL(VALOR_CIF_CNTR,0) ValorCifConteiner, 
                            ISNULL(VALOR_CIF_CS,0) ValorCifCargaSolta, 
                            CASE WHEN ISNULL(VOLUME_M3, 0) = 0 THEN 1 ELSE VOLUME_M3 END VolumeM3, 
                            ISNULL(ARMADOR,0) Armador, 
                            CASE WHEN ISNULL(PERIODOS,0) = 0 THEN 1 ELSE PERIODOS END Periodos, 
                            Margem,
                            Usuario As CriadoPor,
                            CASE WHEN Regime = 0 THEN 'FCL' WHEN Regime = 1 THEN 'LCL' END As Regime
                        FROM 
                            SGIPA..TB_SIMULADOR_CALCULO 
                        WHERE 
                            AUTONUM = @SimuladorId", parametros).FirstOrDefault();
                }
            }
        }

        public CargaConteiner ObterDadosCargaConteiner20(int simuladorId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    return con.Query<CargaConteiner>(@"SELECT NVL(SUM(PESO),0) Peso, NVL(SUM(QTDE),0) Quantidade FROM SGIPA.TB_SIMULADOR_CARGA_CNTR WHERE AUTONUM_CALCULO = :SimuladorId AND TAMANHO = 20", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    return con.Query<CargaConteiner>(@"SELECT ISNULL(SUM(PESO),0) Peso, ISNULL(SUM(QTDE),0) Quantidade FROM SGIPA..TB_SIMULADOR_CARGA_CNTR WHERE AUTONUM_CALCULO = @SimuladorId AND TAMANHO = 20", parametros).FirstOrDefault();
                }
            }
        }

        public CargaConteiner ObterDadosCargaConteiner40(int simuladorId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    return con.Query<CargaConteiner>(@"SELECT NVL(SUM(PESO),0) Peso, NVL(SUM(QTDE),1) Quantidade FROM SGIPA.TB_SIMULADOR_CARGA_CNTR WHERE AUTONUM_CALCULO = :SimuladorId AND TAMANHO = 40", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    return con.Query<CargaConteiner>(@"SELECT ISNULL(SUM(PESO),0) Peso, ISNULL(SUM(QTDE),1) Quantidade FROM SGIPA..TB_SIMULADOR_CARGA_CNTR WHERE AUTONUM_CALCULO = @SimuladorId AND TAMANHO = 40", parametros).FirstOrDefault();
                }
            }
        }

        public CargaSolta ObterDadosCargaSolta(int simuladorId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    return con.Query<CargaSolta>(@"SELECT NVL(SUM(PESO),0) PESO, NVL(SUM(QTDE),0) Quantidade FROM SGIPA.TB_SIMULADOR_CARGA_CS WHERE AUTONUM_CALCULO = :SimuladorId", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    return con.Query<CargaSolta>(@"SELECT ISNULL(SUM(PESO),0) PESO, ISNULL(SUM(QTDE),0) Quantidade FROM SGIPA..TB_SIMULADOR_CARGA_CS WHERE AUTONUM_CALCULO = @SimuladorId", parametros).FirstOrDefault();
                }
            }
        }

        public int ObterServicoSimuladorId(int simuladorId, int servicoId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ServicoId", value: servicoId, direction: ParameterDirection.Input);

                    return con.Query<int>(@"SELECT NVL(MAX(AUTONUM),0) AUTONUM FROM SGIPA.TB_SIMULADOR_SERVICOS 
                    WHERE AUTONUM_CALCULO = :SimuladorId AND AUTONUM_SERVICO = :ServicoId", parametros).Single();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ServicoId", value: servicoId, direction: ParameterDirection.Input);

                    return con.Query<int>(@"SELECT ISNULL(MAX(AUTONUM),0) AUTONUM FROM SGIPA..TB_SIMULADOR_SERVICOS 
                    WHERE AUTONUM_CALCULO = @SimuladorId AND AUTONUM_SERVICO = @ServicoId", parametros).Single();
                }
            }
        }

        public int GravarServicoSimulador(int simuladorId, int servicoId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ServicoId", value: servicoId, direction: ParameterDirection.Input);

                    parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    con.Execute(@"INSERT INTO SGIPA.TB_SIMULADOR_SERVICOS (AUTONUM, AUTONUM_CALCULO, AUTONUM_SERVICO) VALUES (SGIPA.SEQ_SIMULADOR_SERVICOS.NEXTVAL,:SimuladorId, :ServicoId) RETURNING AUTONUM INTO :Id", parametros);

                    return parametros.Get<int>("Id");
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ServicoId", value: servicoId, direction: ParameterDirection.Input);

                    return con.Query<int>(@"INSERT INTO SGIPA..TB_SIMULADOR_SERVICOS (AUTONUM_CALCULO, AUTONUM_SERVICO) VALUES (@SimuladorId, @ServicoId); SELECT CAST(SCOPE_IDENTITY() AS INT)", parametros).Single();
                }
            }
        }

        public int GravarServicoCalculado(ServicoCalculo servicoCalculo)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ServicoCalculoId", value: servicoCalculo.ServicoCalculoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "BaseCalculo", value: servicoCalculo.BaseCalculo, direction: ParameterDirection.Input);
                    parametros.Add(name: "DescricaoBaseCalculo", value: servicoCalculo.DescricaoBaseCalculo, direction: ParameterDirection.Input);
                    parametros.Add(name: "Lista", value: servicoCalculo.Lista, direction: ParameterDirection.Input);
                    parametros.Add(name: "PrecoUnitario", value: servicoCalculo.PrecoUnitario, direction: ParameterDirection.Input);
                    parametros.Add(name: "PrecoMinimo", value: servicoCalculo.PrecoMinimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "Quantidade", value: servicoCalculo.Quantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "Parcela", value: servicoCalculo.Parcela, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoServico", value: servicoCalculo.TipoServico, direction: ParameterDirection.Input);
                    parametros.Add(name: "Margem", value: servicoCalculo.Margem, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoCarga", value: servicoCalculo.TipoCarga, direction: ParameterDirection.Input);

                    parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    con.Execute(@"
                        INSERT INTO
                              SGIPA.TB_SIMULADOR_SERVICOS_CALC
                                (
                                    AUTONUM,
                                    AUTONUM_SERVICO_CALCULO,
                                    BASE_CALCULO,
                                    DESCR_BASE_CALC,
                                    LISTA,
                                    PRECO_UNITARIO,
                                    PRECO_MINIMO,
                                    QUANTIDADE,
                                    TIPO_SERVICO,
                                    UNIDADE,
                                    VALOR_FINAL,
                                    MARGEM,
                                    TIPO_CARGA
                                ) VALUES (
                                     SGIPA.SEQ_SIMULADOR_SERVICOS_CALC.NEXTVAL,
                                    :ServicoCalculoId,
                                    :BaseCalculo,
                                    :DescricaoBaseCalculo,
                                    :Lista,
                                    :PrecoUnitario,
                                    :PrecoMinimo,
                                    :Quantidade,
                                    :TipoServico,
                                    '',
                                    :Parcela,
                                    :Margem,
                                    :TipoCarga
                                ) RETURNING AUTONUM INTO :Id", parametros);

                    return parametros.Get<int>("Id");
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ServicoCalculoId", value: servicoCalculo.ServicoCalculoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "BaseCalculo", value: servicoCalculo.BaseCalculo, direction: ParameterDirection.Input);
                    parametros.Add(name: "DescricaoBaseCalculo", value: servicoCalculo.DescricaoBaseCalculo, direction: ParameterDirection.Input);
                    parametros.Add(name: "Lista", value: servicoCalculo.Lista, direction: ParameterDirection.Input);
                    parametros.Add(name: "PrecoUnitario", value: servicoCalculo.PrecoUnitario, direction: ParameterDirection.Input);
                    parametros.Add(name: "PrecoMinimo", value: servicoCalculo.PrecoMinimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "Quantidade", value: servicoCalculo.Quantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "Parcela", value: servicoCalculo.Parcela, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoServico", value: servicoCalculo.TipoServico, direction: ParameterDirection.Input);
                    parametros.Add(name: "Margem", value: servicoCalculo.Margem, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoCarga", value: servicoCalculo.TipoCarga, direction: ParameterDirection.Input);

                    parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    return con.Query<int>(@"
                    INSERT INTO
                          SGIPA..TB_SIMULADOR_SERVICOS_CALC
                            (
                                AUTONUM_SERVICO_CALCULO,
                                BASE_CALCULO,
                                DESCR_BASE_CALC,
                                LISTA,
                                PRECO_UNITARIO,
                                PRECO_MINIMO,
                                QUANTIDADE,
                                TIPO_SERVICO,
                                UNIDADE,
                                VALOR_FINAL,
                                MARGEM,
                                TIPO_CARGA
                            ) VALUES (
                                @ServicoCalculoId,
                                @BaseCalculo,
                                @DescricaoBaseCalculo,
                                @Lista,
                                @PrecoUnitario,
                                @PrecoMinimo,
                                @Quantidade,
                                @TipoServico,
                                '',
                                @Parcela,
                                @Margem,
                                @TipoCarga
                            ); SELECT CAST(SCOPE_IDENTITY() AS INT)", parametros).Single();
                }
            }
        }

        public void ExcluirCalculosAntigosSimulador(int simuladorId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    con.Open();

                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    using (var transaction = con.BeginTransaction())
                    {
                        con.Execute($@"DELETE FROM SGIPA.TB_SIMULADOR_SERVICOS_CALC WHERE AUTONUM_SERVICO_CALCULO IN (SELECT AUTONUM FROM SGIPA.TB_SIMULADOR_SERVICOS WHERE AUTONUM_CALCULO = :SimuladorId)", parametros, transaction);
                        con.Execute($@"DELETE FROM SGIPA.TB_SIMULADOR_SERVICOS WHERE AUTONUM_CALCULO = :SimuladorId", parametros, transaction);
                        con.Execute($@"DELETE FROM SGIPA.TB_SIMULADOR_CARGA_CNTR WHERE AUTONUM_CALCULO = :SimuladorId", parametros, transaction);
                        con.Execute($@"DELETE FROM SGIPA.TB_SIMULADOR_CALCULO WHERE AUTONUM = :SimuladorId", parametros, transaction);

                        transaction.Commit();
                    }
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    con.Open();

                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    using (var transaction = con.BeginTransaction())
                    {
                        con.Execute(@"DELETE FROM SGIPA..TB_SIMULADOR_SERVICOS_CALC WHERE AUTONUM_SERVICO_CALCULO IN (SELECT AUTONUM FROM SGIPA..TB_SIMULADOR_SERVICOS WHERE AUTONUM_CALCULO = @SimuladorId)", parametros, transaction);
                        con.Execute(@"DELETE FROM SGIPA..TB_SIMULADOR_SERVICOS WHERE AUTONUM_CALCULO = @SimuladorId", parametros, transaction);
                        con.Execute(@"DELETE FROM SGIPA..TB_SIMULADOR_CARGA_CNTR WHERE AUTONUM_CALCULO = @SimuladorId", parametros, transaction);
                        con.Execute(@"DELETE FROM SGIPA..TB_SIMULADOR_CALCULO WHERE AUTONUM = @SimuladorId", parametros, transaction);

                        transaction.Commit();
                    }
                }
            }
        }

        public void ExcluirCalculosAntigosSimuladorIPA(int simuladorId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    con.Open();

                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    using (var transaction = con.BeginTransaction())
                    {
                        con.Execute($@"DELETE FROM SGIPA.TB_SIMULADOR_SERVICOS_CALC WHERE AUTONUM_SERVICO_CALCULO IN (SELECT AUTONUM FROM SGIPA.TB_SIMULADOR_SERVICOS WHERE AUTONUM_CALCULO = :SimuladorId)", parametros, transaction);
                        con.Execute($@"DELETE FROM SGIPA.TB_SIMULADOR_SERVICOS WHERE AUTONUM_CALCULO = :SimuladorId", parametros, transaction);

                        transaction.Commit();
                    }
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    con.Open();

                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    using (var transaction = con.BeginTransaction())
                    {
                        con.Execute(@"DELETE FROM SGIPA..TB_SIMULADOR_SERVICOS_CALC WHERE AUTONUM_SERVICO_CALCULO IN (SELECT AUTONUM FROM SGIPA..TB_SIMULADOR_SERVICOS WHERE AUTONUM_CALCULO = @SimuladorId)", parametros, transaction);
                        con.Execute(@"DELETE FROM SGIPA..TB_SIMULADOR_SERVICOS WHERE AUTONUM_CALCULO = @SimuladorId", parametros, transaction);

                        transaction.Commit();
                    }
                }
            }
        }

        public int GravarTicketMedio(SimuladorTicketMedio ticketMedio)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "SimuladorId", value: ticketMedio.SimuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ValorTicketMedio", value: ticketMedio.ValorTicketMedio, direction: ParameterDirection.Input);
                    parametros.Add(name: "ValorCif", value: ticketMedio.ValorCif, direction: ParameterDirection.Input);
                    parametros.Add(name: "Regime", value: ticketMedio.Regime, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoDocumento", value: ticketMedio.TipoDocumento, direction: ParameterDirection.Input);
                    parametros.Add(name: "LocalAtracacao", value: ticketMedio.LocalAtracacao, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: ticketMedio.TabelaId, direction: ParameterDirection.Input);

                    parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    con.Execute(@"INSERT INTO SGIPA.TB_LISTA_PRECO_SIMUL (AUTONUM, AUTONUM_CALCULO, DT_SIMUL, VLR_TICKET_MEDIO, CIF, REGIME, TIPO_DOC, LOCAL_ATRACACAO, ID_TABELA) 
                        VALUES (SGIPA.SEQ_LISTA_PRECO_SIMUL.NEXTVAL, :SimuladorId, SYSDATE, :ValorTicketMedio, :ValorCif, :Regime, :TipoDocumento, :LocalAtracacao, :TabelaId) RETURNING AUTONUM INTO :Id", parametros);

                    return parametros.Get<int>("Id");
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "SimuladorId", value: ticketMedio.SimuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ValorTicketMedio", value: ticketMedio.ValorTicketMedio, direction: ParameterDirection.Input);
                    parametros.Add(name: "ValorCif", value: ticketMedio.ValorCif, direction: ParameterDirection.Input);
                    parametros.Add(name: "Regime", value: ticketMedio.Regime, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoDocumento", value: ticketMedio.TipoDocumento, direction: ParameterDirection.Input);
                    parametros.Add(name: "LocalAtracacao", value: ticketMedio.LocalAtracacao, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: ticketMedio.TabelaId, direction: ParameterDirection.Input);

                    parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    return con.Query<int>(@"INSERT INTO SGIPA..TB_LISTA_PRECO_SIMUL (AUTONUM_CALCULO, DT_SIMUL, VLR_TICKET_MEDIO, CIF, REGIME, TIPO_DOC, LOCAL_ATRACACAO, ID_TABELA) 
                    VALUES (@SimuladorId, GETDATE(), @ValorTicketMedio, @ValorCif, @Regime, @TipoDocumento, @LocalAtracacao, @TabelaId); SELECT CAST(SCOPE_IDENTITY() AS INT)", parametros).Single();
                }
            }
        }

        public IEnumerable<Tabela> ObterTabelasCalculadas(int simuladorId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    return con.Query<Tabela>($@"
                    SELECT
                      LISTA As TabelaId, 'ID ' || LISTA || ' / ' || DESCR AS Descricao
                    FROM
                        (
                          SELECT
                             DISTINCT B.LISTA, C.DESCR
                          FROM
                            SGIPA.TB_SIMULADOR_SERVICOS A
                          INNER JOIN
                            SGIPA.TB_SIMULADOR_SERVICOS_CALC B ON A.AUTONUM = B.AUTONUM_SERVICO_CALCULO
                          INNER JOIN
                            {_schema}.TB_LISTAS_PRECOS C ON B.LISTA = C.AUTONUM
                          WHERE
                            A.AUTONUM_CALCULO = :SimuladorId
                          ORDER BY
                            B.LISTA)", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    return con.Query<Tabela>($@"
                        SELECT
                            LISTA As TabelaId, 'ID ' + CONVERT(VARCHAR, LISTA) + ' / ' + DESCR AS Descricao
                        FROM
                            (
                                SELECT
                                    DISTINCT B.LISTA, C.DESCR
                                FROM
			                        SGIPA..TB_SIMULADOR_SERVICOS A
                                INNER JOIN
			                        SGIPA..TB_SIMULADOR_SERVICOS_CALC B ON A.AUTONUM = B.AUTONUM_SERVICO_CALCULO
                                INNER JOIN
			                        {_schema}..TB_LISTAS_PRECOS C ON B.LISTA = C.AUTONUM
                                WHERE
			                        A.AUTONUM_CALCULO = @SimuladorId       
                            )Q", parametros);
                }
            }
        }

        public IEnumerable<ServicoFixoVariavel> ObterServicosSemArmazenagemSimulador(int simuladorId, string regime)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    var filtroFixos = new StringBuilder();
                    var filtroVariaveis = new StringBuilder();

                    if (regime == "LCL")
                    {
                        filtroFixos.Append(" AND C.SERVICO <> 245 AND (C.TIPO_CARGA IN('CRGST','BBK','VEIC') OR D.FLAG_DESOVA = 1) ");
                        filtroVariaveis.Append(" AND (C.TIPO_CARGA IN('CRGST','BBK','VEIC') OR D.FLAG_DESOVA = 1) ");
                    }
                    else
                    {
                        filtroFixos.Append(" AND (C.TIPO_CARGA IN('SVAR','SVAR40','SVAR20') AND D.FLAG_DESOVA = 0) ");
                        filtroVariaveis.Append(" AND (C.TIPO_CARGA IN('SVAR','SVAR40','SVAR20') AND D.FLAG_DESOVA = 0) ");
                    }

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT
                          DISTINCT
                            D.AUTONUM As ServicoId,
                            D.DESCR AS Descricao,
                            C.BASE_CALCULO As BaseCalculo,
                            C.PRECO_MINIMO As PrecoMinimo
                        FROM
                            SGIPA.TB_SIMULADOR_SERVICOS A
                        INNER JOIN
                            SGIPA.TB_SIMULADOR_SERVICOS_CALC B ON A.AUTONUM = B.AUTONUM_SERVICO_CALCULO
                        INNER JOIN
                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS C ON B.LISTA = C.LISTA
                        INNER JOIN
                            SGIPA.TB_SERVICOS_IPA D ON C.SERVICO = D.AUTONUM
                        WHERE
                            A.AUTONUM_CALCULO = :SimuladorId
                 
                        {filtroFixos.ToString()}

                        UNION ALL

                        SELECT
                          DISTINCT
                            D.AUTONUM As ServicoId,
                            D.DESCR AS Descricao,
                            C.BASE_CALCULO As BaseCalculo,
                            C.PRECO_MINIMO As PrecoMinimo
                        FROM
                            SGIPA.TB_SIMULADOR_SERVICOS A
                        INNER JOIN
                            SGIPA.TB_SIMULADOR_SERVICOS_CALC B ON A.AUTONUM = B.AUTONUM_SERVICO_CALCULO
                        INNER JOIN
                            {_schema}.TB_LISTA_P_S_PERIODO C ON B.LISTA = C.LISTA
                        INNER JOIN
                            SGIPA.TB_SERVICOS_IPA D ON C.SERVICO = D.AUTONUM
                        WHERE 
                            C.SERVICO <> 52 
                        AND
                            A.AUTONUM_CALCULO = :SimuladorId
                    
                        {filtroVariaveis.ToString()}

                        ORDER BY
                            1", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    var filtroFixos = new StringBuilder();
                    var filtroVariaveis = new StringBuilder();

                    if (regime == "LCL")
                    {
                        filtroFixos.Append(" AND C.SERVICO <> 245 AND (C.TIPO_CARGA IN('CRGST','BBK','VEIC') OR D.FLAG_DESOVA = 1) ");
                        filtroVariaveis.Append(" AND (C.TIPO_CARGA IN('CRGST','BBK','VEIC') OR D.FLAG_DESOVA = 1) ");
                    }
                    else
                    {
                        filtroFixos.Append(" AND (C.TIPO_CARGA IN('SVAR','SVAR40','SVAR20') AND D.FLAG_DESOVA = 0) ");
                        filtroVariaveis.Append(" AND (C.TIPO_CARGA IN('SVAR','SVAR40','SVAR20') AND D.FLAG_DESOVA = 0) ");
                    }

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT
                          DISTINCT
                            D.AUTONUM As ServicoId,
                            D.DESCR AS Descricao,
                            C.BASE_CALCULO As BaseCalculo,
                            C.PRECO_MINIMO As PrecoMinimo
                        FROM
                            SGIPA..TB_SIMULADOR_SERVICOS A
                        INNER JOIN
                            SGIPA..TB_SIMULADOR_SERVICOS_CALC B ON A.AUTONUM = B.AUTONUM_SERVICO_CALCULO
                        INNER JOIN
                            {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS C ON B.LISTA = C.LISTA
                        INNER JOIN
                            SGIPA..TB_SERVICOS_IPA D ON C.SERVICO = D.AUTONUM
                        WHERE
                            A.AUTONUM_CALCULO = @SimuladorId
                 
                        {filtroFixos.ToString()}

                        UNION ALL

                        SELECT
                          DISTINCT
                            D.AUTONUM As ServicoId,
                            D.DESCR AS Descricao,
                            C.BASE_CALCULO As BaseCalculo,
                            C.PRECO_MINIMO As PrecoMinimo
                        FROM
                            SGIPA..TB_SIMULADOR_SERVICOS A
                        INNER JOIN
                            SGIPA..TB_SIMULADOR_SERVICOS_CALC B ON A.AUTONUM = B.AUTONUM_SERVICO_CALCULO
                        INNER JOIN
                            {_schema}..TB_LISTA_P_S_PERIODO C ON B.LISTA = C.LISTA
                        INNER JOIN
                            SGIPA.TB_SERVICOS_IPA D ON C.SERVICO = D.AUTONUM
                        WHERE 
                            C.SERVICO <> 52 
                        AND
                            A.AUTONUM_CALCULO = @SimuladorId
                    
                        {filtroVariaveis.ToString()}

                        ORDER BY
                            1", parametros);
                }
            }
        }

        public IEnumerable<ServicoFixoVariavel> ObterServicosSimulador(int simuladorId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT * FROM
                        (
                            SELECT
                                DISTINCT
                                B.AUTONUM As ServicoId,
                                B.DESCR As Descricao,
                                C.DESCR_BASE_CALC As BaseCalculo,
                                MAX(C.PRECO_MINIMO) As PrecoMinimo,
                                C.MARGEM,
                                C.TIPO_SERVICO As TipoServico
                            FROM
                                SGIPA.TB_SIMULADOR_SERVICOS A
                            INNER JOIN
                                SGIPA.TB_SERVICOS_IPA B ON A.AUTONUM_SERVICO = B.AUTONUM
                            INNER JOIN
                                SGIPA.TB_SIMULADOR_SERVICOS_CALC C ON A.AUTONUM = C.AUTONUM_SERVICO_CALCULO
                            WHERE
                                A.AUTONUM_CALCULO = :SimuladorId
                            GROUP BY
                                    B.AUTONUM,
                                    B.DESCR,
                                    C.DESCR_BASE_CALC,        
                                    C.MARGEM,
                                    C.TIPO_SERVICO
                        )    
                        PIVOT
                        (
                            Sum(PrecoMinimo) FOR Margem IN ('MDIR' PrecoMinimoMDir, 'MESQ' PrecoMinimoMEsq)
                        )
                        ORDER BY
                            Descricao", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT ServicoId, Descricao, BaseCalculo, MDIR As PrecoMinimoMDir, MESQ As PrecoMinimoMEsq FROM
                        (
                            SELECT
                                B.AUTONUM As ServicoId,
                                B.DESCR As Descricao,
                                C.DESCR_BASE_CALC As BaseCalculo,
                                SUM(C.VALOR_FINAL) As PrecoMinimo,
                                C.MARGEM
                            FROM
                                SGIPA..TB_SIMULADOR_SERVICOS A
                            INNER JOIN
                                SGIPA..TB_SERVICOS_IPA B ON A.AUTONUM_SERVICO = B.AUTONUM
                            INNER JOIN
                                SGIPA..TB_SIMULADOR_SERVICOS_CALC C ON A.AUTONUM = C.AUTONUM_SERVICO_CALCULO
                            WHERE
                                A.AUTONUM_CALCULO = @SimuladorId
                                GROUP BY
                                    B.AUTONUM,
                                    B.DESCR,
                                    C.DESCR_BASE_CALC,        
                                    C.MARGEM,
                                    C.TIPO_SERVICO
                        ) As Q
                        PIVOT
                        (
                            Sum(PrecoMinimo) FOR Margem IN ([MDIR], [MESQ])
                        ) P
                        ORDER BY
                            1", parametros);
                }
            }
        }

        public decimal ObterPercentualServicos(int simuladorId, int servicoId, int tamanho)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: servicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: tamanho == 20 ? "SVAR20" : "SVAR40", direction: ParameterDirection.Input);

                return con.Query<decimal>($@"
                        SELECT
                            MIN(C.Preco_Unitario) / 100
                        FROM
                            SGIPA.TB_SIMULADOR_SERVICOS A
                        INNER JOIN
                            SGIPA.TB_SERVICOS_IPA B ON A.AUTONUM_SERVICO = B.AUTONUM
                        INNER JOIN
                            SGIPA.TB_SIMULADOR_SERVICOS_CALC C ON A.AUTONUM = C.AUTONUM_SERVICO_CALCULO
                        WHERE
                            A.AUTONUM_CALCULO = :SimuladorId 
                        AND 
                            B.AUTONUM = :ServicoId 
                        AND 
                            C.Tipo_CARGA = :TipoCarga", parametros).FirstOrDefault();
            }
        }

        public decimal ObterValorImposto(decimal valorImposto, int simuladorId, int tabelaId, string margem, string tipoCarga = "")
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ValorImposto", value: valorImposto, direction: ParameterDirection.Input);
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Margem", value: margem, direction: ParameterDirection.Input);

                    string filtroSQL = string.Empty;

                    if (!string.IsNullOrWhiteSpace(tipoCarga))
                    {
                        filtroSQL = " AND A.TIPO_CARGA = '" + tipoCarga + "' ";
                    }

                    return con.Query<decimal>($@"SELECT ROUND(NVL(SUM(A.VALOR_FINAL),0) / 0.8575 - NVL(SUM(A.VALOR_FINAL),0),6) TOTAL  
                                FROM SGIPA.TB_SIMULADOR_SERVICOS_CALC A INNER JOIN SGIPA.TB_SIMULADOR_SERVICOS B ON A.AUTONUM_SERVICO_CALCULO = B.AUTONUM 
                                    WHERE B.AUTONUM_CALCULO = :SimuladorId AND A.LISTA = :TabelaId AND A.Margem = :Margem {filtroSQL}", parametros).Single();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ValorImposto", value: valorImposto, direction: ParameterDirection.Input);
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Margem", value: margem, direction: ParameterDirection.Input);

                    string filtroSQL = string.Empty;

                    if (!string.IsNullOrWhiteSpace(tipoCarga))
                    {
                        filtroSQL = " AND A.TIPO_CARGA = '" + tipoCarga + "' ";
                    }

                    return con.Query<decimal>($@"SELECT ROUND(ROUND(ISNULL(SUM(A.VALOR_FINAL),0) / @ValorImposto, 2) - ISNULL(SUM(A.VALOR_FINAL),0),2) TOTAL 
                                FROM SGIPA..TB_SIMULADOR_SERVICOS_CALC A INNER JOIN SGIPA..TB_SIMULADOR_SERVICOS B ON A.AUTONUM_SERVICO_CALCULO = B.AUTONUM 
                                    WHERE B.AUTONUM_CALCULO = @SimuladorId AND A.LISTA = @TabelaId AND A.Margem = @Margem {filtroSQL}", parametros).Single();
                }
            }
        }

        public decimal ObterValorComImposto(decimal valorImposto, int simuladorId, int tabelaId, string margem, string tipoCarga = "")
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ValorImposto", value: valorImposto, direction: ParameterDirection.Input);
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Margem", value: margem, direction: ParameterDirection.Input);
                    string filtroSQL = string.Empty;
                    if (!string.IsNullOrWhiteSpace(tipoCarga))
                    {
                        filtroSQL = " AND A.TIPO_CARGA = '" + tipoCarga + "' ";
                    }
                    return con.Query<decimal>($@"SELECT  ROUND(NVL(SUM(A.VALOR_FINAL),0) / :ValorImposto, 2)    TOTAL 
                                FROM SGIPA.TB_SIMULADOR_SERVICOS_CALC A INNER JOIN SGIPA.TB_SIMULADOR_SERVICOS B ON A.AUTONUM_SERVICO_CALCULO = B.AUTONUM 
                                    WHERE B.AUTONUM_CALCULO = :SimuladorId AND A.LISTA = :TabelaId AND A.Margem = :Margem {filtroSQL}", parametros).Single();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ValorImposto", value: valorImposto, direction: ParameterDirection.Input);
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Margem", value: margem, direction: ParameterDirection.Input);
                    string filtroSQL = string.Empty;
                    if (!string.IsNullOrWhiteSpace(tipoCarga))
                    {
                        filtroSQL = " AND A.TIPO_CARGA = '" + tipoCarga + "' ";
                    }

                    return con.Query<decimal>($@"SELECT ROUND(ISNULL(SUM(A.VALOR_FINAL),0) / @ValorImposto, 2)   TOTAL 
                                FROM SGIPA..TB_SIMULADOR_SERVICOS_CALC A INNER JOIN SGIPA..TB_SIMULADOR_SERVICOS B ON A.AUTONUM_SERVICO_CALCULO = B.AUTONUM 
                                    WHERE B.AUTONUM_CALCULO = @SimuladorId AND A.LISTA = @TabelaId AND A.Margem = @Margem {filtroSQL}", parametros).Single();
                }
            }
        }

        public IEnumerable<ResumoPierHouse> ObterResumoPierHouse(int simuladorId, string regime, string dataPgtoInicial, string dataPgtoFinal, int tabelaId = 0)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "Regime", value: regime, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    var filtro = new StringBuilder();

                    var sql = $@"
                        SELECT
                            TabelaId,
                            Descricao,
                            MAX(FATURADO) Faturado,
                            Lotes
                        FROM
                        (
                        SELECT
                            DISTINCT
                                A.LISTA AS TabelaId,
                                D.DESCR As Descricao,
                                NVL(C.FATURADO, 0) FATURADO,
                                E.DATAPAGAMENTO,
                                NVL(C.QTD, 0) AS LOTES
                        FROM
                            SGIPA.TB_SIMULADOR_SERVICOS_CALC A
                        INNER JOIN
                            SGIPA.TB_SIMULADOR_SERVICOS B ON A.AUTONUM_SERVICO_CALCULO = B.AUTONUM
                        LEFT JOIN
                            (
                                SELECT
                                    TABELA_GR,
                                    COUNT(1) QTD,
                                    MAX(FATURADO) FATURADO
                            FROM (
                                    SELECT
                                        B.TABELA_GR,
                                        B.BL,
                                        SUM(VALOR_GR) FATURADO
                                    FROM
                                        SGIPA.TB_GR_BL B
                                    INNER JOIN
                                    (
                                        SELECT
                                            MAX(CNTR) CNTR,
                                            BL
                                        FROM
                                            SGIPA.TB_AMR_CNTR_BL
                                        GROUP BY
                                            BL
                                    ) C ON B.BL = C.BL
                                    INNER JOIN
                                        SGIPA.TB_CNTR_BL D ON C.CNTR = D.AUTONUM
                                    WHERE
                                        D.REGIME = :Regime
                                    AND
                                        B.STATUS_GR = 'IM'";

                    if (DateTime.TryParse(dataPgtoInicial, out DateTime inicioB))
                    {
                        parametros.Add(name: "DataPgtoInicialB", value: inicioB, direction: ParameterDirection.Input);
                        sql = sql + " AND B.DATAPAGAMENTO >= :DataPgtoInicialB ";
                    }

                    if (DateTime.TryParse(dataPgtoFinal, out DateTime terminoB))
                    {
                        parametros.Add(name: "DataPgtoFinalB", value: terminoB, direction: ParameterDirection.Input);
                        sql = sql + " AND B.DATAPAGAMENTO <= :DataPgtoFinalB ";
                    }

                    sql = sql + $@"
                                GROUP BY
                                        B.TABELA_GR,
                                        B.BL
                                )
                            GROUP BY TABELA_GR) C ON A.LISTA = C.TABELA_GR
                        LEFT JOIN
                            {_schema}.TB_LISTAS_PRECOS D ON A.LISTA = D.AUTONUM
                        LEFT JOIN
                            SGIPA.TB_GR_BL E ON A.LISTA = E.TABELA_GR
                        WHERE
                            B.AUTONUM_CALCULO = :SimuladorId";

                    if (DateTime.TryParse(dataPgtoInicial, out DateTime inicioE))
                    {
                        parametros.Add(name: "DataPgtoInicialE", value: inicioE, direction: ParameterDirection.Input);
                        sql = sql + " AND E.DATAPAGAMENTO >= :DataPgtoInicialE ";
                    }

                    if (DateTime.TryParse(dataPgtoFinal, out DateTime terminoE))
                    {
                        parametros.Add(name: "DataPgtoFinalE", value: terminoE, direction: ParameterDirection.Input);
                        sql = sql + " AND E.DATAPAGAMENTO <= :DataPgtoFinalE ";
                    }

                    if (tabelaId > 0)
                        sql = sql + " AND A.LISTA = :TabelaId ";

                    sql = sql + $@"
                        )
                    GROUP BY
                        TabelaId,
                        Descricao,
                        Lotes";

                    return con.Query<ResumoPierHouse>(sql, parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "Regime", value: regime, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    var filtro = new StringBuilder();

                    var sql = $@"
                        SELECT
                            TabelaId,
                            Descricao,
                            MAX(FATURADO) Faturado,
                            Lotes
                        FROM
                        (
                        SELECT
                            DISTINCT
                                A.LISTA AS TabelaId,
                                D.DESCR As Descricao,
                                ISNULL(C.FATURADO, 0) FATURADO,
                                E.DATAPAGAMENTO,
                                ISNULL(C.QTD, 0) AS LOTES
                        FROM
                            SGIPA..TB_SIMULADOR_SERVICOS_CALC A
                        INNER JOIN
                            SGIPA..TB_SIMULADOR_SERVICOS B ON A.AUTONUM_SERVICO_CALCULO = B.AUTONUM
                        LEFT JOIN
                            (
                                SELECT
                                    TABELA_GR,
                                    COUNT(1) QTD,
                                    MAX(FATURADO) FATURADO
                            FROM (
                                    SELECT
                                        B.TABELA_GR,
                                        B.BL,
                                        SUM(VALOR_GR) FATURADO
                                    FROM
                                        SGIPA..TB_GR_BL B
                                    INNER JOIN
                                    (
                                        SELECT
                                            MAX(CNTR) CNTR,
                                            BL
                                        FROM
                                            SGIPA..TB_AMR_CNTR_BL
                                        GROUP BY
                                            BL
                                    ) C ON B.BL = C.BL
                                    INNER JOIN
                                        SGIPA..TB_CNTR_BL D ON C.CNTR = D.AUTONUM
                                    WHERE
                                        D.REGIME = @Regime
                                    AND
                                        B.STATUS_GR = 'IM'";

                    if (DateTime.TryParse(dataPgtoInicial, out DateTime inicioB))
                    {
                        parametros.Add(name: "DataPgtoInicialB", value: inicioB, direction: ParameterDirection.Input);
                        sql = sql + " AND B.DATAPAGAMENTO >= @DataPgtoInicialB ";
                    }

                    if (DateTime.TryParse(dataPgtoFinal, out DateTime terminoB))
                    {
                        parametros.Add(name: "DataPgtoFinalB", value: terminoB, direction: ParameterDirection.Input);
                        sql = sql + " AND B.DATAPAGAMENTO <= @DataPgtoFinalB ";
                    }

                    sql = sql + $@"
                                GROUP BY
                                        B.TABELA_GR,
                                        B.BL
                                )
                            GROUP BY TABELA_GR) C ON A.LISTA = C.TABELA_GR
                        LEFT JOIN
                             {_schema}.dbo.TB_LISTAS_PRECOS D ON A.LISTA = D.AUTONUM
                        LEFT JOIN
                            SGIPA..TB_GR_BL E ON A.LISTA = E.TABELA_GR
                        WHERE
                            B.AUTONUM_CALCULO = @SimuladorId";

                    if (DateTime.TryParse(dataPgtoInicial, out DateTime inicioE))
                    {
                        parametros.Add(name: "DataPgtoInicialE", value: inicioE, direction: ParameterDirection.Input);
                        sql = sql + " AND E.DATAPAGAMENTO >= @DataPgtoInicialE ";
                    }

                    if (DateTime.TryParse(dataPgtoFinal, out DateTime terminoE))
                    {
                        parametros.Add(name: "DataPgtoFinalE", value: terminoE, direction: ParameterDirection.Input);
                        sql = sql + " AND E.DATAPAGAMENTO <= @DataPgtoFinalE ";
                    }

                    if (tabelaId > 0)
                        sql = sql + " AND A.LISTA = @TabelaId ";

                    sql = sql + $@"
                        )
                    GROUP BY
                        TabelaId,
                        Descricao,
                        Lotes";

                    return con.Query<ResumoPierHouse>(sql, parametros);
                }
            }
        }

        public IEnumerable<ValoresPierHouse> ObterValoresPierHouse(int simuladorId, string regime, string dataPgtoInicial, string dataPgtoFinal, int tabelaId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "Regime", value: regime, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    var filtro = new StringBuilder();

                    var sql = $@"
                    SELECT
                        DISTINCT
                              A.DESCR As Descricao,
                              '{regime}' AS REGIME,
                              A.AUTONUM AS TabelaId,
                              TO_CHAR(A.DATA_INICIO,'DD/MM/YYYY') DataIncial,
                              TO_CHAR(A.FINAL_VALIDADE,'DD/MM/YYYY') DataFinal,
                              CASE WHEN (A.FLAG_LIBERADA = 1 AND (A.FINAL_VALIDADE > SYSDATE OR A.FINAL_VALIDADE IS NULL)) THEN 'Sim' ELSE 'Não' END Status,
                              C.SEQ_GR As SeqGr,
                              C.BL,
                              1 AS LOTES,
                              C.DATAPAGAMENTO, 
                              NVL(C.VALOR_GR,0) FATURADO
                            FROM 
                                {_schema}.TB_LISTAS_PRECOS A
                            INNER JOIN
                                SGIPA.TB_GR_BL C ON A.AUTONUM = C.TABELA_GR
                            INNER JOIN
                            (
                             SELECT 
                                TABELA_GR, 
                                COUNT(1) QTD, 
                                MAX(FATURADO) FATURADO 
                            FROM (  
                                SELECT
                                    B.TABELA_GR,
                                    B.BL,
                                    SUM(VALOR_GR) FATURADO
                                FROM
                                    SGIPA.TB_GR_BL B
                                    INNER JOIN
                                    (SELECT MAX(CNTR) CNTR , BL FROM SGIPA.TB_AMR_CNTR_BL GROUP BY BL) C ON B.BL = C.BL
                                    INNER JOIN
                                    SGIPA.TB_CNTR_BL D ON C.CNTR = D.AUTONUM
                                WHERE
                                    D.REGIME = :Regime AND B.STATUS_GR = 'IM'";

                    if (DateTime.TryParse(dataPgtoInicial, out DateTime inicioB))
                    {
                        parametros.Add(name: "DataPgtoInicialB", value: inicioB, direction: ParameterDirection.Input);
                        sql = sql + " AND B.DATAPAGAMENTO >= :DataPgtoInicialB ";
                    }

                    if (DateTime.TryParse(dataPgtoFinal, out DateTime terminoB))
                    {
                        parametros.Add(name: "DataPgtoFinalB", value: terminoB, direction: ParameterDirection.Input);
                        sql = sql + " AND B.DATAPAGAMENTO <= :DataPgtoFinalB ";
                    }

                    sql = sql + $@"
                            GROUP BY
                                    B.TABELA_GR,B.BL) GROUP BY TABELA_GR
                            ) D ON A.AUTONUM = D.TABELA_GR
                            INNER JOIN
                                SGIPA.TB_SIMULADOR_SERVICOS_CALC E ON A.AUTONUM = E.LISTA
                            INNER JOIN
                                SGIPA.TB_SIMULADOR_SERVICOS F ON E.AUTONUM_SERVICO_CALCULO = F.AUTONUM
                            WHERE
                                 F.AUTONUM_CALCULO = :SimuladorId AND A.AUTONUM = :TabelaId AND C.STATUS_GR = 'IM'";

                    if (DateTime.TryParse(dataPgtoInicial, out DateTime inicioC))
                    {
                        parametros.Add(name: "DataPgtoInicialC", value: inicioC, direction: ParameterDirection.Input);
                        sql = sql + " AND C.DATAPAGAMENTO >= :DataPgtoInicialC ";
                    }

                    if (DateTime.TryParse(dataPgtoFinal, out DateTime terminoC))
                    {
                        parametros.Add(name: "DataPgtoFinalC", value: terminoC, direction: ParameterDirection.Input);
                        sql = sql + " AND C.DATAPAGAMENTO <= :DataPgtoFinalC ";
                    }

                    return con.Query<ValoresPierHouse>(sql, parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "Regime", value: regime, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    var filtro = new StringBuilder();

                    var sql = $@"
                        SELECT
                            DISTINCT
                                  A.DESCR As Descricao,
                                  '{regime}' AS REGIME,
                                  A.AUTONUM AS TabelaId,
                                  CONVERT(VARCHAR, A.DATA_INICIO, 103) DataIncial,
                                  CONVERT(VARCHAR, A.FINAL_VALIDADE,103) DataFinal,
                                  CASE WHEN (A.FLAG_LIBERADA = 1 AND (A.FINAL_VALIDADE > GETDATE() OR A.FINAL_VALIDADE IS NULL)) THEN 'Sim' ELSE 'Não' END Status,
                                  C.SEQ_GR As SeqGr,
                                  C.BL,
                                  1 AS LOTES,
                                  C.DATAPAGAMENTO, 
                                  ISNULL(C.VALOR_GR,0) FATURADO
                                FROM 
                                    {_schema}..TB_LISTAS_PRECOS A
                                INNER JOIN
                                    SGIPA..TB_GR_BL C ON A.AUTONUM = C.TABELA_GR
                                INNER JOIN
                                (
                                 SELECT 
                                    TABELA_GR, 
                                    COUNT(1) QTD, 
                                    MAX(FATURADO) FATURADO 
                                FROM (  
                                    SELECT
                                        B.TABELA_GR,
                                        B.BL,
                                        SUM(VALOR_GR) FATURADO
                                    FROM
                                        SGIPA..TB_GR_BL B
                                        INNER JOIN
                                        (SELECT MAX(CNTR) CNTR , BL FROM SGIPA..TB_AMR_CNTR_BL GROUP BY BL) C ON B.BL = C.BL
                                        INNER JOIN
                                        SGIPA..TB_CNTR_BL D ON C.CNTR = D.AUTONUM
                                    WHERE
                                        D.REGIME = @Regime AND B.STATUS_GR = 'IM'";

                    if (DateTime.TryParse(dataPgtoInicial, out DateTime inicioB))
                    {
                        parametros.Add(name: "DataPgtoInicialB", value: inicioB, direction: ParameterDirection.Input);
                        sql = sql + " AND B.DATAPAGAMENTO >= @DataPgtoInicialB ";
                    }

                    if (DateTime.TryParse(dataPgtoFinal, out DateTime terminoB))
                    {
                        parametros.Add(name: "DataPgtoFinalB", value: terminoB, direction: ParameterDirection.Input);
                        sql = sql + " AND B.DATAPAGAMENTO <= @DataPgtoFinalB ";
                    }

                    sql = sql + $@"
                                GROUP BY
                                        B.TABELA_GR,B.BL) GROUP BY TABELA_GR
                                ) D ON A.AUTONUM = D.TABELA_GR
                                INNER JOIN
                                    SGIPA..TB_SIMULADOR_SERVICOS_CALC E ON A.AUTONUM = E.LISTA
                                INNER JOIN
                                    SGIPA..TB_SIMULADOR_SERVICOS F ON E.AUTONUM_SERVICO_CALCULO = F.AUTONUM
                                WHERE
                                     F.AUTONUM_CALCULO = @SimuladorId AND A.AUTONUM = @TabelaId AND C.STATUS_GR = 'IM'";

                    if (DateTime.TryParse(dataPgtoInicial, out DateTime inicioC))
                    {
                        parametros.Add(name: "DataPgtoInicialC", value: inicioC, direction: ParameterDirection.Input);
                        sql = sql + " AND C.DATAPAGAMENTO >= @DataPgtoInicialC ";
                    }

                    if (DateTime.TryParse(dataPgtoFinal, out DateTime terminoC))
                    {
                        parametros.Add(name: "DataPgtoFinalC", value: terminoC, direction: ParameterDirection.Input);
                        sql = sql + " AND C.DATAPAGAMENTO <= @DataPgtoFinalC ";
                    }

                    return con.Query<ValoresPierHouse>(sql, parametros);
                }
            }
        }

        public IEnumerable<ValoresGerais> ObterValoresGerais(int simuladorId, string dataPgtoInicial, string dataPgtoFinal)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    var filtro = new StringBuilder();

                    var sql = $@"
                    SELECT
                        DISTINCT
                              A.DESCR As Descricao,
                              H.RAZAO AS Importador,
                              I.RAZAO AS Despachante,
                              D.REGIME,
                              A.AUTONUM AS TabelaId,
                              TO_CHAR(A.DATA_INICIO,'DD/MM/YYYY') DataInicial,
                              TO_CHAR(A.FINAL_VALIDADE,'DD/MM/YYYY') DataFinal,
                              CASE WHEN (A.FLAG_LIBERADA = 1 AND (A.FINAL_VALIDADE > SYSDATE OR A.FINAL_VALIDADE IS NULL)) THEN 'Sim' ELSE 'Não' END Status,
                              C.SEQ_GR As SeqGr,
                              C.BL,
                              1 AS LOTES,
                              C.DATAPAGAMENTO, NVL(C.VALOR_GR,0) FATURADO
                            FROM 
                                {_schema}.TB_LISTAS_PRECOS A
                            INNER JOIN
                                SGIPA.TB_GR_BL C ON A.AUTONUM=C.TABELA_GR
                            INNER JOIN
                            (
                                SELECT 
                                   TABELA_GR, 
                                   REGIME, 
                                   COUNT(1) QTD, 
                                   MAX(FATURADO) FATURADO 
                                FROM
                                    (  
                                        SELECT
                                            B.TABELA_GR,
                                            D.REGIME,
                                            B.BL,
                                            SUM(VALOR_GR) FATURADO
                                        FROM
                                            SGIPA.TB_GR_BL B
                                        INNER JOIN
                                        (
                                            SELECT 
                                                MAX(CNTR) CNTR, 
                                                BL 
                                            FROM 
                                                SGIPA.TB_AMR_CNTR_BL 
                                            GROUP BY BL
                                        ) C ON B.BL = C.BL
                                    INNER JOIN
                                        SGIPA.TB_CNTR_BL D ON C.CNTR = D.AUTONUM
                                WHERE
                                    B.STATUS_GR='IM'";

                    if (DateTime.TryParse(dataPgtoInicial, out DateTime inicioB))
                    {
                        parametros.Add(name: "DataPgtoInicialB", value: inicioB, direction: ParameterDirection.Input);
                        sql = sql + " AND B.DATAPAGAMENTO >= :DataPgtoInicialB ";
                    }

                    if (DateTime.TryParse(dataPgtoFinal, out DateTime terminoB))
                    {
                        parametros.Add(name: "DataPgtoFinalB", value: terminoB, direction: ParameterDirection.Input);
                        sql = sql + " AND B.DATAPAGAMENTO <= :DataPgtoFinalB ";
                    }

                    sql = sql + $@"GROUP BY
                                    B.TABELA_GR,B.BL, D.REGIME) GROUP BY TABELA_GR, REGIME
                            )D ON A.AUTONUM = D.TABELA_GR
                            INNER JOIN
                                SGIPA.TB_SIMULADOR_SERVICOS_CALC E ON A.AUTONUM = E.LISTA
                            INNER JOIN
                                SGIPA.TB_SIMULADOR_SERVICOS F ON E.AUTONUM_SERVICO_CALCULO = F.AUTONUM
                            INNER JOIN
                                SGIPA.TB_BL G ON C.BL = G.AUTONUM
                            LEFT JOIN
                                SGIPA.TB_CAD_PARCEIROS H ON G.IMPORTADOR = H.AUTONUM
                            LEFT JOIN
                                SGIPA.TB_CAD_PARCEIROS I ON G.DESPACHANTE = I.AUTONUM
                            WHERE
                                 F.AUTONUM_CALCULO = :SimuladorId AND C.STATUS_GR = 'IM'";

                    if (DateTime.TryParse(dataPgtoInicial, out DateTime inicioC))
                    {
                        parametros.Add(name: "DataPgtoInicialC", value: inicioC, direction: ParameterDirection.Input);
                        sql = sql + " AND C.DATAPAGAMENTO >= :DataPgtoInicialC ";
                    }

                    if (DateTime.TryParse(dataPgtoFinal, out DateTime terminoC))
                    {
                        parametros.Add(name: "DataPgtoFinalC", value: terminoC, direction: ParameterDirection.Input);
                        sql = sql + " AND C.DATAPAGAMENTO <= :DataPgtoFinalC ";
                    }

                    sql = sql + $@"
                    ORDER BY
                      A.DESCR, 
                      C.SEQ_GR, 
                      C.BL, 
                      C.DATAPAGAMENTO";

                    return con.Query<ValoresGerais>(sql, parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    var filtro = new StringBuilder();

                    var sql = $@"
                        SELECT
                            DISTINCT
                                  A.DESCR As Descricao,
                                  H.RAZAO AS Importador,
                                  I.RAZAO AS Despachante,
                                  D.REGIME,
                                  A.AUTONUM AS TabelaId,
                                  CONVERT(VARCHAR, A.DATA_INICIO, 103) DataInicial,
                                  CONVERT(VARcHAR, A.FINAL_VALIDADE, 103) DataFinal,
                                  CASE WHEN (A.FLAG_LIBERADA = 1 AND (A.FINAL_VALIDADE > GETDATE() OR A.FINAL_VALIDADE IS NULL)) THEN 'Sim' ELSE 'Não' END Status,
                                  C.SEQ_GR As SeqGr,
                                  C.BL,
                                  1 AS LOTES,
                                  C.DATAPAGAMENTO, ISNULL(C.VALOR_GR,0) FATURADO
                                FROM 
                                    {_schema}..TB_LISTAS_PRECOS A
                                INNER JOIN
                                    SGIPA.TB_GR_BL C ON A.AUTONUM=C.TABELA_GR
                                INNER JOIN
                                (
                                    SELECT 
                                       TABELA_GR, 
                                       REGIME, 
                                       COUNT(1) QTD, 
                                       MAX(FATURADO) FATURADO 
                                    FROM
                                        (  
                                            SELECT
                                                B.TABELA_GR,
                                                D.REGIME,
                                                B.BL,
                                                SUM(VALOR_GR) FATURADO
                                            FROM
                                                SGIPA..TB_GR_BL B
                                            INNER JOIN
                                            (
                                                SELECT 
                                                    MAX(CNTR) CNTR, 
                                                    BL 
                                                FROM 
                                                    SGIPA..TB_AMR_CNTR_BL 
                                                GROUP BY BL
                                            ) C ON B.BL = C.BL
                                        INNER JOIN
                                            SGIPA..TB_CNTR_BL D ON C.CNTR = D.AUTONUM
                                    WHERE
                                        B.STATUS_GR='IM'";

                    if (DateTime.TryParse(dataPgtoInicial, out DateTime inicioB))
                    {
                        parametros.Add(name: "DataPgtoInicialB", value: inicioB, direction: ParameterDirection.Input);
                        sql = sql + " AND B.DATAPAGAMENTO >= @DataPgtoInicialB ";
                    }

                    if (DateTime.TryParse(dataPgtoFinal, out DateTime terminoB))
                    {
                        parametros.Add(name: "DataPgtoFinalB", value: terminoB, direction: ParameterDirection.Input);
                        sql = sql + " AND B.DATAPAGAMENTO <= @DataPgtoFinalB ";
                    }

                    sql = sql + $@"GROUP BY
                                        B.TABELA_GR,B.BL, D.REGIME) GROUP BY TABELA_GR, REGIME
                                )D ON A.AUTONUM = D.TABELA_GR
                                INNER JOIN
                                    SGIPA..TB_SIMULADOR_SERVICOS_CALC E ON A.AUTONUM = E.LISTA
                                INNER JOIN
                                    SGIPA..TB_SIMULADOR_SERVICOS F ON E.AUTONUM_SERVICO_CALCULO = F.AUTONUM
                                INNER JOIN
                                    SGIPA..TB_BL G ON C.BL = G.AUTONUM
                                LEFT JOIN
                                    SGIPA..TB_CAD_PARCEIROS H ON G.IMPORTADOR = H.AUTONUM
                                LEFT JOIN
                                    SGIPA..TB_CAD_PARCEIROS I ON G.DESPACHANTE = I.AUTONUM
                                WHERE
                                     F.AUTONUM_CALCULO = @SimuladorId AND C.STATUS_GR = 'IM'";

                    if (DateTime.TryParse(dataPgtoInicial, out DateTime inicioC))
                    {
                        parametros.Add(name: "DataPgtoInicialC", value: inicioC, direction: ParameterDirection.Input);
                        sql = sql + " AND C.DATAPAGAMENTO >= @DataPgtoInicialC ";
                    }

                    if (DateTime.TryParse(dataPgtoFinal, out DateTime terminoC))
                    {
                        parametros.Add(name: "DataPgtoFinalC", value: terminoC, direction: ParameterDirection.Input);
                        sql = sql + " AND C.DATAPAGAMENTO <= @DataPgtoFinalC ";
                    }

                    sql = sql + $@"
                        ORDER BY
                          A.DESCR, 
                          C.SEQ_GR, 
                          C.BL, 
                          C.DATAPAGAMENTO";

                    return con.Query<ValoresGerais>(sql, parametros);
                }
            }
        }

        public decimal ObterTotalValorEstimadoPorLoteLCLPier(int simuladorId, int tabelaId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    return con.Query<decimal>($@"SELECT ROUND(NVL(SUM(A.VALOR_FINAL),0)/(1-0.1225),2) TOTAL FROM SGIPA.TB_SIMULADOR_SERVICOS_CALC A INNER JOIN SGIPA.TB_SIMULADOR_SERVICOS B ON A.AUTONUM_SERVICO_CALCULO = B.AUTONUM WHERE B.AUTONUM_CALCULO = :SimuladorId AND A.LISTA = :TabelaId", parametros).Single();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    return con.Query<decimal>($@"SELECT ROUND(ISNULL(SUM(A.VALOR_FINAL),0)/(1-0.1225),2) TOTAL FROM SGIPA..TB_SIMULADOR_SERVICOS_CALC A INNER JOIN SGIPA..TB_SIMULADOR_SERVICOS B ON A.AUTONUM_SERVICO_CALCULO = B.AUTONUM WHERE B.AUTONUM_CALCULO = @SimuladorId AND A.LISTA = @TabelaId", parametros).Single();
                }
            }
        }

        public int ObterClienteSimulador(int simuladorId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    return con.Query<int>($@"SELECT NVL(MAX(AUTONUM_CLIENTE),0) AUTONUM_CLIENTE FROM SGIPA.TB_SIMULADOR_CALCULO_CLIENTES WHERE AUTONUM_CALCULO = :SimuladorId", parametros).Single();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);

                    return con.Query<int>($@"SELECT ISNULL(MAX(AUTONUM_CLIENTE),0) AUTONUM_CLIENTE FROM SGIPA..TB_SIMULADOR_CALCULO_CLIENTES WHERE AUTONUM_CALCULO = @SimuladorId", parametros).Single();
                }
            }
        }

        public ParametrosSimuladorCRM ObterParametroSimuladorCRMPorId(int id)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                    return con.Query<ParametrosSimuladorCRM>(@"
                        SELECT
                            A.Id,
                            A.ModeloSimuladorId,
                            A.DocumentoId,
                            A.GrupoAtracacaoId,
                            A.Margem,
                            A.Periodos,
                            A.VolumeM3,
                            A.Peso,
                            A.NumeroLotes,
                            A.Qtde20, 
                            A.Qtde40,
                            A.Observacoes,
                            A.ValorCif,
                            B.Regime
                        FROM
                            CRM.TB_CRM_PARAMETROS_SIMULADOR A
                        INNER JOIN
                            CRM.TB_CRM_SIMULADOR_MODELO B ON A.ModeloSimuladorId = B.Id
                        WHERE
                            A.Id = :Id", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                    return con.Query<ParametrosSimuladorCRM>(@"
                        SELECT
                            Id,
                            ModeloSimuladorId,
                            DocumentoId,
                            GrupoAtracacaoId,
                            Margem,
                            Periodos,
                            VolumeM3,
                            Peso,
                            NumeroLotes,
                            Qtde20, 
                            Qtde40,
                            Observacoes,
                            ValorCif
                        FROM
                            CRM..TB_CRM_PARAMETROS_SIMULADOR
                        WHERE
                            Id = @Id", parametros).FirstOrDefault();
                }
            }
        }

        public IEnumerable<ServicoIPA> ObterServicosComplementares(int modeloSimuladorId, int oportunidadeId,int Tipo_Carga)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ModeloSimuladorId", value: modeloSimuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    if (Tipo_Carga == 1)
                    {
                        return con.Query<ServicoIPA>(@"
                        SELECT 
                            DISTINCT
                                B.DESCR As Descricao,
                                C.PRECO_UNITARIO As PrecoUnitario,
                                C.PRECO_MINIMO As PrecoMinimo,                               
                               C.BASE_CALCULO   As BaseCalculo 
                        FROM 
                            CRM.TB_CRM_SIMULADOR_SERVICOS A
                        INNER JOIN
                            SGIPA.TB_SERVICOS_IPA B ON A.SERVICOID = B.AUTONUM
                        INNER JOIN
                            CRM.TB_LISTA_PRECO_SERVICOS_FIXOS C ON B.AUTONUM = C.SERVICO
                        WHERE
                            A.ModeloSimuladorId = :ModeloSimuladorId
                        AND
                            C.OportunidadeId = :OportunidadeId AND TIPO_CARGA IN('CRGST','BBK','VEIC')", parametros);
                    }
                    else
                   {
                        return con.Query<ServicoIPA>(@"
                        SELECT 
                            DISTINCT
                              
                                B.DESCR As Descricao,
                                C.PRECO_UNITARIO As PrecoUnitario,
                                C.PRECO_MINIMO As PrecoMinimo,
                               case when C.BASE_CALCULO <>'UNID' THEN 
                                C.BASE_CALCULO ELSE C.TIPO_CARGA END As BaseCalculo 
                        FROM 
                            CRM.TB_CRM_SIMULADOR_SERVICOS A
                        INNER JOIN
                            SGIPA.TB_SERVICOS_IPA B ON A.SERVICOID = B.AUTONUM
                        INNER JOIN
                            CRM.TB_LISTA_PRECO_SERVICOS_FIXOS C ON B.AUTONUM = C.SERVICO
                        WHERE
                            A.ModeloSimuladorId = :ModeloSimuladorId
                        AND
                            C.OportunidadeId = :OportunidadeId AND TIPO_CARGA NOT IN('CRGST','BBK','VEIC')
                            ", parametros);
                    }
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ModeloSimuladorId", value: modeloSimuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    return con.Query<ServicoIPA>(@"
                        SELECT 
                            DISTINCT
                                B.AUTONUM As Id,
                                B.DESCR As Descricao,
                                C.PRECO_UNITARIO As PrecoUnitario,
                                C.PRECO_MINIMO As PrecoMinimo,
                                C.BASE_CALCULO As BaseCalculo
                        FROM 
                            CRM..TB_CRM_SIMULADOR_SERVICOS A
                        INNER JOIN
                            SGIPA..TB_SERVICOS_IPA B ON A.SERVICOID = B.AUTONUM
                        INNER JOIN
                            CRM..TB_LISTA_PRECO_SERVICOS_FIXOS C ON B.AUTONUM = C.SERVICO
                        WHERE
                            A.ModeloSimuladorId = @ModeloSimuladorId
                        AND
                            C.OportunidadeId = @OportunidadeId", parametros);
                }
            }
        }

        public int CadastrarTabelaCobrancaCRM(Oportunidade oportunidade)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "OportunidadeId", value: oportunidade.Id, direction: ParameterDirection.Input);
                    parametros.Add(name: "Descricao", value: oportunidade.Descricao, direction: ParameterDirection.Input);
                    parametros.Add(name: "FormaPagamento", value: oportunidade.FormaPagamento, direction: ParameterDirection.Input);

                    parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    con.Execute($"INSERT INTO {_schema}.TB_LISTAS_PRECOS (AUTONUM, DESCR, FLAG_LIBERADA, FORMA_PAGAMENTO, OportunidadeId) VALUES ({_schema}.SEQ_TB_TABELAS_COBRANCA.NEXTVAL, :Descricao, 0, 1, :OportunidadeId) RETURNING AUTONUM INTO :Id", parametros);

                    return parametros.Get<int>("Id");
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    con.Open();

                    using (var transaction = con.BeginTransaction())
                    {
                        var parametros = new DynamicParameters();

                        parametros.Add(name: "Id", value: oportunidade.Id, direction: ParameterDirection.Input);
                        parametros.Add(name: "ContaId", value: oportunidade.ContaId, direction: ParameterDirection.Input);
                        parametros.Add(name: "ContaDocumento", value: oportunidade.ContaDocumento, direction: ParameterDirection.Input);
                        parametros.Add(name: "Descricao", value: oportunidade.Descricao, direction: ParameterDirection.Input);
                        parametros.Add(name: "FormaPagamento", value: oportunidade.FormaPagamento, direction: ParameterDirection.Input);

                        var tabelaId = con.Query<int>("INSERT INTO CRM..TB_LISTAS_PRECOS (DESCR, FLAG_LIBERADA, FORMA_PAGAMENTO, OportunidadeId) VALUES (@Descricao, 0, 1, @Id); SELECT CAST(SCOPE_IDENTITY() AS INT)", parametros, transaction).Single();

                        var parceiroId = con.Query<int>("SELECT AUTONUM FROM CRM..TB_CAD_PARCEIROS WHERE CGC = @ContaDocumento", parametros, transaction).FirstOrDefault();

                        if (parceiroId == 0)
                        {
                            parceiroId = con.Query<int>("INSERT INTO CRM..TB_CAD_PARCEIROS (RAZAO, FANTASIA, CGC) SELECT DESCRICAO, NOMEFANTASIA, DOCUMENTO FROM VW_CRM_CONTAS WHERE Id = @ContaId; SELECT CAST(SCOPE_IDENTITY() AS INT)", parametros, transaction).Single();
                        }

                        parametros = new DynamicParameters();

                        parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                        parametros.Add(name: "ParceiroId", value: parceiroId, direction: ParameterDirection.Input);

                        switch (oportunidade.SegmentoId)
                        {
                            case Segmento.IMPORTADOR:
                                parametros.Add(name: "Tipo", value: "I", direction: ParameterDirection.Input);
                                break;
                            case Segmento.DESPACHANTE:
                                parametros.Add(name: "Tipo", value: "D", direction: ParameterDirection.Input);
                                break;
                            case Segmento.AGENTE:
                                parametros.Add(name: "Tipo", value: "C", direction: ParameterDirection.Input);
                                break;
                            case Segmento.COLOADER:
                                parametros.Add(name: "Tipo", value: "S", direction: ParameterDirection.Input);
                                break;
                        }

                        con.Execute("INSERT INTO TB_TP_GRUPOS (LISTA, AUTONUM_PARCEIRO, TIPO) VALUES (@TabelaId, @ParceiroId, @Tipo)", parametros, transaction);

                        transaction.Commit();

                        return tabelaId;
                    }
                }
            }
        }

        public void ExcluirTabelasCobrancasPorOportunidade(int oportunidadeId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    //
                    //con.Execute($"DELETE FROM {_schema}.TB_LISTA_CFG_VALORMINIMO WHERE AUTONUMSV IN (SELECT AUTONUM FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE LISTA IN (SELECT AUTONUM FROM {_schema}.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId))", parametros);
                    //con.Execute($"DELETE FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE LISTA IN (SELECT AUTONUM FROM {_schema}.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId)", parametros);
                    //con.Execute($"DELETE FROM {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE LISTA IN (SELECT AUTONUM FROM {_schema}.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId)", parametros);
                    //con.Execute($"DELETE FROM {_schema}.TB_TP_GRUPOS WHERE AUTONUMLISTA IN (SELECT AUTONUM FROM {_schema}.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId)", parametros);
                    //con.Execute($"DELETE FROM {_schema}.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId", parametros);

                    con.Execute($"DELETE FROM {_schema}.TB_LISTA_P_S_FAIXASCIF_PER WHERE AUTONUMSV IN (SELECT AUTONUM FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE OportunidadeId = :OportunidadeId)", parametros);
                    con.Execute($"DELETE FROM {_schema}.TB_LISTA_CFG_VALORMINIMO WHERE AUTONUMSV IN (SELECT AUTONUM FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE OportunidadeId = :OportunidadeId)", parametros);
                    con.Execute($"DELETE FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE OportunidadeId = :OportunidadeId", parametros);
                    con.Execute($"DELETE FROM {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE OportunidadeId = :OportunidadeId", parametros);
                    con.Execute($"DELETE FROM {_schema}.TB_TP_GRUPOS WHERE AUTONUMLISTA IN (SELECT AUTONUM FROM {_schema}.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId)", parametros);
                    con.Execute($"DELETE FROM {_schema}.TB_LP_IMPOSTOS WHERE ID_TABELA IN (SELECT AUTONUM FROM {_schema}.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId)", parametros);
                    con.Execute($"DELETE FROM {_schema}.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    con.Execute($"DELETE FROM {_schema}.dbo.TB_LISTA_CFG_VALORMINIMO WHERE AUTONUMSV IN (SELECT AUTONUM FROM {_schema}.dbo.TB_LISTA_P_S_PERIODO WHERE LISTA IN (SELECT AUTONUM FROM {_schema}.dbo.TB_LISTAS_PRECOS WHERE OportunidadeId = @OportunidadeId))", parametros);
                    con.Execute($"DELETE FROM {_schema}.dbo.TB_LISTA_P_S_PERIODO WHERE LISTA IN (SELECT AUTONUM FROM {_schema}.dbo.TB_LISTAS_PRECOS WHERE OportunidadeId = @OportunidadeId)", parametros);
                    con.Execute($"DELETE FROM {_schema}.dbo.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE LISTA IN (SELECT AUTONUM FROM {_schema}.dbo.TB_LISTAS_PRECOS WHERE OportunidadeId = @OportunidadeId)", parametros);
                    con.Execute($"DELETE FROM {_schema}.dbo.TB_TP_GRUPOS WHERE LISTA IN (SELECT AUTONUM FROM {_schema}.dbo.TB_LISTAS_PRECOS WHERE OportunidadeId = @OportunidadeId)", parametros);
                    con.Execute($"DELETE FROM {_schema}.dbo.TB_LISTAS_PRECOS WHERE OportunidadeId = @OportunidadeId", parametros);
                }
            }
        }
    }
}