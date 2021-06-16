using Dapper;
using Ecoporto.CRM.Business.Enums;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using WsSimuladorCalculoTabelas.Configuracao;
using WsSimuladorCalculoTabelas.DTO;
using WsSimuladorCalculoTabelas.Enums;
using WsSimuladorCalculoTabelas.Extensions;
using WsSimuladorCalculoTabelas.Helpers;
using WsSimuladorCalculoTabelas.Models;
using WsSimuladorCalculoTabelas.Requests;

namespace WsSimuladorCalculoTabelas.DAO
{
    public class TabelasDAO
    {
        private readonly bool _crm;
        private readonly string _schema;

        public TabelasDAO(bool crm)
        {
            _crm = crm;
            _schema = crm ? "CRM" : "SGIPA";
        }

        public IEnumerable<Tabela> ObterTabelas(TabelasRequest filtro)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                var SQL = $@"
                    SELECT 
                        DISTINCT
                            A.AUTONUM AS TabelaId,
                            A.DESCR As Descricao,
                            B.FANTASIA AS Importador,
                            C.FANTASIA AS Despachante,
                            D.FANTASIA AS NVOCC,
                            E.FANTASIA AS Coloader,
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
                        SGIPA.TB_CAD_PARCEIROS E ON A.COLOADER = E.AUTONUM ";

                if (filtro.CRM)
                {
                    SQL += $" WHERE A.AUTONUM > 0 AND D.CGC IN :ClienteCnpj ";
                }
                else
                {
                    if (filtro.CalculoAutomatico && filtro.ClasseCliente > 0)
                    {
                        SQL += " WHERE A.AUTONUM > 0 ";

                        switch (filtro.ClasseCliente)
                        {
                            case 1:
                                SQL += " AND A.IMPORTADOR > 0 ";
                                break;
                            case 2:
                                SQL += " AND A.DESPACHANTE > 0 ";
                                break;
                            case 3:
                                SQL += " AND A.CAPTADOR > 0 ";
                                break;
                            case 4:
                                SQL += " AND A.COLOADER > 0 ";
                                break;
                        }
                    }
                    else
                    {
                        if (filtro.ClienteId > 0)
                        {
                            SQL += $" AND (A.IMPORTADOR = :ClienteId OR A.DESPACHANTE = :ClienteId OR A.CAPTADOR = :ClienteId)";
                        }
                        else
                        {
                            if (filtro.ClasseCliente > 0)
                            {
                                SQL += $@"
                                    INNER JOIN 
                                        (
                                            SELECT 
                                                B.AUTONUM_CLIENTE 
                                            FROM 
                                                SGIPA.TB_SIMULADOR_CALCULO A 
                                            INNER JOIN
                                                SGIPA.TB_SIMULADOR_CALCULO_CLIENTES B ON A.AUTONUM = B.AUTONUM_CALCULO
                                            WHERE
                                                A.AUTONUM = :SimuladorId AND NVL(A.FLAG_CRM, 0) = 0
                                        ) CLI ON ";

                                switch (filtro.ClasseCliente)
                                {
                                    case 1:
                                        SQL += " A.IMPORTADOR = CLI.AUTONUM_CLIENTE ";
                                        break;
                                    case 2:
                                        SQL += " A.DESPACHANTE = CLI.AUTONUM_CLIENTE ";
                                        break;
                                    case 3:
                                        SQL += " A.CAPTADOR = CLI.AUTONUM_CLIENTE ";
                                        break;
                                    case 4:
                                        SQL += " A.COLOADER = CLI.AUTONUM_CLIENTE ";
                                        break;
                                }
                            }

                            SQL += " WHERE A.AUTONUM > 0 ";
                        }
                    }
                }

                if (filtro.TabelaId > 0)
                {
                    SQL += " AND A.AUTONUM = :TabelaId ";
                }

                SQL += " AND A.FLAG_LIBERADA = 1 AND A.FLAG_ACORDO = 0 AND A.COD_EMPRESA = 1 AND (A.FINAL_VALIDADE > SYSDATE OR A.FINAL_VALIDADE IS NULL) ";

                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ClienteId", value: filtro.ClienteId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ClienteCnpj", value: filtro.Cnpjs, direction: ParameterDirection.Input);
                    parametros.Add(name: "SimuladorId", value: filtro.SimuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: filtro.TabelaId, direction: ParameterDirection.Input);

