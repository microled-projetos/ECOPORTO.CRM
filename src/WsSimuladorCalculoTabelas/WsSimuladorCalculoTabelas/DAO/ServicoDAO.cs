using Dapper;
using Ecoporto.CRM.Business.Enums;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using WsSimuladorCalculoTabelas.Configuracao;
using WsSimuladorCalculoTabelas.DTO;
using WsSimuladorCalculoTabelas.Models;

namespace WsSimuladorCalculoTabelas.DAO
{
    public class ServicoDAO
    {
        private readonly string _schema = string.Empty;

        public ServicoDAO(bool crm)
        {
            _schema = crm ? "CRM" : "SGIPA";
        }

        public IEnumerable<ServicoFixoVariavel> ObterServicosLCLPorTabela(int tabelaId, string margem)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    var filtrosSQL = new StringBuilder();
                    
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Margem", value: margem, direction: ParameterDirection.Input);
                    filtrosSQL.Append(" AND (A.VARIANTE_LOCAL = :Margem or a.variante_local='SVAR')");
                
                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            DISTINCT
                                A.SERVICO As ServicoId, 
                                B.FLAG_DESOVA, 
                                B.DESCR,
                                NVL(FLAG_DESOVA + FLAG_FITO + FLAG_HUB_PORT + FLAG_ANVISA + FLAG_CARGA_SOLTA + FLAG_NVOCC + FLAG_BREAK + FLAG_TRANSP_ESPECIAL + FLAG_LAVAGEM + FLAG_DDC + FLAG_DEV_VAZIO + FLAG_CROSS + FLAG_ARMAZENAGEM,0) AS FLAG_FLC,
                                NVL(GRUPO_ATRACACAO, 0) AS GrupoAtracacaoId
                        FROM
                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS A
                        INNER JOIN
                            SGIPA.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                        WHERE
                            A.LISTA = :TabelaId
                        AND
                            B.FLAG_CALC_AUTO = 1 {filtrosSQL}
                        AND 
                            B.AUTONUM <> 245                       
                        AND
                            B.AUTONUM IN (SELECT AUTONUM_SERVICO FROM SGIPA.TB_SIMULADOR_PARAM_SERV_AUTO)

                        UNION ALL

                        SELECT 
                            DISTINCT
                                A.SERVICO,  
                                B.FLAG_DESOVA,
                                B.DESCR,
                                0 AS FLAG_FLC,
                                NVL(GRUPO_ATRACACAO, 0) AS GrupoAtracacaoId
                        FROM
                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS A
                        INNER JOIN
                            SGIPA.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                        WHERE
                            A.LISTA = :TabelaId
                        AND
                            B.FLAG_CALC_AUTO = 0    {filtrosSQL}                 
                        AND 
                            B.AUTONUM IN (SELECT AUTONUM_SERVICO FROM SGIPA.TB_SIMULADOR_PARAM_SERV_CONV)", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            DISTINCT
                                A.SERVICO As ServicoId, 
                                B.FLAG_DESOVA, 
                                B.DESCR,
                                ISNULL(FLAG_DESOVA + FLAG_FITO + FLAG_HUB_PORT + FLAG_ANVISA + FLAG_CARGA_SOLTA + FLAG_NVOCC + FLAG_BREAK + FLAG_TRANSP_ESPECIAL + FLAG_LAVAGEM + FLAG_DDC + FLAG_DEV_VAZIO,0) AS FLAG_FLC,
                                ISNULL(GRUPO_ATRACACAO, 0) AS GrupoAtracacaoId
                        FROM
                            {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS A
                        INNER JOIN
                            SGIPA..TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                        WHERE
                            A.LISTA = @TabelaId
                        AND
                            B.FLAG_CALC_AUTO = 1 
                        AND 
                            B.AUTONUM <> 245                       
                        AND
                            B.AUTONUM IN (SELECT AUTONUM_SERVICO FROM SGIPA..TB_SIMULADOR_PARAM_SERV_AUTO)

                        UNION ALL

                        SELECT 
                            DISTINCT
                                A.SERVICO,  
                                B.FLAG_DESOVA,
                                B.DESCR,
                                0 AS FLAG_FLC,
                                ISNULL(GRUPO_ATRACACAO, 0) AS GrupoAtracacaoId
                        FROM
                            {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS A
                        INNER JOIN
                            SGIPA..TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                        WHERE
                            A.LISTA = @TabelaId
                        AND
                            B.FLAG_CALC_AUTO = 0                      
                        AND 
                            B.AUTONUM IN (SELECT AUTONUM_SERVICO FROM SGIPA..TB_SIMULADOR_PARAM_SERV_CONV)", parametros);
                }
            }
        }

        public IEnumerable<ServicoFixoVariavel> ObterServicosFCLPorTabela(int tabelaId, string margem)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    var filtrosSQL = new StringBuilder();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Margem", value: margem, direction: ParameterDirection.Input);
                    filtrosSQL.Append(" AND (A.VARIANTE_LOCAL = :Margem or a.variante_local='SVAR')");

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            DISTINCT
                                A.SERVICO As ServicoId, 
                                B.FLAG_DESOVA, 
                                B.DESCR,
                                NVL(FLAG_DESOVA + FLAG_FITO + FLAG_HUB_PORT + FLAG_ANVISA + FLAG_CARGA_SOLTA + FLAG_NVOCC + FLAG_BREAK + FLAG_TRANSP_ESPECIAL + FLAG_LAVAGEM + FLAG_DDC +FLAG_DEV_VAZIO + FLAG_CROSS + FLAG_ARMAZENAGEM,0) AS FLAG_FLC,
                                NVL(GRUPO_ATRACACAO, 0) AS GrupoAtracacaoId
                        FROM
                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS A
                        INNER JOIN
                            SGIPA.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                        WHERE
                            A.LISTA = :TabelaId
                        AND
                            B.FLAG_CALC_AUTO = 1  {filtrosSQL}
                        AND 
                            FLAG_DESOVA = 0 
                        AND  
                            B.AUTONUM IN (SELECT AUTONUM_SERVICO FROM SGIPA.TB_SIMULADOR_PARAM_SERV_AUTO)

                        UNION ALL