                    return con.Query<Tabela>(SQL, parametros).ToList();
                }
            }
            else
            {
                var SQL = $@"
                    SELECT 
                        DISTINCT
                            A.AUTONUM AS TabelaId,
                            A.DESCR As Descricao,
                            B.FANTASIA AS Importador,
                            C.FANTASIA AS Despachante,
                            D.FANTASIA AS NVOCC,
                            E.FANTASIA AS Coloader,
                            A.Proposta
                    FROM
                        SGIPA..TB_LISTAS_PRECOS A
                    LEFT JOIN
                        SGIPA..TB_CAD_PARCEIROS B ON A.IMPORTADOR = B.AUTONUM
                    LEFT JOIN
                        SGIPA..TB_CAD_PARCEIROS C ON A.DESPACHANTE = C.AUTONUM
                    LEFT JOIN
                        SGIPA..TB_CAD_PARCEIROS D ON A.CAPTADOR = D.AUTONUM
                    LEFT JOIN
                        SGIPA..TB_CAD_PARCEIROS E ON A.COLOADER = E.AUTONUM ";

                if (filtro.CRM)
                {
                    SQL += $" WHERE A.AUTONUM > 0 AND D.CGC IN @ClienteCnpj ";
                }
                else
                {
                    if (filtro.CalculoAutomatico && filtro.ClasseCliente > 0)
                    {
                        SQL += " WHERE A.AUTONUM > 0 ";

                        switch (filtro.ClasseCliente)
                        {
                            case 1:
                                SQL += " AND A.IMPORTADOR > 0 ";
                                break;
                            case 2:
                                SQL += " AND A.DESPACHANTE > 0 ";
                                break;
                            case 3:
                                SQL += " AND A.CAPTADOR > 0 ";
                                break;
                            case 4:
                                SQL += " AND A.COLOADER > 0 ";
                                break;
                        }
                    }
                    else
                    {
                        if (filtro.ClienteId > 0)
                        {
                            SQL += $" AND (A.IMPORTADOR = @ClienteId OR A.DESPACHANTE = @ClienteId OR A.CAPTADOR = @ClienteId)";
                        }
                        else
                        {
                            if (filtro.ClasseCliente > 0)
                            {
                                SQL += $@"
                                    INNER JOIN 
                                        (
                                            SELECT 
                                                B.AUTONUM_CLIENTE 
                                            FROM 
                                                SGIPA..TB_SIMULADOR_CALCULO A 
                                            INNER JOIN
                                                SGIPA..TB_SIMULADOR_CALCULO_CLIENTES B ON A.AUTONUM = B.AUTONUM_CALCULO
                                            WHERE
                                                A.AUTONUM = @SimuladorId AND ISNULL(A.FLAG_CRM, 0) = 0
                                        ) CLI ON ";

                                switch (filtro.ClasseCliente)
                                {
                                    case 1:
                                        SQL += " A.IMPORTADOR = CLI.AUTONUM_CLIENTE ";
                                        break;
                                    case 2:
                                        SQL += " A.DESPACHANTE = CLI.AUTONUM_CLIENTE ";
                                        break;
                                    case 3:
                                        SQL += " A.CAPTADOR = CLI.AUTONUM_CLIENTE ";
                                        break;
                                    case 4:
                                        SQL += " A.COLOADER = CLI.AUTONUM_CLIENTE ";
                                        break;
                                }
                            }

                            SQL += " WHERE A.AUTONUM > 0 ";
                        }
                    }
                }

                if (filtro.TabelaId > 0)
                {
                    SQL += " AND A.AUTONUM = @TabelaId ";
                }

                SQL += " AND A.FLAG_LIBERADA = 1 AND A.FLAG_ACORDO = 0 AND A.COD_EMPRESA = 1 AND (A.FINAL_VALIDADE > GETDATE() OR A.FINAL_VALIDADE IS NULL) ";

                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ClienteId", value: filtro.ClienteId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ClienteCnpj", value: filtro.Cnpjs, direction: ParameterDirection.Input);
                    parametros.Add(name: "SimuladorId", value: filtro.SimuladorId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: filtro.TabelaId, direction: ParameterDirection.Input);

                    return con.Query<Tabela>(SQL, parametros).ToList();
                }
            }
        }

        public IEnumerable<ServicoFixoVariavel> ObterServicoPeriodoPorLinha(int linha, int oportunidadeId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"SELECT AUTONUM As ServicoFixoVariavelId, N_PERIODO As Periodo, OportunidadeId, Linha, Preco_Minimo, OportunidadeId, VARIANTE_LOCAL As VarianteLocal, TIPO_CARGA As TipoCarga, BASE_CALCULO As BaseCalculo FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE Linha = :Linha AND OportunidadeId = :oportunidadeId order by autonum ", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"SELECT AUTONUM As ServicoFixoVariavelId, N_PERIODO As Periodo, OportunidadeId, Linha, Preco_Minimo, OportunidadeId, VARIANTE_LOCAL As VarianteLocal, TIPO_CARGA As TipoCarga, BASE_CALCULO As BaseCalculo FROM {_schema}..TB_LISTA_P_S_PERIODO WHERE Linha = @Linha AND OportunidadeId = @oportunidadeId order by autonum ", parametros);
                }
            }
        }

        public IEnumerable<ServicoFixoVariavel> ObterServicoFixoPorLinha(int linha, int oportunidadeId, string tipocarga)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                    if (tipocarga != "")
                    {
                        parametros.Add(name: "Tipo_carga", value: tipocarga, direction: ParameterDirection.Input);
                        return con.Query<ServicoFixoVariavel>($@"SELECT AUTONUM As ServicoFixoVariavelId, OportunidadeId, Linha, Preco_Minimo, OportunidadeId, VARIANTE_LOCAL As VarianteLocal FROM {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE Linha = :Linha AND OportunidadeId = :oportunidadeId AND tipo_carga = :Tipo_carga", parametros);
                    }
                    else
                    {
                        return con.Query<ServicoFixoVariavel>($@"SELECT AUTONUM As ServicoFixoVariavelId, OportunidadeId, Linha, Preco_Minimo, OportunidadeId, VARIANTE_LOCAL As VarianteLocal FROM {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE Linha = :Linha AND OportunidadeId = :oportunidadeId", parametros);
                    }
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    return con.Query<ServicoFixoVariavel>($@"SELECT AUTONUM As ServicoFixoVariavelId, OportunidadeId, Linha, Preco_Minimo, OportunidadeId, VARIANTE_LOCAL As VarianteLocal FROM {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS WHERE Linha = @Linha AND OportunidadeId = @oportunidadeId", parametros);
                }
            }
        }



        public IEnumerable<int> ObterServicosArmazenagemAnteriores(int periodo, string tipoCarga, string baseCalculo, int lista, int linha)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "Periodo", value: periodo, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoCarga", value: tipoCarga, direction: ParameterDirection.Input);
                    parametros.Add(name: "BaseCalculo", value: baseCalculo, direction: ParameterDirection.Input);
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);
                    parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                    if ((tipoCarga != "CRGST") && (tipoCarga != "BBK") && (tipoCarga != "VEIC"))
                    {
                        return con.Query<int>($@"
                        SELECT AUTONUM FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE SERVICO = 52 AND TIPO_CARGA = :TipoCarga 
                         AND N_PERIODO>=:periodo  AND BASE_CALCULO = :BaseCalculo AND LISTA = :Lista AND LINHA <= :Linha", parametros);
                    }
                    else
                    {
                        return con.Query<int>($@"
                        SELECT AUTONUM FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE SERVICO = 52 AND TIPO_CARGA = :TipoCarga 
                            AND BASE_CALCULO = :BaseCalculo AND LISTA = :Lista AND  N_PERIODO>=:periodo ", parametros);
                    }
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "Periodo", value: periodo, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoCarga", value: tipoCarga, direction: ParameterDirection.Input);
                    parametros.Add(name: "BaseCalculo", value: baseCalculo, direction: ParameterDirection.Input);
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);
                    parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);

                    return con.Query<int>($@"
                        SELECT AUTONUM FROM {_schema}.dbo.TB_LISTA_P_S_PERIODO WHERE SERVICO = 52 AND TIPO_CARGA = @TipoCarga AND BASE_CALCULO = @BaseCalculo AND LISTA = @Lista AND LINHA <= @Linha", parametros);
                }
            }
        }

        public void AtualizarPrecoMinimo(decimal precoMinimo, int limiteBls, int id, string tipoCarga, int oportunidadeId, string margem)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "PrecoMinimo", value: precoMinimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "LimiteBls", value: limiteBls, direction: ParameterDirection.Input);
                    parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoCarga", value: tipoCarga, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Variante_local", value: margem, direction: ParameterDirection.Input);

                    con.Execute($@"UPDATE {_schema}.TB_LISTA_P_S_PERIODO SET PRECO_MINIMO = :PrecoMinimo, LIMITE_BLS = :LimiteBls WHERE Servico = 52 AND TIPO_CARGA = :TipoCarga AND OportunidadeId = :OportunidadeId and Variante_local=:Variante_local and AUTONUM >= :Id", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "PrecoMinimo", value: precoMinimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "LimiteBls", value: limiteBls, direction: ParameterDirection.Input);
                    parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);
                    parametros.Add(name: "TipoCarga", value: tipoCarga, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Variante_local", value: margem, direction: ParameterDirection.Input);

                    con.Execute($@"UPDATE {_schema}..TB_LISTA_P_S_PERIODO SET PRECO_MINIMO = @PrecoMinimo, LIMITE_BLS = @LimiteBls WHERE Servico = 52 AND TIPO_CARGA = @TipoCarga AND OportunidadeId = @OportunidadeId and Variante_local=:Variante_local and  AUTONUM >= @Id", parametros);
                }
            }
        }

        public void AtualizarPrecoMinimoVariavel(decimal precoMinimo, int id)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "PrecoMinimo", value: precoMinimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);
                    con.Execute($@"UPDATE {_schema}.TB_LISTA_P_S_PERIODO SET PRECO_MINIMO = :PrecoMinimo WHERE AUTONUM = :Id", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "PrecoMinimo", value: precoMinimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);
                    con.Execute($@"UPDATE {_schema}..TB_LISTA_P_S_PERIODO SET PRECO_MINIMO = @PrecoMinimo  WHERE AUTONUM = :Id", parametros);
                }
            }
        }


        public void CadastrarFaixaMinimo(decimal valorMinimo, string tipo, int numeroBls, int[] ids)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ValorMinimo", value: valorMinimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);
                    parametros.Add(name: "NumeroBls", value: numeroBls, direction: ParameterDirection.Input);

                    foreach (var id in ids)
                    {
                        parametros.Add(name: "ServicoId", value: id, direction: ParameterDirection.Input);

                        con.Execute($@"DELETE FROM {_schema}.TB_LISTA_CFG_VALORMINIMO WHERE AUTONUMSV=:ServicoId AND NBLS =:NumeroBls AND TIPO =:Tipo", parametros);

                        con.Execute($@"INSERT INTO {_schema}.TB_LISTA_CFG_VALORMINIMO (AUTONUM, AUTONUMSV, NBLS, VALORMINIMO, TIPO, PERCMULTA) VALUES ({_schema}.SEQ_LISTA_CFG_VALORMINIMO.NEXTVAL, :ServicoId, :NumeroBls, :ValorMinimo, :Tipo, 0)", parametros);

                        con.Execute($@"UPDATE {_schema}.TB_LISTA_P_S_PERIODO SET PRECO_MINIMO = 0 WHERE AUTONUM= :ServicoId", parametros);
                    }
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ValorMinimo", value: valorMinimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);
                    parametros.Add(name: "NumeroBls", value: numeroBls, direction: ParameterDirection.Input);

                    foreach (var id in ids)
                    {
                        parametros.Add(name: "ServicoId", value: id, direction: ParameterDirection.Input);

                        con.Execute($@"INSERT INTO {_schema}..TB_LISTA_CFG_VALORMINIMO (AUTONUMSV, NBLS, VALORMINIMO, TIPO, PERCMULTA) VALUES (@ServicoId, @NumeroBls, @ValorMinimo, @Tipo, 0)", parametros);
                    }
                }
            }
        }

        public void AtualizarPrecoMinimoFixo(decimal precoMinimo, int id)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ServicoFixoVariavelId", value: id, direction: ParameterDirection.Input);
                    parametros.Add(name: "PrecoMinimo", value: precoMinimo, direction: ParameterDirection.Input);

                    con.Execute($@"UPDATE {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS SET PRECO_MINIMO = :PrecoMinimo WHERE AUTONUM = :ServicoFixoVariavelId", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "ServicoFixoVariavelId", value: id, direction: ParameterDirection.Input);
                    parametros.Add(name: "PrecoMinimo", value: precoMinimo, direction: ParameterDirection.Input);

                    con.Execute($@"UPDATE {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS SET PRECO_MINIMO = @PrecoMinimo WHERE AUTONUM = @ServicoFixoVariavelId", parametros);
                }
            }
        }

        public void DeletarServicosFixosVariaveis(int oportunidadeId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    con.Open();

                    var parametros = new DynamicParameters();
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    using (var transaction = con.BeginTransaction())
                    {
                        con.Execute($@"DELETE FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE OportunidadeId = :OportunidadeId", parametros, transaction);
                        con.Execute($@"DELETE FROM {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE OportunidadeId = :OportunidadeId", parametros, transaction);

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
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    using (var transaction = con.BeginTransaction())
                    {
                        con.Execute($@"DELETE FROM {_schema}..TB_LISTA_P_S_PERIODO WHERE OportunidadeId = @OportunidadeId", parametros, transaction);
                        con.Execute($@"DELETE FROM {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS WHERE OportunidadeId = @OportunidadeId", parametros, transaction);

                        transaction.Commit();
                    }
                }
            }
        }

        public void GravarLotes(int tabelaId, int tabelaIdRevisada, string[] lotes)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    foreach (var lote in lotes)
                    {
                        var parametros = new DynamicParameters();

                        parametros.Add(name: "Lote", value: lote, direction: ParameterDirection.Input);
                        parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                        parametros.Add(name: "TabelaIdRevisada", value: tabelaIdRevisada, direction: ParameterDirection.Input);

                        if (tabelaIdRevisada > 0)
                        {
                            con.Execute($@"DELETE FROM SGIPA.TB_LP_LOTES WHERE AUTONUM_LISTA = :TabelaIdRevisada AND LOTE = :Lote", parametros);
                        }

                        con.Execute($@"INSERT INTO SGIPA.TB_LP_LOTES (AUTONUM_LISTA, LOTE, DATA) VaLUES (:TabelaId, :Lote, SYSDATE)", parametros);
                        con.Execute($@"update SGIPA.tb_bl set autonum_lista=:TabelaId  where autonum=:Lote", parametros);
                        con.Execute($@"update SGIPA.tb_gr_bl set tabela_gr=:TabelaId where bl=:Lote", parametros);
                        con.Execute($@"update SGIPA.tb_gr_pre_calculo set lista=:TabelaId where bl=:Lote", parametros);
                    }
                    var parametros1 = new DynamicParameters();

                    parametros1.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    var tabelas = con.Query<Tabela>(@" 
                           SELECT DataFinalValidade FROM (
                           SELECT 
                           Max(C.ULTIMA_SAIDA) As DataFinalValidade 
                           FROM SGIPA.TB_LISTAS_PRECOS A  
                           INNER JOIN SGIPA.TB_LP_LOTES B 
                           ON A.AUTONUM=B.AUTONUM_LISTA
                           INNER JOIN SGIPA.TB_BL C ON C.AUTONUM=B.LOTE
                           WHERE FLAG_ACORDO=1  AND A.AUTONUM=:TabelaId 
                           GROUP BY A.AUTONUM) WHERE DataFinalValidade IS NOT NULL 
                            ", parametros1);

 
                    foreach (var tabela in tabelas)
                    {
                        parametros1.Add(name: "FimVigencia", value: tabela.DataFinalValidade?.Date, direction: ParameterDirection.Input);
                        con.Execute(@"UPDATE 
                                        SGIPA.TB_LISTAS_PRECOS SET 
                                            FINAL_VALIDADE = :FimVigencia 
                                        WHERE 
                                            AUTONUM = :TabelaId ", parametros1);
                    }

                            
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    foreach (var lote in lotes)
                    {
                        var parametros = new DynamicParameters();

                        parametros.Add(name: "Lote", value: lote, direction: ParameterDirection.Input);
                        parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                        con.Execute($@"INSERT INTO SGIPA..TB_TP_LOTES (AUTONUM_LISTA, LOTE, DATA) VaLUES (@TabelaId, @Lote, GetDate())", parametros);
                    }
                }
            }
        }

        public void ExportarLotes(int novaTabela, Oportunidade oportunidade)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "OportunidadeId", value: oportunidade.Id, direction: ParameterDirection.Input);

                    var oportunidadeRevisada = con.Query<string>(@"SELECT RevisaoId FROM TB_CRM_OPORTUNIDADES WHERE Id = :OportunidadeId", parametros).FirstOrDefault();

                    parametros = new DynamicParameters();
                    parametros.Add(name: "OportunidadeRevisada", value: oportunidadeRevisada, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeId", value: oportunidade.Id, direction: ParameterDirection.Input);

                    var tabelaIdRevisada = con.Query<int>(@"SELECT NVL(MAX(TabelaId),0) TabelaId FROM TB_CRM_OPORTUNIDADES WHERE Id = :OportunidadeRevisada", parametros).FirstOrDefault();
                    
                    var lotesProposta = con.Query<string>(@"SELECT LOTE FROM TB_CRM_OPORTUNIDADES WHERE Id = :OportunidadeId", parametros).FirstOrDefault();

                    if (!string.IsNullOrEmpty(lotesProposta))
                    {
                        var lotes = lotesProposta.SubstituirCaracteresEspeciais(",").Split(',');

                        GravarLotes(novaTabela, tabelaIdRevisada, lotes);
                    }
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "OportunidadeId", value: oportunidade.Id, direction: ParameterDirection.Input);

                    var lotesProposta = con.Query<string>(@"SELECT LOTE FROM TB_CRM_OPORTUNIDADES WHERE Id = @OportunidadeId", parametros).FirstOrDefault();

                    if (!string.IsNullOrEmpty(lotesProposta))
                    {
                        var lotes = lotesProposta.SubstituirCaracteresEspeciais(",").Split(',');

                        GravarLotes(novaTabela,0, lotes);
                    }
                }
            }
        }

        public bool ExisteServicoAdicional(ServicoFixoVariavel servicoFixoVariavel)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: servicoFixoVariavel.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: servicoFixoVariavel.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "PrecoUnitario", value: servicoFixoVariavel.PrecoUnitario, direction: ParameterDirection.Input);
                parametros.Add(name: "PrecoMinimo", value: servicoFixoVariavel.PrecoMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorAnvisa", value: servicoFixoVariavel.ValorAnvisa, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorAcrescimo", value: servicoFixoVariavel.ValorAcrescimo, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: servicoFixoVariavel.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: servicoFixoVariavel.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: servicoFixoVariavel.OportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Tipocarga", value: servicoFixoVariavel.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "VarianteLocal", value: servicoFixoVariavel.VarianteLocal, direction: ParameterDirection.Input);
                parametros.Add(name: "Dias", value: servicoFixoVariavel.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "GrupoAtracacao", value: servicoFixoVariavel.GrupoAtracacaoId, direction: ParameterDirection.Input);

                return con.Query<bool>($@"
                    SELECT 
                        AUTONUM FROM 
                    {_schema}.TB_LISTA_P_S_PERIODO WHERE LINHA = :Linha AND SERVICO = :ServicoId 
                        AND QTDE_DIAS=:Dias AND PRECO_UNITARIO = :PrecoUnitario AND PRECO_MINIMO = :PrecoMinimo 
                        AND VALOR_ANVISA = :ValorAnvisa AND VALOR_ACRESCIMO = :ValorAcrescimo AND TIPO_CARGA=:Tipocarga AND VARIANTE_LOCAL = :VarianteLocal
                        AND NVL(grupo_atracacao,0)=:GrupoAtracacao AND BASE_CALCULO = :BaseCalculo AND N_PERIODO = :Periodo AND OPORTUNIDADEID = :OportunidadeId", parametros).Any();
            }
        }

        public void GravarServicoVariavel(ServicoFixoVariavel servicoFixoVariavel, int MinimoCIF)
        {
            long Autonumsv;
            Double Ciffinal;
            long CifAnt;
            long TemProrata;

            GravarServicoVariavel_ValidaPeriodo(servicoFixoVariavel);

            if (!servicoFixoVariavel.GrupoAtracacaoId.HasValue)
                servicoFixoVariavel.GrupoAtracacaoId = 0;

            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "OPORTUNIDADEID", value: servicoFixoVariavel.OportunidadeId, direction: ParameterDirection.Input);
                    parametros.Add(name: "LISTA", value: servicoFixoVariavel.Lista, direction: ParameterDirection.Input);
                    parametros.Add(name: "LINHA", value: servicoFixoVariavel.Linha, direction: ParameterDirection.Input);
                    parametros.Add(name: "SERVICO", value: servicoFixoVariavel.ServicoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_CARGA", value: servicoFixoVariavel.TipoCarga, direction: ParameterDirection.Input);
                    parametros.Add(name: "BASE_CALCULO", value: servicoFixoVariavel.BaseCalculo, direction: ParameterDirection.Input);
                    parametros.Add(name: "VARIANTE_LOCAL", value: servicoFixoVariavel.VarianteLocal, direction: ParameterDirection.Input);
                    parametros.Add(name: "BASE_EXCESSO", value: servicoFixoVariavel.BaseExcesso, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_EXCESSO", value: servicoFixoVariavel.ValorExcesso, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_DOC", value: servicoFixoVariavel.TipoDocumentoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "MOEDA", value: servicoFixoVariavel.Moeda, direction: ParameterDirection.Input);
                    if (MinimoCIF == 1)
                    {
                        parametros.Add(name: "PRECO_UNITARIO", value: 0, direction: ParameterDirection.Input);
                        parametros.Add(name: "PRECO_MINIMO", value: 0, direction: ParameterDirection.Input);
                    }
                    else
                    {
                        parametros.Add(name: "PRECO_UNITARIO", value: servicoFixoVariavel.PrecoUnitario, direction: ParameterDirection.Input);
                        parametros.Add(name: "PRECO_MINIMO", value: servicoFixoVariavel.PrecoMinimo, direction: ParameterDirection.Input);
                    }

                    parametros.Add(name: "VALOR_ACRESCIMO", value: servicoFixoVariavel.ValorAcrescimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "EXERCITO", value: servicoFixoVariavel.Exercito, direction: ParameterDirection.Input);
                    parametros.Add(name: "N_PERIODO", value: servicoFixoVariavel.Periodo, direction: ParameterDirection.Input);
                    parametros.Add(name: "GRUPO_ATRACACAO", value: servicoFixoVariavel.GrupoAtracacaoId.Value, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ACRESC_PESO", value: servicoFixoVariavel.ValorAcrescimoPeso, direction: ParameterDirection.Input);
                    parametros.Add(name: "PESO_LIMITE", value: servicoFixoVariavel.PesoLimite, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_MAXIMO", value: servicoFixoVariavel.PrecoMaximo, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ANVISA", value: servicoFixoVariavel.ValorAnvisa, direction: ParameterDirection.Input);
                    parametros.Add(name: "FLAG_COBRAR_NVOCC", value: servicoFixoVariavel.CobrarNVOCC.ToInt(), direction: ParameterDirection.Input);
                    parametros.Add(name: "FORMA_PAGAMENTO_NVOCC", value: servicoFixoVariavel.FormaPagamentoNVOCC, direction: ParameterDirection.Input);
                    parametros.Add(name: "ADICIONAL_GRC", value: servicoFixoVariavel.AdicionalGRC, direction: ParameterDirection.Input);

                    if ((servicoFixoVariavel.ServicoId == 45) || (servicoFixoVariavel.ServicoId == 295))
                    {
                        if (servicoFixoVariavel.QtdeDias == 1)
                        {
                            TemProrata = con.Query<int>($@" SELECT nvl(max(QTDE_DIAS),0) FROM { _schema}.TB_LISTA_P_S_PERIODO WHERE LISTA = :LISTA AND SERVICO = 52
                            AND TIPO_CARGA = :TIPO_CARGA AND VARIANTE_LOCAL = :VARIANTE_LOCAL AND N_PERIODO = 1 ", parametros).Single();
                            if (TemProrata != 1)
                            {
                                parametros.Add(name: "FLAG_PRORATA", value: 1, direction: ParameterDirection.Input);
                                parametros.Add(name: "QTDE_DIAS", value: TemProrata, direction: ParameterDirection.Input);

                            }
                        }
                        else
                        {
                            parametros.Add(name: "FLAG_PRORATA", value: servicoFixoVariavel.ProRata.ToInt(), direction: ParameterDirection.Input);
                            parametros.Add(name: "QTDE_DIAS", value: servicoFixoVariavel.QtdeDias, direction: ParameterDirection.Input);
                        }
                    }
                    else
                    {
                        parametros.Add(name: "FLAG_PRORATA", value: servicoFixoVariavel.ProRata.ToInt(), direction: ParameterDirection.Input);
                        parametros.Add(name: "QTDE_DIAS", value: servicoFixoVariavel.QtdeDias, direction: ParameterDirection.Input);
                    }
                    Autonumsv = con.Query<int>($@"SELECT nvl(max(autonum),0) FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE LISTA = :LISTA AND SERVICO = :SERVICO 
                                AND TIPO_CARGA = :TIPO_CARGA AND BASE_CALCULO = :BASE_CALCULO AND VARIANTE_LOCAL = :VARIANTE_LOCAL AND N_PERIODO = :N_PERIODO AND GRUPO_ATRACACAO =:GRUPO_ATRACACAO", parametros).Single();

                    if (Autonumsv == 0)
                    {
                        Autonumsv = con.Query<int>($@"select { _schema}.SEQ_LISTA_P_S_PERIODO.NEXTVAL from dual").Single();
                        parametros.Add(name: "AUTONUMSV", value: Autonumsv, direction: ParameterDirection.Input);


                        con.Execute($@"
                        INSERT INTO
                            {_schema}.TB_LISTA_P_S_PERIODO (
                                AUTONUM,
                                LISTA,
                                OPORTUNIDADEID,
                                LINHA,
                                SERVICO,
                                TIPO_CARGA,
                                BASE_CALCULO,
                                VARIANTE_LOCAL,
                                BASE_EXCESSO,
                                VALOR_EXCESSO,
                                TIPO_DOC,
                                MOEDA,
                                PRECO_UNITARIO,
                                PRECO_MINIMO,
                                VALOR_ACRESCIMO,
                                N_PERIODO,
                                QTDE_DIAS,
                                GRUPO_ATRACACAO,
                                FLAG_PRORATA,
                                VALOR_ACRESC_PESO,
                                PESO_LIMITE,
                                PRECO_MAXIMO,
                                VALOR_ANVISA,
                                FLAG_COBRAR_NVOCC,
                                FORMA_PAGAMENTO_NVOCC,EXERCITO
                            ) VALUES (
                                :AUTONUMSV,
                                :LISTA,
                                :OPORTUNIDADEID,
                                :LINHA,
                                :SERVICO,
                                :TIPO_CARGA,
                                :BASE_CALCULO,
                                :VARIANTE_LOCAL,
                                :BASE_EXCESSO,
                                :VALOR_EXCESSO,
                                :TIPO_DOC,
                                :MOEDA,
                                :PRECO_UNITARIO,
                                :PRECO_MINIMO,
                                :VALOR_ACRESCIMO,
                                :N_PERIODO,
                                :QTDE_DIAS,
                                :GRUPO_ATRACACAO,
                                :FLAG_PRORATA,
                                :VALOR_ACRESC_PESO,
                                :PESO_LIMITE,
                                :PRECO_MAXIMO,
                                :VALOR_ANVISA,
                                :FLAG_COBRAR_NVOCC,
                                :FORMA_PAGAMENTO_NVOCC,
                                :EXERCITO               
                            )", parametros);
                    }
                    if (MinimoCIF == 1)
                    {
                        parametros.Add(name: "AUTONUMSV", value: Autonumsv, direction: ParameterDirection.Input);

                        parametros.Add(name: "CIFINICIAL", value: servicoFixoVariavel.ValorCif, direction: ParameterDirection.Input);
                        var Autonumcif = con.Query<int>($@"select { _schema}.SEQ_LISTA_P_S_FAIXASCIF_PER.NEXTVAL from dual").Single();
                        parametros.Add(name: "AUTONUMCIF", value: Autonumcif, direction: ParameterDirection.Input);
                        parametros.Add(name: "PRECO_UNITARIOCIF", value: servicoFixoVariavel.PrecoUnitario, direction: ParameterDirection.Input);
                        CifAnt = con.Query<int>($@"SELECT nvl(max(autonum),0) FROM {_schema}.TB_LISTA_P_S_FAIXASCIF_PER WHERE AUTONUMSV=:AUTONUMSV", parametros).Single();
                        if (Autonumsv > 0)
                        {


                            con.Execute($@"INSERT INTO {_schema}.TB_LISTA_P_S_FAIXASCIF_PER(
                                       AUTONUM, AUTONUMSV, VALORINICIAL,
                                       VALORFINAL, PERCENTUAL, MINIMO,
                                       TIPO)
                                       VALUES(
                                       :AUTONUMCIF,
                                       :AUTONUMSV,
                                       :CIFINICIAL,
                                       99999999,
                                       :PRECO_UNITARIOCIF,
                                       :PRECO_MINIMO,
                                       'C' )", parametros);
                        }
                    }
                }

            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "OPORTUNIDADEID", value: servicoFixoVariavel.OportunidadeId, direction: ParameterDirection.Input);
                    parametros.Add(name: "LISTA", value: servicoFixoVariavel.Lista, direction: ParameterDirection.Input);
                    parametros.Add(name: "LINHA", value: servicoFixoVariavel.Linha, direction: ParameterDirection.Input);
                    parametros.Add(name: "SERVICO", value: servicoFixoVariavel.ServicoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_CARGA", value: servicoFixoVariavel.TipoCarga, direction: ParameterDirection.Input);
                    parametros.Add(name: "BASE_CALCULO", value: servicoFixoVariavel.BaseCalculo, direction: ParameterDirection.Input);
                    parametros.Add(name: "VARIANTE_LOCAL", value: servicoFixoVariavel.VarianteLocal, direction: ParameterDirection.Input);
                    parametros.Add(name: "BASE_EXCESSO", value: servicoFixoVariavel.BaseExcesso, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_EXCESSO", value: servicoFixoVariavel.ValorExcesso, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_DOC", value: servicoFixoVariavel.TipoDocumentoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "MOEDA", value: servicoFixoVariavel.Moeda, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_UNITARIO", value: servicoFixoVariavel.PrecoUnitario, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_MINIMO", value: servicoFixoVariavel.PrecoMinimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ACRESCIMO", value: servicoFixoVariavel.ValorAcrescimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "EXERCITO", value: servicoFixoVariavel.Exercito, direction: ParameterDirection.Input);
                    parametros.Add(name: "N_PERIODO", value: servicoFixoVariavel.Periodo, direction: ParameterDirection.Input);
                    parametros.Add(name: "QTDE_DIAS", value: servicoFixoVariavel.QtdeDias, direction: ParameterDirection.Input);
                    parametros.Add(name: "GRUPO_ATRACACAO", value: servicoFixoVariavel.GrupoAtracacaoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "FLAG_PRORATA", value: servicoFixoVariavel.ProRata.ToInt(), direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ACRESC_PESO", value: servicoFixoVariavel.ValorAcrescimoPeso, direction: ParameterDirection.Input);
                    parametros.Add(name: "PESO_LIMITE", value: servicoFixoVariavel.PesoLimite, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_MAXIMO", value: servicoFixoVariavel.PrecoMaximo, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ANVISA", value: servicoFixoVariavel.ValorAnvisa, direction: ParameterDirection.Input);
                    parametros.Add(name: "FLAG_COBRAR_NVOCC", value: servicoFixoVariavel.CobrarNVOCC.ToInt(), direction: ParameterDirection.Input);
                    parametros.Add(name: "FORMA_PAGAMENTO_NVOCC", value: servicoFixoVariavel.FormaPagamentoNVOCC, direction: ParameterDirection.Input);
                    parametros.Add(name: "ADICIONAL_GRC", value: servicoFixoVariavel.AdicionalGRC, direction: ParameterDirection.Input);
                    Autonumsv = con.Query<int>($@"SELECT autonum FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE LISTA = :LISTA AND SERVICO = :SERVICO 
                                AND TIPO_CARGA = :TIPO_CARGA AND BASE_CALCULO = :BASE_CALCULO AND VARIANTE_LOCAL = :VARIANTE_LOCAL AND N_PERIODO = :N_PERIODO", parametros).Single();

                    if (Autonumsv == 0)
                    {

                        con.Execute($@"
                        INSERT INTO
                            {_schema}..TB_LISTA_P_S_PERIODO (
                                LISTA,
                                OPORTUNIDADEID,
                                LINHA,
                                SERVICO,
                                TIPO_CARGA,
                                BASE_CALCULO,
                                VARIANTE_LOCAL,
                                BASE_EXCESSO,
                                VALOR_EXCESSO,
                                TIPO_DOC,
                                MOEDA,
                                PRECO_UNITARIO,
                                PRECO_MINIMO,
                                VALOR_ACRESCIMO,
                                N_PERIODO,
                                QTDE_DIAS,
                                GRUPO_ATRACACAO,
                                FLAG_PRORATA,
                                VALOR_ACRESC_PESO,
                                PESO_LIMITE,
                                PRECO_MAXIMO,
                                VALOR_ANVISA,
                                FLAG_COBRAR_NVOCC,
                                FORMA_PAGAMENTO_NVOCC,
                                EXERCITO
                            ) VALUES (
                                @LISTA,
                                @OPORTUNIDADEID,
                                @LINHA,
                                @SERVICO,
                                @TIPO_CARGA,
                                @BASE_CALCULO,
                                @VARIANTE_LOCAL,
                                @BASE_EXCESSO,
                                @VALOR_EXCESSO,
                                @TIPO_DOC,
                                @MOEDA,
                                @PRECO_UNITARIO,
                                @PRECO_MINIMO,
                                @VALOR_ACRESCIMO,
                                @N_PERIODO,
                                @QTDE_DIAS,
                                @GRUPO_ATRACACAO,
                                @FLAG_PRORATA,
                                @VALOR_ACRESC_PESO,
                                @PESO_LIMITE,
                                @PRECO_MAXIMO,
                                @VALOR_ANVISA,
                                @FLAG_COBRAR_NVOCC,
                                @FORMA_PAGAMENTO_NVOCC,
                                @EXERCITO
                            )", parametros);
                    }
                }
            }
        }
        public void GravarServicoVariavel_ValidaPeriodo(ServicoFixoVariavel servicoFixoVariavel)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "OPORTUNIDADEID", value: servicoFixoVariavel.OportunidadeId, direction: ParameterDirection.Input);
                    parametros.Add(name: "LISTA", value: servicoFixoVariavel.Lista, direction: ParameterDirection.Input);
                    parametros.Add(name: "LINHA", value: servicoFixoVariavel.Linha, direction: ParameterDirection.Input);
                    parametros.Add(name: "SERVICO", value: servicoFixoVariavel.ServicoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_CARGA", value: servicoFixoVariavel.TipoCarga, direction: ParameterDirection.Input);
                    parametros.Add(name: "BASE_CALCULO", value: servicoFixoVariavel.BaseCalculo, direction: ParameterDirection.Input);
                    parametros.Add(name: "VARIANTE_LOCAL", value: servicoFixoVariavel.VarianteLocal, direction: ParameterDirection.Input);
                    parametros.Add(name: "BASE_EXCESSO", value: servicoFixoVariavel.BaseExcesso, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_EXCESSO", value: servicoFixoVariavel.ValorExcesso, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_DOC", value: servicoFixoVariavel.TipoDocumentoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "MOEDA", value: servicoFixoVariavel.Moeda, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_UNITARIO", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_MINIMO", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ACRESCIMO", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "EXERCITO", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "N_PERIODO", value: servicoFixoVariavel.Periodo - 1, direction: ParameterDirection.Input);
                    parametros.Add(name: "N_PERIODON", value: servicoFixoVariavel.Periodo - 2, direction: ParameterDirection.Input);
                    parametros.Add(name: "QTDE_DIAS", value: servicoFixoVariavel.QtdeDias, direction: ParameterDirection.Input);
                    parametros.Add(name: "GRUPO_ATRACACAO", value: servicoFixoVariavel.GrupoAtracacaoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "FLAG_PRORATA", value: servicoFixoVariavel.ProRata.ToInt(), direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ACRESC_PESO", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "PESO_LIMITE", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_MAXIMO", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ANVISA", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "FLAG_COBRAR_NVOCC", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "FORMA_PAGAMENTO_NVOCC", value: servicoFixoVariavel.FormaPagamentoNVOCC, direction: ParameterDirection.Input);
                    parametros.Add(name: "ADICIONAL_GRC", value: 0, direction: ParameterDirection.Input);

                    if (servicoFixoVariavel.Periodo > 2)
                    {
                        var result = con.Query<int>($@"SELECT COUNT(1) FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE LISTA = :LISTA AND SERVICO = :SERVICO 
                                AND TIPO_CARGA = :TIPO_CARGA AND BASE_CALCULO = :BASE_CALCULO AND VARIANTE_LOCAL = :VARIANTE_LOCAL AND N_PERIODO = :N_PERIODO", parametros).Single();

                        if (result == 0)
                        {
                            con.Execute($@"INSERT INTO { _schema}.TB_LISTA_P_S_periodo(
                                    Autonum, 
                                    Lista,
                                    OPORTUNIDADEID,
                                    LINHA, 
                                    SERVICO,
                                    N_PERIODO,
                                    QTDE_DIAS,
                                    TIPO_CARGA,
                                    BASE_CALCULO,
                                    VARIANTE_LOCAL,
                                    TIPO_DOC,
                                    ARMADOR,
                                    BASE_EXCESSO,
                                    VALOR_EXCESSO,
                                    PRECO_UNITARIO,
                                    MOEDA,
                                    PRECO_MINIMO,
                                    PRECO_MAXIMO,
                                    VALOR_ANVISA,
                                    VALOR_ACRESCIMO,
                                    USUARIO_SIS,
                                    USUARIO_REDE,
                                    MAQUINA_REDE,
                                    LOCAL_ATRACACAO,
                                    FLAG_PRORATA,
                                    AUTONUM_VINCULADO,
                                    GRUPO_ATRACACAO,
                                    VALOR_ACRESC_PESO,
                                    PESO_LIMITE,EXERCITO) 
                                     SELECT { _schema}.SEQ_LISTA_p_s_periodo.NEXTVAL,:LISTA,
                                    OPORTUNIDADEID,
                                    LINHA, 
                                    SERVICO,
                                    :N_PERIODO,
                                    QTDE_DIAS,
                                    TIPO_CARGA,
                                    BASE_CALCULO,
                                    VARIANTE_LOCAL,
                                    TIPO_DOC,
                                    ARMADOR,
                                    BASE_EXCESSO,
                                    VALOR_EXCESSO,
                                    PRECO_UNITARIO,
                                    MOEDA,
                                    PRECO_MINIMO,
                                    PRECO_MAXIMO,
                                    VALOR_ANVISA,
                                    VALOR_ACRESCIMO,
                                    USUARIO_SIS,
                                    USUARIO_REDE,
                                    MAQUINA_REDE,
                                    LOCAL_ATRACACAO,
                                    FLAG_PRORATA,
                                    AUTONUM_VINCULADO,
                                    GRUPO_ATRACACAO,
                                    VALOR_ACRESC_PESO,
                                    PESO_LIMITE  ,EXERCITO
                                    FROM { _schema}.TB_LISTA_P_S_periodo  WHERE LISTA = :LISTA AND SERVICO = :SERVICO 
                                    AND TIPO_CARGA = :TIPO_CARGA AND BASE_CALCULO = :BASE_CALCULO AND VARIANTE_LOCAL = :VARIANTE_LOCAL AND N_PERIODO = :N_PERIODON", parametros);

                        }
                    }
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();


                    parametros.Add(name: "OPORTUNIDADEID", value: servicoFixoVariavel.OportunidadeId, direction: ParameterDirection.Input);
                    parametros.Add(name: "LISTA", value: servicoFixoVariavel.Lista, direction: ParameterDirection.Input);
                    parametros.Add(name: "LINHA", value: servicoFixoVariavel.Linha, direction: ParameterDirection.Input);
                    parametros.Add(name: "SERVICO", value: servicoFixoVariavel.ServicoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_CARGA", value: servicoFixoVariavel.TipoCarga, direction: ParameterDirection.Input);
                    parametros.Add(name: "BASE_CALCULO", value: servicoFixoVariavel.BaseCalculo, direction: ParameterDirection.Input);
                    parametros.Add(name: "VARIANTE_LOCAL", value: servicoFixoVariavel.VarianteLocal, direction: ParameterDirection.Input);
                    parametros.Add(name: "BASE_EXCESSO", value: servicoFixoVariavel.BaseExcesso, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_EXCESSO", value: servicoFixoVariavel.ValorExcesso, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_DOC", value: servicoFixoVariavel.TipoDocumentoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "MOEDA", value: servicoFixoVariavel.Moeda, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_UNITARIO", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_MINIMO", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ACRESCIMO", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "EXERCITO", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "N_PERIODO", value: servicoFixoVariavel.Periodo - 1, direction: ParameterDirection.Input);
                    parametros.Add(name: "N_PERIODON", value: servicoFixoVariavel.Periodo - 2, direction: ParameterDirection.Input);
                    parametros.Add(name: "QTDE_DIAS", value: servicoFixoVariavel.QtdeDias, direction: ParameterDirection.Input);
                    parametros.Add(name: "GRUPO_ATRACACAO", value: servicoFixoVariavel.GrupoAtracacaoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "FLAG_PRORATA", value: servicoFixoVariavel.ProRata.ToInt(), direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ACRESC_PESO", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "PESO_LIMITE", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_MAXIMO", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ANVISA", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "FLAG_COBRAR_NVOCC", value: 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "FORMA_PAGAMENTO_NVOCC", value: servicoFixoVariavel.FormaPagamentoNVOCC, direction: ParameterDirection.Input);
                    parametros.Add(name: "ADICIONAL_GRC", value: 0, direction: ParameterDirection.Input);

                    if (servicoFixoVariavel.Periodo > 2)
                    {
                        var result = con.Query<int>($@"SELECT COUNT(1) FROM {_schema}.TB_LISTA_P_S_PERIODO WHERE LISTA = :LISTA AND SERVICO = :SERVICO 
                                AND TIPO_CARGA = :TIPO_CARGA AND BASE_CALCULO = :BASE_CALCULO AND VARIANTE_LOCAL = :VARIANTE_LOCAL AND N_PERIODO = :N_PERIODO", parametros).Single();

                        if (result == 0)
                        {
                            con.Execute($@"INSERT INTO { _schema}.TB_LISTA_P_S_periodo(
                                  
                                    Lista,
                                    OPORTUNIDADEID,
                                    LINHA, 
                                    SERVICO,
                                    N_PERIODO,
                                    QTDE_DIAS,
                                    TIPO_CARGA,
                                    BASE_CALCULO,
                                    VARIANTE_LOCAL,
                                    TIPO_DOC,
                                    ARMADOR,
                                    BASE_EXCESSO,
                                    VALOR_EXCESSO,
                                    PRECO_UNITARIO,
                                    MOEDA,
                                    PRECO_MINIMO,
                                    PRECO_MAXIMO,
                                    VALOR_ANVISA,
                                    VALOR_ACRESCIMO,
                                    USUARIO_SIS,
                                    USUARIO_REDE,
                                    MAQUINA_REDE,
                                    LOCAL_ATRACACAO,
                                    FLAG_PRORATA,
                                    AUTONUM_VINCULADO,
                                    GRUPO_ATRACACAO,
                                    VALOR_ACRESC_PESO,
                                    PESO_LIMITE,EXERCITO) 
                                      SELECT  :LISTA,
                                    OPORTUNIDADEID,
                                    LINHA, 
                                    SERVICO,
                                    :N_PERIODO,
                                    QTDE_DIAS,
                                    TIPO_CARGA,
                                    BASE_CALCULO,
                                    VARIANTE_LOCAL,
                                    TIPO_DOC,
                                    ARMADOR,
                                    BASE_EXCESSO,
                                    VALOR_EXCESSO,
                                    PRECO_UNITARIO,
                                    MOEDA,
                                    PRECO_MINIMO,
                                    PRECO_MAXIMO,
                                    VALOR_ANVISA,
                                    VALOR_ACRESCIMO,
                                    USUARIO_SIS,
                                    USUARIO_REDE,
                                    MAQUINA_REDE,
                                    LOCAL_ATRACACAO,
                                    FLAG_PRORATA,
                                    AUTONUM_VINCULADO,
                                    GRUPO_ATRACACAO,
                                    VALOR_ACRESC_PESO,
                                    PESO_LIMITE  ,EXERCITO
                                    FROM { _schema}.TB_LISTA_P_S_periodo WHERE WHERE LISTA = :LISTA AND SERVICO = :SERVICO 
                                    AND TIPO_CARGA = :TIPO_CARGA AND BASE_CALCULO = :BASE_CALCULO AND VARIANTE_LOCAL = :VARIANTE_LOCAL AND N_PERIODO = :N_PERIODON", parametros);

                        }
                    }

                }
            }
        }

        public void GravarServicoFixo(ServicoFixoVariavel servicoFixoVariavel)
        {
            if ((servicoFixoVariavel.ValorAcrescimoPeso == 0) && (servicoFixoVariavel.PesoMaximo > 0))
            {
                servicoFixoVariavel.PrecoUnitario = 0;
                servicoFixoVariavel.PrecoMinimo = 0;
            }

            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    if (servicoFixoVariavel.ServicoId == 1)
                    {
                        System.Console.WriteLine("Taxa Liberação");
                    }

                    parametros.Add(name: "OPORTUNIDADEID", value: servicoFixoVariavel.OportunidadeId, direction: ParameterDirection.Input);
                    parametros.Add(name: "LISTA", value: servicoFixoVariavel.Lista, direction: ParameterDirection.Input);
                    parametros.Add(name: "LINHA", value: servicoFixoVariavel.Linha, direction: ParameterDirection.Input);
                    parametros.Add(name: "SERVICO", value: servicoFixoVariavel.ServicoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_CARGA", value: servicoFixoVariavel.TipoCarga, direction: ParameterDirection.Input);
                    parametros.Add(name: "BASE_CALCULO", value: servicoFixoVariavel.BaseCalculo, direction: ParameterDirection.Input);
                    parametros.Add(name: "VARIANTE_LOCAL", value: servicoFixoVariavel.VarianteLocal, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_UNITARIO", value: servicoFixoVariavel.PrecoUnitario, direction: ParameterDirection.Input);
                    parametros.Add(name: "MOEDA", value: servicoFixoVariavel.Moeda, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_MINIMO", value: servicoFixoVariavel.PrecoMinimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ACRESCIMO", value: servicoFixoVariavel.ValorAcrescimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "EXERCITO", value: servicoFixoVariavel.Exercito, direction: ParameterDirection.Input);
                    parametros.Add(name: "LOCAL_ATRACACAO", value: servicoFixoVariavel.LocalAtracacaoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "GRUPO_ATRACACAO", value: servicoFixoVariavel.GrupoAtracacaoId != null ? servicoFixoVariavel.GrupoAtracacaoId.Value : 0, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ACRESC_PESO", value: servicoFixoVariavel.ValorAcrescimoPeso, direction: ParameterDirection.Input);
                    parametros.Add(name: "PESO_LIMITE", value: servicoFixoVariavel.PesoMaximo, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_OPER", value: servicoFixoVariavel.TipoOperacao, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_DOC", value: servicoFixoVariavel.TipoDocumentoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "BASE_EXCESSO", value: servicoFixoVariavel.BaseExcesso, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_EXCESSO", value: servicoFixoVariavel.ValorExcesso, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_MAXIMO", value: servicoFixoVariavel.PrecoMaximo, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ANVISA", value: servicoFixoVariavel.ValorAnvisa, direction: ParameterDirection.Input);
                    parametros.Add(name: "FORMA_PAGAMENTO_NVOCC", value: servicoFixoVariavel.FormaPagamentoNVOCC, direction: ParameterDirection.Input);
                    parametros.Add(name: "PORTO_HUBPORT", value: servicoFixoVariavel.Destino, direction: ParameterDirection.Input);

                    //FLAG_COBRAR_NVOCC
                    if (servicoFixoVariavel.FormaPagamentoNVOCC > 0)
                    {
                        parametros.Add(name: "FLAG_COBRAR_NVOCC", value: 1, direction: ParameterDirection.Input);
                    }
                    else
                    {
                        parametros.Add(name: "FLAG_COBRAR_NVOCC", value: 0, direction: ParameterDirection.Input);
                    }

                    parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    con.Execute($@"
                        INSERT INTO
                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS (
                                AUTONUM,
                                LISTA,
                                OPORTUNIDADEID,
                                LINHA,
                                SERVICO,
                                TIPO_CARGA,
                                BASE_CALCULO,
                                VARIANTE_LOCAL,
                                PRECO_UNITARIO,
                                MOEDA,
                                PRECO_MINIMO,
                                VALOR_ACRESCIMO,
                                LOCAL_ATRACACAO,
                                GRUPO_ATRACACAO,
                                VALOR_ACRESC_PESO,
                                PESO_LIMITE,
                                TIPO_OPER,
                                TIPO_DOC,
                                BASE_EXCESSO,
                                VALOR_EXCESSO,
                                PRECO_MAXIMO,
                                VALOR_ANVISA,
                                FLAG_COBRAR_NVOCC,
                                FORMA_PAGAMENTO_NVOCC,
                                PORTO_HUBPORT,EXERCITO
                            ) VALUES (
                                {_schema}.SEQ_LISTA_PRECO_SERVICOS_FIXOS.NEXTVAL,
                                :LISTA,
                                :OPORTUNIDADEID,
                                :LINHA,
                                :SERVICO,
                                :TIPO_CARGA,
                                :BASE_CALCULO,
                                :VARIANTE_LOCAL,
                                :PRECO_UNITARIO,
                                :MOEDA,
                                :PRECO_MINIMO,
                                :VALOR_ACRESCIMO,
                                :LOCAL_ATRACACAO,
                                :GRUPO_ATRACACAO,
                                :VALOR_ACRESC_PESO,
                                :PESO_LIMITE,
                                :TIPO_OPER,
                                :TIPO_DOC,
                                :BASE_EXCESSO,
                                :VALOR_EXCESSO,
                                :PRECO_MAXIMO,
                                :VALOR_ANVISA,
                                :FLAG_COBRAR_NVOCC,
                                :FORMA_PAGAMENTO_NVOCC,
                                :PORTO_HUBPORT,
                                :EXERCITO
                            ) RETURNING AUTONUM INTO :Id", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "OPORTUNIDADEID", value: servicoFixoVariavel.OportunidadeId, direction: ParameterDirection.Input);
                    parametros.Add(name: "LISTA", value: servicoFixoVariavel.Lista, direction: ParameterDirection.Input);
                    parametros.Add(name: "LINHA", value: servicoFixoVariavel.Linha, direction: ParameterDirection.Input);
                    parametros.Add(name: "SERVICO", value: servicoFixoVariavel.ServicoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_CARGA", value: servicoFixoVariavel.TipoCarga, direction: ParameterDirection.Input);
                    parametros.Add(name: "BASE_CALCULO", value: servicoFixoVariavel.BaseCalculo, direction: ParameterDirection.Input);
                    parametros.Add(name: "VARIANTE_LOCAL", value: servicoFixoVariavel.VarianteLocal, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_UNITARIO", value: servicoFixoVariavel.PrecoUnitario, direction: ParameterDirection.Input);
                    parametros.Add(name: "MOEDA", value: servicoFixoVariavel.Moeda, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_MINIMO", value: servicoFixoVariavel.PrecoMinimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ACRESCIMO", value: servicoFixoVariavel.ValorAcrescimo, direction: ParameterDirection.Input);
                    parametros.Add(name: "EXERCITO", value: servicoFixoVariavel.Exercito, direction: ParameterDirection.Input);
                    parametros.Add(name: "LOCAL_ATRACACAO", value: servicoFixoVariavel.LocalAtracacaoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "GRUPO_ATRACACAO", value: servicoFixoVariavel.GrupoAtracacaoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ACRESC_PESO", value: servicoFixoVariavel.ValorAcrescimoPeso, direction: ParameterDirection.Input);
                    parametros.Add(name: "PESO_LIMITE", value: servicoFixoVariavel.PesoLimite, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_OPER", value: servicoFixoVariavel.TipoOperacao, direction: ParameterDirection.Input);
                    parametros.Add(name: "TIPO_DOC", value: servicoFixoVariavel.TipoDocumentoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "BASE_EXCESSO", value: servicoFixoVariavel.BaseExcesso, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_EXCESSO", value: servicoFixoVariavel.ValorExcesso, direction: ParameterDirection.Input);
                    parametros.Add(name: "PRECO_MAXIMO", value: servicoFixoVariavel.PrecoMaximo, direction: ParameterDirection.Input);
                    parametros.Add(name: "VALOR_ANVISA", value: servicoFixoVariavel.ValorAnvisa, direction: ParameterDirection.Input);
                    parametros.Add(name: "FLAG_COBRAR_NVOCC", value: servicoFixoVariavel.CobrarNVOCC.ToInt(), direction: ParameterDirection.Input);
                    parametros.Add(name: "FORMA_PAGAMENTO_NVOCC", value: servicoFixoVariavel.FormaPagamentoNVOCC, direction: ParameterDirection.Input);
                    parametros.Add(name: "PORTO_HUBPORT", value: servicoFixoVariavel.Destino, direction: ParameterDirection.Input);

                    con.Execute($@"
                        INSERT INTO
                            {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS (
                                LISTA,
                                OPORTUNIDADEID,
                                LINHA,
                                SERVICO,
                                TIPO_CARGA,
                                BASE_CALCULO,
                                VARIANTE_LOCAL,
                                PRECO_UNITARIO,
                                MOEDA,
                                PRECO_MINIMO,
                                VALOR_ACRESCIMO,
                                LOCAL_ATRACACAO,
                                GRUPO_ATRACACAO,
                                VALOR_ACRESC_PESO,
                                PESO_LIMITE,
                                TIPO_OPER,
                                TIPO_DOC,
                                BASE_EXCESSO,
                                VALOR_EXCESSO,
                                PRECO_MAXIMO,
                                VALOR_ANVISA,
                                FLAG_COBRAR_NVOCC,
                                FORMA_PAGAMENTO_NVOCC,
                                PORTO_HUBPORT,EXERCITO
                            ) VALUES (
                                @LISTA,
                                @OPORTUNIDADEID,
                                @LINHA,
                                @SERVICO,
                                @TIPO_CARGA,
                                @BASE_CALCULO,
                                @VARIANTE_LOCAL,
                                @PRECO_UNITARIO,
                                @MOEDA,
                                @PRECO_MINIMO,
                                @VALOR_ACRESCIMO,
                                @LOCAL_ATRACACAO,
                                @GRUPO_ATRACACAO,
                                @VALOR_ACRESC_PESO,
                                @PESO_LIMITE,
                                @TIPO_OPER,
                                @TIPO_DOC,
                                @BASE_EXCESSO,
                                @VALOR_EXCESSO,
                                @PRECO_MAXIMO,
                                @VALOR_ANVISA,
                                @FLAG_COBRAR_NVOCC,
                                @FORMA_PAGAMENTO_NVOCC,
                                @PORTO_HUBPORT,@EXERCITO
                            )", parametros);
                }
            }
        }

        public void ImportarServicosFixos(int tabelaOrigem, int tabelaDestino)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaOrigem", value: tabelaOrigem, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaDestino", value: tabelaDestino, direction: ParameterDirection.Input);

                    con.Execute($@"
                        INSERT INTO
                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS (
                                AUTONUM,
                                LISTA,
                                OPORTUNIDADEID,
                                LINHA,
                                SERVICO,
                                TIPO_CARGA,
                                BASE_CALCULO,
                                VARIANTE_LOCAL,
                                PRECO_UNITARIO,
                                MOEDA,
                                PRECO_MINIMO,
                                VALOR_ACRESCIMO,
                                LOCAL_ATRACACAO,
                                GRUPO_ATRACACAO,
                                VALOR_ACRESC_PESO,
                                PESO_LIMITE,
                                TIPO_OPER,
                                TIPO_DOC,
                                BASE_EXCESSO,
                                VALOR_EXCESSO,
                                PRECO_MAXIMO,
                                VALOR_ANVISA,
                                FLAG_COBRAR_NVOCC,
                                FORMA_PAGAMENTO_NVOCC,
                                PORTO_HUBPORT,
                                AUTONUM_VINCULADO,EXERCITO
                            ) SELECT
                                {_schema}.SEQ_LISTA_PRECO_SERVICOS_FIXOS.NEXTVAL,
                                :TabelaDestino,
                                OPORTUNIDADEID,
                                LINHA,
                                SERVICO,
                                TIPO_CARGA,
                                BASE_CALCULO,
                                VARIANTE_LOCAL,
                                PRECO_UNITARIO,
                                MOEDA,
                                PRECO_MINIMO,
                                VALOR_ACRESCIMO,
                                LOCAL_ATRACACAO,
                                GRUPO_ATRACACAO,
                                VALOR_ACRESC_PESO,
                                PESO_LIMITE,
                                TIPO_OPER,
                                TIPO_DOC,
                                BASE_EXCESSO,
                                VALOR_EXCESSO,
                                PRECO_MAXIMO,
                                VALOR_ANVISA,
                                FLAG_COBRAR_NVOCC,
                                FORMA_PAGAMENTO_NVOCC,
                                PORTO_HUBPORT,
                                AUTONUM_VINCULADO,EXERCITO
                            FROM
                                {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS
                            WHERE
                                LISTA = :TabelaOrigem", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaOrigem", value: tabelaOrigem, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaDestino", value: tabelaDestino, direction: ParameterDirection.Input);

                    con.Execute($@"
                        INSERT INTO
                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS (
                                LISTA,
                                OPORTUNIDADEID,
                                LINHA,
                                SERVICO,
                                TIPO_CARGA,
                                BASE_CALCULO,
                                VARIANTE_LOCAL,
                                PRECO_UNITARIO,
                                MOEDA,
                                PRECO_MINIMO,
                                VALOR_ACRESCIMO,
                                LOCAL_ATRACACAO,
                                GRUPO_ATRACACAO,
                                VALOR_ACRESC_PESO,
                                PESO_LIMITE,
                                TIPO_OPER,
                                TIPO_DOC,
                                BASE_EXCESSO,
                                VALOR_EXCESSO,
                                PRECO_MAXIMO,
                                VALOR_ANVISA,
                                FLAG_COBRAR_NVOCC,
                                FORMA_PAGAMENTO_NVOCC,
                                PORTO_HUBPORT,
                                AUTONUM_VINCULADO,EXERCITO
                            ) SELECT
                                @TabelaDestino,
                                OPORTUNIDADEID,
                                LINHA,
                                SERVICO,
                                TIPO_CARGA,
                                BASE_CALCULO,
                                VARIANTE_LOCAL,
                                PRECO_UNITARIO,
                                MOEDA,
                                PRECO_MINIMO,
                                VALOR_ACRESCIMO,
                                LOCAL_ATRACACAO,
                                GRUPO_ATRACACAO,
                                VALOR_ACRESC_PESO,
                                PESO_LIMITE,
                                TIPO_OPER,
                                TIPO_DOC,
                                BASE_EXCESSO,
                                VALOR_EXCESSO,
                                PRECO_MAXIMO,
                                VALOR_ANVISA,
                                FLAG_COBRAR_NVOCC,
                                FORMA_PAGAMENTO_NVOCC,
                                PORTO_HUBPORT,
                                AUTONUM_VINCULADO,EXERCITO
                            FROM
                                {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS
                            WHERE
                                LISTA = @TabelaOrigem", parametros);
                }
            }
        }

        public void ImportarServicosVariaveis(int tabelaOrigem, int tabelaDestino)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaOrigem", value: tabelaOrigem, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaDestino", value: tabelaDestino, direction: ParameterDirection.Input);

                    con.Execute($@"
                        INSERT INTO
                            {_schema}.TB_LISTA_P_S_PERIODO (
                                AUTONUM,
                                LISTA,
                                OPORTUNIDADEID,
                                LINHA,
                                SERVICO,
                                TIPO_CARGA,
                                BASE_CALCULO,
                                VARIANTE_LOCAL,
                                BASE_EXCESSO,
                                VALOR_EXCESSO,
                                TIPO_DOC,
                                MOEDA,
                                PRECO_UNITARIO,
                                PRECO_MINIMO,
                                VALOR_ACRESCIMO,
                                N_PERIODO,
                                QTDE_DIAS,
                                GRUPO_ATRACACAO,
                                FLAG_PRORATA,
                                VALOR_ACRESC_PESO,
                                PESO_LIMITE,
                                PRECO_MAXIMO,
                                VALOR_ANVISA,
                                FLAG_COBRAR_NVOCC,
                                FORMA_PAGAMENTO_NVOCC,
                                AUTONUM_VINCULADO,EXERCITO
                            ) SELECT
                                {_schema}.SEQ_LISTA_P_S_PERIODO.NEXTVAL,
                                :TabelaDestino,
                                OPORTUNIDADEID,
                                LINHA,
                                SERVICO,
                                TIPO_CARGA,
                                BASE_CALCULO,
                                VARIANTE_LOCAL,
                                BASE_EXCESSO,
                                VALOR_EXCESSO,
                                TIPO_DOC,
                                MOEDA,
                                PRECO_UNITARIO,
                                PRECO_MINIMO,
                                VALOR_ACRESCIMO,
                                N_PERIODO,
                                QTDE_DIAS,
                                GRUPO_ATRACACAO,
                                FLAG_PRORATA,
                                VALOR_ACRESC_PESO,
                                PESO_LIMITE,
                                PRECO_MAXIMO,
                                VALOR_ANVISA,
                                FLAG_COBRAR_NVOCC,
                                FORMA_PAGAMENTO_NVOCC,
                                AUTONUM_VINCULADO,EXERCITO
                            FROM
                                {_schema}.TB_LISTA_P_S_PERIODO
                            WHERE
                                LISTA = :TabelaOrigem", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaOrigiem", value: tabelaOrigem, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaDestino", value: tabelaDestino, direction: ParameterDirection.Input);

                    con.Execute($@"
                        INSERT INTO
                            {_schema}..TB_LISTA_P_S_PERIODO (
                                LISTA,
                                OPORTUNIDADEID,
                                LINHA,
                                SERVICO,
                                TIPO_CARGA,
                                BASE_CALCULO,
                                VARIANTE_LOCAL,
                                BASE_EXCESSO,
                                VALOR_EXCESSO,
                                TIPO_DOC,
                                MOEDA,
                                PRECO_UNITARIO,
                                PRECO_MINIMO,
                                VALOR_ACRESCIMO,
                                N_PERIODO,
                                QTDE_DIAS,
                                GRUPO_ATRACACAO,
                                FLAG_PRORATA,
                                VALOR_ACRESC_PESO,
                                PESO_LIMITE,
                                PRECO_MAXIMO,
                                VALOR_ANVISA,
                                FLAG_COBRAR_NVOCC,
                                FORMA_PAGAMENTO_NVOCC,EXERCITO
                            ) SELECT
                                @TabelaDestino,
                                OPORTUNIDADEID,
                                LINHA,
                                SERVICO,
                                TIPO_CARGA,
                                BASE_CALCULO,
                                VARIANTE_LOCAL,
                                BASE_EXCESSO,
                                VALOR_EXCESSO,
                                TIPO_DOC,
                                MOEDA,
                                PRECO_UNITARIO,
                                PRECO_MINIMO,
                                VALOR_ACRESCIMO,
                                N_PERIODO,
                                QTDE_DIAS,
                                GRUPO_ATRACACAO,
                                FLAG_PRORATA,
                                VALOR_ACRESC_PESO,
                                PESO_LIMITE,
                                PRECO_MAXIMO,
                                VALOR_ANVISA,
                                FLAG_COBRAR_NVOCC,
                                FORMA_PAGAMENTO_NVOCC,EXERCITO
                            WHERE
                                LISTA = @TabelaOrigem", parametros);
                }
            }
        }

        public void ImportarImpostos(int tabelaOrigem, int tabelaDestino)
        {
            if (tabelaOrigem > 0)
            {
                if (Configuracoes.BancoEmUso() == "ORACLE")
                {
                    using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                    {
                        var parametros = new DynamicParameters();

                        parametros.Add(name: "TabelaOrigem", value: tabelaOrigem, direction: ParameterDirection.Input);
                        parametros.Add(name: "TabelaDestino", value: tabelaDestino, direction: ParameterDirection.Input);

                        con.Execute($@"INSERT INTO {_schema}.TB_LP_IMPOSTOS (ID_TABELA, ID_IMPOSTO, PADRAO) SELECT :TabelaDestino, ID_IMPOSTO, PADRAO FROM {_schema}.TB_LP_IMPOSTOS WHERE ID_TABELA = :TabelaOrigem", parametros);

                        con.Execute($@"Insert into {_schema}.TB_LP_SERVICOS_IMPOSTOS(Autonum,id_tabela,
                                   ID_SERVICO,ID_IMPOSTO,FLAG_PADRAO,CALCULAR) Select {_schema}.seq_tb_lp_servicos_impostos.NEXTVAL,:TabelaDestino,
                                   ID_SERVICO,ID_IMPOSTO,FLAG_PADRAO,CALCULAR From {_schema}.tb_lp_servicos_impostos WHERE ID_TABELA = :TabelaOrigem", parametros);
                    }

                }
                else
                {
                    using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                    {
                        var parametros = new DynamicParameters();

                        parametros.Add(name: "TabelaOrigem", value: tabelaOrigem, direction: ParameterDirection.Input);
                        parametros.Add(name: "TabelaDestino", value: tabelaDestino, direction: ParameterDirection.Input);

                        con.Execute($@"INSERT INTO {_schema}.TB_LP_IMPOSTOS (ID_TABELA, ID_IMPOSTO, PADRAO) SELECT @TabelaDestino, ID_IMPOSTO, PADRAO FROM {_schema}..TB_LP_IMPOSTOS WHERE ID_TABELA = @TabelaOrigem", parametros);
                    }
                }
            }
            else
            {
                if (Configuracoes.BancoEmUso() == "ORACLE")
                {
                    using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                    {
                        var parametros = new DynamicParameters();

                        parametros.Add(name: "TabelaDestino", value: tabelaDestino, direction: ParameterDirection.Input);

                        con.Execute($@"delete from Sgipa.tb_lp_impostos where id_tabela = :TabelaDestino", parametros);
                        con.Execute($@"insert into SGIPA.tb_lp_impostos(id_tabela, id_imposto, padrao) values(:TabelaDestino,3,1)", parametros);
                        con.Execute($@"insert into SGIPA.tb_lp_impostos(id_tabela, id_imposto, padrao) values(:TabelaDestino,2,1)", parametros);
                        con.Execute($@"insert into SGIPA.tb_lp_impostos(id_tabela, id_imposto, padrao) values(:TabelaDestino,1,1)", parametros);

                        con.Execute($@"delete from Sgipa.tb_lp_servicos_impostos where id_tabela = :TabelaDestino", parametros);
                        con.Execute($@"insert into sgipa.tb_lp_servicos_impostos(autonum, id_tabela, id_servico, id_imposto, calcular, flag_padrao)
                                       select Sgipa.seq_tb_lp_servicos_impostos.nextval,:TabelaDestino, autonum,1,1,1 from  sgipa.tb_servicos_ipa", parametros);
                        con.Execute($@"insert into sgipa.tb_lp_servicos_impostos(autonum, id_tabela, id_servico, id_imposto, calcular, flag_padrao)
                                       select Sgipa.seq_tb_lp_servicos_impostos.nextval,:TabelaDestino, autonum,2,1,1 from  sgipa.tb_servicos_ipa", parametros);
                        con.Execute($@"insert into sgipa.tb_lp_servicos_impostos(autonum, id_tabela, id_servico, id_imposto, calcular, flag_padrao)
                                       select Sgipa.seq_tb_lp_servicos_impostos.nextval,:TabelaDestino, autonum,3,1,1 from  sgipa.tb_servicos_ipa", parametros);
                    }


                }
                else
                {
                    using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                    {
                        var parametros = new DynamicParameters();

                        parametros.Add(name: "TabelaDestino", value: tabelaDestino, direction: ParameterDirection.Input);

                        con.Execute($@"delete from Sgipa.tb_lp_impostos where id_tabela = :TabelaDestino", parametros);
                        con.Execute($@"insert into SGIPA.tb_lp_impostos(id_tabela, id_imposto, padrao) values(:TabelaDestino,3,1)", parametros);
                        con.Execute($@"insert into SGIPA.tb_lp_impostos(id_tabela, id_imposto, padrao) values(:TabelaDestino,2,1)", parametros);
                        con.Execute($@"insert into SGIPA.tb_lp_impostos(id_tabela, id_imposto, padrao) values(:TabelaDestino,1,1)", parametros);

                        con.Execute($@"delete from Sgipa.tb_lp_servicos_impostos where id_tabela = = :TabelaDestino", parametros);
                        con.Execute($@"insert into sgipa.tb_lp_servicos_impostos(  id_tabela, id_servico, id_imposto, calcular, flag_padrao)
                                       select  :TabelaDestino, autonum,1,1,1) from  sgipa.tb_servicos_ipa", parametros);
                        con.Execute($@"insert into sgipa.tb_lp_servicos_impostos(  id_tabela, id_servico, id_imposto, calcular, flag_padrao)
                                       select  :TabelaDestino, autonum,2,1,1) from  sgipa.tb_servicos_ipa", parametros);
                        con.Execute($@"insert into sgipa.tb_lp_servicos_impostos(  id_tabela, id_servico, id_imposto, calcular, flag_padrao)
                                       select  :TabelaDestino, autonum,3,1,1) from  sgipa.tb_servicos_ipa", parametros);
                    }
                }

            }
        }

        public void AtualizaDataVendedor(int tabelaId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                var tabelas = con.Query<Tabela>("SELECT DATA_INICIO As DataInicio, FINAL_VALIDADE As DataFinalValidade FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :TabelaId", parametros);

                foreach (var tabela in tabelas)
                {
                    var vendedores = con.Query<Vendedor>("SELECT AUTONUM As Id, DATA_INI_VIG As InicioVigencia, DATA_FIM_VIG As TerminoVigencia FROM SGIPA.TB_LISTAS_PRECOS_VENDEDORES WHERE LISTA = :TabelaId ORDER BY DATA_INI_VIG", parametros);

                    foreach (var vendedor in vendedores)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                        parametros.Add(name: "DataInicio", value: tabela.DataInicio?.Date, direction: ParameterDirection.Input);
                        parametros.Add(name: "InicioVigencia", value: vendedor.InicioVigencia.Date, direction: ParameterDirection.Input);
                        parametros.Add(name: "DataAtual", value: DateTime.Now.Date, direction: ParameterDirection.Input);

                        con.Execute(@"UPDATE 
                                        SGIPA.TB_LISTAS_PRECOS_VENDEDORES SET 
                                            DATA_INI_VIG = :DataInicio 
                                        WHERE 
                                            LISTA = :TabelaId 
                                        AND 
                                            (DATA_INI_VIG is null or DATA_INI_VIG = :InicioVigencia )", parametros);
                    }

                    vendedores = con.Query<Vendedor>("SELECT AUTONUM As Id, DATA_INI_VIG As InicioVigencia, DATA_FIM_VIG As TerminoVigencia FROM SGIPA.TB_LISTAS_PRECOS_VENDEDORES WHERE DESLIGADO = 0 AND LISTA = :TabelaId AND DATA_FIM_VIG IS NOT NULL ORDER BY DATA_FIM_VIG DESC", parametros);

                    foreach (var vendedor in vendedores)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                        parametros.Add(name: "DataInicio", value: tabela.DataInicio?.Date, direction: ParameterDirection.Input);
                        parametros.Add(name: "TerminoVigencia", value: vendedor.TerminoVigencia.Value.Date, direction: ParameterDirection.Input);
                        parametros.Add(name: "DataAtual", value: DateTime.Now.Date, direction: ParameterDirection.Input);

                        con.Execute(@"UPDATE 
                                        SGIPA.TB_LISTAS_PRECOS_VENDEDORES SET 
                                            DATA_FIM_VIG = :TerminoVigencia
                                        WHERE 
                                            LISTA = :TabelaId 
                                        AND 
                                            (
                                                DATA_FIM_VIG IS NULL 
                                                    OR TO_DATE(DATA_FIM_VIG, 'DD/MM/YYYY') = :TerminoVigencia
                                            )", parametros);
                    }
                }
            }
        }

        public void AtualizaDataVendedorCancelamentoOportunidade(int tabelaId, DateTime dataCancelamentoOportunidade)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                var tabelas = con.Query<Tabela>("SELECT DATA_INICIO As DataInicio, FINAL_VALIDADE As DataFinalValidade FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :TabelaId", parametros);

                foreach (var tabela in tabelas)
                {
                    var vendedores = con.Query<Vendedor>("SELECT AUTONUM As Id, DATA_INI_VIG As InicioVigencia, DATA_FIM_VIG As TerminoVigencia FROM SGIPA.TB_LISTAS_PRECOS_VENDEDORES WHERE LISTA = :TabelaId ORDER BY DATA_INI_VIG", parametros);

                    foreach (var vendedor in vendedores)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                        parametros.Add(name: "DataInicio", value: tabela.DataInicio?.Date, direction: ParameterDirection.Input);
                        parametros.Add(name: "InicioVigencia", value: vendedor.InicioVigencia.Date, direction: ParameterDirection.Input);
                        parametros.Add(name: "DataAtual", value: DateTime.Now.Date, direction: ParameterDirection.Input);

                        con.Execute(@"UPDATE 
                                        SGIPA.TB_LISTAS_PRECOS_VENDEDORES SET 
                                            DATA_INI_VIG = :DataInicio 
                                        WHERE 
                                            LISTA = :TabelaId 
                                        AND 
                                            (DATA_INI_VIG is null or DATA_INI_VIG = :InicioVigencia )", parametros);
                    }

                    vendedores = con.Query<Vendedor>(@"
                        SELECT AUTONUM As Id, DATA_INI_VIG As InicioVigencia, DATA_FIM_VIG As TerminoVigencia 
                            FROM SGIPA.TB_LISTAS_PRECOS_VENDEDORES WHERE DESLIGADO = 0 AND LISTA = :TabelaId 
                                AND DATA_FIM_VIG IS NOT NULL ORDER BY DATA_FIM_VIG DESC", parametros);

                    foreach (var vendedor in vendedores)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Id", value: vendedor.Id, direction: ParameterDirection.Input);
                        parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);                        
                        parametros.Add(name: "DataInicio", value: tabela.DataInicio?.Date, direction: ParameterDirection.Input);                        
                        parametros.Add(name: "DataAtual", value: DateTime.Now.Date, direction: ParameterDirection.Input);

                        if (DateTime.Now > dataCancelamentoOportunidade)
                        {
                            var dataAtual = DateTime.Now;
                            var dataTermino = new DateTime(dataAtual.Year, dataAtual.Month, dataAtual.Day);
                            
                            parametros.Add(name: "TerminoVigencia", value: dataTermino, direction: ParameterDirection.Input);
                        }
                        else
                        {
                            var dataCancelamento = dataCancelamentoOportunidade;
                            var dataTermino = new DateTime(dataCancelamento.Year, dataCancelamento.Month, dataCancelamento.Day);
                            parametros.Add(name: "TerminoVigencia", value: dataTermino, direction: ParameterDirection.Input);
                        }

                        con.Execute(@"UPDATE 
                                        SGIPA.TB_LISTAS_PRECOS_VENDEDORES SET 
                                            DATA_FIM_VIG = :TerminoVigencia
                                        WHERE 
                                            AUTONUM = :Id", parametros);
                    }
                }
            }
        }

        public void ImportarFontePagadora(int tabelaOrigem, int tabelaDestino)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaOrigem", value: tabelaOrigem, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaDestino", value: tabelaDestino, direction: ParameterDirection.Input);

                    var FormaPag = con.Query($@"SELECT FORMA_PAGAMENTO FROM SGIPA.TB_LISTAS_PRECOS WHERE FORMA_PAGAMENTO=3 AND AUTONUM = :TabelaOrigem", parametros).Any();

                    var ContaForma = con.Query($@"SELECT AUTONUM_LISTA FROM SGIPA.TB_DADOS_FATURAMENTO_IPA WHERE AUTONUM_LISTA=:TabelaOrigem", parametros).Any();

                    if (FormaPag == true)
                    {
                        if (ContaForma == true)
                        {
                            con.Execute($@"
                                INSERT INTO SGIPA.TB_DADOS_FATURAMENTO_IPA 
                         	        (
                                        AUTONUM,  
                         	            AUTONUM_LISTA,  
                         	            AUTONUM_CLIENTE_NOTA,  
                         	            AUTONUM_CLIENTE_ENVIO_NOTA,  
                         	            AUTONUM_CLIENTE_PAGAMENTO,  
                         	            AUTONUM_DIA_FATURAMENTO,  
                         	            DIA,  
                         	            CORTE,  
                                        EMAIL,
                         	            AUTONUM_FORMA_PAGAMENTO,
                                        FLAG_ENTREGA_ELETRONICA, 
                                        FLAG_ENTREGA_MANUAL, 
                                        FLAG_ENVIO_CORREIO_COMUM, 
                                        FLAG_ENVIO_CORREIO_SEDEX,
                                        FLAG_ULTIMO_DIA_DO_MES_CORTE, 
                                        FLAG_VENCIMENTO_DIA_UTIL, 
                                        FLAG_ULTIMO_DIA_DA_SEMANA, 
                                        FLAG_ULTIMO_DIA_DO_MES, 
                                        FLAG_ULTIMO_DIA_DO_MES_VCTO
                                    )  
                                    SELECT  
                                        SGIPA.SEQ_DADOS_FATURAMENTO_IPA.NEXTVAL,  
                         	            :TabelaDestino,
                         	            AUTONUM_CLIENTE_NOTA,  
                         	            AUTONUM_CLIENTE_ENVIO_NOTA,  
                         	            AUTONUM_CLIENTE_PAGAMENTO,  
                         	            AUTONUM_DIA_FATURAMENTO,  
                         	            DIA,  
                         	            CORTE,
                                        EMAIL,
                         	            AUTONUM_FORMA_PAGAMENTO,
                                        FLAG_ENTREGA_ELETRONICA, 
                                        FLAG_ENTREGA_MANUAL, 
                                        FLAG_ENVIO_CORREIO_COMUM, 
                                        FLAG_ENVIO_CORREIO_SEDEX,
                                        FLAG_ULTIMO_DIA_DO_MES_CORTE, 
                                        FLAG_VENCIMENTO_DIA_UTIL, 
                                        FLAG_ULTIMO_DIA_DA_SEMANA, 
                                        FLAG_ULTIMO_DIA_DO_MES, 
                                        FLAG_ULTIMO_DIA_DO_MES_VCTO
                                    FROM  
                                        SGIPA.TB_DADOS_FATURAMENTO_IPA  
                                    WHERE  
                                        AUTONUM_LISTA =  :TabelaOrigem", parametros);
                        }
                    }
                }
            }
        }

        public void GravarServicoFixosSincronismo(int tabelaId, int oportunidadeId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var servicosSincronismo = con.Query<ServicoFixoVariavel>($@"                            
                            SELECT 
                                DISTINCT
                                    A.LISTA As TabelaSincronismoId,
                                    A.SERVICO As ServicoId,
                                    A.TIPO_CARGA As TipoCarga,
                                    A.BASE_CALCULO As BaseCalculo,
                                    A.VARIANTE_LOCAL As VarianteLocal,
                                    A.PRECO_UNITARIO As PrecoUnitario,
                                    A.MOEDA,
                                    A.PRECO_MINIMO As PrecoMinimo,
                                    A.VALOR_ACRESCIMO As ValorAcrescimo,
                                    A.LOCAL_ATRACACAO As LocalAtracacaoId,
                                    A.GRUPO_ATRACACAO As GrupoAtracacaoId,
                                    A.VALOR_ACRESC_PESO As ValorAcrescimoPeso,
                                    A.PESO_LIMITE As PesoLimite,
                                    A.TIPO_OPER As TipoOperacao,
                                    A.TIPO_DOC As TipoDocumentoId,
                                    A.BASE_EXCESSO As BaseExcesso,
                                    A.VALOR_EXCESSO As ValorExcesso,
                                    A.PRECO_MAXIMO As PrecoMaximo,
                                    A.VALOR_ANVISA As ValorAnvisa,
                                    A.FLAG_COBRAR_NVOCC As CobrarNVOCC,
                                    A.FORMA_PAGAMENTO_NVOCC As FormaPagamentoNVOCC,
                                    A.EXERCITO AS EXERCITO
                            FROM 
                                SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS A 
                            INNER JOIN
                                SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS B ON A.AUTONUM = B.AUTONUM_VINCULADO
                            WHERE
                                A.LISTA = 1 
                            AND 
                                B.LISTA <> 1");

                    foreach (var servicoSincronismo in servicosSincronismo)
                    {
                        var parametros = new DynamicParameters();

                        parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                        parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                        parametros.Add(name: "ServicoId", value: servicoSincronismo.ServicoId, direction: ParameterDirection.Input);
                        parametros.Add(name: "TipoCarga", value: servicoSincronismo.TipoCarga, direction: ParameterDirection.Input);
                        parametros.Add(name: "BaseCalculo", value: servicoSincronismo.BaseCalculo, direction: ParameterDirection.Input);
                        parametros.Add(name: "VarianteLocal", value: servicoSincronismo.VarianteLocal, direction: ParameterDirection.Input);
                        parametros.Add(name: "GrupoAtracacaoId", value: servicoSincronismo.GrupoAtracacaoId, direction: ParameterDirection.Input);

                        var existe = con.Query(@"SELECT AUTONUM FROM SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE Lista = :TabelaId 
                                    AND OportunidadeId = :OportunidadeId AND Servico = :ServicoId AND TIPO_CARGA = :TipoCarga AND BASE_CALCULO = :BaseCalculo 
                                        AND VARIANTE_LOCAL = :VarianteLocal AND NVL(GRUPO_ATRACACAO, 0) = :GrupoAtracacaoId", parametros).Any();

                        if (existe == false)
                        {
                            parametros = new DynamicParameters();

                            parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                            parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                            parametros.Add(name: "ServicoId", value: servicoSincronismo.ServicoId, direction: ParameterDirection.Input);
                            parametros.Add(name: "TipoCarga", value: servicoSincronismo.TipoCarga, direction: ParameterDirection.Input);
                            parametros.Add(name: "BaseCalculo", value: servicoSincronismo.BaseCalculo, direction: ParameterDirection.Input);
                            parametros.Add(name: "VarianteLocal", value: servicoSincronismo.VarianteLocal, direction: ParameterDirection.Input);
                            parametros.Add(name: "PrecoUnitario", value: servicoSincronismo.PrecoUnitario, direction: ParameterDirection.Input);
                            parametros.Add(name: "Moeda", value: servicoSincronismo.Moeda, direction: ParameterDirection.Input);
                            parametros.Add(name: "PrecoMinimo", value: servicoSincronismo.PrecoMinimo, direction: ParameterDirection.Input);
                            parametros.Add(name: "ValorAcrescimo", value: servicoSincronismo.ValorAcrescimo, direction: ParameterDirection.Input);
                            parametros.Add(name: "EXERCITO", value: servicoSincronismo.Exercito, direction: ParameterDirection.Input);
                            parametros.Add(name: "LocalAtracacaoId", value: servicoSincronismo.LocalAtracacaoId, direction: ParameterDirection.Input);
                            parametros.Add(name: "GrupoAtracacaoId", value: servicoSincronismo.GrupoAtracacaoId, direction: ParameterDirection.Input);
                            parametros.Add(name: "ValorAcrescimoPeso", value: servicoSincronismo.ValorAcrescimoPeso, direction: ParameterDirection.Input);
                            parametros.Add(name: "PesoLimite", value: servicoSincronismo.PesoLimite, direction: ParameterDirection.Input);
                            parametros.Add(name: "TipoOperacao", value: servicoSincronismo.TipoOperacao, direction: ParameterDirection.Input);
                            parametros.Add(name: "TipoDocumentoId", value: servicoSincronismo.TipoDocumentoId, direction: ParameterDirection.Input);
                            parametros.Add(name: "BaseExcesso", value: servicoSincronismo.BaseExcesso, direction: ParameterDirection.Input);
                            parametros.Add(name: "ValorExcesso", value: servicoSincronismo.ValorExcesso, direction: ParameterDirection.Input);
                            parametros.Add(name: "PrecoMaximo", value: servicoSincronismo.PrecoMaximo, direction: ParameterDirection.Input);
                            parametros.Add(name: "ValorAnvisa", value: servicoSincronismo.ValorAnvisa, direction: ParameterDirection.Input);
                            parametros.Add(name: "CobrarNVOCC", value: servicoSincronismo.CobrarNVOCC.ToInt(), direction: ParameterDirection.Input);
                            parametros.Add(name: "FormaPagamentoNVOCC", value: servicoSincronismo.FormaPagamentoNVOCC, direction: ParameterDirection.Input);
                            parametros.Add(name: "TabelaSincronismoId", value: servicoSincronismo.TabelaSincronismoId, direction: ParameterDirection.Input);

                            con.Execute($@"
	                            INSERT INTO
		                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS (
			                            AUTONUM,
			                            LISTA,
			                            OPORTUNIDADEID,
			                            SERVICO,
			                            TIPO_CARGA,
			                            BASE_CALCULO,
			                            VARIANTE_LOCAL,
			                            PRECO_UNITARIO,
			                            MOEDA,
			                            PRECO_MINIMO,
			                            VALOR_ACRESCIMO,
			                            LOCAL_ATRACACAO,
			                            GRUPO_ATRACACAO,
			                            VALOR_ACRESC_PESO,
			                            PESO_LIMITE,
			                            TIPO_OPER,
			                            TIPO_DOC,
			                            BASE_EXCESSO,
			                            VALOR_EXCESSO,
			                            PRECO_MAXIMO,
			                            VALOR_ANVISA,
			                            FLAG_COBRAR_NVOCC,
			                            FORMA_PAGAMENTO_NVOCC,
                                        AUTONUM_VINCULADO,EXERCITO
		                            ) VALUES (
			                            {_schema}.SEQ_LISTA_PRECO_SERVICOS_FIXOS.NEXTVAL,
			                            :TabelaId,
			                            :OportunidadeId,
			                            :ServicoId,
			                            :TipoCarga,
			                            :BaseCalculo,
			                            :VarianteLocal,
			                            :PrecoUnitario,
			                            :Moeda,
			                            :PrecoMinimo,
			                            :ValorAcrescimo,
			                            :LocalAtracacaoId,
			                            :GrupoAtracacaoId,
			                            :ValorAcrescimoPeso,
			                            :PesoLimite,
			                            :TipoOperacao,
			                            :TipoDocumentoId,
			                            :BaseExcesso,
			                            :ValorExcesso,
			                            :PrecoMaximo,
			                            :ValorAnvisa,
			                            :CobrarNVOCC,
			                            :FormaPagamentoNVOCC,
                                        :TabelaSincronismoId,
                                        :EXERCITO
		                            )", parametros);
                        }
                    }
                }
            }
        }

        public void GravarServicoLiberacao(int tabelaId, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TipoRegistro", value: TipoRegistro.SERVICO_LIBERACAO, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: TipoCarga.CONTEINER, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);


                con.Execute($@"INSERT INTO {_schema}.Tb_lista_preco_servicos_fixos(AUTONUM, LISTA, SERVICO,
                                       TIPO_CARGA, BASE_CALCULO, VARIANTE_LOCAL,
                                       PRECO_UNITARIO, MOEDA, PRECO_MINIMO,
                                       VALOR_ACRESCIMO, USUARIO_SIS, USUARIO_REDE,
                                       MAQUINA_REDE, LOCAL_ATRACACAO, AUTONUM_VINCULADO,
                                       FLAG_HP, GRUPO_ATRACACAO, VALOR_ACRESC_PESO,
                                       PESO_LIMITE, PRECO_MINIMO_DESOVA,  
                                       PORTO_HUBPORT, FLAG_SERVICO_UNICO, FLAG_ISENTO,
                                       TIPO_DOC, BASE_EXCESSO, VALOR_EXCESSO,
                                       ARMADOR, PRECO_MAXIMO, VALOR_ANVISA,
                                       FLAG_COBRAR_NVOCC, FORMA_PAGAMENTO_NVOCC, LINHA,
                                       OPORTUNIDADEID, QTDE_PACOTE, PESO_MINIMO,
                                       VOLUME_MINIMO,EXERCITO)
                                       SELECT
                                       {_schema}.SEQ_LISTA_PRECO_SERVICOS_FIXOS.NEXTVAL, LISTA, SERVICO, 
                                       TIPO_CARGA, BASE_CALCULO, VARIANTE_LOCAL, 
                                       0, MOEDA, 0, 
                                       0, B.USUARIO_SIS, B.USUARIO_REDE, 
                                       B.MAQUINA_REDE, B.LOCAL_ATRACACAO, B.AUTONUM_VINCULADO, 
                                       0,  1,  0, 
                                       0,  0,    
                                       0,  0,  0, 
                                       0,  0,  0, 
                                       0,  0,  0, 
                                       0,  0,  B.LINHA, 
                                       B.OPORTUNIDADEID, 0, 0,0,0
                                       From {_schema}.tb_listas_Precos a  Inner
                                       Join {_schema}.Tb_Lista_Preco_Servicos_Fixos B
                                       on A.Autonum = B.Lista 
                                       Where B.Servico = 80 And
                                       (Preco_Unitario > 0 or Preco_Minimo > 0) And 
                                       Variante_Local = 'MDIR' And
                                       B.OportunidadeId = :OportunidadeId AND 
                                       TIPO_CARGA = 'SVAR' AND
                                       NVL(B.GRUPO_ATRACACAO,0)=0 ", parametros);

                

                var resultado = con.Query<LayoutDTO>($@"
                  SELECT  A.LINHA,B.LINHAREFERENCIA,A.Valor20, A.Valor40 ,A.valor,
                  NVL(B.VALORMINIMO,0) ValorMinimo FROM CRM.TB_CRM_OPORTUNIDADE_LAYOUT A
                  LEFT JOIN 
                 CRM.TB_CRM_OPORTUNIDADE_LAYOUT B ON A.LINHA=B.LINHAREFERENCIA
                                    WHERE A.TipoRegistro  = :TipoRegistro 
                                     AND A.OportunidadeId = :OportunidadeId
                 AND NVL(B.OportunidadeId,:OportunidadeId) = :OportunidadeId    ", parametros);

                bool entrou = true;

                if (resultado != null)
                {
                    foreach (var linha in resultado)
                    {
                        if (linha.Valor20 > 0 || linha.Valor40 > 0 || linha.Valor > 0 || linha.ValorMinimo > 0)
                        {
                            entrou = true;
                        }
                    }
                }
                else
                {
                    entrou = true;
                }


                if (entrou)
                {
                    GravarServicoFixosLiberacao(tabelaId, oportunidadeId);
                }
            }
        }

        public void GravarServicoFixosLiberacao(int tabelaId, int oportunidadeId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametrosT = new DynamicParameters();

                    parametrosT.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    var TemCntr = con.Query($@"SELECT AUTONUM FROM  {_schema}.TB_LISTA_P_S_PERIODO 
                             WHERE Lista = :TabelaId AND Servico = 52 and TIPO_CARGA  NOT IN ('CRGST','BBK','VEIC')", parametrosT).Any();

                    var TemCs = con.Query($@"SELECT AUTONUM FROM  {_schema}.TB_LISTA_P_S_PERIODO 
                             WHERE Lista = :TabelaId AND Servico = 52 and TIPO_CARGA IN ('CRGST','BBK','VEIC')", parametrosT).Any();

                    if (TemCntr == true)


                    {
                        var servicosSincronismocntr = con.Query<ServicoFixoVariavel>($@"                            
                            SELECT 
                                DISTINCT
                                    A.LISTA As TabelaSincronismoId,
                                    A.SERVICO As ServicoId,                                 
                                    A.LOCAL_ATRACACAO As LocalAtracacaoId,
                                    A.GRUPO_ATRACACAO As GrupoAtracacaoId                                     
                                    FROM  {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS A INNER JOIN  SGIPA.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM 
                                   WHERE  tipo_carga NOT IN ('CRGST','BBK','VEIC') AND (B.FLAG_TAXA_LIBERACAO >0  ) AND A.LISTA = 1");

                        foreach (var servicoSincronismo in servicosSincronismocntr)
                        {
                            var parametros = new DynamicParameters();

                            parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                            parametros.Add(name: "ServicoId", value: servicoSincronismo.ServicoId, direction: ParameterDirection.Input);
                            parametros.Add(name: "VarianteLocal", value: servicoSincronismo.VarianteLocal, direction: ParameterDirection.Input);
                            parametros.Add(name: "GrupoAtracacaoId", value: servicoSincronismo.GrupoAtracacaoId, direction: ParameterDirection.Input);

                            var Auto = con.Query<long>($@"SELECT NVL(MAX(AUTONUM),0) FROM  {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE Lista = :TabelaId 
                                     and preco_unitario=0 AND tipo_carga NOT IN ('CRGST','BBK','VEIC') AND Servico = :ServicoId    
                                          ", parametros).FirstOrDefault();

                            if (Auto > 0)
                            {
                                parametros = new DynamicParameters();

                                parametros.Add(name: "LocalAtracacaoId", value: servicoSincronismo.LocalAtracacaoId, direction: ParameterDirection.Input);
                                parametros.Add(name: "GrupoAtracacaoId", value: servicoSincronismo.GrupoAtracacaoId, direction: ParameterDirection.Input);
                                parametros.Add(name: "Autonum", value: Auto, direction: ParameterDirection.Input);

                                con.Execute($@"
	                            update 
		                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS SET 
			                            LOCAL_ATRACACAO=:LocalAtracacaoId,
			                            GRUPO_ATRACACAO=:GrupoAtracacaoId 
			                            WHERE AUTONUM=:AUTONUM
		                            ", parametros);
                            }
                        }
                        /*  var servicosSincronismo = con.Query<ServicoFixoVariavel>($@"                            
                              SELECT 
                                  DISTINCT
                                      A.LISTA As TabelaSincronismoId,
                                      A.SERVICO As ServicoId,
                                      A.TIPO_CARGA As TipoCarga,
                                      A.BASE_CALCULO As BaseCalculo,
                                      A.VARIANTE_LOCAL As VarianteLocal,
                                      A.PRECO_UNITARIO As PrecoUnitario,
                                      A.MOEDA,
                                      A.PRECO_MINIMO As PrecoMinimo,
                                      A.VALOR_ACRESCIMO As ValorAcrescimo,
                                      A.LOCAL_ATRACACAO As LocalAtracacaoId,
                                      A.GRUPO_ATRACACAO As GrupoAtracacaoId,
                                      A.VALOR_ACRESC_PESO As ValorAcrescimoPeso,
                                      A.PESO_LIMITE As PesoLimite,
                                      A.TIPO_OPER As TipoOperacao,
                                      A.TIPO_DOC As TipoDocumentoId,
                                      A.BASE_EXCESSO As BaseExcesso,
                                      A.VALOR_EXCESSO As ValorExcesso,
                                      A.PRECO_MAXIMO As PrecoMaximo,
                                      A.VALOR_ANVISA As ValorAnvisa,
                                      A.FLAG_COBRAR_NVOCC As CobrarNVOCC,
                                      A.FORMA_PAGAMENTO_NVOCC As FormaPagamentoNVOCC
                                      FROM SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS A INNER JOIN SGIPA.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM 
                              WHERE A.TIPO_CARGA<>'CRGST' AND B.FLAG_TAXA_LIBERACAO >0 AND A.LISTA = 1");

                          foreach (var servicoSincronismo in servicosSincronismo)
                          {
                              var parametros = new DynamicParameters();

                              parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                              parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                              parametros.Add(name: "ServicoId", value: servicoSincronismo.ServicoId, direction: ParameterDirection.Input);
                              parametros.Add(name: "TipoCarga", value: servicoSincronismo.TipoCarga, direction: ParameterDirection.Input);
                              parametros.Add(name: "BaseCalculo", value: servicoSincronismo.BaseCalculo, direction: ParameterDirection.Input);
                              parametros.Add(name: "VarianteLocal", value: servicoSincronismo.VarianteLocal, direction: ParameterDirection.Input);
                              parametros.Add(name: "GrupoAtracacaoId", value: servicoSincronismo.GrupoAtracacaoId, direction: ParameterDirection.Input);

                              var existe = con.Query(@"SELECT AUTONUM FROM SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE Lista = :TabelaId 
                                       AND Servico = :ServicoId AND TIPO_CARGA = :TipoCarga  
                                          AND VARIANTE_LOCAL = :VarianteLocal AND NVL(GRUPO_ATRACACAO, 0) = :GrupoAtracacaoId", parametros).Any();

                              if (existe == false)
                              {
                                  parametros = new DynamicParameters();

                                  parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                                  parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                                  parametros.Add(name: "ServicoId", value: servicoSincronismo.ServicoId, direction: ParameterDirection.Input);
                                  parametros.Add(name: "TipoCarga", value: servicoSincronismo.TipoCarga, direction: ParameterDirection.Input);
                                  parametros.Add(name: "BaseCalculo", value: servicoSincronismo.BaseCalculo, direction: ParameterDirection.Input);
                                  parametros.Add(name: "VarianteLocal", value: servicoSincronismo.VarianteLocal, direction: ParameterDirection.Input);
                                  parametros.Add(name: "PrecoUnitario", value: servicoSincronismo.PrecoUnitario, direction: ParameterDirection.Input);
                                  parametros.Add(name: "Moeda", value: servicoSincronismo.Moeda, direction: ParameterDirection.Input);
                                  parametros.Add(name: "PrecoMinimo", value: servicoSincronismo.PrecoMinimo, direction: ParameterDirection.Input);
                                  parametros.Add(name: "ValorAcrescimo", value: servicoSincronismo.ValorAcrescimo, direction: ParameterDirection.Input);
                                  parametros.Add(name: "LocalAtracacaoId", value: servicoSincronismo.LocalAtracacaoId, direction: ParameterDirection.Input);
                                  parametros.Add(name: "GrupoAtracacaoId", value: servicoSincronismo.GrupoAtracacaoId, direction: ParameterDirection.Input);
                                  parametros.Add(name: "ValorAcrescimoPeso", value: servicoSincronismo.ValorAcrescimoPeso, direction: ParameterDirection.Input);
                                  parametros.Add(name: "PesoLimite", value: servicoSincronismo.PesoLimite, direction: ParameterDirection.Input);
                                  parametros.Add(name: "TipoOperacao", value: servicoSincronismo.TipoOperacao, direction: ParameterDirection.Input);
                                  parametros.Add(name: "TipoDocumentoId", value: servicoSincronismo.TipoDocumentoId, direction: ParameterDirection.Input);
                                  parametros.Add(name: "BaseExcesso", value: servicoSincronismo.BaseExcesso, direction: ParameterDirection.Input);
                                  parametros.Add(name: "ValorExcesso", value: servicoSincronismo.ValorExcesso, direction: ParameterDirection.Input);
                                  parametros.Add(name: "PrecoMaximo", value: servicoSincronismo.PrecoMaximo, direction: ParameterDirection.Input);
                                  parametros.Add(name: "ValorAnvisa", value: servicoSincronismo.ValorAnvisa, direction: ParameterDirection.Input);
                                  parametros.Add(name: "CobrarNVOCC", value: servicoSincronismo.CobrarNVOCC.ToInt(), direction: ParameterDirection.Input);
                                  parametros.Add(name: "FormaPagamentoNVOCC", value: servicoSincronismo.FormaPagamentoNVOCC, direction: ParameterDirection.Input);
                                  parametros.Add(name: "TabelaSincronismoId", value: servicoSincronismo.TabelaSincronismoId, direction: ParameterDirection.Input);

                                  con.Execute($@"
                                      INSERT INTO
                                          {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS (
                                              AUTONUM,
                                              LISTA,
                                              OPORTUNIDADEID,
                                              SERVICO,
                                              TIPO_CARGA,
                                              BASE_CALCULO,
                                              VARIANTE_LOCAL,
                                              PRECO_UNITARIO,
                                              MOEDA,
                                              PRECO_MINIMO,
                                              VALOR_ACRESCIMO,
                                              LOCAL_ATRACACAO,
                                              GRUPO_ATRACACAO,
                                              VALOR_ACRESC_PESO,
                                              PESO_LIMITE,
                                              TIPO_OPER,
                                              TIPO_DOC,
                                              BASE_EXCESSO,
                                              VALOR_EXCESSO,
                                              PRECO_MAXIMO,
                                              VALOR_ANVISA,
                                              FLAG_COBRAR_NVOCC,
                                              FORMA_PAGAMENTO_NVOCC,
                                              AUTONUM_VINCULADO
                                          ) VALUES (
                                              {_schema}.SEQ_LISTA_PRECO_SERVICOS_FIXOS.NEXTVAL,
                                              :TabelaId,
                                              :OportunidadeId,
                                              :ServicoId,
                                              :TipoCarga,
                                              :BaseCalculo,
                                              :VarianteLocal,
                                              :PrecoUnitario,
                                              :Moeda,
                                              :PrecoMinimo,
                                              :ValorAcrescimo,
                                              :LocalAtracacaoId,
                                              :GrupoAtracacaoId,
                                              :ValorAcrescimoPeso,
                                              :PesoLimite,
                                              :TipoOperacao,
                                              :TipoDocumentoId,
                                              :BaseExcesso,
                                              :ValorExcesso,
                                              :PrecoMaximo,
                                              :ValorAnvisa,
                                              :CobrarNVOCC,
                                              :FormaPagamentoNVOCC,
                                              :TabelaSincronismoId
                                          )", parametros);
                              }
                          }*/
                    }
                    if (TemCs == true)
                    {
                        var servicosSincronismo = con.Query<ServicoFixoVariavel>($@"                            
                            SELECT 
                                DISTINCT
                                    A.LISTA As TabelaSincronismoId,
                                    A.SERVICO As ServicoId,
                                    A.TIPO_CARGA As TipoCarga,
                                    A.BASE_CALCULO As BaseCalculo,
                                    A.VARIANTE_LOCAL As VarianteLocal,
                                    A.PRECO_UNITARIO As PrecoUnitario,
                                    A.MOEDA,
                                    A.PRECO_MINIMO As PrecoMinimo,
                                    A.VALOR_ACRESCIMO As ValorAcrescimo,
                                    A.LOCAL_ATRACACAO As LocalAtracacaoId,
                                    A.GRUPO_ATRACACAO As GrupoAtracacaoId,
                                    A.VALOR_ACRESC_PESO As ValorAcrescimoPeso,
                                    A.PESO_LIMITE As PesoLimite,
                                    A.TIPO_OPER As TipoOperacao,
                                    A.TIPO_DOC As TipoDocumentoId,
                                    A.BASE_EXCESSO As BaseExcesso,
                                    A.VALOR_EXCESSO As ValorExcesso,
                                    A.PRECO_MAXIMO As PrecoMaximo,
                                    A.VALOR_ANVISA As ValorAnvisa,
                                    A.FLAG_COBRAR_NVOCC As CobrarNVOCC,
                                    A.FORMA_PAGAMENTO_NVOCC As FormaPagamentoNVOCC,
                                    A.EXERCITO AS EXERCITO
                                    FROM  {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS A INNER JOIN  SGIPA.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM 
                                   WHERE   (B.FLAG_TAXA_LIBERACAO >0 or a.servico=1) AND A.LISTA = 1");

                        foreach (var servicoSincronismo in servicosSincronismo)
                        {
                            var parametros = new DynamicParameters();

                            parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                            parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                            parametros.Add(name: "ServicoId", value: servicoSincronismo.ServicoId, direction: ParameterDirection.Input);
                            parametros.Add(name: "TipoCarga", value: servicoSincronismo.TipoCarga, direction: ParameterDirection.Input);
                            parametros.Add(name: "BaseCalculo", value: servicoSincronismo.BaseCalculo, direction: ParameterDirection.Input);
                            parametros.Add(name: "VarianteLocal", value: servicoSincronismo.VarianteLocal, direction: ParameterDirection.Input);
                            parametros.Add(name: "GrupoAtracacaoId", value: servicoSincronismo.GrupoAtracacaoId, direction: ParameterDirection.Input);

                            var Auto = con.Query<long>($@"SELECT NVL(MAX(AUTONUM),0) FROM  {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE Lista = :TabelaId 
                                     AND Servico = :ServicoId    
                                        AND VARIANTE_LOCAL = :VarianteLocal ", parametros).FirstOrDefault();

                            if (Auto > 0)
                            {
                                parametros = new DynamicParameters();

                                parametros.Add(name: "LocalAtracacaoId", value: servicoSincronismo.LocalAtracacaoId, direction: ParameterDirection.Input);
                                parametros.Add(name: "GrupoAtracacaoId", value: servicoSincronismo.GrupoAtracacaoId, direction: ParameterDirection.Input);
                                parametros.Add(name: "Autonum", value: Auto, direction: ParameterDirection.Input);

                                con.Execute($@"
	                            update 
		                            {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS SET 
			                            LOCAL_ATRACACAO=:LocalAtracacaoId,
			                            GRUPO_ATRACACAO=:GrupoAtracacaoId 
			                            WHERE AUTONUM=:AUTONUM
		                            ", parametros);
                            }
                        }
                    }
                    // servivo liberação na tgp motivo que delete, com zero é isento
                    if (TemCntr == true && TemCs == false)
                    {
                        con.Execute($@"DELETE  FROM SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS 
                                WHERE AUTONUM IN(SELECT A.AUTONUM FROM SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS 
                                A INNER JOIN SGIPA.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM 
                                WHERE preco_unitario>0 and  a.servico <>1  and B.FLAG_TAXA_LIBERACAO >0 AND A.LISTA = :TabelaId)", parametrosT);

                    }


                    if (TemCs == false)
                    {
                        con.Execute($@"DELETE  FROM {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS 
                            WHERE servico=1 and LISTA = :TabelaId", parametrosT);

                    }
                }
            }
        }

        public void ExcluirServicosEntreMargem(int tabelaId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    con.Execute($@"DELETE FROM  {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE VARIANTE_LOCAL = 'ENTR' AND LISTA = :TabelaId AND SERVICO in( 332,80)", parametros);
                }
            }
        }

        public void ExcluirTabelaCobrancaIPA(int oportunidadeId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    con.Execute("DELETE FROM SGIPA.TB_LP_LOTES WHERE AUTONUM_LISTA IN (SELECT AUTONUM FROM SGIPA.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId)", parametros);
                    con.Execute("DELETE FROM SGIPA.TB_LISTA_CFG_VALORMINIMO WHERE AUTONUMSV IN (SELECT AUTONUM FROM SGIPA.TB_LISTA_P_S_PERIODO WHERE OportunidadeId = :OportunidadeId)", parametros);
                    con.Execute("DELETE FROM SGIPA.TB_LISTA_P_S_PERIODO WHERE OportunidadeId = :OportunidadeId", parametros);
                    con.Execute("DELETE FROM SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE OportunidadeId = :OportunidadeId", parametros);
                    con.Execute("DELETE FROM SGIPA.TB_TP_GRUPOS WHERE AUTONUMLISTA IN (SELECT AUTONUM FROM SGIPA.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId)", parametros);
                    con.Execute("DELETE FROM SGIPA.TB_LISTAS_PRECOS_VENDEDORES WHERE LISTA IN (SELECT AUTONUM FROM SGIPA.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId)", parametros);
                    con.Execute("DELETE FROM SGIPA.TB_LP_IMPOSTOS WHERE ID_TABELA IN (SELECT AUTONUM FROM SGIPA.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId)", parametros);
                    con.Execute("DELETE FROM SGIPA.TB_LISTAS_EXCESSAO_IMPOSTOS WHERE LISTA IN (SELECT AUTONUM FROM SGIPA.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId)", parametros);
                    con.Execute("DELETE FROM SGIPA.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    con.Execute("DELETE FROM SGIPA..TB_LISTA_CFG_VALORMINIMO WHERE AUTONUMSV IN (SELECT AUTONUM FROM SGIPA..TB_LISTA_P_S_PERIODO WHERE OportunidadeId = @OportunidadeId)", parametros);
                    con.Execute("DELETE FROM SGIPA..TB_LISTA_P_S_PERIODO WHERE OportunidadeId = @OportunidadeId", parametros);
                    con.Execute("DELETE FROM SGIPA..TB_LISTA_PRECO_SERVICOS_FIXOS WHERE OportunidadeId = @OportunidadeId", parametros);
                    con.Execute("DELETE FROM SGIPA..TB_TP_GRUPOS WHERE LISTA IN (SELECT AUTONUM FROM SGIPA..TB_LISTAS_PRECOS WHERE OportunidadeId = @OportunidadeId)", parametros);
                    con.Execute("DELETE FROM SGIPA..TB_LISTAS_PRECOS_VENDEDORES WHERE LISTA IN (SELECT AUTONUM FROM SGIPA.TB_LISTAS_PRECOS WHERE OportunidadeId = :OportunidadeId)", parametros);
                    con.Execute("DELETE FROM SGIPA..TB_LISTAS_PRECOS WHERE OportunidadeId = @OportunidadeId", parametros);
                }
            }
        }

        public DateTime? ObterDataInicio(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                var lotesProposta = con.Query<string>(@"SELECT LOTE FROM TB_CRM_OPORTUNIDADES WHERE Id = :OportunidadeId", parametros).FirstOrDefault();

                if (!string.IsNullOrEmpty(lotesProposta))
                {
                    var lotesArray = Array.ConvertAll(lotesProposta.Split(','), int.Parse);

                    parametros.Add(name: "LotesProposta", value: lotesArray, direction: ParameterDirection.Input);

                    return con.Query<DateTime?>(@"SELECT MIN(PRIMEIRA_ENTRADA) FROM SGIPA.TB_BL WHERE AUTONUM IN :LotesProposta", parametros).FirstOrDefault();
                }
            }

            return null;
        }

        public Conta ObterContaPorDocumento(string documento)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Documento", value: documento, direction: ParameterDirection.Input);

                return con.Query<Conta>(@"SELECT * FROM CRM.TB_CRM_CONTAS WHERE Documento = :Documento", parametros).FirstOrDefault();
            }
        }

        public void CancelarTabelaCobrancaSGIPA(Oportunidade oportunidade)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidade.Id, direction: ParameterDirection.Input);
                parametros.Add(name: "DataCancelamento", value: oportunidade.DataCancelamento.AddHours(23).AddMinutes(59).AddSeconds(59), direction: ParameterDirection.Input);

                con.Execute($@"UPDATE SGIPA.TB_LISTAS_PRECOS SET CANCELADO_CRM = 1, OBS = 'CANCELADA EM: {DateTime.Now} ' || OBS, FINAL_VALIDADE = :DataCancelamento WHERE AUTONUM = :TabelaId", parametros);
                con.Execute($@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET DataTermino = :DataCancelamento WHERE Id = :OportunidadeId", parametros);
            }
        }

        public int TabelaCancelada(int tabelaId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                return con.Query<int>($@"SELECT NVL(MAX(CANCELADO_CRM),0) Cancelado FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :TabelaId", parametros).Single();
            }
        }

        public int CadastrarTabelaCobrancaSGIPA(Oportunidade oportunidade)
        {
            int? importador = 0;
            int? despachante = 0;
            int? nvocc = 0;
            int? coloader = 0;
            int? cocoloader = 0;
            int? cocoloader2 = 0;
            int? freetime = 0;
            int? hubport = 0;
            int? tiposervico = 0;
            int? tabelaEspecial = 0;
            int? formaPagamento = 0;
            string perfil = string.Empty;

            DateTime validadeProposta;

            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                con.Open();
                   using (var transaction = con.BeginTransaction())
                    {
                    try
                    {
                        var contaCrmBusca = ObterContaPorDocumento(oportunidade.ContaDocumento);

                        if (oportunidade.TabelaReferencia > 0)
                        {
                            freetime = ConsultaFreetime(oportunidade.TabelaReferencia);

                            tiposervico = ConsultaTiposervico(oportunidade.TabelaReferencia);

                            hubport = ConsultaHubPort(oportunidade.TabelaReferencia);

                            tabelaEspecial = ConsultaTabelaEspecial(oportunidade.TabelaReferencia);

                            var formaPagamentoTabelaVinculo = ConsultaFormaPagamento(oportunidade.TabelaReferencia);

                            perfil = ConsultaPerfil(oportunidade.TabelaReferencia);

                            if (formaPagamentoTabelaVinculo == 2 && oportunidade.FormaPagamentoId == FormaPagamento.FATURADO)
                            {
                                perfil = "A";
                            }
                        }
                        else
                        {
                            freetime = oportunidade.DiasFreeTime;
                            tiposervico = oportunidade.TipoServicoModelo;
                            hubport = oportunidade.HubPort.ToInt();
                            tabelaEspecial = oportunidade.CobrancaEspecial.ToInt();

                            if (oportunidade.FormaPagamentoId == FormaPagamento.FATURADO)
                            {
                                if (string.IsNullOrEmpty(oportunidade.ClassificacaoCliente))
                                {
                                    perfil = "A";
                                }
                                else
                                {
                                    perfil = oportunidade.ClassificacaoCliente;
                                }
                            }
                            else
                            {
                                perfil = string.Empty;
                            }
                        }

                        //if (oportunidade.HubPort)
                        //{
                        //    formaPagamento = 2;
                        //}
                        //else
                        //{

                        //}
                        formaPagamento = ConverteFormaPagamentoIPA.FormaPagamentoIPA(oportunidade.FormaPagamentoId.ToValue());

                        var parceiroPrincipalBusca = ConsultaParceiro(oportunidade.ContaDocumento);

                        if (parceiroPrincipalBusca == null)
                        {
                            parceiroPrincipalBusca = new Parceiro();

                            var parceiro = new Parceiro
                            {
                                RazaoSocial = contaCrmBusca.Descricao,
                                NomeFantasia = contaCrmBusca?.NomeFantasia,
                                CNPJ = oportunidade.ContaDocumento
                            };

                            parceiroPrincipalBusca.Id = CadastrarParceiro(parceiro);
                        }

                        AtualizarSegmentoParceiro(parceiroPrincipalBusca.Id, oportunidade.SegmentoId);

                        switch (oportunidade.SegmentoId)
                        {
                            case Segmento.AGENTE:
                            case Segmento.FREIGHT_FORWARDER:
                                nvocc = parceiroPrincipalBusca.Id;
                                break;
                            case Segmento.IMPORTADOR:
                                importador = parceiroPrincipalBusca.Id;
                                break;
                            case Segmento.DESPACHANTE:
                                despachante = parceiroPrincipalBusca.Id;
                                break;
                            case Segmento.COLOADER:
                                coloader = parceiroPrincipalBusca.Id;
                                break;
                        }

                        oportunidade.TabelaId = con.Query<int>("SELECT SGIPA.SEQ_TB_TABELAS_COBRANCA.NEXTVAL FROM DUAL").Single();

                        var validade = oportunidade.DataTermino;

                        if (oportunidade.Cancelado && oportunidade.StatusOportunidade == "Cancelada")
                        {
                            validadeProposta = oportunidade.DataCancelamento;
                        }
                        else
                        {
                            if (validade.Year < DateTime.Now.Year)
                            {
                                validadeProposta = new DateTime(2100, validade.Month, validade.Day, 23, 59, 59);

                            }
                            else
                            {
                                validadeProposta = new DateTime(validade.Year, validade.Month, validade.Day, 23, 59, 59);
                            }
                        }

                        var parametros = new DynamicParameters();

                        if (oportunidade.TabelaReferencia > 0)
                        {
                            parametros.Add(name: "TabelaId", value: oportunidade.TabelaReferencia, direction: ParameterDirection.Input);

                            oportunidade.ClassificacaoCliente = con.Query<string>("SELECT PERFIL FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :TabelaId", parametros).FirstOrDefault();
                        }
                        else
                        {
                            if (oportunidade.FormaPagamentoId == FormaPagamento.FATURADO)
                            {
                                if (string.IsNullOrEmpty(oportunidade.ClassificacaoCliente))
                                {
                                    oportunidade.ClassificacaoCliente = "A";
                                }
                            }
                            else
                            {
                                oportunidade.ClassificacaoCliente = string.Empty;
                            }
                        }

                        parametros = new DynamicParameters();

                        parametros.Add(name: "Id", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                        parametros.Add(name: "OportunidadeId", value: oportunidade.Id, direction: ParameterDirection.Input);
                        parametros.Add(name: "ContaDocumento", value: oportunidade.ContaDocumento, direction: ParameterDirection.Input);
                        parametros.Add(name: "Descricao", value: CorrigeNomeTabela(oportunidade.Descricao), direction: ParameterDirection.Input);
                        parametros.Add(name: "Importador", value: importador, direction: ParameterDirection.Input);
                        parametros.Add(name: "Despachante", value: despachante, direction: ParameterDirection.Input);
                        parametros.Add(name: "NVOCC", value: nvocc, direction: ParameterDirection.Input);
                        parametros.Add(name: "Coloader", value: coloader, direction: ParameterDirection.Input);
                        parametros.Add(name: "DiasAposGR", value: freetime, direction: ParameterDirection.Input);
                        parametros.Add(name: "FatorDesova", value: oportunidade.DesovaParcial, direction: ParameterDirection.Input);
                        parametros.Add(name: "FatorCarga", value: oportunidade.FatorCP, direction: ParameterDirection.Input);
                        parametros.Add(name: "Acordo", value: oportunidade.Acordo.ToInt(), direction: ParameterDirection.Input);
                        parametros.Add(name: "HubPort", value: hubport, direction: ParameterDirection.Input);
                        parametros.Add(name: "PosicIsento", value: oportunidade.PosicIsento, direction: ParameterDirection.Input);
                        parametros.Add(name: "Especial", value: tabelaEspecial, direction: ParameterDirection.Input);
                        parametros.Add(name: "UsuarioId", value: 90, direction: ParameterDirection.Input);
                        parametros.Add(name: "DataCadastro", value: DateTime.Now, direction: ParameterDirection.Input);
                        parametros.Add(name: "TipoServico", value: tiposervico, direction: ParameterDirection.Input);
                        parametros.Add(name: "Perfil", value: perfil, direction: ParameterDirection.Input);
                        parametros.Add(name: "ImpostoId", value: oportunidade.ImpostoId, direction: ParameterDirection.Input);

                        if (string.IsNullOrEmpty(oportunidade.Lote))
                            parametros.Add(name: "InicioProposta", value: oportunidade.DataInicio, direction: ParameterDirection.Input);
                        else
                        {
                            var dataInicio = ObterDataInicio(oportunidade.Id);

                            parametros.Add(name: "InicioProposta", value: dataInicio, direction: ParameterDirection.Input);
                        }

                        if (oportunidade.Acordo)
                        {
                            var dataInicio = ObterDataInicio(oportunidade.Id);

                            if (dataInicio.HasValue)
                            {
                                var dataInicioSemHoras = dataInicio.Value;

                                dataInicioSemHoras = dataInicioSemHoras.AddHours(-dataInicioSemHoras.Hour);
                                dataInicioSemHoras = dataInicioSemHoras.AddMinutes(-dataInicioSemHoras.Minute);
                                dataInicioSemHoras = dataInicioSemHoras.AddSeconds(-dataInicioSemHoras.Second);

                                parametros.Add(name: "InicioProposta", value: dataInicioSemHoras, direction: ParameterDirection.Input);
                            }
                            else
                            {
                                parametros.Add(name: "InicioProposta", value: oportunidade.DataInicio, direction: ParameterDirection.Input);
                            }
                        }

                        parametros.Add(name: "ValidadeProposta", value: validadeProposta, direction: ParameterDirection.Input);
                        parametros.Add(name: "FormaPagamento", value: formaPagamento, direction: ParameterDirection.Input);

                        if (oportunidade.Acordo)
                        {
                            parametros.Add(name: "Proposta", value: $"AC {oportunidade.Identificacao}", direction: ParameterDirection.Input);
                            parametros.Add(name: "Liberada", value: 0, direction: ParameterDirection.Input);
                        }
                        else
                        {
                            parametros.Add(name: "Proposta", value: oportunidade.Identificacao, direction: ParameterDirection.Input);
                            parametros.Add(name: "Liberada", value: 0, direction: ParameterDirection.Input);
                        }

                        con.Execute(@"
                            INSERT INTO 
                                SGIPA.TB_LISTAS_PRECOS 
                                    (
                                        AUTONUM, 
                                        DESCR, 
                                        FLAG_LIBERADA, 
                                        FORMA_PAGAMENTO, 
                                        OportunidadeId, 
                                        Data_Inicio, 
                                        Final_Validade, 
                                        IMPORTADOR, 
                                        DESPACHANTE, 
                                        CAPTADOR, 
                                        COLOADER,
                                        DIAS_APOS_GR,
                                        FATOR_DESOVA,
                                        FATOR_CARGA,
                                        FLAG_ACORDO,
                                        FLAG_HUBPORT,
                                        POSIC_ISENTO,
                                        FLAG_ESPECIAL,
                                        PROPOSTA,
                                        USUARIO_CAD,
                                        DATA_CAD,
                                        TIPO_SERVICO,
                                        PERFIL,
                                        IMPOSTOID
                                    ) VALUES (
                                        :Id, 
                                        :Descricao, 
                                        :Liberada, 
                                        :FormaPagamento, 
                                        :OportunidadeId, 
                                        :InicioProposta, 
                                        :ValidadeProposta, 
                                        :Importador, 
                                        :Despachante, 
                                        :NVOCC, 
                                        :Coloader,
                                        :DiasAposGR, 
                                        :FatorDesova, 
                                        :FatorCarga, 
                                        :Acordo, 
                                        :HubPort, 
                                        :PosicIsento, 
                                        :Especial,
                                        :Proposta,
                                        :UsuarioId,
                                        :DataCadastro,
                                        :TipoServico,
                                        :Perfil,
                                        :ImpostoId
                                    )", parametros, transaction);

                        transaction.Commit();
                        if (oportunidade.TabelaId > 0)
                        {
                            if (nvocc > 0)
                            {
                                parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                parametros.Add(name: "Tipo", value: "C", direction: ParameterDirection.Input);
                                parametros.Add(name: "ParceiroId", value: nvocc, direction: ParameterDirection.Input);

                                var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                if (!existe)
                                {
                                    con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                }
                            }
                            if (despachante > 0)
                            {
                                parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                parametros.Add(name: "Tipo", value: "D", direction: ParameterDirection.Input);
                                parametros.Add(name: "ParceiroId", value: despachante, direction: ParameterDirection.Input);

                                var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                if (!existe)
                                {
                                    con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                }
                            }
                            if (importador > 0)
                            {
                                parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                parametros.Add(name: "Tipo", value: "I", direction: ParameterDirection.Input);
                                parametros.Add(name: "ParceiroId", value: importador, direction: ParameterDirection.Input);

                                var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                if (!existe)
                                {
                                    con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                }
                            }
                            if (coloader > 0)
                            {
                                parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                parametros.Add(name: "Tipo", value: "S", direction: ParameterDirection.Input);
                                parametros.Add(name: "ParceiroId", value: coloader, direction: ParameterDirection.Input);

                                var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                if (!existe)
                                {
                                    con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                }
                            }

                            var vendedorBusca = ConsultaVendedor(oportunidade.VendedorCpf);

                            if (!string.IsNullOrEmpty(oportunidade.VendedorCpf))
                            {
                                if (vendedorBusca == null)
                                {
                                    parametros = new DynamicParameters();
                                    parametros.Add(name: "VendedorId", value: oportunidade.VendedorId, direction: ParameterDirection.Input);

                                    var vendedorObj = con.Query<Vendedor>("SELECT Id, Nome, CPF FROM CRM.TB_CRM_USUARIOS WHERE ID = :VendedorId", parametros).FirstOrDefault();

                                    vendedorBusca = new Vendedor
                                    {
                                        Id = CadastrarVendedor(new Vendedor
                                        {
                                            Nome = vendedorObj.Nome,
                                            CPF = vendedorObj.CPF
                                        })
                                    };
                                }
                                else
                                {
                                    AtualizarFlagVendedor(vendedorBusca.Id);
                                }

                                parametros = new DynamicParameters();

                                if (string.IsNullOrEmpty(oportunidade.Lote))
                                    parametros.Add(name: "DataInicio", value: oportunidade.DataInicio, direction: ParameterDirection.Input);
                                else
                                {
                                    var dataInicio = ObterDataInicio(oportunidade.Id);
                                    parametros.Add(name: "DataInicio", value: dataInicio, direction: ParameterDirection.Input);
                                }

                                parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                parametros.Add(name: "VendedorId", value: vendedorBusca.Id, direction: ParameterDirection.Input);
                                parametros.Add(name: "DataTermino", value: oportunidade.DataTermino, direction: ParameterDirection.Input);

                                con.Execute(@"INSERT INTO SGIPA.TB_LISTAS_PRECOS_VENDEDORES (AUTONUM, LISTA, VENDEDOR, DATA_INI_VIG, DATA_FIM_VIG) VALUES (SGIPA.SEQ_LP_VENDEDORES.NEXTVAL, :TabelaId, :VendedorId, :DataInicio, :DataTermino)", parametros, transaction);
                            }

                            parametros = new DynamicParameters();
                            parametros.Add(name: "OportunidadeId", value: oportunidade.Id, direction: ParameterDirection.Input);

                            var clientesGrupoCnpj = con.Query<ClienteProposta>(@"
                                SELECT
                                    B.Id,
                                    B.ContaId,
                                    C.Login As CriadoPor,
                                    A.Descricao,
                                    A.Documento,
                                    A.NomeFantasia,
                                    B.DataCriacao
                                FROM
                                    CRM.TB_CRM_CONTAS A 
                                INNER JOIN
                                    CRM.TB_CRM_OPORTUNIDADE_GRUPO_CNPJ B ON A.Id = B.ContaId
                                INNER JOIN
                                    CRM.TB_CRM_USUARIOS C ON B.CriadoPor = C.Id
                                WHERE
                                    B.OportunidadeId = :OportunidadeId", parametros, transaction);

                            foreach (var clienteGrupo in clientesGrupoCnpj)
                            {
                                var parceiroBusca = ConsultaParceiro(clienteGrupo.Documento);

                                if (parceiroBusca == null)
                                {
                                    parceiroBusca = new Parceiro();

                                    parceiroBusca.Id = CadastrarParceiro(new Parceiro
                                    {
                                        RazaoSocial = clienteGrupo.Descricao,
                                        NomeFantasia = clienteGrupo.NomeFantasia,
                                        CNPJ = clienteGrupo.Documento,
                                        Vendedor = false
                                    });
                                }

                                //AtualizarSegmentoParceiro(parceiroBusca.Id, clienteGrupo.Segmento);
                                AtualizarSegmentoParceiro(parceiroBusca.Id, oportunidade.SegmentoId);

                                parametros = new DynamicParameters();

                                parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                parametros.Add(name: "ParceiroId", value: parceiroBusca.Id, direction: ParameterDirection.Input);

                                switch (oportunidade.SegmentoId)
                                {
                                    case Segmento.IMPORTADOR:
                                        parametros.Add(name: "Tipo", value: "I", direction: ParameterDirection.Input);
                                        break;
                                    case Segmento.DESPACHANTE:
                                        parametros.Add(name: "Tipo", value: "D", direction: ParameterDirection.Input);
                                        break;
                                    case Segmento.AGENTE:
                                    case Segmento.FREIGHT_FORWARDER:
                                        parametros.Add(name: "Tipo", value: "C", direction: ParameterDirection.Input);
                                        break;
                                    case Segmento.COLOADER:
                                        parametros.Add(name: "Tipo", value: "S", direction: ParameterDirection.Input);
                                        break;
                                }

                                var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                if (!existe)
                                {
                                    con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                }
                            }

                            // 15/01/2020 - Jéssica - Doc. Integracao11.doc:
                            // 7.Integração Oportunidade Segmento = Despachante e Tipo Prop. = Geral, não integrar os sub clientes

                            // 27/05/2020 - Jéssica - Email
                            // se a oportunidade possuir o segmento Agente de Carga / Freight Forward – Tipo Proposta = Geral e Tipo Serviço = FCL;
                            if (!(oportunidade.SegmentoId == Segmento.DESPACHANTE && oportunidade.TipoDePropostaId == TipoDeProposta.GERAL))
                            {
                                if (!((oportunidade.SegmentoId == Segmento.AGENTE || oportunidade.SegmentoId == Segmento.FREIGHT_FORWARDER) && oportunidade.TipoDePropostaId == TipoDeProposta.GERAL && oportunidade.TipoServicoId == TipoServico.FCL))
                                {
                                    parametros = new DynamicParameters();
                                    parametros.Add(name: "OportunidadeId", value: oportunidade.Id, direction: ParameterDirection.Input);

                                    var subClientes = con.Query<ClienteProposta>(@"
                                    SELECT
                                        B.Id,
                                        B.ContaId,
                                        C.Login As CriadoPor,
                                        A.Descricao,
                                        A.Documento,
                                        A.NomeFantasia,
                                        B.DataCriacao,
                                        B.Segmento
                                    FROM
                                        CRM.TB_CRM_CONTAS A 
                                    INNER JOIN
                                        CRM.TB_CRM_OPORTUNIDADE_CLIENTES B ON A.Id = B.ContaId
                                    INNER JOIN
                                        CRM.TB_CRM_USUARIOS C ON B.CriadoPor = C.Id
                                    WHERE
                                        B.OportunidadeId = :OportunidadeId", parametros, transaction);

                                    var subClientesImportador = subClientes
                                        .Where(s => s.Segmento == SegmentoSubCliente.IMPORTADOR)
                                        .Select(c => new ClienteProposta(c.Documento, c.Segmento, c.Descricao, c.NomeFantasia)).ToList()
                                        .Distinct()
                                        .ToList();

                                    var subClientesDespachante = subClientes
                                        .Where(s => s.Segmento == SegmentoSubCliente.DESPACHANTE)
                                        .Select(c => new ClienteProposta(c.Documento, c.Segmento, c.Descricao, c.NomeFantasia)).ToList()
                                        .Distinct()
                                        .ToList();

                                    var subClientesColoader = subClientes
                                        .Where(s => s.Segmento == SegmentoSubCliente.COLOADER)
                                        .Select(c => new ClienteProposta(c.Documento, c.Segmento, c.Descricao, c.NomeFantasia)).ToList()
                                        .Distinct()
                                        .ToList();

                                    var subClientesCoColoader = subClientes
                                        .Where(s => s.Segmento == SegmentoSubCliente.CO_COLOADER1)
                                        .Select(c => new ClienteProposta(c.Documento, c.Segmento, c.Descricao, c.NomeFantasia)).ToList()
                                        .Distinct()
                                        .ToList();

                                    var subClientesCoColoader2 = subClientes
                                        .Where(s => s.Segmento == SegmentoSubCliente.CO_COLOADER2)
                                        .Select(c => new ClienteProposta(c.Documento, c.Segmento, c.Descricao, c.NomeFantasia)).ToList()
                                        .Distinct()
                                        .ToList();

                                    if (subClientesImportador.Count >= 1)
                                    {
                                        var subCliente = subClientesImportador.First();

                                        var parceiroImportador = ConsultaParceiro(subCliente.Documento);

                                        if (parceiroImportador == null)
                                        {
                                            parceiroImportador = new Parceiro
                                            {
                                                Id = CadastrarParceiro(new Parceiro
                                                {
                                                    RazaoSocial = subCliente.Descricao,
                                                    NomeFantasia = subCliente.NomeFantasia,
                                                    CNPJ = subCliente.Documento,
                                                    Importador = true
                                                })
                                            };
                                        }

                                        AtualizarSegmentoParceiro(parceiroImportador.Id, SegmentoSubCliente.IMPORTADOR);

                                        importador = parceiroImportador.Id;

                                        var restantes = subClientesImportador
                                            .Where(c => c.Documento != subCliente.Documento).ToList();

                                        foreach (var restante in restantes)
                                        {
                                            var parceiroBusca = ConsultaParceiro(restante.Documento);

                                            if (parceiroBusca == null)
                                            {
                                                parceiroBusca = new Parceiro
                                                {
                                                    Id = CadastrarParceiro(new Parceiro
                                                    {
                                                        RazaoSocial = restante.Descricao,
                                                        NomeFantasia = restante.NomeFantasia,
                                                        CNPJ = restante.Documento,
                                                        Importador = true
                                                    })
                                                };
                                            }

                                            AtualizarSegmentoParceiro(parceiroBusca.Id, SegmentoSubCliente.IMPORTADOR);

                                            parametros = new DynamicParameters();

                                            parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                            parametros.Add(name: "ParceiroId", value: parceiroBusca.Id, direction: ParameterDirection.Input);
                                            parametros.Add(name: "Tipo", value: "I", direction: ParameterDirection.Input);

                                            if (restante.Segmento == SegmentoSubCliente.IMPORTADOR)
                                            {
                                                var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                                if (!existe)
                                                {
                                                    con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                                }
                                            }
                                        }
                                    }

                                    if (subClientesDespachante.Count >= 1)
                                    {
                                        var subCliente = subClientesDespachante.First();

                                        var parceiroDespachante = ConsultaParceiro(subCliente.Documento);

                                        if (parceiroDespachante == null)
                                        {
                                            parceiroDespachante = new Parceiro
                                            {
                                                Id = CadastrarParceiro(new Parceiro
                                                {
                                                    RazaoSocial = subCliente.Descricao,
                                                    NomeFantasia = subCliente.NomeFantasia,
                                                    CNPJ = subCliente.Documento,
                                                    Despachante = true
                                                })
                                            };
                                        }

                                        AtualizarSegmentoParceiro(parceiroDespachante.Id, SegmentoSubCliente.DESPACHANTE);

                                        despachante = parceiroDespachante.Id;

                                        var restantes = subClientesDespachante
                                            .Where(c => c.Documento != subCliente.Documento).ToList();

                                        foreach (var restante in restantes)
                                        {
                                            var parceiroBusca = ConsultaParceiro(restante.Documento);

                                            if (parceiroBusca == null)
                                            {
                                                parceiroBusca = new Parceiro
                                                {
                                                    Id = CadastrarParceiro(new Parceiro
                                                    {
                                                        RazaoSocial = restante.Descricao,
                                                        NomeFantasia = restante.NomeFantasia,
                                                        CNPJ = restante.Documento,
                                                        Despachante = true
                                                    })
                                                };
                                            }

                                            AtualizarSegmentoParceiro(parceiroBusca.Id, SegmentoSubCliente.DESPACHANTE);

                                            parametros = new DynamicParameters();

                                            parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                            parametros.Add(name: "ParceiroId", value: parceiroBusca.Id, direction: ParameterDirection.Input);
                                            parametros.Add(name: "Tipo", value: "D", direction: ParameterDirection.Input);

                                            if (restante.Segmento == SegmentoSubCliente.DESPACHANTE)
                                            {
                                                var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                                if (!existe)
                                                {
                                                    con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                                }
                                            }
                                        }
                                    }

                                    if (subClientesColoader.Count >= 1)
                                    {
                                        var subCliente = subClientesColoader.First();

                                        var parceiroColoader = ConsultaParceiro(subCliente.Documento);

                                        if (parceiroColoader == null)
                                        {
                                            parceiroColoader = new Parceiro
                                            {
                                                Id = CadastrarParceiro(new Parceiro
                                                {
                                                    RazaoSocial = subCliente.Descricao,
                                                    NomeFantasia = subCliente.NomeFantasia,
                                                    CNPJ = subCliente.Documento,
                                                    Coloaders = true
                                                })
                                            };
                                        }

                                        AtualizarSegmentoParceiro(parceiroColoader.Id, SegmentoSubCliente.COLOADER);

                                        coloader = parceiroColoader.Id;

                                        var restantes = subClientesColoader
                                            .Where(c => c.Documento != subCliente.Documento).ToList();

                                        foreach (var restante in restantes)
                                        {
                                            var parceiroBusca = ConsultaParceiro(restante.Documento);

                                            if (parceiroBusca == null)
                                            {
                                                parceiroBusca = new Parceiro
                                                {
                                                    Id = CadastrarParceiro(new Parceiro
                                                    {
                                                        RazaoSocial = restante.Descricao,
                                                        NomeFantasia = restante.NomeFantasia,
                                                        CNPJ = restante.Documento,
                                                        Coloaders = true
                                                    })
                                                };
                                            }

                                            AtualizarSegmentoParceiro(parceiroBusca.Id, SegmentoSubCliente.COLOADER);

                                            parametros = new DynamicParameters();

                                            parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                            parametros.Add(name: "ParceiroId", value: parceiroBusca.Id, direction: ParameterDirection.Input);
                                            parametros.Add(name: "Tipo", value: "S", direction: ParameterDirection.Input);

                                            if (restante.Segmento == SegmentoSubCliente.COLOADER)
                                            {
                                                var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                                if (!existe)
                                                {
                                                    con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                                }
                                            }
                                        }
                                    }

                                    if (subClientesCoColoader.Count >= 1)
                                    {
                                        var subCliente = subClientesCoColoader.First();

                                        var parceiroCoColoader = ConsultaParceiro(subCliente.Documento);

                                        if (parceiroCoColoader == null)
                                        {
                                            parceiroCoColoader = new Parceiro
                                            {
                                                Id = CadastrarParceiro(new Parceiro
                                                {
                                                    RazaoSocial = subCliente.Descricao,
                                                    NomeFantasia = subCliente.NomeFantasia,
                                                    CNPJ = subCliente.Documento,
                                                    Coloaders = true
                                                })
                                            };
                                        }

                                        AtualizarSegmentoParceiro(parceiroCoColoader.Id, SegmentoSubCliente.COLOADER);

                                        cocoloader = parceiroCoColoader.Id;

                                        var restantes = subClientesCoColoader
                                            .Where(c => c.Documento != subCliente.Documento).ToList();

                                        foreach (var restante in restantes)
                                        {
                                            var parceiroBusca = ConsultaParceiro(restante.Documento);

                                            if (parceiroBusca == null)
                                            {
                                                parceiroBusca = new Parceiro
                                                {
                                                    Id = CadastrarParceiro(new Parceiro
                                                    {
                                                        RazaoSocial = restante.Descricao,
                                                        NomeFantasia = restante.NomeFantasia,
                                                        CNPJ = restante.Documento,
                                                        Coloaders = true
                                                    })
                                                };
                                            }

                                            AtualizarSegmentoParceiro(parceiroBusca.Id, SegmentoSubCliente.COLOADER);

                                            parametros = new DynamicParameters();

                                            parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                            parametros.Add(name: "ParceiroId", value: parceiroBusca.Id, direction: ParameterDirection.Input);
                                            parametros.Add(name: "Tipo", value: "U", direction: ParameterDirection.Input);

                                            if (restante.Segmento == SegmentoSubCliente.CO_COLOADER1)
                                            {
                                                var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                                if (!existe)
                                                {
                                                    con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, 'U')", parametros, transaction);
                                                }
                                            }
                                        }
                                    }

                                    if (subClientesCoColoader2.Count >= 1)
                                    {
                                        var subCliente = subClientesCoColoader2.First();

                                        var parceiroCoColoader2 = ConsultaParceiro(subCliente.Documento);

                                        if (parceiroCoColoader2 == null)
                                        {
                                            parceiroCoColoader2 = new Parceiro
                                            {
                                                Id = CadastrarParceiro(new Parceiro
                                                {
                                                    RazaoSocial = subCliente.Descricao,
                                                    NomeFantasia = subCliente.NomeFantasia,
                                                    CNPJ = subCliente.Documento,
                                                    Coloaders = true
                                                })
                                            };
                                        }

                                        AtualizarSegmentoParceiro(parceiroCoColoader2.Id, SegmentoSubCliente.COLOADER);

                                        cocoloader2 = parceiroCoColoader2.Id;

                                        var restantes = subClientesCoColoader2
                                            .Where(c => c.Documento != subCliente.Documento).ToList();

                                        foreach (var restante in restantes)
                                        {
                                            var parceiroBusca = ConsultaParceiro(restante.Documento);

                                            if (parceiroBusca == null)
                                            {
                                                parceiroBusca = new Parceiro
                                                {
                                                    Id = CadastrarParceiro(new Parceiro
                                                    {
                                                        RazaoSocial = restante.Descricao,
                                                        NomeFantasia = restante.NomeFantasia,
                                                        CNPJ = restante.Documento,
                                                        Coloaders = true
                                                    })
                                                };
                                            }

                                            AtualizarSegmentoParceiro(parceiroBusca.Id, SegmentoSubCliente.COLOADER);

                                            parametros = new DynamicParameters();

                                            parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                            parametros.Add(name: "ParceiroId", value: parceiroBusca.Id, direction: ParameterDirection.Input);
                                            parametros.Add(name: "Tipo", value: "B", direction: ParameterDirection.Input);

                                            if (restante.Segmento == SegmentoSubCliente.CO_COLOADER2)
                                            {
                                                var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                                if (!existe)
                                                {
                                                    con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                                }
                                            }
                                        }
                                    }

                                    parametros = new DynamicParameters();

                                    parametros.Add(name: "Importador", value: importador, direction: ParameterDirection.Input);
                                    parametros.Add(name: "Despachante", value: despachante, direction: ParameterDirection.Input);
                                    parametros.Add(name: "Coloader", value: coloader, direction: ParameterDirection.Input);
                                    parametros.Add(name: "CoColoader", value: cocoloader, direction: ParameterDirection.Input);
                                    parametros.Add(name: "CoColoader2", value: cocoloader2, direction: ParameterDirection.Input);
                                    parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);

                                    con.Execute(@"
                                    UPDATE
                                        SGIPA.TB_LISTAS_PRECOS
                                            SET
                                                IMPORTADOR = :Importador,
                                                DESPACHANTE = :Despachante,
                                                COLOADER = :Coloader,
                                                COCOLOADER = :CoColoader,
                                                COCOLOADER2 = :CoColoader2
                                            WHERE AUTONUM = :TabelaId", parametros, transaction);

                                    if (importador > 0)
                                    {
                                        parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                        parametros.Add(name: "Tipo", value: "I", direction: ParameterDirection.Input);
                                        parametros.Add(name: "ParceiroId", value: importador, direction: ParameterDirection.Input);

                                        var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                        if (!existe)
                                        {
                                            con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                        }
                                    }

                                    if (despachante > 0)
                                    {
                                        parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                        parametros.Add(name: "Tipo", value: "D", direction: ParameterDirection.Input);
                                        parametros.Add(name: "ParceiroId", value: despachante, direction: ParameterDirection.Input);

                                        var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                        if (!existe)
                                        {
                                            con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                        }
                                    }

                                    if (coloader > 0)
                                    {
                                        parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                        parametros.Add(name: "Tipo", value: "S", direction: ParameterDirection.Input);
                                        parametros.Add(name: "ParceiroId", value: coloader, direction: ParameterDirection.Input);

                                        var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                        if (!existe)
                                        {
                                            con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                        }
                                    }
                                    if (cocoloader > 0)
                                    {
                                        parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                        parametros.Add(name: "Tipo", value: "U", direction: ParameterDirection.Input);
                                        parametros.Add(name: "ParceiroId", value: cocoloader, direction: ParameterDirection.Input);

                                        var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                        if (!existe)
                                        {
                                            con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                        }
                                    }

                                    if (cocoloader2 > 0)
                                    {
                                        parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                                        parametros.Add(name: "Tipo", value: "B", direction: ParameterDirection.Input);
                                        parametros.Add(name: "ParceiroId", value: cocoloader2, direction: ParameterDirection.Input);

                                        var existe = con.Query<bool>("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE TIPO = :Tipo AND AUTONUM_PARCEIRO = :ParceiroId AND AUTONUMLISTA = :TabelaId", parametros).Any();

                                        if (!existe)
                                        {
                                            con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros, transaction);
                                        }
                                    }
                                }
                            }

                            parceiroPrincipalBusca = ConsultaParceiro(oportunidade.ContaDocumento);

                            if (parceiroPrincipalBusca != null)
                            {
                                switch (oportunidade.SegmentoId)
                                {
                                    case Segmento.AGENTE:
                                    case Segmento.FREIGHT_FORWARDER:
                                        nvocc = parceiroPrincipalBusca.Id;
                                        break;
                                    case Segmento.IMPORTADOR:
                                        importador = parceiroPrincipalBusca.Id;
                                        break;
                                    case Segmento.DESPACHANTE:
                                        despachante = parceiroPrincipalBusca.Id;
                                        break;
                                    case Segmento.COLOADER:
                                        coloader = parceiroPrincipalBusca.Id;
                                        break;
                                }

                                parametros = new DynamicParameters();

                                parametros.Add(name: "Importador", value: importador, direction: ParameterDirection.Input);
                                parametros.Add(name: "Despachante", value: despachante, direction: ParameterDirection.Input);
                                parametros.Add(name: "Coloader", value: coloader, direction: ParameterDirection.Input);
                                parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);

                                con.Execute(@"
                                    UPDATE
                                        SGIPA.TB_LISTAS_PRECOS
                                            SET
                                                IMPORTADOR = :Importador,
                                                DESPACHANTE = :Despachante,
                                                COLOADER = :Coloader
                                            WHERE AUTONUM = :TabelaId", parametros, transaction);

                            }

                         

                            var descricaoTabela = CriarNomeTabela(oportunidade, parceiroPrincipalBusca.CNPJ);

                            AtualizarDescricaoTabela(oportunidade.TabelaId, descricaoTabela);
                            AtualizarObservacaoTabela(oportunidade.TabelaId, oportunidade.RevisaoId, oportunidade.Id);
                            AtualizarUsuarioCriacao(oportunidade.TabelaId, oportunidade.UsuarioIntegracaoId);
                            AtualizarFreeTime(oportunidade.TabelaId, oportunidade);
                            AtualizarTabelaOportunidade(oportunidade.TabelaId, oportunidade.Id);

                            IntegrarExcessaoImpostos(oportunidade);

                            if (oportunidade.TabelaReferencia > 0)
                            {
                                ImportarServicosFixos(oportunidade.TabelaReferencia, oportunidade.TabelaId);
                                ImportarServicosVariaveis(oportunidade.TabelaReferencia, oportunidade.TabelaId);
                                //ImportarImpostos(oportunidade.TabelaReferencia, oportunidade.TabelaId);
                                //ImportarFontePagadora(oportunidade.TabelaReferencia, oportunidade.TabelaId);

                                parametros = new DynamicParameters();

                                parametros.Add(name: "TabelaOrigemId", value: oportunidade.TabelaReferencia, direction: ParameterDirection.Input);
                                parametros.Add(name: "TabelaNovaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);

                                con.Execute(@"
                                INSERT INTO 
	                                SGIPA.TB_LISTA_CFG_VALORMINIMO
		                                (
			                                AUTONUM,
			                                AUTONUMSV,
			                                NBLS,
			                                VALORMINIMO,
			                                TIPO,
			                                PERCMULTA  
		                                )
			                                SELECT  
				                                SGIPA.SEQ_LISTA_CFG_VALORMINIMO.NEXTVAL, 
				                                C.AUTONUM,
				                                NBLS,
				                                VALORMINIMO,
				                                TIPO,
				                                PERCMULTA  
			                                FROM  
				                                SGIPA.TB_LISTA_CFG_VALORMINIMO A, 
				                                SGIPA.TB_LISTA_P_S_PERIODO B, 
				                                SGIPA.TB_LISTA_P_S_PERIODO C 
			                                WHERE 
				                                A.AUTONUMSV = B.AUTONUM 
			                                AND 
				                                B.LISTA = :TabelaOrigemId  
			                                AND 
				                                C.LISTA = :TabelaNovaId
			                                AND 
				                                B.SERVICO = C.SERVICO
			                                AND 
				                                B.N_PERIODO = C.N_PERIODO
			                                AND 
				                                B.QTDE_DIAS = C.QTDE_DIAS
			                                AND 
				                                B.TIPO_CARGA = C.TIPO_CARGA
			                                AND 
				                                B.BASE_CALCULO = C.BASE_CALCULO
			                                AND 
				                                B.TIPO_DOC = C.TIPO_DOC
			                                AND 
				                                B.VARIANTE_LOCAL = C.VARIANTE_LOCAL
			                                AND 
				                                B.GRUPO_ATRACACAO = C.GRUPO_ATRACACAO", parametros);

                                con.Execute(@"
                                INSERT INTO   
                                    SGIPA.TB_LISTA_P_S_FAIXASCIF_PER
                                        (
                                            AUTONUM,
                                            AUTONUMSV,
                                            VALORINICIAL,
                                            VALORFINAL,   
                                            PERCENTUAL ,MINIMO,TIPO 
                                        )  
                                            SELECT   
                                                SGIPA.SEQ_LISTA_P_S_FAIXASCIF_PER.NEXTVAL, 
                                                C.AUTONUM,
                                                VALORINICIAL,
                                                VALORFINAL,   
                                                PERCENTUAL,
                                                MINIMO,
                                                TIPO  
                                            FROM  
                                                SGIPA.TB_LISTA_P_S_FAIXASCIF_PER A, 
                                                SGIPA.TB_LISTA_P_S_PERIODO B, 
                                                SGIPA.TB_LISTA_P_S_PERIODO C 
                                            WHERE 
                                                A.AUTONUMSV = B.AUTONUM 
                                            AND 
				                                B.LISTA = :TabelaOrigemId  
			                                AND 
				                                C.LISTA = :TabelaNovaId
                                            AND 
                                                B.SERVICO = C.SERVICO
                                            AND 
                                                B.N_PERIODO = C.N_PERIODO
                                            AND 
                                                B.QTDE_DIAS = C.QTDE_DIAS
                                            AND 
                                                B.TIPO_CARGA = C.TIPO_CARGA
                                            AND 
                                                B.BASE_CALCULO = C.BASE_CALCULO
                                            AND 
                                                B.VARIANTE_LOCAL = C.VARIANTE_LOCAL
                                            AND 
                                                B.TIPO_DOC = C.TIPO_DOC
                                            AND 
                                                B.GRUPO_ATRACACAO = C.GRUPO_ATRACACAO", parametros);

                                con.Execute(@"
                                INSERT INTO   
                                    SGIPA.TB_LISTA_P_S_FAIXASCIF_FIX
                                        (
                                            AUTONUM,
                                            AUTONUMSV,
                                            VALORINICIAL,
                                            VALORFINAL,   
                                            PERCENTUAL,
                                            TIPO
                                        )  
                                            SELECT   
                                                SGIPA.SEQ_LISTA_P_S_FAIXASCIF_FIX.NEXTVAL, 
                                                C.AUTONUM,
                                                VALORINICIAL,
                                                VALORFINAL,   
                                                PERCENTUAL,   
                                                TIPO   
                                            FROM   
                                                SGIPA.TB_LISTA_P_S_FAIXASCIF_FIX A, 
                                                SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS B, 
                                                SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS C 
                                            WHERE 
                                                A.AUTONUMSV = B.AUTONUM 
                                            AND 
				                                B.LISTA = :TabelaOrigemId  
			                                AND 
				                                C.LISTA = :TabelaNovaId
                                            AND 
                                                B.SERVICO = C.SERVICO
                                            AND 
                                                B.TIPO_CARGA = C.TIPO_CARGA
                                            AND 
                                                B.BASE_CALCULO = C.BASE_CALCULO
                                            AND 
                                                B.VARIANTE_LOCAL = C.VARIANTE_LOCAL
                                            AND 
                                                B.TIPO_DOC = C.TIPO_DOC
                                            AND 
                                                B.TIPO_OPER = C.TIPO_OPER
                                            AND 
                                                B.VARIANTE_LOCAL = C.VARIANTE_LOCAL
                                            AND 
                                                B.GRUPO_ATRACACAO = C.GRUPO_ATRACACAO", parametros);

                                con.Execute(@"
                                INSERT INTO 
	                                SGIPA.TB_LISTA_FAIXA_PESO
			                                SELECT
				                                SGIPA.SEQ_LISTA_FAIXA_PESO.NEXTVAL, 
				                                C.AUTONUM, 
				                                PESOINICIAL, 
				                                PESOFINAL,    
				                                PRECO,
                                                TIPO,
                                                TIPO_VALOR
			                                FROM   
				                                SGIPA.TB_LISTA_FAIXA_PESO A,  
				                                SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS B,  
				                                SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS C  
			                                WHERE 
				                                A.AUTONUMSV = B.AUTONUM 
			                                AND 
				                                B.LISTA = :TabelaOrigemId  
			                                AND 
				                                C.LISTA = :TabelaNovaId
			                                AND
				                                B.SERVICO = C.SERVICO 
			                                AND 
				                                NVL(B.TIPO_CARGA,' ')  = NVL(C.TIPO_CARGA,' ')
			                                AND 
				                                NVL(B.BASE_CALCULO,' ') = NVL(C.BASE_CALCULO,'') 
			                                AND 
				                                NVL(B.VARIANTE_LOCAL,' ') = NVL(C.VARIANTE_LOCAL,' ') 
			                                AND 
				                                B.TIPO_DOC = C.TIPO_DOC 
			                                AND 
				                                NVL(B.LOCAL_ATRACACAO,' ') = NVL(C.LOCAL_ATRACACAO,' ') 
			                                AND 
				                                NVL(B.GRUPO_ATRACACAO,0) = NVL(C.GRUPO_ATRACACAO,0) 
			                                AND 
				                                NVL(B.PORTO_HUBPORT,0) = NVL(C.PORTO_HUBPORT,0) 
			                                AND 
				                                NVL(B.TIPO_OPER,' ') = NVL(C.TIPO_OPER,'')", parametros);

                                con.Execute(@"
                                INSERT INTO   
                                    SGIPA.TB_LISTA_FAIXA_VOLUME                                 
                                            SELECT   
                                                SGIPA.SEQ_LISTA_FAIXA_VOLUME.NEXTVAL, 
                                                C.AUTONUM, 
                                                VOLUMEINICIAL, 
                                                VOLUMEFINAL,    
                                                PRECO,
                                                TIPO,
                                                TIPO_VALOR
                                            FROM   
                                                SGIPA.TB_LISTA_FAIXA_VOLUME  A,  
                                                SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS B,  
                                                SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS  C  
                                            WHERE 
                                                A.AUTONUMSV=B.AUTONUM 
                                            AND 
				                                B.LISTA = :TabelaOrigemId  
			                                AND 
				                                C.LISTA = :TabelaNovaId
                                            AND 
                                                B.SERVICO= C.SERVICO 
                                            AND 
                                                NVL(B.TIPO_CARGA,' ')= NVL(C.TIPO_CARGA,' ')
                                            AND
                                                NVL(B.BASE_CALCULO,' ')= NVL(C.BASE_CALCULO,'') 
                                            AND 
                                                NVL(B.VARIANTE_LOCAL,' ')= NVL(C.VARIANTE_LOCAL,' ') 
                                            AND 
                                                B.TIPO_DOC = C.TIPO_DOC 
                                            AND 
                                                NVL(B.LOCAL_ATRACACAO,' ')=NVL(C.LOCAL_ATRACACAO,' ') 
                                            AND 
                                                NVL(B.GRUPO_ATRACACAO,0)= NVL(C.GRUPO_ATRACACAO,0) 
                                            AND 
                                                NVL(B.PORTO_HUBPORT,0)= NVL(C.PORTO_HUBPORT,0) 
                                            AND 
                                                NVL(B.TIPO_OPER,' ') = NVL(C.TIPO_OPER, ' ') ", parametros);

                                // Fonte Pagadora
                                // 30/07/2020 - Jéssica pediu para desabilitar esta integração temporariamente
                                //IntegrarFontePagadora(oportunidade.TabelaReferencia, oportunidade.TabelaId, oportunidade.Id);

                            }

                            return oportunidade.TabelaId;
                        }
                    }
               
                catch (Exception ex)
                {
                     
                }
                return 0;
            }
            }
        }

        public void IntegrarExcessaoImpostos(Oportunidade oportunidade)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: oportunidade.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidade.Id, direction: ParameterDirection.Input);

                con.Execute(@"
                    INSERT INTO
                        SGIPA.TB_LISTAS_EXCESSAO_IMPOSTOS
                            (
                                AUTONUM,
                                Lista, 
                                ServicoId, 
                                ImpostoId, 
                                Isento, 
                                Percentual 
                            )
                    SELECT 
                        SGIPA.SEQ_LISTAS_EXCESSAO_IMPOSTOS.NEXTVAL As AUTONUM,
                        :TabelaId As Lista, 
                        ServicoId, 
                        ImpostoId, 
                        Isento, 
                        Percentual 
                    FROM 
                        (
                            SELECT 
                                C.ServicoFaturamentoId As ServicoId, 1 As ImpostoId, ISS As Isento, ValorISS as Percentual 
                            FROM 
                                CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS A
                            INNER JOIN
                                CRM.TB_CRM_SERVICOS B ON A.ServicoId = B.Id
                            LEFT JOIN
                                CRM.TB_CRM_SERVICO_IPA C ON B.Id = C.ServicoId              
                            WHERE 
                                A.MODELOID = :ModeloId AND A.OportunidadeId = :OportunidadeId
                        UNION ALL
     
                            SELECT 
                                C.ServicoFaturamentoId As ServicoId, 2 As ImpostoId, COFINS As Isento, ValorCOFINS as Percentual
                            FROM 
                                CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS A
                            INNER JOIN
                                CRM.TB_CRM_SERVICOS B ON A.ServicoId = B.Id
                            LEFT JOIN
                                CRM.TB_CRM_SERVICO_IPA C ON B.Id = C.ServicoId              
                            WHERE 
                                A.MODELOID = :ModeloId AND A.OportunidadeId = :OportunidadeId
                        UNION ALL      
                            SELECT 
                                C.ServicoFaturamentoId As ServicoId, 3 As ImpostoId, PIS As Isento, ValorPIS as Percentual
                            FROM 
                                CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS A
                            INNER JOIN
                                CRM.TB_CRM_SERVICOS B ON A.ServicoId = B.Id
                            LEFT JOIN
                                CRM.TB_CRM_SERVICO_IPA C ON B.Id = C.ServicoId              
                            WHERE 
                                A.MODELOID = :ModeloId AND A.OportunidadeId = :OportunidadeId
                    ) WHERE (Isento = 1 OR Percentual > 0) ", parametros);
            }

        }

        public void IntegrarFontePagadora(int tabelaOrigemId, int novaTabelaId, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        var sequenciaFontePagadora = con.Query<int>("SELECT SGIPA.SEQ_DADOS_FATURAMENTO_IPA.NEXTVAL FROM DUAL").Single();

                        var parametros = new DynamicParameters();

                        parametros.Add(name: "TabelaOrigemId", value: tabelaOrigemId, direction: ParameterDirection.Input);
                        parametros.Add(name: "TabelaNovaId", value: novaTabelaId, direction: ParameterDirection.Input);
                        parametros.Add(name: "SequenciaFonte", value: sequenciaFontePagadora, direction: ParameterDirection.Input);
                        parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                        var FormaPag = con.Query($@"SELECT FORMA_PAGAMENTO FROM SGIPA.TB_LISTAS_PRECOS WHERE FORMA_PAGAMENTO=3 AND AUTONUM = :TabelaOrigemId", parametros).Any();

                        var ContaForma = con.Query($@"SELECT AUTONUM_LISTA FROM SGIPA.TB_DADOS_FATURAMENTO_IPA WHERE AUTONUM_LISTA = :TabelaOrigemId", parametros).Any();

                        if (FormaPag == true)
                        {
                            if (ContaForma == true)
                            {
                                con.Execute("DELETE FROM SGIPA.TB_DADOS_FAT_IPA_COND_PG_DIAS WHERE AUTONUM_FONTE_PAGADORA IN (SELECT AUTONUM FROM SGIPA.TB_DADOS_FATURAMENTO_IPA WHERE OPORTUNIDADE_ID = :OportunidadeId)", parametros, transaction);
                                con.Execute("DELETE FROM SGIPA.TB_DADOS_FAT_IPA_DIAS_SEMANA WHERE AUTONUM_FONTE_PAGADORA IN (SELECT AUTONUM FROM SGIPA.TB_DADOS_FATURAMENTO_IPA WHERE OPORTUNIDADE_ID = :OportunidadeId)", parametros, transaction);
                                con.Execute("DELETE FROM SGIPA.TB_DADOS_FATURAMENTO_IPA WHERE OPORTUNIDADE_ID = :OportunidadeId", parametros, transaction);

                                con.Execute($@"
                                    INSERT INTO SGIPA.TB_DADOS_FATURAMENTO_IPA 
                         	            (
                                            AUTONUM,  
                         	                AUTONUM_LISTA,  
                         	                AUTONUM_CLIENTE_NOTA,  
                         	                AUTONUM_CLIENTE_ENVIO_NOTA,  
                         	                AUTONUM_CLIENTE_PAGAMENTO,  
                         	                AUTONUM_DIA_FATURAMENTO,  
                         	                DIA,  
                         	                CORTE,  
                                            EMAIL,
                         	                AUTONUM_FORMA_PAGAMENTO,
                                            FLAG_ENTREGA_ELETRONICA, 
                                            FLAG_ENTREGA_MANUAL, 
                                            FLAG_ENVIO_CORREIO_COMUM, 
                                            FLAG_ENVIO_CORREIO_SEDEX,
                                            FLAG_ULTIMO_DIA_DO_MES_CORTE, 
                                            FLAG_VENCIMENTO_DIA_UTIL, 
                                            FLAG_ULTIMO_DIA_DA_SEMANA, 
                                            FLAG_ULTIMO_DIA_DO_MES, 
                                            FLAG_ULTIMO_DIA_DO_MES_VCTO,
                                            OPORTUNIDADE_ID
                                        )  
                                        SELECT  
                                            :SequenciaFonte,  
                         	                :TabelaNovaId,
                         	                AUTONUM_CLIENTE_NOTA,  
                         	                AUTONUM_CLIENTE_ENVIO_NOTA,  
                         	                AUTONUM_CLIENTE_PAGAMENTO,  
                         	                AUTONUM_DIA_FATURAMENTO,  
                         	                DIA,  
                         	                CORTE,
                                            EMAIL,
                         	                AUTONUM_FORMA_PAGAMENTO,
                                            FLAG_ENTREGA_ELETRONICA, 
                                            FLAG_ENTREGA_MANUAL, 
                                            FLAG_ENVIO_CORREIO_COMUM, 
                                            FLAG_ENVIO_CORREIO_SEDEX,
                                            FLAG_ULTIMO_DIA_DO_MES_CORTE, 
                                            FLAG_VENCIMENTO_DIA_UTIL, 
                                            FLAG_ULTIMO_DIA_DA_SEMANA, 
                                            FLAG_ULTIMO_DIA_DO_MES, 
                                            FLAG_ULTIMO_DIA_DO_MES_VCTO,
                                            :OportunidadeId
                                        FROM  
                                            SGIPA.TB_DADOS_FATURAMENTO_IPA  
                                        WHERE  
                                            AUTONUM_LISTA = :TabelaOrigemId", parametros, transaction);

                                con.Execute(@"
                                    INSERT INTO 
                                        SGIPA.TB_DADOS_FAT_IPA_DIAS_SEMANA 
                                            (
                                                AUTONUM,
                                                AUTONUM_FONTE_PAGADORA,
                                                DIA
                                            )
                                        SELECT
                                            SGIPA.SEQ_DADOS_FAT_IPA_DIAS_SEMANA.NEXTVAL,
                                            :SequenciaFonte,
                                            A.DIA
                                        FROM
                                            SGIPA.TB_DADOS_FAT_IPA_DIAS_SEMANA A
                                        INNER JOIN
                                            SGIPA.TB_DADOS_FATURAMENTO_IPA B ON A.AUTONUM_FONTE_PAGADORA = B.AUTONUM
                                        WHERE
                                            B.AUTONUM_LISTA = :TabelaOrigemId", parametros, transaction);

                                con.Execute(@"
                                    INSERT INTO 
                                        SGIPA.TB_DADOS_FAT_IPA_COND_PG_DIAS 
                                            (
                                                AUTONUM,
                                                AUTONUM_FONTE_PAGADORA,
                                                DIA
                                            )
                                        SELECT
                                            SGIPA.SEQ_DADOS_FAT_IPA_COND_PG_DIAS.NEXTVAL,
                                            :SequenciaFonte,
                                            A.DIA
                                        FROM
                                            SGIPA.TB_DADOS_FAT_IPA_COND_PG_DIAS A
                                        INNER JOIN
                                            SGIPA.TB_DADOS_FATURAMENTO_IPA B ON A.AUTONUM_FONTE_PAGADORA = B.AUTONUM
                                        WHERE
                                            B.AUTONUM_LISTA = :TabelaOrigemId", parametros, transaction);

                                con.Execute(@"
                                    INSERT INTO 
                                        SGIPA.TB_DADOS_FAT_IPA_DIAS 
                                            (
                                                AUTONUM,
                                                AUTONUM_FONTE_PAGADORA,
                                                DIA
                                            )
                                        SELECT
                                            SGIPA.SEQ_DADOS_FAT_IPA_DIAS_FAT.NEXTVAL,
                                            :SequenciaFonte,
                                            A.DIA
                                        FROM
                                            SGIPA.TB_DADOS_FAT_IPA_DIAS A
                                        INNER JOIN
                                            SGIPA.TB_DADOS_FATURAMENTO_IPA B ON A.AUTONUM_FONTE_PAGADORA = B.AUTONUM
                                        WHERE
                                            B.AUTONUM_LISTA = :TabelaOrigemId", parametros, transaction);

                                con.Execute(@"
                                    INSERT INTO 
                                        SGIPA.TB_DADOS_FAT_IPA_DIAS_PGTO 
                                            (
                                                AUTONUM,
                                                AUTONUM_FONTE_PAGADORA,
                                                DIA
                                            )
                                        SELECT
                                            SGIPA.SEQ_DADOS_FAT_IPA_DIAS_PGTO.NEXTVAL,
                                            :SequenciaFonte,
                                            A.DIA
                                        FROM
                                            SGIPA.TB_DADOS_FAT_IPA_DIAS_PGTO A
                                        INNER JOIN
                                            SGIPA.TB_DADOS_FATURAMENTO_IPA B ON A.AUTONUM_FONTE_PAGADORA = B.AUTONUM
                                        WHERE
                                            B.AUTONUM_LISTA = :TabelaOrigemId", parametros, transaction);


                                var grupos = con.Query<FonteGrupoDTO>(@"
                                    SELECT
                                        A.AUTONUM,
                                        :TabelaNovaId As TabelaId,
                                        A.AUTONUM_CLIENTE_NOTA,
                                        A.AUTONUM_CLIENTE_ENVIO_NOTA,
                                        A.AUTONUM_CLIENTE_PAGAMENTO,
                                        D.AUTONUM As AUTONUM_GRUPO_LISTA,
                                        A.CORTE,
                                        A.DIA,
                                        A.AUTONUM_FORMA_PAGAMENTO,
                                        A.EMAIL,
                                        A.FLAG_ENTREGA_ELETRONICA,
                                        A.FLAG_ENTREGA_MANUAL,
                                        A.FLAG_ENVIO_CORREIO_COMUM,
                                        A.FLAG_ENVIO_CORREIO_SEDEX,
                                        A.FLAG_ULTIMO_DIA_DO_MES_CORTE,
                                        A.FLAG_ULTIMO_DIA_DO_MES_VCTO,
                                        A.FLAG_VENCIMENTO_DIA_UTIL
                                    FROM  
                                        SGIPA.TB_DADOS_FATURAMENTO_IPA_GRP  A
                                    INNER JOIN
                                        SGIPA.TB_TP_GRUPOS B ON A.AUTONUM_GRUPO_LISTA = B.AUTONUM
                                    INNER JOIN
                                        SGIPA.TB_CAD_PARCEIROS C ON B.AUTONUM_PARCEIRO = C.AUTONUM
                                    INNER JOIN
                                        SGIPA.TB_TP_GRUPOS D ON C.AUTONUM = D.AUTONUM_PARCEIRO AND D.AUTONUMLISTA = :TabelaNovaId
                                    WHERE  
                                        A.AUTONUM_LISTA = :TabelaOrigemId", parametros);

                                con.Execute("DELETE FROM SGIPA.TB_DADOS_FAT_GRU_DIAS_SEMANA WHERE AUTONUM_FONTE_PAGADORA IN (SELECT AUTONUM FROM SGIPA.TB_DADOS_FATURAMENTO_IPA_GRP WHERE OPORTUNIDADE_ID = :OportunidadeId)", parametros, transaction);
                                con.Execute("DELETE FROM SGIPA.TB_DADOS_FAT_GRU_COND_PG_DIAS WHERE AUTONUM_FONTE_PAGADORA IN (SELECT AUTONUM FROM SGIPA.TB_DADOS_FATURAMENTO_IPA_GRP WHERE OPORTUNIDADE_ID = :OportunidadeId)", parametros, transaction);
                                con.Execute("DELETE FROM SGIPA.TB_DADOS_FATURAMENTO_IPA_GRP WHERE OPORTUNIDADE_ID = :OportunidadeId", parametros, transaction);

                                foreach (var grupo in grupos)
                                {
                                    var sequenciaGrupo = con.Query<int>("SELECT SGIPA.SEQ_DADOS_FATURAMENTO_IPA_GRP.NEXTVAL FROM DUAL").Single();

                                    parametros.Add(name: "SequenciaGrupo", value: sequenciaGrupo, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpAUTONUM", value: grupo.AUTONUM, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpTabelaId", value: grupo.TabelaId, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpAUTONUM_CLIENTE_NOTA", value: grupo.AUTONUM_CLIENTE_NOTA, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpCLIENTE_ENVIO_NOTA", value: grupo.AUTONUM_CLIENTE_ENVIO_NOTA, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpCLIENTE_PAGAMENTO", value: grupo.AUTONUM_CLIENTE_PAGAMENTO, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpAUTONUM_GRUPO_LISTA", value: grupo.AUTONUM_GRUPO_LISTA, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpCORTE", value: grupo.CORTE, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpDIA", value: grupo.DIA, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpFORMA_PAGAMENTO", value: grupo.AUTONUM_FORMA_PAGAMENTO, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpEMAIL", value: grupo.EMAIL, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpFLAG_ENTREGA_ELETRONICA", value: grupo.FLAG_ENTREGA_ELETRONICA, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpFLAG_ENTREGA_MANUAL", value: grupo.FLAG_ENTREGA_MANUAL, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpFLAG_ENVIO_CORREIO_COMUM", value: grupo.FLAG_ENVIO_CORREIO_COMUM, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpFLAG_ENVIO_CORREIO_SEDEX", value: grupo.FLAG_ENVIO_CORREIO_SEDEX, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpULTIMO_DIA_DO_MES_CORTE", value: grupo.FLAG_ULTIMO_DIA_DO_MES_CORTE, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpULTIMO_DIA_DO_MES_VCTO", value: grupo.FLAG_ULTIMO_DIA_DO_MES_VCTO, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpFLAG_VENCIMENTO_DIA_UTIL", value: grupo.FLAG_VENCIMENTO_DIA_UTIL, direction: ParameterDirection.Input);
                                    parametros.Add(name: "GrpOportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                                    var grupoId = con.Execute(@"
                                        INSERT INTO
                                            SGIPA.TB_DADOS_FATURAMENTO_IPA_GRP 
                                                (
                                                    AUTONUM,
                                                    AUTONUM_LISTA,
                                                    AUTONUM_CLIENTE_NOTA,
                                                    AUTONUM_CLIENTE_ENVIO_NOTA,
                                                    AUTONUM_CLIENTE_PAGAMENTO,
                                                    AUTONUM_GRUPO_LISTA,
                                                    CORTE,
                                                    DIA,
                                                    AUTONUM_FORMA_PAGAMENTO,
                                                    EMAIL,
                                                    FLAG_ENTREGA_ELETRONICA,
                                                    FLAG_ENTREGA_MANUAL,
                                                    FLAG_ENVIO_CORREIO_COMUM,
                                                    FLAG_ENVIO_CORREIO_SEDEX,
                                                    FLAG_ULTIMO_DIA_DO_MES_CORTE,
                                                    FLAG_ULTIMO_DIA_DO_MES_VCTO,
                                                    FLAG_VENCIMENTO_DIA_UTIL,
                                                    OPORTUNIDADE_ID
                                                )
                                            SELECT
                                                :SequenciaGrupo,
                                                :GrpTabelaId,
                                                :GrpAUTONUM_CLIENTE_NOTA,
                                                :GrpCLIENTE_ENVIO_NOTA,
                                                :GrpCLIENTE_PAGAMENTO,
                                                :GrpAUTONUM_GRUPO_LISTA,
                                                :GrpCORTE,
                                                :GrpDIA,
                                                :GrpFORMA_PAGAMENTO,
                                                :GrpEMAIL,
                                                :GrpFLAG_ENTREGA_ELETRONICA,
                                                :GrpFLAG_ENTREGA_MANUAL,
                                                :GrpFLAG_ENVIO_CORREIO_COMUM,
                                                :GrpFLAG_ENVIO_CORREIO_SEDEX,
                                                :GrpULTIMO_DIA_DO_MES_CORTE,
                                                :GrpULTIMO_DIA_DO_MES_VCTO,
                                                :GrpFLAG_VENCIMENTO_DIA_UTIL,
                                                :GrpOportunidadeId
                                            FROM  
                                                SGIPA.TB_DADOS_FATURAMENTO_IPA_GRP  
                                            WHERE  
                                                AUTONUM = :GrpAUTONUM", parametros, transaction);

                                    con.Execute(@"
                                            INSERT INTO 
                                                SGIPA.TB_DADOS_FAT_GRU_DIAS_SEMANA 
                                                    (
                                                        AUTONUM,
                                                        AUTONUM_FONTE_PAGADORA,
                                                        DIA
                                                    )
                                                SELECT
                                                    SGIPA.SEQ_DADOS_FAT_GRU_DIAS_SEMANA.NEXTVAL,
                                                    :SequenciaGrupo,
                                                    A.DIA
                                                FROM
                                                    SGIPA.TB_DADOS_FAT_GRU_DIAS_SEMANA A
                                                INNER JOIN
                                                    SGIPA.TB_DADOS_FATURAMENTO_IPA_GRP B ON A.AUTONUM_FONTE_PAGADORA = B.AUTONUM
                                                WHERE
                                                    A.AUTONUM_FONTE_PAGADORA = :GrpAUTONUM", parametros, transaction);

                                    con.Execute(@"
                                            INSERT INTO 
                                                SGIPA.TB_DADOS_FAT_GRU_COND_PG_DIAS 
                                                    (
                                                        AUTONUM,
                                                        AUTONUM_FONTE_PAGADORA,
                                                        DIA
                                                    )
                                                SELECT
                                                    SGIPA.SEQ_DADOS_FAT_GRU_COND_PG_DIAS.NEXTVAL,
                                                    :SequenciaGrupo,
                                                    A.DIA
                                                FROM
                                                    SGIPA.TB_DADOS_FAT_GRU_COND_PG_DIAS A
                                                INNER JOIN
                                                    SGIPA.TB_DADOS_FATURAMENTO_IPA_GRP B ON A.AUTONUM_FONTE_PAGADORA = B.AUTONUM
                                                WHERE
                                                    A.AUTONUM_FONTE_PAGADORA = :GrpAUTONUM", parametros, transaction);

                                    con.Execute(@"
                                            INSERT INTO 
                                                SGIPA.TB_DADOS_FAT_GRU_DIAS 
                                                    (
                                                        AUTONUM,
                                                        AUTONUM_FONTE_PAGADORA,
                                                        DIA
                                                    )
                                                SELECT
                                                    SGIPA.SEQ_DADOS_FAT_GRU_DIAS.NEXTVAL,
                                                    :SequenciaGrupo,
                                                    A.DIA
                                                FROM
                                                    SGIPA.TB_DADOS_FAT_GRU_DIAS A
                                                INNER JOIN
                                                    SGIPA.TB_DADOS_FATURAMENTO_IPA_GRP B ON A.AUTONUM_FONTE_PAGADORA = B.AUTONUM
                                                WHERE
                                                    A.AUTONUM_FONTE_PAGADORA = :GrpAUTONUM", parametros, transaction);

                                    con.Execute(@"
                                            INSERT INTO 
                                                SGIPA.TB_DADOS_FAT_GRU_DIAS_PGTO 
                                                    (
                                                        AUTONUM,
                                                        AUTONUM_FONTE_PAGADORA,
                                                        DIA
                                                    )
                                                SELECT
                                                    SGIPA.SEQ_DADOS_FAT_IPA_DIAS_PGTO.NEXTVAL,
                                                    :SequenciaGrupo,
                                                    A.DIA
                                                FROM
                                                    SGIPA.TB_DADOS_FAT_GRU_DIAS_PGTO A
                                                INNER JOIN
                                                    SGIPA.TB_DADOS_FATURAMENTO_IPA_GRP B ON A.AUTONUM_FONTE_PAGADORA = B.AUTONUM
                                                WHERE
                                                    A.AUTONUM_FONTE_PAGADORA = :GrpAUTONUM", parametros, transaction);
                                }

                                transaction.Commit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        private static string CorrigeNomeTabela(string descricao)
        {
            if (descricao.Length > 150)
                return descricao.Substring(0, 150);

            return descricao;
        }

        private static string CriarNomeTabela(Oportunidade oportunidade, string importadorCnpj)
        {
            if (oportunidade.TabelaId == 0)
                throw new Exception("Tabela ID não informado");

            var tabelaBusca = ObterTabelaPorId(oportunidade.TabelaId);

            if (tabelaBusca == null)
                throw new Exception($"Tabela {oportunidade.TabelaId} não encontrada");

            var importador = tabelaBusca.Importador.ObterPrimeiroNome();
            var despachante = tabelaBusca.Despachante.ObterPrimeiroNome();
            var nvocc = tabelaBusca.NVOCC.ObterPrimeiroNome();
            var coloader = tabelaBusca.Coloader.ObterPrimeiroNome();
            var cocoloader = tabelaBusca.CoColoader.ObterPrimeiroNome();
            var cocoloader2 = tabelaBusca.CoColoader2.ObterPrimeiroNome();

            var descricaoColoader = string.Empty;

            if (!string.IsNullOrEmpty(coloader) && !string.IsNullOrEmpty(cocoloader) && !string.IsNullOrEmpty(cocoloader2))
            {
                descricaoColoader += "{";
            }

            if (!string.IsNullOrEmpty(coloader) && !string.IsNullOrEmpty(cocoloader))
            {
                descricaoColoader += "[";
            }

            if (!string.IsNullOrWhiteSpace(coloader))
                descricaoColoader += $"({coloader})";

            if (!string.IsNullOrWhiteSpace(cocoloader))
                descricaoColoader += $" {cocoloader}";

            if (descricaoColoader.Contains("["))
                descricaoColoader += "]";

            if (!string.IsNullOrWhiteSpace(cocoloader2))
                descricaoColoader += $" {cocoloader2}";

            if (descricaoColoader.Contains("{"))
                descricaoColoader += "}";

            if (oportunidade.SegmentoId == Segmento.IMPORTADOR && oportunidade.ContaDocumento == importadorCnpj)
            {
                if (!string.IsNullOrWhiteSpace(despachante))
                    tabelaBusca.Descricao += $" |{despachante}|";

                if (!string.IsNullOrWhiteSpace(descricaoColoader))
                    tabelaBusca.Descricao += $" {descricaoColoader}";

                if (!string.IsNullOrWhiteSpace(importador))
                    tabelaBusca.Descricao += $" {importador} ";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(nvocc))
                    tabelaBusca.Descricao = $" {nvocc}";

                if (!string.IsNullOrWhiteSpace(despachante))
                    tabelaBusca.Descricao += $" |{despachante}|";

                if (!string.IsNullOrWhiteSpace(descricaoColoader))
                    tabelaBusca.Descricao += $" {descricaoColoader}";

                if (!string.IsNullOrWhiteSpace(importador))
                    tabelaBusca.Descricao += $" X {importador} ";
            }

            tabelaBusca.Descricao = tabelaBusca.Descricao.Trim();

            if (!string.IsNullOrWhiteSpace(oportunidade.Identificacao))
            {
                if (oportunidade.Acordo)
                {
                    tabelaBusca.Descricao += $" - AC {oportunidade.Identificacao} ";
                }
                else
                {
                    tabelaBusca.Descricao += $" - PROP {oportunidade.Identificacao} ";
                }
            }

            return CorrigeNomeTabela(tabelaBusca.Descricao.Trim());
        }

        public static string ObterCondicoesIniciaisProposta(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<string>(@"SELECT CondicoesIniciais FROM CRM.TB_CRM_OPORTUNIDADE_LAYOUT WHERE CondicoesIniciais LIKE '%l:SubClientes [Importador]%' AND OportunidadeId = :OportunidadeId", parametros).FirstOrDefault();
            }
        }

        public static bool ImportadorIgualIndicadorDoAcordo(Oportunidade oportunidade)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var condicoesIniciais = ObterCondicoesIniciaisProposta(oportunidade.Id);

                if (condicoesIniciais == string.Empty)
                    return false;

                MatchCollection variaveis = Regex.Matches(condicoesIniciais, @"(?<=\{)[^}]*(?=\})");

                foreach (Match variavel in variaveis)
                {
                    foreach (Capture capture in variavel.Captures)
                    {
                        var variavelPura = StringHelpers.LimparTagsHtml(capture.Value);

                        if (!Regex.IsMatch(variavelPura, @"^[a-z]:[\s\S]"))
                            return false;

                        var conjunto = variavelPura.Split(':');
                        var campo = conjunto[1].ToString();

                        if (campo.Contains("s:Conta"))
                        {
                            var parametros = new DynamicParameters();
                            parametros.Add(name: "ContaId", value: oportunidade.ContaId, direction: ParameterDirection.Input);

                            var contaBusca = con.Query<Conta>(@"
                                SELECT
                                    Id,                                    
                                    Descricao,
                                    Documento,
                                    NomeFantasia,                      
                                    Segmento                        
                                FROM
                                    CRM.TB_CRM_CONTAS
                                WHERE
                                    Id = :ContaId", parametros).FirstOrDefault();

                            if (contaBusca != null)
                            {
                                if (contaBusca.Segmento == Segmento.IMPORTADOR)
                                {

                                }
                            }
                        }

                        if (campo.Contains("SubClientes"))
                        {
                            var segmento = Regex.Match(campo, @"\[(.*?)\]");

                            if (segmento.Success == false)
                                return false;

                            var nomeSegmento = segmento.Value
                                .ToUpper()
                                .Replace("[", string.Empty)
                                .Replace("]", string.Empty);

                            if (!Enum.TryParse(nomeSegmento, out SegmentoSubCliente segmentoSubCliente))
                                return false;

                            if (segmentoSubCliente != SegmentoSubCliente.IMPORTADOR)
                                return false;

                            var parametros = new DynamicParameters();

                            parametros.Add(name: "OportunidadeId", value: oportunidade.Id, direction: ParameterDirection.Input);
                            parametros.Add(name: "Segmento", value: SegmentoSubCliente.IMPORTADOR, direction: ParameterDirection.Input);
                            parametros.Add(name: "ImportadorId", value: oportunidade.ContaId, direction: ParameterDirection.Input);

                            return con.Query<ClienteProposta>(@"
                                SELECT
                                    A.Id,
                                    B.ContaId,
                                    C.Login As CriadoPor,
                                    A.Descricao,
                                    A.Documento,
                                    A.NomeFantasia,                      
                                    B.Segmento                        
                                FROM
                                    CRM.TB_CRM_CONTAS A 
                                INNER JOIN
                                    CRM.TB_CRM_OPORTUNIDADE_CLIENTES B ON A.Id = B.ContaId
                                INNER JOIN
                                    CRM.TB_CRM_USUARIOS C ON A.CriadoPor = C.Id
                                LEFT JOIN
                                    CRM.TB_CRM_CONTAS D ON B.ContaId = D.Id 
                                WHERE
                                    B.OportunidadeId = :OportunidadeId
                                AND 
                                    B.Segmento = :Segmento
                                AND
                                    A.Descricao = D.Descricao", parametros).Any();

                        }
                    }
                }
            }

            return false;
        }

        private static void AtualizarTabelaOportunidade(int tabelaId, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET TabelaId = :TabelaId WHERE Id = :OportunidadeId", parametros);
            }
        }

        public void AtualizarUsuarioCriacao(int tabelaId, int usuarioCrmId)
        {
            var parametros = new DynamicParameters();
            parametros.Add(name: "UsuarioCrmId", value: usuarioCrmId, direction: ParameterDirection.Input);

            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var usuarioChronos = con.Query<int>(@"SELECT AUTONUM FROM SGIPA.TB_CAD_USUARIOS WHERE REPLACE(REPLACE(CPF,'.',''),'-','') = (SELECT REPLACE(REPLACE(CPF,'.',''),'-','') FROM CRM.TB_CRM_USUARIOS WHERE ID = :UsuarioCrmId)", parametros).FirstOrDefault();

                parametros = new DynamicParameters();

                parametros.Add(name: "UsuarioChronos", value: usuarioChronos, direction: ParameterDirection.Input);
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE SGIPA.TB_LISTAS_PRECOS SET USUARIO_CAD = :UsuarioChronos WHERE AUTONUM = :TabelaId", parametros);
            }
        }

        public void AtualizarFreeTime(int tabelaId, Oportunidade oportunidade)
        {
            if (oportunidade.ModeloId == 11 && oportunidade.TabelaReferencia > 0)
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaReferencia", value: oportunidade.TabelaReferencia, direction: ParameterDirection.Input);

                    con.Execute(@"UPDATE SGIPA.TB_LISTAS_PRECOS SET DIAS_APOS_GR = (SELECT DIAS_APOS_GR As FreeTime FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :TabelaReferencia) WHERE AUTONUM = :TabelaId", parametros);
                }
            }
        }

        private static Tabela ObterTabelaPorId(int tabelaId)
        {
            var parametros = new DynamicParameters();
            parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                return con.Query<Tabela>(@"
                    SELECT 
                        B.RAZAO As Importador,
                        C.RAZAO As Despachante,
                        D.RAZAO As NVOCC,
                        E.RAZAO As Coloader,
                        F.RAZAO As CoColoader,
                        G.RAZAO As CoColoader2,
                        A.DATA_INICIO As DataInicio,
                        A.FINAL_VALIDADE As DataFinalValidade
                    FROM
                        SGIPA.TB_LISTAS_PRECOS A
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS B ON A.Importador = B.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS C ON A.Despachante = C.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS D ON A.Captador = D.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS E ON A.Coloader = E.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS F ON a.CoColoader = F.AUTONUM    
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS G ON A.CoColoader2 = G.AUTONUM
                    WHERE
                        A.AUTONUM = :TabelaId", parametros).FirstOrDefault();
            }
        }

        private static void AtualizarDescricaoTabela(int tabelaId, string descricao)
        {
            var parametros = new DynamicParameters();

            parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
            parametros.Add(name: "Descricao", value: descricao, direction: ParameterDirection.Input);

            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                con.Execute(@"UPDATE SGIPA.TB_LISTAS_PRECOS SET DESCR = :Descricao WHERE AUTONUM = :TabelaId", parametros);
            }
        }

        private static void AtualizarObservacaoTabela(int novaTabelaId, int? oportunidadeRevisaoId, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                string mensagem = $"TABELA CADASTRADA EM {DateTime.Now.ToString("dd/MM/yyyy")}";

                if (oportunidadeRevisaoId.HasValue)
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "RevisaoId", value: oportunidadeRevisaoId.Value, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    var oportunidadeRevisada = con.Query<Oportunidade>("SELECT TabelaId FROM CRM.TB_CRM_OPORTUNIDADES WHERE Id = :RevisaoId", parametros).FirstOrDefault();
                    var oportunidadeNova = con.Query<Oportunidade>("SELECT Acordo FROM CRM.TB_CRM_OPORTUNIDADES WHERE Id = :OportunidadeId", parametros).FirstOrDefault();

                    if (oportunidadeRevisada != null)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "TabelaRevisadaId", value: oportunidadeRevisada.TabelaId, direction: ParameterDirection.Input);
                        parametros.Add(name: "NovaTabelaId", value: novaTabelaId, direction: ParameterDirection.Input);
                        parametros.Add(name: "ObservacaoTabelaRevisada", value: $" Novo ID: {novaTabelaId}", direction: ParameterDirection.Input);
                        parametros.Add(name: "ObservacaoNovaTabela", value: $"{mensagem} ID Anterior: {oportunidadeRevisada.TabelaId}", direction: ParameterDirection.Input);

                        con.Execute($@"UPDATE SGIPA.TB_LISTAS_PRECOS SET OBS = OBS || :ObservacaoTabelaRevisada WHERE AUTONUM = :TabelaRevisadaId", parametros);
                        con.Execute($@"UPDATE SGIPA.TB_LISTAS_PRECOS SET OBS = :ObservacaoNovaTabela WHERE AUTONUM = :NovaTabelaId", parametros);

                        var tabelaRevisada = ObterTabelaPorId(novaTabelaId);
                        var tabelaAntiga = ObterTabelaPorId(oportunidadeRevisada.TabelaId);

                        if (tabelaRevisada.DataInicio <= tabelaAntiga.DataFinalValidade)
                            tabelaAntiga.DataFinalValidade = tabelaRevisada?.DataInicio?.AddDays(-1);

                        if (tabelaRevisada.DataFinalValidade == null)
                            tabelaAntiga.DataFinalValidade = tabelaRevisada?.DataInicio?.AddDays(-1);

                        if (tabelaAntiga.DataFinalValidade?.Hour == 0 && tabelaAntiga.DataFinalValidade?.Minute == 0)
                            tabelaAntiga.DataFinalValidade = tabelaAntiga.DataFinalValidade?.AddHours(23).AddMinutes(59).AddSeconds(59);

                        var vendedores = con.Query<Vendedor>("SELECT AUTONUM As Id FROM SGIPA.TB_LISTAS_PRECOS_VENDEDORES WHERE LISTA = :TabelaRevisadaId", parametros);

                        if (vendedores.Any())
                        {
                            var ultimoVendedor = vendedores.OrderByDescending(c => c.Id).First();

                            if (DateTime.Now > tabelaAntiga.DataFinalValidade)
                                parametros.Add(name: "DataFinalValidadeVendedor", value: DateTime.Now.Date, direction: ParameterDirection.Input);
                            else
                                parametros.Add(name: "DataFinalValidadeVendedor", value: tabelaAntiga.DataFinalValidade?.Date, direction: ParameterDirection.Input);

                            parametros.Add(name: "VendedorId", value: ultimoVendedor.Id, direction: ParameterDirection.Input);

                            con.Execute("UPDATE SGIPA.TB_LISTAS_PRECOS_VENDEDORES SET DATA_FIM_VIG = :DataFinalValidadeVendedor WHERE AUTONUM = :VendedorId", parametros);
                        }

                        parametros.Add(name: "DataFinalValidade", value: tabelaAntiga.DataFinalValidade, direction: ParameterDirection.Input);

                        if (oportunidadeNova.Acordo == false)
                        {
                            con.Execute("UPDATE SGIPA.TB_LISTAS_PRECOS SET FINAL_VALIDADE = :DataFinalValidade WHERE AUTONUM = :TabelaRevisadaId", parametros);
                        }
                    }
                }
                else
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: novaTabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Obs", value: $"TABELA CADASTRADA EM {DateTime.Now.ToString("dd/MM/yyyy")}", direction: ParameterDirection.Input);

                    con.Execute(@"UPDATE SGIPA.TB_LISTAS_PRECOS SET OBS = :Obs WHERE AUTONUM = :TabelaId", parametros);
                }
            }
        }

        public Parceiro ConsultaParceiro(string documento)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "ContaDocumento", value: documento.RemoverCaracteresEspeciais(), direction: ParameterDirection.Input);

                    return con.Query<Parceiro>("SELECT AUTONUM As Id, RAZAO As RazaoSocial, FANTASIA AS NomeFantasia, CGC As CNPJ FROM SGIPA.TB_CAD_PARCEIROS WHERE REPLACE(REPLACE(REPLACE(CGC,'.',''),'/',''),'-','') = :ContaDocumento", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "ContaDocumento", value: documento.RemoverCaracteresEspeciais(), direction: ParameterDirection.Input);

                    return con.Query<Parceiro>("SELECT AUTONUM As Id, RAZAO As RazaoSocial, FANTASIA AS NomeFantasia, CGC As CNPJ FROM SGIPA..TB_CAD_PARCEIROS WHERE REPLACE(REPLACE(REPLACE(CGC,'.',''),'/',''),'-','') = @ContaDocumento", parametros).FirstOrDefault();
                }
            }
        }

        public Parceiro ConsultaParceiroFontePagadora(string documento, int lista, SegmentoSubCliente segmento)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ContaDocumento", value: documento.RemoverCaracteresEspeciais(), direction: ParameterDirection.Input);
                parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);

                switch (segmento)
                {
                    case SegmentoSubCliente.IMPORTADOR:
                        parametros.Add(name: "Tipo", value: "I", direction: ParameterDirection.Input);
                        break;
                    case SegmentoSubCliente.DESPACHANTE:
                        parametros.Add(name: "Tipo", value: "D", direction: ParameterDirection.Input);
                        break;
                    case SegmentoSubCliente.COLOADER:
                        parametros.Add(name: "Tipo", value: "S", direction: ParameterDirection.Input);
                        break;
                    case SegmentoSubCliente.CO_COLOADER1:
                        parametros.Add(name: "Tipo", value: "U", direction: ParameterDirection.Input);
                        break;
                    case SegmentoSubCliente.CO_COLOADER2:
                        parametros.Add(name: "Tipo", value: "B", direction: ParameterDirection.Input);
                        break;
                }

                return con.Query<Parceiro>(@"
                    SELECT
                        B.AUTONUM As Id,
                        A.RAZAO As RazaoSocial, 
                        A.FANTASIA AS NomeFantasia, 
                        A.CGC As CNPJ
                    FROM
                        SGIPA.TB_CAD_PARCEIROS A
                    INNER JOIN
                        SGIPA.TB_TP_GRUPOS B ON A.AUTONUM = B.AUTONUM_PARCEIRO
                    WHERE 
                        REPLACE(REPLACE(REPLACE(A.CGC,'.',''),'/',''),'-','') = :ContaDocumento
                    AND
                        B.TIPO = :Tipo
                    AND
                        B.AUTONUMLISTA = :Lista", parametros).FirstOrDefault();
            }
        }

        public static int ConsultaFreetime(int lista)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);

                    return con.Query<int>("SELECT dias_apos_gr  FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :LISTA", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);

                    return con.Query<int>("SELECT dias_apos_gr  FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :LISTA", parametros).FirstOrDefault();
                }
            }
        }

        public static int ConsultaHubPort(int lista)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);

                    return con.Query<int>("SELECT NVL(FLAG_HUBPORT, 0) FLAG_HUBPORT FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :LISTA", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);

                    return con.Query<int>("SELECT ISNULL(FLAG_HUBPORT, 0) FLAG_HUBPORT FROM SGIPA..TB_LISTAS_PRECOS WHERE AUTONUM = :LISTA", parametros).FirstOrDefault();
                }
            }
        }

        public static int ConsultaTabelaEspecial(int lista)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);

                    return con.Query<int>("SELECT NVL(FLAG_ESPECIAL, 0) FLAG_ESPECIAL FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :LISTA", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);

                    return con.Query<int>("SELECT ISNULL(FLAG_ESPECIAL, 0) FLAG_ESPECIAL FROM SGIPA..TB_LISTAS_PRECOS WHERE AUTONUM = :LISTA", parametros).FirstOrDefault();
                }
            }
        }

        public static int ConsultaFormaPagamento(int lista)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);

                    return con.Query<int>("SELECT FORMA_PAGAMENTO FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :LISTA", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);

                    return con.Query<int>("SELECT FORMA_PAGAMENTO FROM SGIPA..TB_LISTAS_PRECOS WHERE AUTONUM = :LISTA", parametros).FirstOrDefault();
                }
            }
        }

        public static string ConsultaPerfil(int lista)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);

                    return con.Query<string>("SELECT PERFIL FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :LISTA", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);

                    return con.Query<string>("SELECT PERFIL FROM SGIPA..TB_LISTAS_PRECOS WHERE AUTONUM = :LISTA", parametros).FirstOrDefault();
                }
            }
        }

        public static int ConsultaTiposervico(int lista)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);

                    return con.Query<int>("SELECT tipo_servico  FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :LISTA", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Lista", value: lista, direction: ParameterDirection.Input);

                    return con.Query<int>("SELECT tipo_servico  FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :LISTA", parametros).FirstOrDefault();
                }
            }
        }


        private Vendedor ConsultaVendedor(string documento)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "ContaDocumento", value: documento.RemoverCaracteresEspeciais(), direction: ParameterDirection.Input);

                    return con.Query<Vendedor>("SELECT AUTONUM As Id, RAZAO As Nome, CGC As CPF FROM SGIPA.TB_CAD_PARCEIROS WHERE REPLACE(REPLACE(REPLACE(CGC,'.',''),'/',''),'-','') = :ContaDocumento", parametros).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "ContaDocumento", value: documento.RemoverCaracteresEspeciais(), direction: ParameterDirection.Input);

                    return con.Query<Vendedor>("SELECT AUTONUM As Id, RAZAO As Nome, CGC As CPF FROM SGIPA..TB_CAD_PARCEIROS WHERE REPLACE(REPLACE(REPLACE(CGC,'.',''),'/',''),'-','') = @ContaDocumento", parametros).FirstOrDefault();
                }
            }
        }

        public int CadastrarParceiro(Parceiro parceiro)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                var id = con.Query<int>("SELECT SGIPA.SEQ_CAD_PARCEIROS.NEXTVAL FROM DUAL").Single();

                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);
                parametros.Add(name: "RazaoSocial", value: parceiro.RazaoSocial, direction: ParameterDirection.Input);
                parametros.Add(name: "NomeFantasia", value: parceiro.NomeFantasia, direction: ParameterDirection.Input);
                parametros.Add(name: "CNPJ", value: parceiro.CNPJ, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM SGIPA.TB_CAD_PARCEIRO_CNPJ WHERE CNPJ = :CNPJ", parametros);

                con.Execute(@"INSERT INTO SGIPA.TB_CAD_PARCEIROS (AUTONUM, RAZAO, FANTASIA, CGC, FLAG_ATIVO, FLAG_CAPTADOR) 
                        VALUES (:Id, :RazaoSocial, :NomeFantasia, :CNPJ, 1, 1)", parametros);

                return id;
            }
        }

        public void AtualizarParceiroIndicador(int id)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1 WHERE AUTONUM = :Id", parametros);
            }
        }

        public void AtualizarSegmentoParceiro(int id, Segmento segmento)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                switch (segmento)
                {
                    case Segmento.IMPORTADOR:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_IMPORTADOR = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    case Segmento.DESPACHANTE:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_DESPACHANTE = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    case Segmento.COLOADER:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_NVOCC = 1, FLAG_IMPORTADOR = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    case Segmento.FREIGHT_FORWARDER:
                    case Segmento.AGENTE:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_NVOCC = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    default:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                }
            }
        }

        public void AtualizarSegmentoParceiroGrupoCNPJ(int id, Segmento segmento)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                switch (segmento)
                {
                    case Segmento.IMPORTADOR:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_IMPORTADOR = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    case Segmento.DESPACHANTE:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_DESPACHANTE = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    case Segmento.COLOADER:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_NVOCC = 1, FLAG_IMPORTADOR = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    default:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                }
            }
        }

        private int CadastrarVendedor(Vendedor vendedor)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    con.Open();

                    var parametros = new DynamicParameters();

                    var id = con.Query<int>("SELECT SGIPA.SEQ_CAD_PARCEIROS.NEXTVAL FROM DUAL").Single();

                    parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);
                    parametros.Add(name: "RazaoSocial", value: vendedor.Nome, direction: ParameterDirection.Input);
                    parametros.Add(name: "CNPJ", value: vendedor.CPF, direction: ParameterDirection.Input);

                    using (var transaction = con.BeginTransaction())
                    {
                        con.Execute(@"DELETE FROM SGIPA.TB_CAD_PARCEIRO_CNPJ WHERE CNPJ = :CNPJ", parametros, transaction);
                        con.Execute(@"INSERT INTO SGIPA.TB_CAD_PARCEIROS (AUTONUM, RAZAO, CGC, FLAG_VENDEDOR, FLAG_ATIVO) VALUES (:Id, :RazaoSocial, :CNPJ, 1, 1)", parametros, transaction);

                        transaction.Commit();
                    }

                    return id;
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    con.Open();

                    var parametros = new DynamicParameters();

                    parametros.Add(name: "RazaoSocial", value: vendedor.Nome, direction: ParameterDirection.Input);
                    parametros.Add(name: "CNPJ", value: vendedor.CPF, direction: ParameterDirection.Input);

                    using (var transaction = con.BeginTransaction())
                    {
                        con.Execute(@"DELETE FROM SGIPA..TB_CAD_PARCEIRO_CNPJ WHERE CNPJ = @CNPJ", parametros, transaction);
                        con.Execute(@"INSERT INTO SGIPA..TB_CAD_PARCEIROS (RAZAO, CGC, FLAG_VENDEDOR, FLAG_ATIVO) VALUES (@RazaoSocial, @CNPJ, 1, 1); SELECT CAST(SCOPE_IDENTITY() AS INT)", parametros, transaction);

                        transaction.Commit();

                        return parametros.Get<int>("Id");
                    }
                }
            }
        }

        public void AtualizarFlagParceiro(int parceiroId, SegmentoSubCliente segmento)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: parceiroId, direction: ParameterDirection.Input);

                switch (segmento)
                {
                    case SegmentoSubCliente.IMPORTADOR:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_IMPORTADOR = 1,FLAG_NVOCC = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    case SegmentoSubCliente.COLOADER:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_IMPORTADOR = 1, FLAG_NVOCC = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    case SegmentoSubCliente.CO_COLOADER1:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_IMPORTADOR = 1, FLAG_NVOCC = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    case SegmentoSubCliente.CO_COLOADER2:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_IMPORTADOR = 1, FLAG_NVOCC = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    case SegmentoSubCliente.DESPACHANTE:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_DESPACHANTE = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    default:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                }
            }
        }

        public void AtualizarFlagVendedor(int vendedorId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Id", value: vendedorId, direction: ParameterDirection.Input);

                    con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_VENDEDOR = 1 WHERE AUTONUM = :Id", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Id", value: vendedorId, direction: ParameterDirection.Input);

                    con.Execute(@"UPDATE SGIPA..TB_CAD_PARCEIROS SET FLAG_VENDEDOR = 1 WHERE AUTONUM = @Id", parametros);
                }
            }
        }

        public void AtualizarFormaPagamentoTabelaIPA(int tabelaId, int formaPagamento, string perfil)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "FormaPagamento", value: formaPagamento, direction: ParameterDirection.Input);
                    parametros.Add(name: "Perfil", value: perfil, direction: ParameterDirection.Input);

                    con.Execute($@"UPDATE {_schema}.TB_LISTAS_PRECOS SET FORMA_PAGAMENTO = :FormaPagamento, PERFIL = :Perfil WHERE AUTONUM = :TabelaId", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "FormaPagamento", value: formaPagamento, direction: ParameterDirection.Input);

                    con.Execute($@"UPDATE {_schema}.TB_LISTAS_PRECOS SET FORMA_PAGAMENTO = @FormaPagamento WHERE AUTONUM = @TabelaId", parametros);
                }
            }
        }

        public void ExcluirFontePagadora(int tabelaId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                con.Execute($@"DELETE FROM {_schema}.TB_DADOS_FATURAMENTO_IPA WHERE AUTONUM_LISTA = :TabelaId", parametros);
            }
        }

        public bool ExisteParceiroGrupo(int tabelaId, int parceiroId, string tipo)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ParceiroId", value: parceiroId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                    return con.Query("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE AUTONUMLISTA = :TabelaId AND AUTONUM_PARCEIRO = :ParceiroId AND TIPO = :Tipo", parametros).Any();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ParceiroId", value: parceiroId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                    return con.Query("SELECT AUTONUM FROM SGIPA..TB_TP_GRUPOS WHERE AUTONUMLISTA = @TabelaId AND AUTONUM_PARCEIRO = @ParceiroId AND TIPO = @Tipo", parametros).Any();
                }
            }
        }

        public bool ExisteOutrosSegmentosNoGrupo(int tabelaId, int parceiroId, string tipo)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ParceiroId", value: parceiroId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                    return con.Query("SELECT AUTONUM FROM SGIPA.TB_TP_GRUPOS WHERE AUTONUMLISTA = :TabelaId AND AUTONUM_PARCEIRO <> :ParceiroId AND TIPO = :Tipo", parametros).Any();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ParceiroId", value: parceiroId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                    return con.Query("SELECT AUTONUM FROM SGIPA..TB_TP_GRUPOS WHERE AUTONUMLISTA = @TabelaId AND AUTONUM_PARCEIRO = @ParceiroId AND TIPO = @Tipo", parametros).Any();
                }
            }
        }

        public void IncluirParceiroGrupo(int tabelaId, int parceiroId, string tipo)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ParceiroId", value: parceiroId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                    con.Execute("INSERT INTO SGIPA.TB_TP_GRUPOS (AUTONUM, AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (SGIPA.SEQ_TB_TP_GRUPOS.NEXTVAL, :TabelaId, :ParceiroId, :Tipo)", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ParceiroId", value: parceiroId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                    con.Execute("INSERT INTO SGIPA..TB_TP_GRUPOS (AUTONUMLISTA, AUTONUM_PARCEIRO, TIPO) VALUES (@TabelaId, @ParceiroId, @Tipo)", parametros);
                }
            }
        }

        public void ExcluirParceiroGrupo(int tabelaId, int parceiroId, string tipo)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ParceiroId", value: parceiroId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                    con.Execute("DELETE FROM SGIPA.TB_TP_GRUPOS WHERE AUTONUMLISTA = :TabelaId AND AUTONUM_PARCEIRO = :ParceiroId AND TIPO = :Tipo", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "ParceiroId", value: parceiroId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                    con.Execute("DELETE FROM SGIPA.TB_TP_GRUPOS WHERE AUTONUMLISTA = @TabelaId AND AUTONUM_PARCEIRO = @ParceiroId AND TIPO = @Tipo", parametros);
                }
            }
        }

        public void AcertaReefer(int tabelaId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    con.Execute(@"
                        UPDATE SGIPA.TB_LISTA_P_S_PERIODO 
                        SET TIPO_CARGA='CHF20'
	                      WHERE AUTONUM IN (SELECT A.AUTONUM FROM SGIPA.TB_LISTA_P_S_PERIODO  A 
                            INNER JOIN   SGIPA.tb_servicos_ipa B ON  A.SERVICO=B.AUTONUM AND  flag_reefer=1
                                  AND  TIPO_CARGA ='SVAR' AND A.LISTA=:TabelaId)", parametros);

                    con.Execute(@"
                        INSERT INTO  SGIPA.TB_LISTA_P_S_PERIODO ( AUTONUM,SERVICO,
	                        N_PERIODO,
	                        QTDE_DIAS,
	                        BASE_CALCULO,
                            TIPO_CARGA,
	                        VARIANTE_LOCAL,
	                        Base_Excesso,
	                        Valor_Excesso,
	                        Tipo_Doc,
	                        Armador,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        FLAG_PRORATA,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE ,LISTA, moeda,linha,oportunidadeid,flag_cobrar_nvocc,forma_pagamento_nvocc) 
                       SELECT SGIPA.SEQ_LISTA_P_S_PERIODO.NEXTVAL,SERVICO,
	                        N_PERIODO,
	                        QTDE_DIAS,
	                        BASE_CALCULO,
                            'CHF40',
	                        VARIANTE_LOCAL,
	                        Base_Excesso,
	                        Valor_Excesso,
	                        Tipo_Doc,
	                        Armador,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        FLAG_PRORATA,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE,LISTA , moeda,linha,oportunidadeid,flag_cobrar_nvocc,forma_pagamento_nvocc  FROM  SGIPA.TB_LISTA_P_S_PERIODO  A 
                            INNER JOIN   SGIPA.tb_servicos_ipa B ON  A.SERVICO=B.AUTONUM AND  flag_reefer=1
                                  AND   TIPO_CARGA IN('CHF20' ) AND A.LISTA=:TabelaId ", parametros);
                    con.Execute(@"
                        INSERT INTO  SGIPA.TB_LISTA_P_S_PERIODO ( AUTONUM,SERVICO,
	                        N_PERIODO,
	                        QTDE_DIAS,
	                        BASE_CALCULO,
                            TIPO_CARGA,
	                        VARIANTE_LOCAL,
	                        Base_Excesso,
	                        Valor_Excesso,
	                        Tipo_Doc,
	                        Armador,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        FLAG_PRORATA,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE ,LISTA, moeda,linha,oportunidadeid,flag_cobrar_nvocc,forma_pagamento_nvocc) 
                       SELECT SGIPA.SEQ_LISTA_P_S_PERIODO.NEXTVAL,SERVICO,
	                        N_PERIODO,
	                        QTDE_DIAS,
	                        BASE_CALCULO,
                            'CHF40',
	                        VARIANTE_LOCAL,
	                        Base_Excesso,
	                        Valor_Excesso,
	                        Tipo_Doc,
	                        Armador,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        FLAG_PRORATA,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE,LISTA , moeda,linha,oportunidadeid,flag_cobrar_nvocc,forma_pagamento_nvocc  FROM  SGIPA.TB_LISTA_P_S_PERIODO  A 
                            INNER JOIN   SGIPA.tb_servicos_ipa B ON  A.SERVICO=B.AUTONUM AND  flag_reefer=1
                                  AND   TIPO_CARGA IN('CPIER') AND A.LISTA=:TabelaId ", parametros); con.Execute(@"
                        INSERT INTO  SGIPA.TB_LISTA_P_S_PERIODO ( AUTONUM,SERVICO,
	                        N_PERIODO,
	                        QTDE_DIAS,
	                        BASE_CALCULO,
                            TIPO_CARGA,
	                        VARIANTE_LOCAL,
	                        Base_Excesso,
	                        Valor_Excesso,
	                        Tipo_Doc,
	                        Armador,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        FLAG_PRORATA,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE ,LISTA, moeda,linha,oportunidadeid,flag_cobrar_nvocc,forma_pagamento_nvocc) 
                       SELECT SGIPA.SEQ_LISTA_P_S_PERIODO.NEXTVAL,SERVICO,
	                        N_PERIODO,
	                        QTDE_DIAS,
	                        BASE_CALCULO,
                            'CHF20',
	                        VARIANTE_LOCAL,
	                        Base_Excesso,
	                        Valor_Excesso,
	                        Tipo_Doc,
	                        Armador,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        FLAG_PRORATA,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE,LISTA , moeda,linha,oportunidadeid,flag_cobrar_nvocc,forma_pagamento_nvocc  FROM  SGIPA.TB_LISTA_P_S_PERIODO  A 
                            INNER JOIN   SGIPA.tb_servicos_ipa B ON  A.SERVICO=B.AUTONUM AND  flag_reefer=1
                                  AND   TIPO_CARGA IN( 'CPIER') AND A.LISTA=:TabelaId ", parametros);
                    con.Execute(@"
                        UPDATE SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS
                        SET TIPO_CARGA='CHF20'
	                      WHERE AUTONUM IN (SELECT A.AUTONUM FROM SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS  A 
                            INNER JOIN   SGIPA.tb_servicos_ipa B ON  A.SERVICO=B.AUTONUM AND  flag_reefer=1
                                  AND  TIPO_CARGA ='SVAR' AND A.LISTA=:TabelaId)", parametros);

                    con.Execute(@"
                        INSERT INTO SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS
                        (   AUTONUM,
                            LISTA,
                            SERVICO,
	                        BASE_CALCULO,
	                        TIPO_CARGA,
                            VARIANTE_LOCAL,
	                        BASE_EXCESSO,
	                        VALOR_EXCESSO,
	                        TIPO_DOC,
	                        ARMADOR,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE,
	                        PRECO_MINIMO_DESOVA,
	                        PORTO_HUBPORT,
	                        TIPO_OPER  )
                         SELECT  SGIPA.SEQ_LISTA_PRECO_SERVICOS_FIXOS.NEXTVAL,  LISTA,
                            SERVICO,
	                        BASE_CALCULO,
	                        'CHF40',
                            VARIANTE_LOCAL,
	                        BASE_EXCESSO,
	                        VALOR_EXCESSO,
	                        TIPO_DOC,
	                        ARMADOR,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE,
	                        PRECO_MINIMO_DESOVA,
	                        PORTO_HUBPORT , TIPO_OPER
	                         FROM SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS A 
                            INNER JOIN   SGIPA.tb_servicos_ipa B ON  A.SERVICO=B.AUTONUM AND  flag_reefer=1
                                  AND  TIPO_CARGA='CHF20' AND A.LISTA=:TabelaId ", parametros);
                }
            }
        }

        public void RemoverDuplicidadesServicosVariaveis(int tabelaId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    con.Execute(@"UPDATE TB_LISTA_P_S_PERIODO SET 
                                    PRECO_UNITARIO = ROUND(PRECO_UNITARIO / QTDE_DIAS, 6),
                                    QTDE_DIAS = 1, 
                                    FLAG_PRORATA = 0  
                                    WHERE FLAG_PRORATA = 1 
                                            AND QTDE_DIAS> 1 
                                            AND SERVICO = 45 AND LISTA = :TabelaId",parametros);

                    var semminimos = con.Query<TabelaSemminimo>(@"SELECT A.AUTONUM,SERVICO, N_PERIODO, 
                                        TIPO_CARGA, BASE_CALCULO, 
                                        VARIANTE_LOCAL, LISTA
                                        FROM SGIPA.TB_LISTA_P_s_periodo A 
                                        LEFT JOIN SGIPA.TB_LISTA_CFG_VALORMINIMO B 
                                        ON A.AUTONUM = B.AUTONUMSV 
                                        WHERE LISTA = :TabelaId AND 
                                        TIPO_CARGA = 'CPIER'
                                        AND N_PERIODO>1  AND B.AUTONUM IS NULL 
                                        AND PRECO_MINIMO = 0 
                                        ORDER BY SERVICO,N_PERIODO ", parametros);

                    foreach (var semminimo in semminimos)
                    {

                        parametros.Add(name: "AutonumSv", value: semminimo.Autonum, direction: ParameterDirection.Input);
                        parametros.Add(name: "Servico", value: semminimo.Servico, direction: ParameterDirection.Input);
                        parametros.Add(name: "N_Periodo", value: semminimo.N_Periodo - 1, direction: ParameterDirection.Input);
                        parametros.Add(name: "Base_calculo", value: semminimo.Base_Calculo, direction: ParameterDirection.Input);
                        parametros.Add(name: "Variante_Local", value: semminimo.Variante_Local, direction: ParameterDirection.Input);
                        con.Execute(@" Insert into SGIPA.TB_lista_cfg_valorminimo(
                              AUTONUM ,
                              AUTONUMSV ,
                              NBLS    ,
                              VALORMINIMO,
                              TIPO      ,
                              PERCMULTA  )
                               Select Sgipa.seq_lista_cfg_valorminimo.NEXTVAL,
                               :AutonumSv ,
                               NBLS    ,
                               VALORMINIMO,
                               TIPO      ,
                               PERCMULTA
                               From Sgipa.TB_lista_cfg_valorminimo a,
                               Sgipa.TB_LISTA_P_S_PERIODO b
                               where a.autonumsv = b.autonum and b.lista = :TabelaId
                               And B.SERVICO = :Servico
                               and B.N_PERIODO = :N_Periodo
                               and B.TIPO_CARGA = 'CPIER'
                               and B.BASE_CALCULO = :Base_calculo
                               and B.VARIANTE_LOCAL = :Variante_Local", parametros);
                    }

                    var duplicidades = con.Query<TabelaDuplicada>(@"
                        SELECT 
	                        COUNT(1) Total, 
	                        MAX(AUTONUM) MAX, 
	                        MIN(AUTONUM) MIN  
                        FROM 
	                        SGIPA.TB_LISTA_P_S_PERIODO 
                        WHERE 
	                        TIPO_CARGA IN ('SVAR20', 'SVAR40') 
                        AND 
	                        LISTA = :TabelaId
                        AND AUTONUM NOT IN(SELECT AUTONUMSV FROM SGIPA.TB_LISTA_P_S_FAIXASCIF_PER)   
                        GROUP BY
	                        SERVICO,
	                        N_PERIODO,
	                        QTDE_DIAS,
	                        BASE_CALCULO,
	                        VARIANTE_LOCAL,
	                        Base_Excesso,
	                        Valor_Excesso,
	                        Tipo_Doc,
	                        Armador,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        FLAG_PRORATA,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE ,EXERCITO
                        HAVING COUNT(1) > 1 ", parametros);

                    foreach (var duplicidade in duplicidades)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Min", value: duplicidade.Min, direction: ParameterDirection.Input);
                        parametros.Add(name: "Max", value: duplicidade.Max, direction: ParameterDirection.Input);

                        con.Execute("DELETE FROM SGIPA.TB_LISTA_P_S_PERIODO WHERE AUTONUM = :Min", parametros);
                        con.Execute("UPDATE SGIPA.TB_LISTA_P_S_PERIODO SET TIPO_CARGA = 'SVAR' WHERE AUTONUM = :Max", parametros);
                    }

                    var saida = true;
                    var loop = 0;
                    while (saida)
                    {
                        loop = loop + 1;
                        if (loop > 15)
                        {
                            saida = false;
                        }
                        parametros = new DynamicParameters();
                        parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                        con.Execute(@" DELETE FROM SGIPA.TB_LISTA_P_S_PERIODO WHERE
                                         AUTONUM NOT IN (SELECT AUTONUMSV FROM SGIPA.TB_LISTA_CFG_VALORMINIMO ) AND
                                         AUTONUM NOT IN (SELECT AUTONUMSV FROM SGIPA.TB_LISTA_P_S_FAIXASCIF_PER)   AND 
                                         AUTONUM in(
                                         SELECT A.autonum FROM SGIPA.TB_LISTA_P_S_PERIODO A 
                                         INNER JOIN
                                             (SELECT MAX(C.N_PERIODO) AS N_PERIODO,
                                             C.LISTA,
                                             C.SERVICO,
                                             C.TIPO_CARGA,
                                             C.QTDE_DIAS,
                                             C.BASE_CALCULO,
                                             C.VARIANTE_LOCAL,
                                             NVL(C.BASE_EXCESSO,'X') AS BASE_EXCESSO,
                                             NVL(C.TIPO_DOC,0) AS TIPO_DOC,
                                             NVL(C.ARMADOR,0) AS ARMADOR,
                                             NVL(C.PRECO_UNITARIO,0) AS PRECO_UNITARIO,
                                             NVL(C.PRECO_MINIMO,0) AS PRECO_MINIMO,
                                             NVL(C.PRECO_MAXIMO,0) AS PRECO_MAXIMO,
                                             NVL(C.VALOR_ANVISA,0) AS VALOR_ANVISA,
                                             NVL(C.VALOR_ACRESCIMO,0) AS VALOR_ACRESCIMO,
                                             NVL(C.LOCAL_ATRACACAO,0) AS LOCAL_ATRACACAO,
                                             NVL(C.FLAG_PRORATA,0) AS FLAG_PRORATA,
                                             NVL(C.AUTONUM_VINCULADO,0) AS AUTONUM_VINCULADO,
                                             NVL(C.GRUPO_ATRACACAO,0) AS GRUPO_ATRACACAO,
                                             NVL(C.VALOR_ACRESC_PESO,0) AS VALOR_ACRESC_PESO,
                                             NVL(C.PESO_LIMITE,0) AS PESO_LIMITE,
                                             NVL(C.EXERCITO,0) AS EXERCITO
                                             FROM SGIPA.TB_LISTA_P_S_PERIODO C 
                                            GROUP BY
                                             C.LISTA,
                                             C.SERVICO,
                                             C.TIPO_CARGA,
                                             C.QTDE_DIAS,
                                             C.BASE_CALCULO,
                                             C.VARIANTE_LOCAL,
                                             NVL(C.BASE_EXCESSO,'X') ,
                                             NVL(C.TIPO_DOC,0),
                                             NVL(C.ARMADOR,0),
                                             NVL(C.PRECO_UNITARIO,0) ,
                                             NVL(C.PRECO_MINIMO,0) ,
                                             NVL(C.PRECO_MAXIMO,0) ,
                                             NVL(C.VALOR_ANVISA,0) ,
                                             NVL(C.VALOR_ACRESCIMO,0),
                                             NVL(C.LOCAL_ATRACACAO,0) ,
                                             NVL(C.FLAG_PRORATA,0) ,
                                             NVL(C.AUTONUM_VINCULADO,0),
                                             NVL(C.GRUPO_ATRACACAO,0) ,
                                             NVL(C.VALOR_ACRESC_PESO,0) ,
                                             NVL(C.PESO_LIMITE,0)
                                             NVL(C.EXERCITO,0)
                                             ) M
                                             ON  A.N_PERIODO=M.N_PERIODO
                                             AND A.LISTA=M.LISTA
                                             AND A.SERVICO=M.SERVICO
                                             AND A.TIPO_CARGA=M.TIPO_CARGA
                                             AND A.QTDE_DIAS=M.QTDE_DIAS
                                             AND A.BASE_CALCULO=M.BASE_CALCULO
                                             AND A.VARIANTE_LOCAL=M.VARIANTE_LOCAL
                                             AND NVL(A.BASE_EXCESSO,'X')=M.BASE_EXCESSO
                                             AND NVL(A.TIPO_DOC,0)=M.TIPO_DOC
                                             AND NVL(A.ARMADOR,0)=M.ARMADOR
                                             AND NVL(A.PRECO_UNITARIO,0)=M.PRECO_UNITARIO
                                             AND NVL(A.PRECO_MINIMO,0)=M.PRECO_MINIMO
                                             AND NVL(A.VALOR_ANVISA,0)=M.VALOR_ANVISA
                                             AND NVL(A.VALOR_ACRESCIMO,0)=M.VALOR_ACRESCIMO
                                             AND NVL(A.FLAG_PRORATA,0)=M.FLAG_PRORATA
                                             AND NVL(A.AUTONUM_VINCULADO,0)=M.AUTONUM_VINCULADO
                                             AND NVL(A.GRUPO_ATRACACAO,0)=M.GRUPO_ATRACACAO
                                             AND NVL(A.VALOR_ACRESC_PESO,0)=M.VALOR_ACRESC_PESO
                                             AND NVL(A.PESO_LIMITE,0)=M.PESO_LIMITE
                                             AND NVL(A.EXERCITO,0)=M.EXERCITO
                                             AND
                                             EXISTS (
                                             SELECT B.AUTONUM 
                                             FROM SGIPA.TB_LISTA_P_S_PERIODO B WHERE
                                             A.LISTA=B.LISTA
                                             AND A.SERVICO=B.SERVICO
                                             AND A.TIPO_CARGA=B.TIPO_CARGA
                                             AND A.QTDE_DIAS=B.QTDE_DIAS
                                             AND A.BASE_CALCULO=B.BASE_CALCULO
                                             AND A.VARIANTE_LOCAL=B.VARIANTE_LOCAL
                                             AND NVL(A.BASE_EXCESSO,'X')=NVL(B.BASE_EXCESSO,'X')
                                             AND NVL(A.VALOR_EXCESSO,0)=NVL(B.VALOR_EXCESSO,0)
                                             AND NVL(A.TIPO_DOC,0)=NVL(B.TIPO_DOC,0)
                                             AND NVL(A.ARMADOR,0)=NVL(B.ARMADOR,0)
                                             AND NVL(A.PRECO_UNITARIO,0)=NVL(B.PRECO_UNITARIO,0)
                                             AND NVL(A.PRECO_MINIMO,0)=NVL(B.PRECO_MINIMO,0)
                                             AND NVL(A.PRECO_MAXIMO,0)=NVL(B.PRECO_MAXIMO,0)
                                             AND NVL(A.VALOR_ANVISA,0)=NVL(B.VALOR_ANVISA,0)
                                             AND NVL(A.VALOR_ACRESCIMO,0)=NVL(B.VALOR_ACRESCIMO,0)
                                             AND NVL(A.LOCAL_ATRACACAO,0)=NVL(B.LOCAL_ATRACACAO,0)
                                             AND NVL(A.FLAG_PRORATA,0)=NVL(B.FLAG_PRORATA,0)
                                             AND NVL(A.AUTONUM_VINCULADO,0)=NVL(B.AUTONUM_VINCULADO,0)
                                             AND NVL(A.GRUPO_ATRACACAO,0)=NVL(B.GRUPO_ATRACACAO,0)
                                             AND NVL(A.VALOR_ACRESC_PESO,0)= NVL(B.VALOR_ACRESC_PESO,0)
                                             AND NVL(A.PESO_LIMITE,0) =NVL(B.PESO_LIMITE,0)
                                             AND NVL(A.EXERCITO,0) =NVL(B.EXERCITO,0)
                                             AND A.N_PERIODO=B.N_PERIODO+1
                                             )
  
                                             AND NOT EXISTS (
                                             SELECT B.AUTONUM 
                                             FROM SGIPA.TB_LISTA_P_S_PERIODO B WHERE
                                             A.LISTA=B.LISTA
                                             AND A.SERVICO=B.SERVICO
                                             AND A.TIPO_CARGA=B.TIPO_CARGA 
                                             AND A.N_PERIODO<B.N_PERIODO
                                             )                                            
                                             AND A.LISTA=:TabelaId  
                                             )", parametros);
                    }

                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    var duplicidades = con.Query<TabelaDuplicada>(@"
                        SELECT 
	                        COUNT(1) Total, 
	                        MAX(AUTONUM) MAX, 
	                        MIN(AUTONUM) MIN  
                        FROM 
	                        SGIPA..TB_LISTA_P_S_PERIODO 
                        WHERE 
	                        TIPO_CARGA IN ('SVAR20', 'SVAR40') 
                        AND 
	                        LISTA = @TabelaId
                        GROUP BY
	                        SERVICO,
	                        N_PERIODO,
	                        QTDE_DIAS,
	                        BASE_CALCULO,
	                        VARIANTE_LOCAL,
	                        Base_Excesso,
	                        Valor_Excesso,
	                        Tipo_Doc,
	                        Armador,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        FLAG_PRORATA,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE ,EXERCITO
                        HAVING COUNT(1) > 1", parametros);

                    foreach (var duplicidade in duplicidades)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Min", value: duplicidade.Min, direction: ParameterDirection.Input);
                        parametros.Add(name: "Max", value: duplicidade.Max, direction: ParameterDirection.Input);

                        con.Execute("DELETE FROM SGIPA..TB_LISTA_P_S_PERIODO WHERE AUTONUM = @Min", parametros);
                        con.Execute("UPDATE SGIPA..TB_LISTA_P_S_PERIODO SET TIPO_CARGA = 'SVAR' WHERE AUTONUM = @Max", parametros);
                    }

                    var saida = true;
                    var loop = 0;

                    while (saida)
                    {
                        loop = loop + 1;
                        if (loop > 20)
                        {
                            saida = false;
                        }
                        parametros = new DynamicParameters();
                        parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                        duplicidades = con.Query<TabelaDuplicada>(@"
                            SELECT 
                                COUNT(1) Total, 
                                MAX(N_PERIODO) Max, 
                                MIN(N_PERIODO) Min,
                                MAX(AUTONUM) MaxAuto
                            FROM  
                                SGIPA.TB_LISTA_P_S_PERIODO   
                            WHERE   
                                LISTA = :TabelaId     
                            GROUP BY   
                                SERVICO,
                                TIPO_CARGA,
                                QTDE_DIAS,
                                BASE_CALCULO,
                                VARIANTE_LOCAL,
                                BASE_EXCESSO,
                                VALOR_EXCESSO,
                                TIPO_DOC,
                                ARMADOR,
                                PRECO_UNITARIO,
                                PRECO_MINIMO,
                                PRECO_MAXIMO,
                                VALOR_ANVISA,
                                VALOR_ACRESCIMO,
                                LOCAL_ATRACACAO,
                                FLAG_PRORATA,
                                AUTONUM_VINCULADO,
                                GRUPO_ATRACACAO,
                                VALOR_ACRESC_PESO,
                                PESO_LIMITE ,EXERCITO
                            HAVING COUNT(1) > 1", parametros);

                        if (!duplicidades.Any())
                            saida = false;

                        foreach (var item in duplicidades)
                        {
                            if (item.Max > item.Min)
                            {
                                parametros = new DynamicParameters();
                                parametros.Add(name: "Max", value: item.MaxAuto, direction: ParameterDirection.Input);
                                var temproximo = con.Query<TabelaSemminimo>(@"SELECT AUTONUM,SERVICO, N_PERIODO, 
                                        TIPO_CARGA, BASE_CALCULO, 
                                        VARIANTE_LOCAL, LISTA   FROM SGIPA.TB_LISTA_P_S_PERIODO WHERE AUTONUM = :Max", parametros);
                                foreach (var proximo in temproximo)
                                {
                                    parametros = new DynamicParameters();
                                    parametros.Add(name: "Servico", value: proximo.Servico, direction: ParameterDirection.Input);
                                    parametros.Add(name: "N_Periodo", value: proximo.N_Periodo - 1, direction: ParameterDirection.Input);
                                    parametros.Add(name: "Base_calculo", value: proximo.Base_Calculo, direction: ParameterDirection.Input);
                                    parametros.Add(name: "Variante_Local", value: proximo.Variante_Local, direction: ParameterDirection.Input);
                                    parametros.Add(name: "Tipo_Carga", value: $"{proximo.Tipo_Carga.Substring(0, 4)}%", direction: ParameterDirection.Input);

                                    var nproximo = con.Query<int>(@"
                                     SELECT COUNT(1) Total FROM SGIPA.TB_LISTA_P_S_PERIODO WHERE 
                                     LISTA = :TabelaId and TIPO_CARGA LIKE :Tipo_Carga AND 
                                     SERVICO=:Servico AND N_PERIODO >:N_Periodo", parametros).Single();


                                    if (nproximo == 0)
                                    {
                                        con.Execute("DELETE FROM SGIPA..TB_LISTA_P_S_PERIODO WHERE AUTONUM = :Max", parametros);
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        public void RemoverDuplicidadesMargem(int tabelaId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    var duplicidades = con.Query<TabelaDuplicada>(@"
                        SELECT 
	                        COUNT(1) Total, 
	                        MAX(AUTONUM) MAX, 
	                        MIN(AUTONUM) MIN,
                            SERVICO 
                        FROM 
	                        SGIPA.TB_LISTA_P_S_PERIODO 
                        WHERE 
	                        VARIANTE_LOCAL IN ('ENTR','MDIR', 'MESQ') 
                        AND 
	                        LISTA = :TabelaId
                        GROUP BY
	                        SERVICO,
	                        N_PERIODO,
	                        QTDE_DIAS,
	                        BASE_CALCULO,
	                        TIPO_CARGA,
	                        Base_Excesso,
	                        Valor_Excesso,
	                        Tipo_Doc,
	                        Armador,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        FLAG_PRORATA,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE 
                        HAVING COUNT(1) > 1 ", parametros);

                    foreach (var duplicidade in duplicidades)
                    {
                        parametros = new DynamicParameters();
                        parametros.Add(name: "Min", value: duplicidade.Min, direction: ParameterDirection.Input);
                        parametros.Add(name: "Max", value: duplicidade.Max, direction: ParameterDirection.Input);
                        parametros.Add(name: "Servico", value: duplicidade.Servico, direction: ParameterDirection.Input);
                        parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                        var temmargem = con.Query<int>(@"
                        SELECT COUNT(1) Total FROM SGIPA.TB_LISTA_P_S_PERIODO WHERE 
                            VARIANTE_LOCAL IN ('MDIR', 'MESQ') AND LISTA = :TabelaId 
                                AND SERVICO = :Servico AND N_PERIODO = 1", parametros).Single();

                        if (temmargem == 0)
                        {
                            parametros.Add(name: "Min", value: duplicidade.Min, direction: ParameterDirection.Input);
                            parametros.Add(name: "Max", value: duplicidade.Max, direction: ParameterDirection.Input);

                            con.Execute("DELETE FROM SGIPA.TB_LISTA_P_S_PERIODO WHERE AUTONUM = :Min", parametros);
                            con.Execute("UPDATE SGIPA.TB_LISTA_P_S_PERIODO SET VARIANTE_LOCAL = 'SVAR' WHERE AUTONUM = :Max", parametros);
                        }
                    }
                }

            }

            else
            {

            }
        }

        public void CorrigeGrupoAtracacao(int tabelaId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    con.Execute($@"UPDATE {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS SET GRUPO_ATRACACAO = 0 WHERE LISTA = :TabelaId AND GRUPO_ATRACACAO IS NULL", parametros);
                    con.Execute($@"UPDATE {_schema}.TB_LISTA_P_S_PERIODO SET GRUPO_ATRACACAO = 0 WHERE LISTA = :TabelaId AND GRUPO_ATRACACAO IS NULL", parametros);
                    con.Execute($@"INSERT INTO {_schema}.Tb_lista_preco_servicos_fixos(AUTONUM, LISTA, SERVICO,
                                       TIPO_CARGA, BASE_CALCULO, VARIANTE_LOCAL,
                                       PRECO_UNITARIO, MOEDA, PRECO_MINIMO,
                                       VALOR_ACRESCIMO, USUARIO_SIS, USUARIO_REDE,
                                       MAQUINA_REDE, LOCAL_ATRACACAO, AUTONUM_VINCULADO,
                                       FLAG_HP, GRUPO_ATRACACAO, VALOR_ACRESC_PESO,
                                       PESO_LIMITE, PRECO_MINIMO_DESOVA,  
                                       PORTO_HUBPORT, FLAG_SERVICO_UNICO, FLAG_ISENTO,
                                       TIPO_DOC, BASE_EXCESSO, VALOR_EXCESSO,
                                       ARMADOR, PRECO_MAXIMO, VALOR_ANVISA,
                                       FLAG_COBRAR_NVOCC, FORMA_PAGAMENTO_NVOCC, LINHA,
                                       OPORTUNIDADEID, QTDE_PACOTE, PESO_MINIMO,
                                       VOLUME_MINIMO)
                                       SELECT
                                       {_schema}.SEQ_LISTA_PRECO_SERVICOS_FIXOS.NEXTVAL, LISTA, SERVICO, 
                                       TIPO_CARGA, BASE_CALCULO, 'MDIR', 
                                       0, MOEDA, 0, 
                                       0, B.USUARIO_SIS, B.USUARIO_REDE, 
                                       B.MAQUINA_REDE, 0, B.AUTONUM_VINCULADO, 
                                       0,  1,  0, 
                                       0,  0,    
                                       0,  0,  0, 
                                       0,  0,  0, 
                                       0,  0,  0, 
                                       0,  0,  B.LINHA, 
                                       B.OPORTUNIDADEID, 0, 0,                                       0
                                       From {_schema}.tb_listas_Precos a  Inner
                                       Join {_schema}.Tb_Lista_Preco_Servicos_Fixos B
                                       on A.Autonum = B.Lista 
                                       Where B.Servico = 332 And
                                       valor_acresc_peso > 0 and peso_limite > 0 And 
                                       Variante_Local = 'SVAR' And 
                                       B.lista = :tabelaId  AND 
                                       TIPO_CARGA = 'SVAR' AND
                                       NVL(B.GRUPO_ATRACACAO,0)=0 ", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    con.Execute($@"UPDATE {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS SET GRUPO_ATRACACAO = 0 WHERE LISTA = @TabelaId AND GRUPO_ATRACACAO IS NULL", parametros);
                    con.Execute($@"UPDATE {_schema}..TB_LISTA_P_S_PERIODO SET GRUPO_ATRACACAO = 0 WHERE LISTA = @TabelaId AND GRUPO_ATRACACAO IS NULL", parametros);
                }
            }
        }

        public void CorrigeMonitoramentoReeferCPIER(int oportunidadeId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    con.Execute($@"UPDATE {_schema}.TB_LISTA_P_S_PERIODO SET PRECO_UNITARIO = 0 WHERE AUTONUM IN (SELECT A.AUTONUM FROM {_schema}.TB_LISTA_P_S_PERIODO A INNER JOIN {_schema}.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM WHERE B.FLAG_REEFER = 1 AND TIPO_CARGA = 'CPIER' AND PRECO_UNITARIO > 0 AND OPORTUNIDADEID = :OportunidadeId)", parametros);
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                    con.Execute($@"UPDATE {_schema}..TB_LISTA_P_S_PERIODO SET PRECO_UNITARIO = 0 WHERE AUTONUM IN (SELECT A.AUTONUM FROM {_schema}.TB_LISTA_P_S_PERIODO A INNER JOIN {_schema}..TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM WHERE B.FLAG_REEFER = 1 AND TIPO_CARGA = 'CPIER' AND PRECO_UNITARIO > 0 AND OPORTUNIDADEID = @OportunidadeId)");
                }
            }
        }

        public void CorrigeFaixasCIF(int oportunidadeId)
        {
            var variantes = new List<string>() { "MDIR", "MESQ", "ENTR", "SVAR" };

            foreach (var margem in variantes)
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                    parametros.Add(name: "Margem", value: margem, direction: ParameterDirection.Input);
                    LayoutDTO[] valoresCif;

                    var valoresCiF = con.Query<LayoutDTO>($@"
                        SELECT 
                            A.AUTONUM As Id, 
                            A.ValorInicial ValorCif, 
                            A.ValorFinal, B.N_PERIODO
                        FROM 
                            {_schema}.TB_LISTA_P_S_FAIXASCIF_PER A 
                        INNER JOIN 
                            {_schema}.TB_LISTA_P_S_PERIODO B ON A.AUTONUMSV = B.AUTONUM 
                        INNER JOIN
                            {_schema}.TB_LISTAS_PRECOS C ON B.LISTA = C.AUTONUM 
                        WHERE 
                            C.OportunidadeId = :OportunidadeId AND B.VARIANTE_LOCAL = :Margem
                        ORDER BY 
                          b.n_periodo,valorinicial", parametros).ToArray();

                    var count = 0;
                    foreach (var faixa in valoresCiF)
                    {

                        if (count < (valoresCiF.Count() - 1))
                        {
                            if (valoresCiF[count + 1].ValorCif - 0.01M > 0)
                            {
                                parametros.Add(name: "ValorFinal", value: valoresCiF[count + 1].ValorCif - 0.01M, direction: ParameterDirection.Input);
                            }
                            else
                            {
                                parametros.Add(name: "ValorFinal", value: 99999999, direction: ParameterDirection.Input);
                            }

                            parametros.Add(name: "Id", value: valoresCiF[count].Id, direction: ParameterDirection.Input);

                            con.Query($"UPDATE {_schema}.TB_LISTA_P_S_FAIXASCIF_PER SET ValorFinal = :ValorFinal WHERE AUTONUM = :Id", parametros);
                        }

                        count++;
                    }
                }
            }
        }

        public void CorrigeFaixasCIFMinimo(int oportunidadeId)
        {
            var variantes = new List<string>() { "MDIR", "MESQ", "ENTR", "SVAR" };

            foreach (var margem in variantes)
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var saida = true;
                    var loop = 0;
                    while (saida)
                    {
                        loop = loop + 1;
                        if (loop > 10)
                        {
                            saida = false;
                        }
                        var parametros = new DynamicParameters();

                        parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                        parametros.Add(name: "Margem", value: margem, direction: ParameterDirection.Input);

                        var faixasCif = con.Query<FaixaCIF>($@"
                       SELECT 
                            A.AUTONUM As Id, 
                            A.AUTONUMSV As ServicoVariavelId, 
                            A.ValorInicial, 
                            A.ValorFinal, 
                            B.TIPO_CARGA As TipoCarga,
                            B.Linha
                       FROM 
                            {_schema}.TB_LISTA_P_S_FAIXASCIF_PER A 
                       INNER JOIN 
                            {_schema}.TB_LISTA_P_S_PERIODO B ON A.AUTONUMSV = B.AUTONUM 
                       INNER JOIN
                            {_schema}.TB_LISTAS_PRECOS C ON B.LISTA = C.AUTONUM 
                       WHERE NVL(A.MINIMO,0)=0 AND 
                            C.OportunidadeId = :OportunidadeId AND B.VARIANTE_LOCAL = :Margem
                       ORDER BY 
                             b.linha, A.valorinicial ", parametros);

                        var count = 1;

                        foreach (var faixa in faixasCif)
                        {
                            parametros = new DynamicParameters();

                            parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                            parametros.Add(name: "TipoRegistroCif", value: TipoRegistro.ARMAZENAGEM_MINIMO_CIF, direction: ParameterDirection.Input);
                            parametros.Add(name: "Linha", value: count, direction: ParameterDirection.Input);

                            LayoutDTO valorCif;

                            if (margem != "SVAR")
                            {
                                if (margem == "MDIR")
                                {
                                    parametros.Add(name: "Margem", value: Margem.DIREITA, direction: ParameterDirection.Input);
                                }
                                else if (margem == "MESQ")
                                {
                                    parametros.Add(name: "Margem", value: Margem.ESQUERDA, direction: ParameterDirection.Input);
                                }
                                else
                                {
                                    parametros.Add(name: "Margem", value: Margem.ENTRE, direction: ParameterDirection.Input);
                                }

                                valorCif = con.Query<LayoutDTO>($@"
                                SELECT Linha, ValorCif, ValorMinimo, ValorMinimo20, ValorMinimo40, TipoCarga FROM
                                (
                                    SELECT ROWNUM As Linha, ValorCif, ValorMinimo, ValorMinimo20, ValorMinimo40, TipoCarga FROM
                                        (
                                            SELECT 
                                                A.ValorCif, A.ValorMinimo, A.ValorMinimo20, A.ValorMinimo40, A.TipoCarga
                                          FROM CRM.TB_CRM_OPORTUNIDADE_LAYOUT A
                                                 LEFT join CRM.TB_CRM_OPORTUNIDADE_LAYOUT B 
                                                 ON A.OportunidadeId =B.OportunidadeId 
                                                 AND B.TIPOREGISTRO=7                                           
                                            WHERE 
                                                A.OportunidadeId = :OportunidadeId AND A.Margem = :Margem AND A.TipoRegistro = :TipoRegistroCif ORDER BY B.LINHA,A.ValorCif
                                        ) 
                                ) WHERE Linha = :Linha", parametros).FirstOrDefault();
                            }
                            else
                            {
                                valorCif = con.Query<LayoutDTO>($@"
                               SELECT Linha, ValorCif, ValorMinimo, ValorMinimo20, ValorMinimo40, TipoCarga FROM
                                (
                                    SELECT ROWNUM As Linha, ValorCif, ValorMinimo, ValorMinimo20, ValorMinimo40, TipoCarga FROM
                                        (
                                            SELECT 
                                                A.ValorCif, A.ValorMinimo, A.ValorMinimo20, A.ValorMinimo40, A.TipoCarga
                                          FROM CRM.TB_CRM_OPORTUNIDADE_LAYOUT A
                                                 LEFT join CRM.TB_CRM_OPORTUNIDADE_LAYOUT B 
                                                 ON A.OportunidadeId =B.OportunidadeId 
                                                 AND B.TIPOREGISTRO=7                                           
                                            WHERE 
                                                A.OportunidadeId = :OportunidadeId   AND A.TipoRegistro = :TipoRegistroCif ORDER BY B.LINHA,A.ValorCif
                                        ) 
                                ) WHERE Linha = :Linha", parametros).FirstOrDefault();
                            }

                            if (valorCif != null)
                            {
                                parametros = new DynamicParameters();

                                if (faixa.TipoCarga == "SVAR20")
                                {
                                    parametros.Add(name: "Valor", value: valorCif.ValorMinimo20, direction: ParameterDirection.Input);
                                }
                                else if (faixa.TipoCarga == "SVAR40")
                                {
                                    parametros.Add(name: "Valor", value: valorCif.ValorMinimo40, direction: ParameterDirection.Input);
                                }
                                else
                                {
                                    if (valorCif.TipoCarga == TipoCarga.CONTEINER || valorCif.TipoCarga == TipoCarga.MUDANCA_REGIME)
                                    {
                                        parametros.Add(name: "Valor", value: valorCif.ValorMinimo20, direction: ParameterDirection.Input);
                                    }
                                    else
                                    {
                                        parametros.Add(name: "Valor", value: valorCif.ValorMinimo, direction: ParameterDirection.Input);
                                    }
                                }

                                parametros.Add(name: "Id", value: faixa.Id, direction: ParameterDirection.Input);

                                con.Query($"UPDATE {_schema}.TB_LISTA_P_S_FAIXASCIF_PER SET Minimo = :Valor WHERE AUTONUM = :Id", parametros);

                            }
                            count++;
                        }
                    }
                }
            }
        }

        public void RemoverDuplicidadesMargemfixo(int tabelaId, int Contar)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    if (Contar == 1)
                    {
                        parametros.Add(name: "Margem1", value: "MDIR", direction: ParameterDirection.Input);
                        parametros.Add(name: "Margem2", value: "MESQ", direction: ParameterDirection.Input);
                    }
                    if (Contar == 2)
                    {
                        parametros.Add(name: "Margem1", value: "ENTR", direction: ParameterDirection.Input);
                        parametros.Add(name: "Margem2", value: "SVAR", direction: ParameterDirection.Input);
                    }
                    var duplicidades = con.Query<TabelaDuplicada>(@"
                        SELECT 
	                        COUNT(1) Total, 	                        
	                        MIN(AUTONUM) Min,
                            MAX(AUTONUM) Max
                        FROM
	                        SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS
                        WHERE 
	                        VARIANTE_LOCAL IN (:Margem1, :Margem2) AND LISTA = :TabelaId     
                        GROUP BY    
	                        SERVICO,
	                        BASE_CALCULO,
	                        TIPO_CARGA,
	                        BASE_EXCESSO,
	                        VALOR_EXCESSO,
	                        TIPO_DOC,
	                        ARMADOR,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE,
	                        PRECO_MINIMO_DESOVA,
	                        PORTO_HUBPORT,
	                        TIPO_OPER  
                        HAVING COUNT(1) > 1", parametros);

                    foreach (var duplicidade in duplicidades)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Min", value: duplicidade.Min, direction: ParameterDirection.Input);
                        parametros.Add(name: "Max", value: duplicidade.Max, direction: ParameterDirection.Input);

                        con.Execute($"DELETE FROM {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE AUTONUM = :Min", parametros);
                        con.Execute($"UPDATE {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS SET VARIANTE_LOCAL = 'SVAR' WHERE AUTONUM = :Max", parametros);
                    }
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                    if (Contar == 1)
                    {
                        parametros.Add(name: "Margem1", value: "MDIR", direction: ParameterDirection.Input);
                        parametros.Add(name: "Margem2", value: "MESQ", direction: ParameterDirection.Input);
                    }
                    if (Contar == 2)
                    {
                        parametros.Add(name: "Margem1", value: "ENTR", direction: ParameterDirection.Input);
                        parametros.Add(name: "Margem2", value: "SVAR", direction: ParameterDirection.Input);
                    }
                    var duplicidades = con.Query<TabelaDuplicada>($@"
                        SELECT 
	                        COUNT(1) Total, 	                        
	                        MIN(AUTONUM) Min,
                            MAX(AUTONUM) Max
                        FROM
	                        {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS
                        WHERE 
	                      VARIANTE_LOCAL IN (:Margem1, :Margem2) AND LISTA = :TabelaId     
                            GROUP BY    
	                        SERVICO,
	                        BASE_CALCULO,
	                        TIPO_CARGA,
	                        BASE_EXCESSO,
	                        VALOR_EXCESSO,
	                        TIPO_DOC,
	                        ARMADOR,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE,
	                        PRECO_MINIMO_DESOVA,
	                        PORTO_HUBPORT,
	                        TIPO_OPER  
                        HAVING COUNT(1) > 1", parametros);

                    foreach (var duplicidade in duplicidades)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Min", value: duplicidade.Min, direction: ParameterDirection.Input);
                        parametros.Add(name: "Max", value: duplicidade.Max, direction: ParameterDirection.Input);

                        con.Execute($"DELETE FROM {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS WHERE AUTONUM = @Min", parametros);
                        con.Execute($"UPDATE {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS SET VARIANTE_LOCAL = 'SVAR'  WHERE AUTONUM = @Max", parametros);
                    }
                }
            }
        }


        public void RemoverDuplicidadesServicosFixos(int tabelaId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    var duplicidades = con.Query<TabelaDuplicada>($@"
                        SELECT 
	                        COUNT(1) Total, 	                        
	                        MIN(AUTONUM) Min,
                            MAX(AUTONUM) Max
                        FROM
	                        {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS
                        WHERE 
	                        TIPO_CARGA IN('SVAR20','SVAR40') AND LISTA = :TabelaId     
                        GROUP BY    
	                        SERVICO,
	                        BASE_CALCULO,
	                        VARIANTE_LOCAL,
	                        BASE_EXCESSO,
	                        VALOR_EXCESSO,
	                        TIPO_DOC,
	                        ARMADOR,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE,
	                        PRECO_MINIMO_DESOVA,
	                        PORTO_HUBPORT,
	                        TIPO_OPER  
                        HAVING COUNT(1) > 1", parametros);

                    foreach (var duplicidade in duplicidades)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Min", value: duplicidade.Min, direction: ParameterDirection.Input);
                        parametros.Add(name: "Max", value: duplicidade.Max, direction: ParameterDirection.Input);

                        con.Execute($"DELETE FROM {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE AUTONUM = :Min", parametros);
                        con.Execute($"UPDATE {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS SET TIPO_CARGA = 'SVAR' WHERE AUTONUM = :Max", parametros);
                    }
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    var duplicidades = con.Query<TabelaDuplicada>($@"
                        SELECT 
	                        COUNT(1) Total, 	                        
	                        MIN(AUTONUM) Min,
                            MAX(AUTONUM) Max
                        FROM
	                        {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS
                        WHERE 
	                        TIPO_CARGA IN('SVAR20','SVAR40') AND LISTA = @TabelaId     
                        GROUP BY    
	                        SERVICO,
	                        BASE_CALCULO,
	                        VARIANTE_LOCAL,
	                        BASE_EXCESSO,
	                        VALOR_EXCESSO,
	                        TIPO_DOC,
	                        ARMADOR,
	                        PRECO_UNITARIO,
	                        PRECO_MINIMO,
	                        PRECO_MAXIMO,
	                        VALOR_ANVISA,
	                        VALOR_ACRESCIMO,
	                        LOCAL_ATRACACAO,
	                        AUTONUM_VINCULADO,
	                        GRUPO_ATRACACAO,
	                        VALOR_ACRESC_PESO,
	                        PESO_LIMITE,
	                        PRECO_MINIMO_DESOVA,
	                        PORTO_HUBPORT,
	                        TIPO_OPER  
                        HAVING COUNT(1) > 1", parametros);

                    foreach (var duplicidade in duplicidades)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Min", value: duplicidade.Min, direction: ParameterDirection.Input);
                        parametros.Add(name: "Max", value: duplicidade.Max, direction: ParameterDirection.Input);

                        con.Execute($"DELETE FROM {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS WHERE AUTONUM = @Min", parametros);
                        con.Execute($"UPDATE {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS SET TIPO_CARGA = 'SVAR' WHERE AUTONUM = @Max", parametros);
                    }
                }
            }
        }

        public void ApagarReembolsosInexistentes(int tabelaId)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    con.Execute($"update TB_LISTA_PRECO_SERVICOS_FIXOS set grupo_atracacao = 0 where LISTA = :TabelaId and grupo_atracacao is null", parametros);
                    con.Execute($"update TB_LISTA_P_S_PERIODO  set grupo_atracacao = 0 where LISTA = :TabelaId and grupo_atracacao is null", parametros);

                    var resultado = con.Query<ServicoFixoVariavel>($@"
                        SELECT A.AUTONUM As ServicoFixoVariavelId, A.SERVICO As ServicoId, A.VARIANTE_LOCAL As VarianteLocal 
                        FROM {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS A INNER JOIN {_schema}.TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM 
                        WHERE tipo_carga NOT IN ('CRGST','BBK','VEIC') AND preco_unitario>0  and (B.FLAG_TAXA_LIBERACAO > 0 or servico=1) AND A.LISTA = :TabelaId", parametros);

                    foreach (var item in resultado)
                    {
                        parametros.Add(name: "ServicoFixoVariavelId", value: item.ServicoFixoVariavelId, direction: ParameterDirection.Input);
                        con.Execute($"DELETE FROM {_schema}.TB_LISTA_PRECO_SERVICOS_FIXOS WHERE AUTONUM = :ServicoFixoVariavelId", parametros);

                    }
                    con.Execute($@"DELETE  FROM SGIPA.TB_LISTA_PRECO_SERVICOS_FIXOS 
                            WHERE tipo_carga IN ('CRGST','BBK','VEIC') AND VARIANTE_LOCAL='MESQ' AND servico=1 and LISTA = :TabelaId", parametros);

                }
            }
            else
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    var resultado = con.Query<ServicoFixoVariavel>($@"SELECT A.AUTONUM As ServicoFixoVariavelId, A.SERVICO As ServicoId, A.VARIANTE_LOCAL As VarianteLocal FROM {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS A INNER JOIN SGIPA..TB_SERVICOS_IPA B ON A.SERVICO = B.AUTONUM WHERE B.FLAG_TAXA_LIBERACAO > 0 AND A.LISTA = @TabelaId", parametros);

                    foreach (var item in resultado)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "ServicoId", value: item.ServicoId, direction: ParameterDirection.Input);
                        parametros.Add(name: "VarianteLocal", value: item.VarianteLocal, direction: ParameterDirection.Input);
                        parametros.Add(name: "ServicoFixoVariavelId", value: item.ServicoFixoVariavelId, direction: ParameterDirection.Input);

                        var existe = con.Query<bool>($"SELECT AUTONUM FROM {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS WHERE LISTA = 1 AND SERVICO = @ServicoId AND VARIANTE_LOCAL = @VarianteLocal", parametros).Any();

                        if (!existe)
                        {
                            con.Execute($"DELETE FROM {_schema}..TB_LISTA_PRECO_SERVICOS_FIXOS WHERE AUTONUM = @ServicoFixoVariavelId", parametros);
                        }
                    }
                }
            }
        }

        public int ObterImportadorPrincipal(int tabelaId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                return con.Query<int>($"SELECT NVL(MAX(IMPORTADOR), 0) FROM {_schema}.TB_LISTAS_PRECOS WHERE AUTONUM = :TabelaId", parametros).FirstOrDefault();
            }
        }

        public int ObterDespachantePrincipal(int tabelaId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                return con.Query<int>($"SELECT NVL(MAX(DESPACHANTE), 0) FROM {_schema}.TB_LISTAS_PRECOS WHERE AUTONUM = :TabelaId", parametros).FirstOrDefault();
            }
        }

        public int ObterColoaderPrincipal(int tabelaId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                return con.Query<int>($"SELECT NVL(MAX(COLOADER), 0) FROM {_schema}.TB_LISTAS_PRECOS WHERE AUTONUM = :TabelaId", parametros).FirstOrDefault();
            }
        }

        public int ObterCoColoaderPrincipal(int tabelaId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                return con.Query<int>($"SELECT NVL(MAX(COCOLOADER), 0) FROM {_schema}.TB_LISTAS_PRECOS WHERE AUTONUM = :TabelaId", parametros).FirstOrDefault();
            }
        }

        public int ObterCoColoader2Principal(int tabelaId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                return con.Query<int>($"SELECT NVL(MAX(COCOLOADER2), 0) FROM {_schema}.TB_LISTAS_PRECOS WHERE AUTONUM = :TabelaId", parametros).FirstOrDefault();
            }
        }

        public int ObterParceiroGrupo(int tabelaId, string tipo)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                parametros.Add(name: "Tipo", value: tipo, direction: ParameterDirection.Input);

                return con.Query<int>($"SELECT NVL(MIN(AUTONUM_PARCEIRO), 0) FROM {_schema}.TB_TP_GRUPOS WHERE AUTONUMLISTA = :TabelaId AND TIPO = :Tipo", parametros).FirstOrDefault();
            }
        }

        public void AtualizarImportadorTabela(int tabelaId, int parceiroId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Importador", value: parceiroId, direction: ParameterDirection.Input);
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                con.Execute($"UPDATE {_schema}.TB_LISTAS_PRECOS SET IMPORTADOR = :Importador WHERE AUTONUM = :TabelaId", parametros);
            }
        }

        public void AtualizarDespachanteTabela(int tabelaId, int parceiroId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Despachante", value: parceiroId, direction: ParameterDirection.Input);
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                con.Execute($"UPDATE {_schema}.TB_LISTAS_PRECOS SET DESPACHANTE = :Despachante WHERE AUTONUM = :TabelaId", parametros);
            }
        }

        public void AtualizarColoaderTabela(int tabelaId, int parceiroId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Coloader", value: parceiroId, direction: ParameterDirection.Input);
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                con.Execute($"UPDATE {_schema}.TB_LISTAS_PRECOS SET COLOADER = :Coloader WHERE AUTONUM = :TabelaId", parametros);
            }
        }

        public void AtualizarCoColoaderTabela(int tabelaId, int parceiroId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "CoColoader", value: parceiroId, direction: ParameterDirection.Input);
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                con.Execute($"UPDATE {_schema}.TB_LISTAS_PRECOS SET COCOLOADER = :CoColoader WHERE AUTONUM = :TabelaId", parametros);
            }
        }

        public void AtualizarCoColoader2Tabela(int tabelaId, int parceiroId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "CoColoader2", value: parceiroId, direction: ParameterDirection.Input);
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                con.Execute($"UPDATE {_schema}.TB_LISTAS_PRECOS SET COCOLOADER2 = :CoColoader2 WHERE AUTONUM = :TabelaId", parametros);
            }
        }

        public void CancelarTabela(int tabelaId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                con.Execute($"UPDATE SGIPA.TB_LISTAS_PRECOS SET FINAL_VALIDADE = SYSDATE, FLAG_LIBERADA = 0 WHERE AUTONUM = :TabelaId", parametros);
            }
        }

        public void AtualizarObservacoesTabela(int tabelaId, string texto)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);
                parametros.Add(name: "Texto", value: texto, direction: ParameterDirection.Input);

                con.Execute("UPDATE SGIPA.TB_LISTAS_PRECOS SET OBS = OBS || ' ' || :Texto WHERE AUTONUM = :TabelaId", parametros);
            }
        }

        public void InativaVendedorAtual(int tabelaId)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                con.Execute("UPDATE SGIPA.TB_LISTAS_PRECOS_VENDEDORES SET DATA_FIM_VIG = SYSDATE - 1 WHERE AUTONUM = (SELECT MAX(DATA_FIM_VIG) FROM SGIPA.TB_LISTAS_PRECOS_VENDEDORES WHERE LISTA = :TabelaId)", parametros);
            }
        }

        public void AtualizarSegmentoParceiro(int id, SegmentoSubCliente segmento)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                switch (segmento)
                {
                    case SegmentoSubCliente.IMPORTADOR:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_IMPORTADOR = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    case SegmentoSubCliente.DESPACHANTE:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_DESPACHANTE = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    case SegmentoSubCliente.COLOADER:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1, FLAG_NVOCC = 1, FLAG_IMPORTADOR = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                    default:
                        con.Execute(@"UPDATE SGIPA.TB_CAD_PARCEIROS SET FLAG_CAPTADOR = 1 WHERE AUTONUM = :Id", parametros);
                        break;
                }
            }
        }
    }
}