                        SELECT 
                            DISTINCT
                                A.SERVICO, 
                                B.FLAG_DESOVA, 
                                B.DESCR, 
                                0 AS FLAG_FLC,
                                NVL(GRUPO_ATRACACAO, 0) AS GrupoAtracacaoId
                        FROM
                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS A
                        INNER JOIN
                            SGIPA.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                        WHERE
                            A.LISTA = :TabelaId
                        AND
                            B.FLAG_CALC_AUTO = 0  {filtrosSQL}
                        AND    
                            FLAG_DESOVA = 0
                        AND 
                            B.AUTONUM IN (SELECT AUTONUM_SERVICO FROM SGIPA.TB_SIMULADOR_PARAM_SERV_CONV)", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            DISTINCT
                                A.SERVICO As ServicoId, 
                                B.FLAG_DESOVA, 
                                B.DESCR,
                                ISNULL(FLAG_DESOVA + FLAG_FITO + FLAG_HUB_PORT + FLAG_ANVISA + FLAG_CARGA_SOLTA + FLAG_NVOCC + FLAG_BREAK + FLAG_TRANSP_ESPECIAL + FLAG_LAVAGEM + FLAG_DDC +FLAG_DEV_VAZIO,0) AS FLAG_FLC,
                                ISNULL(GRUPO_ATRACACAO, 0) AS GrupoAtracacaoId
                        FROM
                            {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS A
                        INNER JOIN
                            SGIPA..TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                        WHERE
                            A.LISTA = @TabelaId
                        AND
                            B.FLAG_CALC_AUTO = 1 
                        AND 
                            FLAG_DESOVA = 0 
                        AND  
                            B.AUTONUM IN (SELECT AUTONUM_SERVICO FROM SGIPA..TB_SIMULADOR_PARAM_SERV_AUTO)

                        UNION ALL

                        SELECT 
                            DISTINCT
                                A.SERVICO, 
                                B.FLAG_DESOVA, 
                                B.DESCR, 
                                0 AS FLAG_FLC,
                                ISNULL(GRUPO_ATRACACAO, 0) AS GrupoAtracacaoId
                        FROM
                            {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS A
                        INNER JOIN
                            SGIPA..TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                        WHERE
                            A.LISTA = @TabelaId
                        AND
                            B.FLAG_CALC_AUTO = 0 
                        AND    
                            FLAG_DESOVA = 0
                        AND 
                            B.AUTONUM IN (SELECT AUTONUM_SERVICO FROM SGIPA..TB_SIMULADOR_PARAM_SERV_CONV)", parametros);
                }
            }
        }

        public IEnumerable<ServicoFixoVariavel> ObterServicosVariaveisPorTabela(int tabelaId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
	                        DISTINCT
		                        A.SERVICO As ServicoId, 
		                        B.FLAG_DESOVA
                        FROM
	                        {_schema}.TB_LISTA_P_S_PERIODO A
                        INNER JOIN
	                        SGIPA.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                        WHERE
	                        A.LISTA = :TabelaId
                        AND
	                        B.FLAG_CALC_AUTO = 1   
                        AND   
	                        B.AUTONUM IN (SELECT AUTONUM_SERVICO FROM SGIPA.TB_SIMULADOR_PARAM_SERV_AUTO)", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
	                        DISTINCT
		                        A.SERVICO As ServicoId, 
		                        B.FLAG_DESOVA
                        FROM
	                        {_schema}..TB_LISTA_P_S_PERIODO A
                        INNER JOIN
	                        SGIPA..TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                        WHERE
	                        A.LISTA = @TabelaId
                        AND
	                        B.FLAG_CALC_AUTO = 1   
                        AND   
	                        B.AUTONUM IN (SELECT AUTONUM_SERVICO FROM SGIPA..TB_SIMULADOR_PARAM_SERV_AUTO)", parametros);
                }
            }
        }

        public IEnumerable<ServicoFixoVariavel> ObterServicosFixos(ServicosFiltro servicosFiltro)
        {
            var parametros = new DynamicParameters();
            var filtrosSQL = new StringBuilder();

            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    if (servicosFiltro.TipoDocumento.HasValue)
                    {
                        parametros.Add(name: "TipoDocumento", value: servicosFiltro.TipoDocumento.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.TIPO_DOC = :TipoDocumento OR NVL(A.TIPO_DOC,0) = 0) ");
                    }

                    if (servicosFiltro.ServicoId.HasValue)
                    {
                        parametros.Add(name: "ServicoId", value: servicosFiltro.ServicoId.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.SERVICO <> 53 AND A.SERVICO = :ServicoId) ");
                    }

                    if (servicosFiltro.Armador.HasValue)
                    {
                        parametros.Add(name: "ArmadorId", value: servicosFiltro.Armador.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.ARMADOR = :ArmadorId OR NVL(A.ARMADOR,0) = 0) ");
                    }

                    if (servicosFiltro.GrupoAtracacao.HasValue)
                    {
                        parametros.Add(name: "GrupoAtracacaoId", value: servicosFiltro.GrupoAtracacao.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.GRUPO_ATRACACAO = :GrupoAtracacaoId AND NVL(A.GRUPO_ATRACACAO ,0) <> 0) ");
                    }
                    else
                    {
                        filtrosSQL.Append(" AND (NVL(A.GRUPO_ATRACACAO,0) = 0) ");
                    }

                    if (servicosFiltro.Lista.HasValue)
                    {
                        parametros.Add(name: "Lista", value: servicosFiltro.Lista.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND A.LISTA = :Lista ");
                    }

                    if (!string.IsNullOrEmpty(servicosFiltro.Margem))
                    {
                        parametros.Add(name: "Margem", value: servicosFiltro.Margem,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.VARIANTE_LOCAL = :Margem) ");
                    }

                    if (!string.IsNullOrEmpty(servicosFiltro.TipoCargaSQL))
                    {
                        filtrosSQL.Append($" AND {servicosFiltro.TipoCargaSQL} ");
                    }

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT
                            ServicoFixoVariavelId,
                            ServicoId,
                            LISTA,
                            PrecoMaximo,
                            PrecoMinimo,
                            BaseCalculo,
                            PrecoUnitario
                        FROM
                        (
                            SELECT
                                A.AUTONUM As ServicoFixoVariavelId,
                                A.SERVICO As ServicoId,
                                A.LISTA,
                                A.PRECO_MAXIMO As PrecoMaximo,
                                A.PRECO_MINIMO As PrecoMinimo,
                                A.BASE_CALCULO As BaseCalculo,
                                A.PRECO_UNITARIO As PrecoUnitario
                            FROM
                                {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS A
                            INNER JOIN
                                SGIPA.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                            WHERE
                                A.AUTONUM > 0 {filtrosSQL.ToString()}              
                            ORDER BY
                                A.TIPO_DOC DESC,
                                (DECODE(SUBSTR(A.TIPO_CARGA,1,4),'SVAR','ZZ'||A.TIPO_CARGA||'99',A.TIPO_CARGA)),
                                A.TIPO_CARGA,
                                A.SERVICO
                        ) WHERE ROWNUM = 1", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    if (servicosFiltro.TipoDocumento.HasValue)
                    {
                        parametros.Add(name: "TipoDocumento", value: servicosFiltro.TipoDocumento.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.TIPO_DOC = @TipoDocumento OR ISNULL(A.TIPO_DOC,0) = 0) ");
                    }

                    if (servicosFiltro.ServicoId.HasValue)
                    {
                        parametros.Add(name: "ServicoId", value: servicosFiltro.ServicoId.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.SERVICO <> 53 AND A.SERVICO = @ServicoId) ");
                    }

                    if (servicosFiltro.Armador.HasValue)
                    {
                        parametros.Add(name: "ArmadorId", value: servicosFiltro.Armador.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.ARMADOR = @ArmadorId OR ISNULL(A.ARMADOR,0) = 0) ");
                    }

                    if (servicosFiltro.GrupoAtracacao.HasValue)
                    {
                        parametros.Add(name: "GrupoAtracacaoId", value: servicosFiltro.GrupoAtracacao.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.GRUPO_ATRACACAO = @GrupoAtracacaoId AND ISNULL(A.GRUPO_ATRACACAO ,0) <> 0) ");
                    }
                    else
                    {
                        filtrosSQL.Append(" AND (ISNULL(A.GRUPO_ATRACACAO,0) = 0) ");
                    }

                    if (servicosFiltro.Lista.HasValue)
                    {
                        parametros.Add(name: "Lista", value: servicosFiltro.Lista.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND A.LISTA = @Lista ");
                    }

                    if (!string.IsNullOrEmpty(servicosFiltro.Margem))
                    {
                        parametros.Add(name: "Margem", value: servicosFiltro.Margem,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.VARIANTE_LOCAL = @Margem) ");
                    }

                    if (!string.IsNullOrEmpty(servicosFiltro.TipoCargaSQL))
                    {
                        filtrosSQL.Append($" AND {servicosFiltro.TipoCargaSQL} ");
                    }

                    return con.Query<ServicoFixoVariavel>($@"                        
                            SELECT
                                TOP 1
                                    A.AUTONUM As ServicoFixoVariavelId,
                                    A.SERVICO As ServicoId,
                                    A.LISTA,
                                    A.PRECO_MAXIMO As PrecoMaximo,
                                    A.PRECO_MINIMO As PrecoMinimo,
                                    A.BASE_CALCULO As BaseCalculo,
                                    A.PRECO_UNITARIO As PrecoUnitario
                            FROM
                                {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS A
                            INNER JOIN
                                SGIPA..TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM
                            WHERE
                                A.AUTONUM > 0 {filtrosSQL.ToString()}              
                            ORDER BY
                                A.TIPO_DOC DESC,
                                (CASE WHEN SUBSTRING(A.TIPO_CARGA,1,4) = 'SVAR' THEN 'ZZ' + A.TIPO_CARGA + '99' ELSE A.TIPO_CARGA END),
                                A.TIPO_CARGA,
                                A.SERVICO", parametros);
                }
            }
        }

        public IEnumerable<ServicoFixoVariavel> ObterServicosVariaveis(ServicosFiltro servicosFiltro)
        {
            var parametros = new DynamicParameters();
            var filtrosSQL = new StringBuilder();

            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    if (servicosFiltro.TipoDocumento.HasValue)
                    {
                        parametros.Add(name: "TipoDocumento", value: servicosFiltro.TipoDocumento.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.TIPO_DOC = :TipoDocumento OR NVL(A.TIPO_DOC,0) = 0) ");
                    }

                    if (servicosFiltro.ServicoId.HasValue)
                    {
                        parametros.Add(name: "ServicoId", value: servicosFiltro.ServicoId.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.SERVICO = :ServicoId) ");
                    }

                    if (servicosFiltro.Armador.HasValue)
                    {
                        parametros.Add(name: "ArmadorId", value: servicosFiltro.Armador.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.ARMADOR = :ArmadorId OR NVL(A.ARMADOR,0) = 0) ");
                    }

                    if (servicosFiltro.GrupoAtracacao.HasValue)
                    {
                        parametros.Add(name: "GrupoAtracacaoId", value: servicosFiltro.GrupoAtracacao.Value,
                            direction: ParameterDirection.Input);

                        if (servicosFiltro.GrupoAtracacao.Value > 0)
                        {
                            filtrosSQL.Append(" AND (A.GRUPO_ATRACACAO = :GrupoAtracacaoId AND NVL(A.GRUPO_ATRACACAO ,0) <> 0) ");
                        }
                        else
                        {
                            filtrosSQL.Append(" AND (NVL(A.GRUPO_ATRACACAO, 0) = 0) ");
                        }
                    }

                    if (servicosFiltro.Lista.HasValue)
                    {
                        parametros.Add(name: "Lista", value: servicosFiltro.Lista.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND A.LISTA = :Lista ");
                    }

                    if (!string.IsNullOrEmpty(servicosFiltro.Margem))
                    {
                        parametros.Add(name: "Margem", value: servicosFiltro.Margem,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.VARIANTE_LOCAL = :Margem or A.VARIANTE_LOCAL = 'SVAR') ");
                    }

                    if (servicosFiltro.Periodo.HasValue)
                    {
                        parametros.Add(name: "Periodo", value: servicosFiltro.Periodo.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND A.N_PERIODO <= :Periodo ");
                    }

                    if (!string.IsNullOrEmpty(servicosFiltro.TipoCargaSQL))
                    {
                        filtrosSQL.Append($" AND {servicosFiltro.TipoCargaSQL} ");
                    }

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT
                            ServicoFixoVariavelId,
                            ServicoId,
                            LISTA,
                            PrecoMaximo,
                            PrecoMinimo,
                            BaseCalculo,
                            PrecoUnitario
                        FROM
                        (
                            SELECT
                                A.AUTONUM As ServicoFixoVariavelId,
                                A.SERVICO As ServicoId,
                                A.LISTA,
                                A.PRECO_MAXIMO As PrecoMaximo,
                                A.PRECO_MINIMO As PrecoMinimo,
                                A.BASE_CALCULO As BaseCalculo,
                                A.PRECO_UNITARIO As PrecoUnitario
                            FROM
                                {_schema}.TB_LISTA_P_S_PERIODO A
                            WHERE
                                A.AUTONUM > 0 {filtrosSQL.ToString()}           
                            ORDER BY
                                A.TIPO_DOC DESC,
                                A.N_PERIODO DESC,
                                (DECODE(SUBSTR(A.TIPO_CARGA,1,4),'SVAR','ZZ'|| A.TIPO_CARGA || '99',A.TIPO_CARGA)),
                                A.TIPO_CARGA
                           ) WHERE ROWNUM = 1", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    if (servicosFiltro.TipoDocumento.HasValue)
                    {
                        parametros.Add(name: "TipoDocumento", value: servicosFiltro.TipoDocumento.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.TIPO_DOC = @TipoDocumento OR ISNULL(A.TIPO_DOC,0) = 0) ");
                    }

                    if (servicosFiltro.ServicoId.HasValue)
                    {
                        parametros.Add(name: "ServicoId", value: servicosFiltro.ServicoId.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.SERVICO = @ServicoId) ");
                    }

                    if (servicosFiltro.Armador.HasValue)
                    {
                        parametros.Add(name: "ArmadorId", value: servicosFiltro.Armador.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.ARMADOR = @ArmadorId OR ISNULL(A.ARMADOR,0) = 0) ");
                    }

                    if (servicosFiltro.GrupoAtracacao.HasValue)
                    {
                        parametros.Add(name: "GrupoAtracacaoId", value: servicosFiltro.GrupoAtracacao.Value,
                            direction: ParameterDirection.Input);

                        if (servicosFiltro.GrupoAtracacao.Value > 0)
                        {
                            filtrosSQL.Append(" AND (A.GRUPO_ATRACACAO = @GrupoAtracacaoId AND ISNULL(A.GRUPO_ATRACACAO ,0) <> 0) ");
                        }
                        else
                        {
                            filtrosSQL.Append(" AND (ISNULL(A.GRUPO_ATRACACAO, 0) = 0) ");
                        }
                    }

                    if (servicosFiltro.Lista.HasValue)
                    {
                        parametros.Add(name: "Lista", value: servicosFiltro.Lista.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND A.LISTA = @Lista ");
                    }

                    if (!string.IsNullOrEmpty(servicosFiltro.Margem))
                    {
                        parametros.Add(name: "Margem", value: servicosFiltro.Margem,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND (A.VARIANTE_LOCAL = @Margem) ");
                    }

                    if (servicosFiltro.Periodo.HasValue)
                    {
                        parametros.Add(name: "Periodo", value: servicosFiltro.Periodo.Value,
                            direction: ParameterDirection.Input);

                        filtrosSQL.Append(" AND A.N_PERIODO <= @Periodo ");
                    }

                    if (!string.IsNullOrEmpty(servicosFiltro.TipoCargaSQL))
                    {
                        filtrosSQL.Append($" AND {servicosFiltro.TipoCargaSQL} ");
                    }

                    return con.Query<ServicoFixoVariavel>($@"                        
                            SELECT
                                TOP 1
                                    A.AUTONUM As ServicoFixoVariavelId,
                                    A.SERVICO As ServicoId,
                                    A.LISTA,
                                    A.PRECO_MAXIMO As PrecoMaximo,
                                    A.PRECO_MINIMO As PrecoMinimo,
                                    A.BASE_CALCULO As BaseCalculo,
                                    A.PRECO_UNITARIO As PrecoUnitario
                            FROM
                                 {_schema}..TB_LISTA_P_S_PERIODO A
                            WHERE
                                A.AUTONUM > 0 {filtrosSQL.ToString()}           
                            ORDER BY
                                A.TIPO_DOC DESC,
                                A.N_PERIODO DESC,
                                (CASE WHEN SUBSTRING(A.TIPO_CARGA,1,4) = 'SVAR' THEN 'ZZ' + A.TIPO_CARGA + '99' ELSE A.TIPO_CARGA END),
                                A.TIPO_CARGA", parametros);
                }
            }
        }

        public ServicoFixoVariavel ObterDetalhesServicoFixoPorFaixas(int servicoFixoId, decimal valor, string tipo)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ServicoFixoId", value: servicoFixoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Valor", value: valor, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"SELECT NVL(PERCENTUAL,0) Percentual, NVL(MINIMO,0) ValorMinimo FROM {_schema}.TB_LISTA_P_S_FAIXASCIF_FIX WHERE TIPO = :Tipo AND VALORINICIAL <= :Valor AND VALORFINAL >= :Valor AND AUTONUMSV = :ServicoFixoId", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ServicoFixoId", value: servicoFixoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Valor", value: valor, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"SELECT ISNULL(PERCENTUAL,0) Percentual, ISNULL(MINIMO,0) ValorMinimo FROM {_schema}..TB_LISTA_P_S_FAIXASCIF_FIX WHERE TIPO = @Tipo AND VALORINICIAL <= @Valor AND VALORFINAL >= @Valor AND AUTONUMSV = @ServicoFixoId", parametros).FirstOrDefault();
                }
            }
        }

        public ServicoFixoVariavel ObterDetalhesServicoVariavelPorFaixas(int servicoVariavelId, decimal valor, string tipo)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ServicoVariavelId", value: servicoVariavelId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Valor", value: valor, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"SELECT NVL(PERCENTUAL,0) Percentual, NVL(MINIMO,0) ValorMinimo FROM {_schema}.TB_LISTA_P_S_FAIXASCIF_PER WHERE TIPO = :Tipo AND VALORINICIAL <= :Valor AND VALORFINAL >= :Valor AND AUTONUMSV = :ServicoVariavelId", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ServicoVariavelId", value: servicoVariavelId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Valor", value: valor, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"SELECT ISNULL(PERCENTUAL,0) Percentual, ISNULL(MINIMO,0) ValorMinimo FROM {_schema}..TB_LISTA_P_S_FAIXASCIF_PER WHERE TIPO = @Tipo AND VALORINICIAL <= @Valor AND VALORFINAL >= @Valor AND AUTONUMSV = @ServicoVariavelId", parametros).FirstOrDefault();
                }
            }
        }

        public ServicoFixoVariavel ObterDetalhesServicoVariavelPorFaixasPeso(int servicoVariavelId, int lotes, string tipo, int servicoIPA = 0, string tipoCarga = "")
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ServicoVariavelId", value: servicoVariavelId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Lotes", value: lotes, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);
                    parametros.Add(name: "ServicoIPA", value: servicoIPA, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoCarga", value: tipoCarga, direction: ParameterDirection.Input);

                    if (servicoIPA == 0)
                        return con.Query<ServicoFixoVariavel>($@"SELECT ValorMinimo FROM {_schema}.TB_LISTA_CFG_VALORMINIMO WHERE TIPO = :Tipo AND NBLS >= :Lotes AND AUTONUMSV = :ServicoVariavelId ORDER BY NBLS", parametros).FirstOrDefault();

                    return con.Query<ServicoFixoVariavel>($@"SELECT ValorMinimo FROM {_schema}.TB_LISTA_CFG_VALORMINIMO WHERE TIPO = :Tipo AND NBLS >= :Lotes 
                        AND AUTONUMSV IN (SELECT MAX(AUTONUM) FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE Servico = :ServicoIPA AND Tipo_Carga = :TipoCarga 
                            AND LISTA IN (SELECT LISTA FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE AUTONUM = :ServicoVariavelId))", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ServicoVariavelId", value: servicoVariavelId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Lotes", value: lotes, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);
                    parametros.Add(name: "ServicoIPA", value: servicoIPA, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoCarga", value: tipoCarga, direction: ParameterDirection.Input);

                    if (servicoIPA == 0)
                        return con.Query<ServicoFixoVariavel>($@"SELECT ValorMinimo FROM {_schema}..TB_LISTA_CFG_VALORMINIMO WHERE TIPO = @Tipo AND NBLS >= @Lotes AND AUTONUMSV = @ServicoVariavelId ORDER BY NBLS", parametros).FirstOrDefault();

                    return con.Query<ServicoFixoVariavel>($@"SELECT ValorMinimo FROM {_schema}..TB_LISTA_CFG_VALORMINIMO WHERE TIPO = @Tipo AND NBLS >= @Lotes 
                        AND AUTONUMSV IN (SELECT MAX(AUTONUM) FROM {_schema}..TB_LISTA_P_S_PERIODO WHERE Servico = @ServicoIPA AND Tipo_Carga = @TipoCarga 
                            AND LISTA IN (SELECT LISTA FROM {_schema}..TB_LISTA_P_S_PERIODO WHERE AUTONUM = @ServicoVariavelId))", parametros).FirstOrDefault();
                }
            }
        }

        public decimal? ObterValorMinimo(int servicoFixoVariavelId, string tipoCarga, int numeroLotes)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ServicoFixoVariavelId", value: servicoFixoVariavelId, direction: ParameterDirection.Input);
                    parametros.Add(name: "NumeroLotes", value: numeroLotes, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoCarga", value: tipoCarga, direction: ParameterDirection.Input);

                    return con.Query<decimal?>($@"
                        SELECT VLM.VALORMINIMO ValorMinimo FROM {_schema}.TB_LISTA_CFG_VALORMINIMO VLM WHERE VLM.TIPO = :TipoCarga AND VLM.NBLS >= :NumeroLotes AND VLM.AUTONUMSV = :ServicoFixoVariavelId
                        UNION
                        SELECT PRECO_MINIMO FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE PRECO_MINIMO > 0 AND AUTONUM = :ServicoFixoVariavelId
                        ORDER BY 1 DESC", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ServicoFixoVariavelId", value: servicoFixoVariavelId, direction: ParameterDirection.Input);
                    parametros.Add(name: "NumeroLotes", value: numeroLotes, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoCarga", value: tipoCarga, direction: ParameterDirection.Input);

                    return con.Query<decimal?>($@"SELECT VLM.VALORMINIMO ValorMinimo FROM {_schema}..TB_LISTA_CFG_VALORMINIMO VLM WHERE VLM.TIPO = @TipoCarga AND VLM.NBLS >= @NumeroLotes AND VLM.AUTONUMSV = @ServicoFixoVariavelId ORDER BY NBLS ", parametros).FirstOrDefault();
                }
            }
        }

        public decimal? ObterValorMinimoAdicionalArmazenagem(int oportunidadeId, string tipoCarga)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: tipoCarga, direction: ParameterDirection.Input);

                return con.Query<decimal?>($@"SELECT MIN(A.valorminimo) from  {_schema}.TB_LISTA_CFG_VALORMINIMO A INNER JOIN  {_schema}.TB_LISTA_P_S_PERIODO B ON A.AUTONUMSV = B.AUTONUM
                    WHERE B.OPORTUNIDADEID = :OportunidadeId AND B.SERVICO = 295 AND TIPO = :TipoCarga AND B.N_PERIODO > 1", parametros).FirstOrDefault();
            }
        }

        public string ObterQuantidadeDias(int tabelaId, bool ehConteiner)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    var filtroSQL = ehConteiner ? " AND B.TIPO_CARGA IN('SVAR','SVAR40','SVAR20') " : " AND B.TIPO_CARGA ='CRGST' ";

                    return con.Query<string>($@"SELECT MAX(NVL(B.QTDE_DIAS,0)) DIAS FROM {_schema}.TB_LISTAS_PRECOS A LEFT JOIN {_schema}.TB_LISTA_P_S_PERIODO B ON A.AUTONUM = B.LISTA WHERE A.AUTONUM = :TabelaId AND B.SERVICO = 52 {filtroSQL}", parametros).Single();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    var filtroSQL = ehConteiner ? " AND B.TIPO_CARGA IN('SVAR','SVAR40','SVAR20') " : " AND B.TIPO_CARGA ='CRGST' ";

                    return con.Query<string>($@"SELECT MAX(ISNULL(B.QTDE_DIAS,0)) DIAS FROM {_schema}..TB_LISTAS_PRECOS A LEFT JOIN {_schema}..TB_LISTA_P_S_PERIODO B ON A.AUTONUM = B.LISTA WHERE A.AUTONUM = @TabelaId AND B.SERVICO = 52 {filtroSQL}", parametros).Single();
                }
            }
        }

        public decimal ObterPrecoUnitarioPorTabelaEServico(int tabelaId, int servicoId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ServicoId", value: servicoId, direction: ParameterDirection.Input);

                    return con.Query<decimal>($@"
                        SELECT 
                            NVL(MIN(PRECO_UNITARIO),0) PRECO_UNITARIO 
                        FROM 
                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS 
                        WHERE 
                            PRECO_UNITARIO > 0 AND LISTA = :TabelaId 
                        AND 
                            SERVICO = :ServicoId 

                        UNION ALL

                        SELECT 
                            NVL(MIN(PRECO_UNITARIO),0) PRECO_UNITARIO 
                        FROM 
                            {_schema}.TB_LISTA_P_S_PERIODO 
                        WHERE 
                            PRECO_UNITARIO > 0 
                        AND 
                            LISTA = :TabelaId 
                        AND 
                            SERVICO = :ServicoId ", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ServicoId", value: servicoId, direction: ParameterDirection.Input);

                    return con.Query<decimal>($@"
                        SELECT 
                            ISNULL(MIN(PRECO_UNITARIO),0) PRECO_UNITARIO 
                        FROM 
                            {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS 
                        WHERE 
                            PRECO_UNITARIO > 0 AND LISTA = :TabelaId 
                        AND 
                            SERVICO = @ServicoId 

                        UNION ALL

                        SELECT 
                            ISNULL(MIN(PRECO_UNITARIO),0) PRECO_UNITARIO 
                        FROM 
                            {_schema}..TB_LISTA_P_S_PERIODO 
                        WHERE 
                            PRECO_UNITARIO > 0 
                        AND 
                            LISTA = @TabelaId 
                        AND 
                            SERVICO = @ServicoId ", parametros).FirstOrDefault();
                }
            }
        }

        public ServicoFixoVariavel ObterValoresServicoArmazenagem(int tabelaId, int periodo, bool ehConteiner, string tipoCarga = "", string varianteLocal = "")
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Periodo", value: periodo, direction: ParameterDirection.Input);

                    var filtroSQL = string.Empty;

                    if (ehConteiner)
                    {
                        if (!string.IsNullOrEmpty(tipoCarga))
                        {
                            filtroSQL += " AND TIPO_CARGA = '" + tipoCarga + "'";
                        }
                        else
                        {
                            filtroSQL += " AND TIPO_CARGA IN('SVAR','SVAR40','SVAR20') ";
                        }
                    }
                    else
                    {
                        filtroSQL += " AND TIPO_CARGA ='CRGST' ";
                    }

                    if (!string.IsNullOrEmpty(varianteLocal))
                    {
                        filtroSQL += " AND VARIANTE_LOCAL = '" + varianteLocal + "'";
                    }

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            AUTONUM As ServicoFixoVariavelId, 
                            SUBSTR(BASE_CALCULO,1,3) AS BaseCalculo, 
                            PRECO_UNITARIO AS PrecoUnitario, 
                            N_PERIODO As Periodo, 
                            PRECO_MINIMO As PrecoMinimo, 
                            LIMITE_BLS As LimiteBls 
                        FROM 
                            {_schema}.TB_LISTA_P_S_PERIODO 
                        WHERE 
                            N_PERIODO = :Periodo 
                        And 
                            LISTA = :TabelaId 
                        And 
                            SERVICO = 52 {filtroSQL} 
                        ORDER BY 
                            TIPO_CARGA DESC", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Periodo", value: periodo, direction: ParameterDirection.Input);

                    var filtroSQL = string.Empty;

                    if (ehConteiner)
                    {
                        if (!string.IsNullOrEmpty(tipoCarga))
                        {
                            filtroSQL += " AND TIPO_CARGA = '" + tipoCarga + "'";
                        }
                        else
                        {
                            filtroSQL += " AND TIPO_CARGA IN('SVAR','SVAR40','SVAR20') ";
                        }
                    }
                    else
                    {
                        filtroSQL += " AND TIPO_CARGA ='CRGST' ";
                    }

                    if (!string.IsNullOrEmpty(varianteLocal))
                    {
                        filtroSQL += " AND VARIANTE_LOCAL = '" + varianteLocal + "'";
                    }

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            AUTONUM As ServicoFixoVariavelId, 
                            SUBSTRING(BASE_CALCULO,1,3) AS BaseCalculo, 
                            PRECO_UNITARIO AS PrecoUnitario, 
                            N_PERIODO As Periodo, 
                            PRECO_MINIMO As PrecoMinimo, 
                            LIMITE_BLS As LimiteBls 
                        FROM 
                            {_schema}..TB_LISTA_P_S_PERIODO 
                        WHERE 
                            N_PERIODO = @Periodo 
                        AND 
                            LISTA = @TabelaId 
                        AND 
                            SERVICO = 52 {filtroSQL} 
                        ORDER BY 
                            TIPO_CARGA DESC", parametros).FirstOrDefault();
                }
            }
        }

        public bool PeriodoEscalonado(int oportunidadeId, int periodo)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: TipoRegistro.ARMAZENAGEM_CIF, direction: ParameterDirection.Input);

                return con.Query<bool>($@"SELECT Id FROM TB_CRM_OPORTUNIDADE_LAYOUT 
                WHERE PERIODO = :Periodo AND OPORTUNIDADEID = :OportunidadeId 
               AND TipoRegistro = :TipoRegistro union all
                SELECT Id FROM TB_CRM_OPORTUNIDADE_LAYOUT 
                WHERE PERIODO = :Periodo and  tiporegistro=7  AND OPORTUNIDADEID = :OportunidadeId 
                     and 
                 oportunidadeid in
                  (select  oportunidadeid  from 
                crm.tb_crm_oportunidade_layout where oportunidadeid=:OportunidadeId  and
                 tiporegistro=22 )  and
                oportunidadeid not in
                  (select  oportunidadeid from
                crm.tb_crm_oportunidade_layout where oportunidadeid =:OportunidadeId and
                 tiporegistro = 21 ) ", parametros).Any();
            }
        }

        public bool PeriodoEscalonado_New(int oportunidadeId, int periodo)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: TipoRegistro.ARMAZENAGEM_CIF, direction: ParameterDirection.Input);

                return con.Query<bool>($@" SELECT Id FROM TB_CRM_OPORTUNIDADE_LAYOUT 
                WHERE PERIODO = :Periodo and  tiporegistro=7  AND OPORTUNIDADEID = :OportunidadeId 
                     and 
                 oportunidadeid in
                  (select  oportunidadeid  from 
                crm.tb_crm_oportunidade_layout where oportunidadeid=:OportunidadeId  and
                 tiporegistro=22 )  and
                oportunidadeid not in
                  (select  oportunidadeid from
                crm.tb_crm_oportunidade_layout where oportunidadeid =:OportunidadeId and
                 tiporegistro = 21 ) ", parametros).Any();
            }
        }

        public ServicoFixoVariavel ObterValoresServicoSimuladorPorTabelaId(int simuladorId, int tabelaId, int servicoId, string margem, string tipoCarga = "")
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ServicoId", value: servicoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Margem", value: margem, direction: ParameterDirection.Input);

                    var filtro = !string.IsNullOrEmpty(margem) ? " AND A.MARGEM = :Margem " : string.Empty;

                    if (!string.IsNullOrEmpty(tipoCarga))
                    {
                        filtro += " AND TIPO_CARGA = '" + tipoCarga + "' ";
                    }

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            MAX(A.VALOR_FINAL) ValorFinal, 
                            MAX(A.PRECO_UNITARIO) PrecoUnitario, 
                            MAX(A.DESCR_BASE_CALC) BaseCalculo    
                        FROM 
                            SGIPA.TB_SIMULADOR_SERVICOS_CALC A 
                        INNER JOIN 
                            SGIPA.TB_SIMULADOR_SERVICOS B ON A.AUTONUM_SERVICO_CALCULO = B.AUTONUM 
                        WHERE 
                            A.LISTA = :TabelaId 
                        AND 
                            B.AUTONUM_SERVICO = :ServicoId
                        AND 
                            B.AUTONUM_CALCULO = :SimuladorId {filtro}", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "SimuladorId", value: simuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ServicoId", value: servicoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Margem", value: margem, direction: ParameterDirection.Input);

                    var filtro = !string.IsNullOrEmpty(margem) ? " AND A.MARGEM = @Margem " : string.Empty;

                    if (!string.IsNullOrEmpty(tipoCarga))
                    {
                        filtro += " AND TIPO_CARGA = '" + tipoCarga + "' ";
                    }

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            SUM(A.VALOR_FINAL) ValorFinal, 
                            MAX(A.PRECO_UNITARIO) PrecoUnitario, 
                            MAX(A.DESCR_BASE_CALC) BaseCalculo    
                        FROM 
                            SGIPA..TB_SIMULADOR_SERVICOS_CALC A 
                        INNER JOIN 
                            SGIPA..TB_SIMULADOR_SERVICOS B ON A.AUTONUM_SERVICO_CALCULO = B.AUTONUM 
                        WHERE 
                            A.LISTA = @TabelaId 
                        AND 
                            B.AUTONUM_SERVICO = @ServicoId
                        AND 
                            B.AUTONUM_CALCULO = @SimuladorId {filtro}", parametros).FirstOrDefault();
                }
            }
        }

        public decimal? ObterPrecoMinimoServicoVariavelPorId(int servicoVariavelId, bool ehConteiner)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "ServicoVariavelId", value: servicoVariavelId, direction: ParameterDirection.Input);

                    var filtroSQL = ehConteiner ? " AND TIPO_CARGA IN('SVAR','SVAR40','SVAR20') " : " AND TIPO_CARGA ='CRGST' ";

                    return con.Query<decimal?>($@"SELECT PRECO_MINIMO As PrecoMinimo FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE AUTONUM = :ServicoVariavelId {filtroSQL}", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "ServicoVariavelId", value: servicoVariavelId, direction: ParameterDirection.Input);

                    var filtroSQL = ehConteiner ? " AND TIPO_CARGA IN('SVAR','SVAR40','SVAR20') " : " AND TIPO_CARGA ='CRGST' ";

                    return con.Query<decimal?>($@"SELECT PRECO_MINIMO As PrecoMinimo FROM {_schema}..TB_LISTA_P_S_PERIODO WHERE AUTONUM = @ServicoVariavelId {filtroSQL}", parametros).FirstOrDefault();
                }
            }
        }

        public IEnumerable<ServicoFixoVariavel> ObterAdicionais(int tabelaId, int Tipo_Carga)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    if (Tipo_Carga==1)
                    {
                        return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            DISTINCT
                                A.DESCR As Descricao, 
                                B.VALOR_ACRESCIMO As ValorAcrescimo,
                                B.SERVICO As ServicoId
                        FROM
                            SGIPA.TB_SERVICOS_IPA A 
                        INNER JOIN 
                            {_schema}.TB_LISTA_P_S_PERIODO B ON A.AUTONUM = B.SERVICO 
                        WHERE 
                            B.LISTA = :TabelaId 
                        AND  B.TIPO_CARGA='CRGST' AND 
                            (B.VALOR_ACRESCIMO > 0 or a.autonum=52 or a.autonum=45)  AND A.AUTONUM <> 295 AND A.FLAG_TAXA_LIBERACAO=0 

                        UNION ALL

                        SELECT 
                            DISTINCT
                                A.DESCR As Descricao, 
                                B.VALOR_ACRESCIMO As ValorAcrescimo,
                                B.SERVICO As ServicoId
                        FROM
                            SGIPA.TB_SERVICOS_IPA A 
                        INNER JOIN 
                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS B ON A.AUTONUM = B.SERVICO 
                        WHERE 
                            B.LISTA = :TabelaId 
                        AND  B.TIPO_CARGA='CRGST' AND 
                            (B.VALOR_ACRESCIMO > 0 or a.autonum=52 or a.autonum=45) AND A.AUTONUM <> 295 AND A.FLAG_TAXA_LIBERACAO=0 ", parametros);
                   
                    }
                    else
                    {
                        return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            DISTINCT
                                A.DESCR As Descricao, 
                                B.VALOR_ACRESCIMO As ValorAcrescimo,
                                B.SERVICO As ServicoId
                        FROM
                            SGIPA.TB_SERVICOS_IPA A 
                        INNER JOIN 
                            {_schema}.TB_LISTA_P_S_PERIODO B ON A.AUTONUM = B.SERVICO 
                        WHERE 
                            B.LISTA = :TabelaId 
                        AND  B.TIPO_CARGA<>'CRGST' AND 
                            (B.VALOR_ACRESCIMO > 0 or a.autonum=52 or a.autonum=45) AND A.AUTONUM <> 295 AND A.FLAG_TAXA_LIBERACAO=0 

                        UNION ALL

                        SELECT 
                            DISTINCT
                                A.DESCR As Descricao, 
                                B.VALOR_ACRESCIMO As ValorAcrescimo,
                                B.SERVICO As ServicoId
                        FROM
                            SGIPA.TB_SERVICOS_IPA A 
                        INNER JOIN 
                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS B ON A.AUTONUM = B.SERVICO 
                        WHERE 
                            B.LISTA = :TabelaId 
                        AND  B.TIPO_CARGA<>'CRGST' AND 
                           (B.VALOR_ACRESCIMO > 0 or a.autonum=52 or a.autonum=45) AND A.AUTONUM <> 295 AND A.FLAG_TAXA_LIBERACAO=0 ", parametros);
                    }
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            DISTINCT
                                A.DESCR As Descricao, 
                                B.VALOR_ACRESCIMO As ValorAcrescimo
                        FROM
                            SGIPA..TB_SERVICOS_IPA A 
                        INNER JOIN 
                            {_schema}..TB_LISTA_P_S_PERIODO B ON A.AUTONUM = B.SERVICO 
                        WHERE 
                            B.LISTA = @TabelaId 
                        AND 
                            B.VALOR_ACRESCIMO > 0 AND A.AUTONUM <> 295

                        UNION ALL

                        SELECT 
                            DISTINCT
                                A.DESCR As Descricao, 
                                B.VALOR_ACRESCIMO As ValorAcrescimo
                        FROM
                            SGIPA..TB_SERVICOS_IPA A 
                        INNER JOIN 
                            {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS B ON A.AUTONUM = B.SERVICO 
                        WHERE 
                            B.LISTA = @TabelaId 
                        AND 
                            B.VALOR_ACRESCIMO > 0 AND A.AUTONUM <> 295", parametros);
                }
            }
        }

        public IEnumerable<ServicoFixoVariavel> ObterAdicionaisAnvisa(int tabelaId, int Tipo_carga)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    if (Tipo_carga == 1)
                    {
                        return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            DISTINCT
                                A.DESCR As Descricao, 
                                B.VALOR_ANVISA As ValorAnvisa
                        FROM
                            SGIPA.TB_SERVICOS_IPA A 
                        INNER JOIN 
                            {_schema}.TB_LISTA_P_S_PERIODO B ON A.AUTONUM = B.SERVICO 
                        WHERE 
                            B.LISTA = :TabelaId 
                        AND  B.TIPO_CARGA='CRGST' AND 
                            B.VALOR_ANVISA > 0 AND A.AUTONUM <> 295

                        UNION ALL

                        SELECT 
                            DISTINCT
                                A.DESCR As Descricao, 
                                B.VALOR_ANVISA As ValorAnvisa
                        FROM
                            SGIPA.TB_SERVICOS_IPA A 
                        INNER JOIN 
                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS B ON A.AUTONUM = B.SERVICO 
                        WHERE 
                            B.LISTA = :TabelaId 
                        AND  B.TIPO_CARGA='CRGST' AND 
                            B.VALOR_ANVISA > 0 AND A.AUTONUM <> 295", parametros);
                    }
                    else
                    {
                        return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            DISTINCT
                                A.DESCR As Descricao, 
                                B.VALOR_ANVISA As ValorAnvisa
                        FROM
                            SGIPA.TB_SERVICOS_IPA A 
                        INNER JOIN 
                            {_schema}.TB_LISTA_P_S_PERIODO B ON A.AUTONUM = B.SERVICO 
                        WHERE 
                            B.LISTA = :TabelaId 
                        AND  B.TIPO_CARGA<>'CRGST' AND 
                            B.VALOR_ANVISA > 0 AND A.AUTONUM <> 295

                        UNION ALL

                        SELECT 
                            DISTINCT
                                A.DESCR As Descricao, 
                                B.VALOR_ANVISA As ValorAnvisa
                        FROM
                            SGIPA.TB_SERVICOS_IPA A 
                        INNER JOIN 
                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS B ON A.AUTONUM = B.SERVICO 
                        WHERE 
                            B.LISTA = :TabelaId 
                        AND  B.TIPO_CARGA<>'CRGST' AND 
                            B.VALOR_ANVISA > 0 AND A.AUTONUM <> 295", parametros);
                    }

                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"
                        SELECT 
                            DISTINCT
                                A.DESCR As Descricao, 
                                B.VALOR_ANVISA As ValorAnvisa
                        FROM
                            SGIPA..TB_SERVICOS_IPA A 
                        INNER JOIN 
                             {_schema}..TB_LISTA_P_S_PERIODO B ON A.AUTONUM = B.SERVICO 
                        WHERE 
                            B.LISTA = @TabelaId 
                        AND 
                            B.VALOR_ANVISA > 0 AND A.AUTONUM <> 295

                        UNION ALL

                        SELECT 
                            DISTINCT
                                A.DESCR As Descricao, 
                                B.VALOR_ANVISA As ValorAnvisa
                        FROM
                            SGIPA..TB_SERVICOS_IPA A 
                        INNER JOIN 
                             {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS B ON A.AUTONUM = B.SERVICO 
                        WHERE 
                            B.LISTA = @TabelaId 
                        AND 
                            B.VALOR_ANVISA > 0 AND A.AUTONUM <> 295", parametros);
                }
            }
        }

        public decimal ObterAdicionalArmazenagem(int tabelaId, int Tipo_carga)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    if (Tipo_carga == 1)
                    {
                        return con.Query<decimal>($@"SELECT NVL(MAX(PRECO_UNITARIO),0) PrecoUnitario FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE LISTA = :TabelaId AND SERVICO = 295 AND TIPO_CARGA='CRGST'", parametros).FirstOrDefault();
                    }
                    else
                    {
                        return con.Query<decimal>($@"SELECT NVL(MAX(PRECO_UNITARIO),0) PrecoUnitario FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE LISTA = :TabelaId AND SERVICO = 295  AND TIPO_CARGA<>'CRGST'", parametros).FirstOrDefault();
                    }

                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    return con.Query<decimal>($@"SELECT ISNULL(MAX(PRECO_UNITARIO),0) PrecoUnitario FROM {_schema}..TB_LISTA_P_S_PERIODO WHERE LISTA = @TabelaId AND SERVICO = 295", parametros).FirstOrDefault();
                }
            }
        }

        public IEnumerable<CIFEscalonadoDTO> ObterFaixasCIF(int oportunidadeId, string margem)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: margem, direction: ParameterDirection.Input);

                return con.Query<CIFEscalonadoDTO>($@"
                    SELECT 
                        A.AUTONUM As Id, 
                        A.AUTONUMSV As ServicoVariavelId, 
                        A.ValorInicial, 
                        A.ValorFinal, 
                        B.TIPO_CARGA As TipoCarga,
                        B.VARIANTE_LOCAL,
                        A.PERCENTUAL,
                        A.MINIMO,B.N_PERIODO PERIODO
                    FROM 
                        CRM.TB_LISTA_P_S_FAIXASCIF_PER A 
                    INNER JOIN 
                        CRM.TB_LISTA_P_S_PERIODO B ON A.AUTONUMSV = B.AUTONUM 
                    INNER JOIN
                        CRM.TB_LISTAS_PRECOS C ON B.LISTA = C.AUTONUM 
                    WHERE 
                        C.OportunidadeId = :OportunidadeId AND B.VARIANTE_LOCAL = :Margem AND A.PERCENTUAL > 0
                    ORDER BY 
                        B.N_PERIODO, B.VARIANTE_LOCAL,A.ValorInicial 
                        ", parametros);
            }
        }
        public int? ObterQuantFaixas(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<int>(@"SELECT 
                         count(1)
                    FROM 
                        CRM.TB_LISTA_P_S_FAIXASCIF_PER A 
                    INNER JOIN 
                        CRM.TB_LISTA_P_S_PERIODO B ON A.AUTONUMSV = B.AUTONUM 
                    INNER JOIN
                        CRM.TB_LISTAS_PRECOS C ON B.LISTA = C.AUTONUM 
                    WHERE 
                        C.OportunidadeId = :OportunidadeId AND B.N_PERIODO = 1 AND A.PERCENTUAL > 0 ", parametros).Single();
            }
        }

        public IEnumerable<ValorMinimoCobrancaDTO> ObterValorMinimoCobranca(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: TipoRegistro.ARMAZENAGEM_MINIMO, direction: ParameterDirection.Input);

                return con.Query<ValorMinimoCobrancaDTO>($@"SELECT Descricao, ValorMinimo, ValorMinimo20, ValorMinimo40 FROM 
                        CRM.TB_CRM_OPORTUNIDADE_LAYOUT WHERE OportunidadeId = :OportunidadeId AND TipoRegistro = :TipoRegistro AND LimiteBls > 0", parametros);
            }
        }
    }
}