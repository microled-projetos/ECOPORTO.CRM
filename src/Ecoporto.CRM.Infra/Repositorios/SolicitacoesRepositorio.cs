using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Filtros;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using Ecoporto.CRM.Business.Helpers;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class SolicitacoesRepositorio : ISolicitacoesRepositorio
    {
        public IEnumerable<SolicitacaoDTO> ObterSolicitacoes(int pagina, int registrosPorPagina, SolicitacoesFiltro filtro, string orderBy, out int totalFiltro)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                var filtroSQL = new StringBuilder();

                if (filtro != null)
                {
                    if (filtro.TipoSolicitacao.HasValue)
                    {
                        parametros.Add(name: "TipoSolicitacao", value: filtro.TipoSolicitacao, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND TipoSolicitacao = :TipoSolicitacao");
                    }

                    if (filtro.UnidadeSolicitacao.HasValue)
                    {
                        parametros.Add(name: "UnidadeSolicitacao", value: filtro.UnidadeSolicitacao, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND UnidadeSolicitacao = :UnidadeSolicitacao");
                    }

                    if (DateTimeHelpers.IsDate(filtro.De))
                    {
                        var de = Convert.ToDateTime(filtro.De);
                        parametros.Add(name: "DataCriacaoDe", value: new DateTime(de.Year, de.Month, de.Day, 0, 0, 0), direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND DataCriacao >= :DataCriacaoDe");
                    }

                    if (DateTimeHelpers.IsDate(filtro.Ate))
                    {
                        var ate = Convert.ToDateTime(filtro.Ate);
                        parametros.Add(name: "DataCriacaoAte", value: new DateTime(ate.Year, ate.Month, ate.Day, 23, 59, 59), direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND DataCriacao <= :DataCriacaoAte");
                    }

                    if (filtro.OcorrenciaId.HasValue)
                    {
                        parametros.Add(name: "OcorrenciaId", value: filtro.OcorrenciaId, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND OcorrenciaId = :OcorrenciaId");
                    }

                    if (filtro.StatusSolicitacao.HasValue)
                    {
                        parametros.Add(name: "StatusSolicitacao", value: filtro.StatusSolicitacao, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND StatusSolicitacao = :StatusSolicitacao");
                    }

                    if (filtro.CriadoPor.HasValue)
                    {
                        parametros.Add(name: "CriadoPor", value: filtro.CriadoPor, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND CriadoPor = :CriadoPor");
                    }

                    if (filtro.Id.HasValue)
                    {
                        parametros.Add(name: "Id", value: filtro.Id, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.Id = :Id ");
                    }

                    if (filtro.Lote.HasValue)
                    {
                        parametros.Add(name: "Lote", value: filtro.Lote, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND (E.Lote = :Lote OR F.Lote = :Lote OR G.Lote = :Lote OR H.Lote = :Lote OR I.LOTE = :Lote) ");
                    }

                    if (filtro.GR.HasValue)
                    {
                        parametros.Add(name: "GR", value: filtro.GR, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND (E.GR = :GR OR F.GR = :GR OR G.GR = :GR OR H.GR = :GR OR I.GR = :GR) ");
                    }

                    if (filtro.Minuta.HasValue)
                    {
                        parametros.Add(name: "Minuta", value: filtro.Minuta, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND (E.Minuta = :Minuta OR F.Minuta = :Minuta OR G.Minuta = :Minuta OR H.Minuta = :Minuta) ");
                    }

                    if (filtro.NotaFiscal.HasValue)
                    {
                        parametros.Add(name: "NotaFiscal", value: filtro.NotaFiscal, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND (E.NotaFiscal = :NotaFiscal OR F.NotaFiscal = :NotaFiscal OR G.NotaFiscal = :NotaFiscal OR H.NotaFiscal = :NotaFiscal) ");
                    }
                }

                var sql = $@"
                    SELECT * FROM (
                        SELECT Oportunidades.*, ROWNUM row_num
                            FROM (
                                SELECT
                                    DISTINCT
                                        A.Id,
                                        A.TipoSolicitacao,
                                        A.StatusSolicitacao,
                                        A.UnidadeSolicitacao,
                                        J.Descricao As UnidadeSolicitacaoDescricao,
                                        A.TipoOperacao,
                                        K.Resumido As TipoOperacaoDescricao,
                                        A.AreaOcorrenciaSolicitacao,
                                        C.Descricao As Ocorrencia,
                                        D.Descricao As Motivo,
                                        CASE
                                            WHEN A.UnidadeSolicitacao <> 1 AND A.UnidadeSolicitacao <> 2 AND A.TipoOperacao = 6 THEN
                                                (
                                                    CASE
                                                        WHEN A.TipoSolicitacao = 1 THEN
                                                            (SELECT Distinct RazaoSocial Cliente FROM CRM.TB_CRM_SOLICITACAO_CANCEL_NF Cancelamento WHERE Cancelamento.SolicitacaoId = A.Id)
                                                        WHEN A.TipoSolicitacao = 2 THEN
                                                            (SELECT Distinct RazaoSocial Cliente FROM CRM.TB_CRM_SOLICITACAO_DESCONTO Desconto WHERE Desconto.SolicitacaoId = A.Id)
                                                        WHEN A.TipoSolicitacao = 3 THEN
                                                            (SELECT Distinct RazaoSocial Cliente FROM CRM.TB_CRM_SOLICITACAO_PRORROGACAO Prorrogacao WHERE Prorrogacao.SolicitacaoId = A.Id)
                                                        WHEN A.TipoSolicitacao = 4 THEN
                                                            (SELECT Distinct RazaoSocial Cliente FROM CRM.TB_CRM_SOLICITACAO_RESTITUICAO Restituicao WHERE Restituicao.SolicitacaoId = A.Id)
                                                        WHEN A.TipoSolicitacao = 5 THEN
                                                            (SELECT Distinct Contas.Descricao FROM CRM.TB_CRM_SOLICITACAO_FORMA_PGTO FormaPgto INNER JOIN TB_CRM_CONTAS Contas ON FormaPgto.NotaClienteId = Contas.Id WHERE FormaPgto.SolicitacaoId = A.Id)
                                                     END
                                                )
                                            ELSE
                                                (                           
                                                CASE
                                                    WHEN A.TipoSolicitacao = 1 THEN
                                                        (SELECT DISTINCT DECODE(NVL(Cancelamento.NotaFiscalId, 0), 0, Conta.Descricao, Nota.NOMCLI) Cliente
                                                            FROM CRM.TB_CRM_SOLICITACAO_CANCEL_NF Cancelamento LEFT JOIN CRM.TB_CRM_CONTAS Conta ON Cancelamento.ContaId = Conta.Id LEFT JOIN
                                                            FATURA.FATURANOTA Nota ON Cancelamento.NotaFiscalId = Nota.Id WHERE Cancelamento.SolicitacaoId = A.Id AND ROWNUM < 2)
                                                    WHEN A.TipoSolicitacao = 2 AND C.Id = 4 AND A.TipoOperacao <> 3 THEN
                                                        (SELECT DISTINCT DECODE(NVL(Cancelamento.NotaFiscalId, 0), 0, Conta.Descricao, Nota.NOMCLI) Cliente
                                                            FROM CRM.TB_CRM_SOLICITACAO_CANCEL_NF Cancelamento LEFT JOIN CRM.TB_CRM_CONTAS Conta ON Cancelamento.ContaId = Conta.Id LEFT JOIN
                                                            FATURA.FATURANOTA Nota ON Cancelamento.NotaFiscalId = Nota.Id WHERE Cancelamento.SolicitacaoId = A.Id AND ROWNUM < 2)
                                                      WHEN A.TipoSolicitacao = 2 AND A.TipoOperacao = 3 THEN
                                                        (
                                                            SELECT
                                                                Distinct Parceiro.Razao As Cliente
                                                            FROM
                                                                REDEX.TB_BOOKING Booking
                                                            LEFT JOIN
                                                                CRM.TB_CRM_SOLICITACAO_DESCONTO Desconto ON Booking.Reference = Desconto.Reserva                                        
                                                            LEFT JOIN
                                                                REDEX.TB_SERVICOS_FATURADOS Servicos ON Booking.AUTONUM_BOO = Servicos.BOOKING                                           
                                                            LEFT JOIN
                                                                REDEX.TB_CAD_PARCEIROS Parceiro ON Servicos.Cliente_Fatura = Parceiro.Autonum
                                                            WHERE
                                                                Desconto.SolicitacaoId = A.Id AND ROWNUM < 2
                                                        )
                                                    WHEN A.TipoSolicitacao = 2 AND C.Id <> 4 AND A.TipoOperacao <> 3 THEN
                                                        (SELECT
                                                            DISTINCT
                                                                DECODE(NVL(Desconto.Minuta, 0), 0, Gr.Cliente ,Minuta.Cliente) Cliente
                                                            FROM
                                                                CRM.TB_CRM_SOLICITACAO_DESCONTO Desconto
                                                            LEFT JOIN
                                                                (SELECT
                                                                    c.Razao as Cliente,
                                                                    gr.Seq_GR,
                                                                    bl.autonum as Lote
                                                                FROM
                                                                    SGIPA.TB_BL bl
                                                                LEFT JOIN
                                                                     SGIPA.TB_GR_BL gr ON gr.BL = bl.AUTONUM
                                                                LEFT JOIN
                                                                    SGIPA.TB_CAD_PARCEIROS C ON bl.Importador = c.Autonum ) Gr ON Desconto.Lote = Gr.Lote
                                                            LEFT JOIN
                                                                (
                                                                    SELECT
                                                                        A.AUTONUM,
                                                                        B.RAzao As Cliente
                                                                    FROM
                                                                        OPERADOR.TB_MINUTA A
                                                                    INNER JOIN
                                                                        OPERADOR.TB_CAD_CLIENTES B ON A.Cliente = B.AUTONUM) Minuta ON Desconto.Minuta = Minuta.AUTONUM
                                                        WHERE Desconto.SolicitacaoId = A.Id AND ROWNUM < 2)
                                                    WHEN A.TipoSolicitacao = 3 THEN           
                                                       (SELECT DISTINCT DECODE(NVL(Prorrogacao.NotaFiscalId,0), 0, Conta.Descricao, Nota.NOMCLI) Cliente
                                                            FROM CRM.TB_CRM_SOLICITACAO_PRORROGACAO Prorrogacao LEFT JOIN CRM.TB_CRM_CONTAS Conta ON Prorrogacao.ContaId = Conta.Id LEFT JOIN
                                                            FATURA.FATURANOTA Nota ON Prorrogacao.NotaFiscalId = Nota.Id WHERE Prorrogacao.SolicitacaoId = A.Id AND ROWNUM < 2)
                                                    WHEN A.TipoSolicitacao = 4 THEN
                                                        (SELECT DISTINCT Conta.Descricao Cliente
                                                            FROM CRM.TB_CRM_SOLICITACAO_RESTITUICAO Restituicao LEFT JOIN CRM.TB_CRM_CONTAS Conta ON Restituicao.FavorecidoId = Conta.Id LEFT JOIN
                                                            FATURA.FATURANOTA Nota ON Restituicao.NotaFiscalId = Nota.Id WHERE Restituicao.SolicitacaoId = A.Id AND ROWNUM < 2)
                                                    WHEN A.TipoSolicitacao = 5 THEN
                                                            (SELECT Distinct Contas.Descricao FROM CRM.TB_CRM_SOLICITACAO_FORMA_PGTO FormaPgto
                                                                INNER JOIN TB_CRM_CONTAS Contas ON FormaPgto.NotaClienteId = Contas.Id WHERE FormaPgto.SolicitacaoId = A.Id AND ROWNUM < 2)
                                                    END                                                                                                       
                                                ) END Cliente,
                                        B.Login As CriadoPor,
                                        count(*) over() TotalLinhas
                                    FROM
                                        CRM.TB_CRM_SOLICITACOES A
                                    LEFT JOIN
                                        CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id
                                    LEFT JOIN
                                        CRM.TB_CRM_SOLICITACAO_OCORRENCIAS C ON A.OcorrenciaId = C.Id
                                    LEFT JOIN   
                                        CRM.TB_CRM_SOLICITACAO_MOTIVOS D ON A.MotivoId = D.Id
                                    LEFT JOIN
                                        (
                                            SELECT SolicitacaoId, Lote, NFE As NotaFiscal, 0 As GR, 0 As Minuta FROM TB_CRM_SOLICITACAO_CANCEL_NF
                                        ) E ON E.SolicitacaoId = A.Id
                                    LEFT JOIN
                                        (
                                            SELECT SolicitacaoId, 0 As Lote, NFE As NotaFiscal, 0 As GR, 0 As Minuta FROM TB_CRM_SOLICITACAO_PRORROGACAO
                                        ) F ON F.SolicitacaoId = A.Id
                                    LEFT JOIN
                                        (
                                            SELECT SolicitacaoId, Lote, NFE As NotaFiscal, 0 As GR, 0 As Minuta FROM TB_CRM_SOLICITACAO_RESTITUICAO
                                        ) G ON G.SolicitacaoId = A.Id
                                    LEFT JOIN
                                        (
                                            SELECT SolicitacaoId, Lote, 0 As NotaFiscal, MinutaGrId As GR, Minuta FROM TB_CRM_SOLICITACAO_DESCONTO
                                        ) H ON H.SolicitacaoId = A.Id
                                    LEFT JOIN
                                        (
                                            SELECT SolicitacaoId, Lote, 0 As NotaFiscal, GR, 0 As Minuta FROM TB_CRM_SOLICITACAO_FORMA_PGTO
                                        ) I ON I.SolicitacaoId = A.Id
                                    LEFT JOIN
                                        CRM.TB_CRM_SOLICITACAO_UNIDADES J ON A.UnidadeSolicitacao = J.Id
                                    LEFT JOIN   
                                        CRM.TB_CRM_SOLICITACAO_TIPO_OPER K ON A.TipoOperacao = K.Id
                                    WHERE
                                        A.Id > 0 {filtroSQL.ToString()} {orderBy}) Oportunidades
                            WHERE ROWNUM < (({pagina} * {registrosPorPagina}) + 1))
                        WHERE row_num >= ((({pagina} - 1) * {registrosPorPagina}) + 1)";

                var query = con.Query<SolicitacaoDTO>(sql, parametros);

                totalFiltro = query.Select(c => c.TotalLinhas).FirstOrDefault();

                return query;
            }
        }

        public int ObterTotalSolicitacoes()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<int>($@"SELECT COUNT(1) FROM TB_CRM_SOLICITACOES").Single();
            }
        }

        public IEnumerable<SolicitacaoDTO> ObterSolicitacoesAcessoExterno(int pagina, int registrosPorPagina, SolicitacoesFiltro filtro, string orderBy, out int totalFiltro, SolicitacoesUsuarioExternoFiltro usuarioExternoFiltro)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                var filtroSQL = new StringBuilder();
                var filtroAcessoExterno = new StringBuilder();

                parametros.Add(name: "UsuarioExternoId", value: usuarioExternoFiltro.UsuarioId, direction: ParameterDirection.Input);

                if (filtro != null)
                {
                    if (filtro.TipoSolicitacao.HasValue)
                    {
                        parametros.Add(name: "TipoSolicitacao", value: filtro.TipoSolicitacao, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND TipoSolicitacao = :TipoSolicitacao");
                    }

                    if (filtro.UnidadeSolicitacao.HasValue)
                    {
                        parametros.Add(name: "UnidadeSolicitacao", value: filtro.UnidadeSolicitacao, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND UnidadeSolicitacao = :UnidadeSolicitacao");
                    }

                    if (DateTimeHelpers.IsDate(filtro.De))
                    {
                        var de = Convert.ToDateTime(filtro.De);
                        parametros.Add(name: "DataCriacaoDe", value: new DateTime(de.Year, de.Month, de.Day, 0, 0, 0), direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND DataCriacao >= :DataCriacaoDe");
                    }

                    if (DateTimeHelpers.IsDate(filtro.Ate))
                    {
                        var ate = Convert.ToDateTime(filtro.Ate);
                        parametros.Add(name: "DataCriacaoAte", value: new DateTime(ate.Year, ate.Month, ate.Day, 23, 59, 59), direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND DataCriacao <= :DataCriacaoAte");
                    }

                    if (filtro.OcorrenciaId.HasValue)
                    {
                        parametros.Add(name: "OcorrenciaId", value: filtro.OcorrenciaId, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND OcorrenciaId = :OcorrenciaId");
                    }

                    if (filtro.StatusSolicitacao.HasValue)
                    {
                        parametros.Add(name: "StatusSolicitacao", value: filtro.StatusSolicitacao, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND StatusSolicitacao = :StatusSolicitacao");
                    }

                    if (filtro.CriadoPor.HasValue)
                    {
                        parametros.Add(name: "CriadoPor", value: filtro.CriadoPor, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND CriadoPor = :CriadoPor");
                    }

                    if (filtro.Id.HasValue)
                    {
                        parametros.Add(name: "Id", value: filtro.Id, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND B.Id = :Id ");
                    }

                    if (filtro.Lote.HasValue)
                    {
                        parametros.Add(name: "Lote", value: filtro.Lote, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND (E.Lote = :Lote OR F.Lote = :Lote OR G.Lote = :Lote OR H.Lote = :Lote OR I.LOTE = :Lote) ");
                    }

                    if (filtro.GR.HasValue)
                    {
                        parametros.Add(name: "GR", value: filtro.GR, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND (E.GR = :GR OR F.GR = :GR OR G.GR = :GR OR H.GR = :GR OR I.GR = :GR) ");
                    }

                    if (filtro.Minuta.HasValue)
                    {
                        parametros.Add(name: "Minuta", value: filtro.Minuta, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND (E.Minuta = :Minuta OR F.Minuta = :Minuta OR G.Minuta = :Minuta OR H.Minuta = :Minuta) ");
                    }

                    if (filtro.NotaFiscal.HasValue)
                    {
                        parametros.Add(name: "NotaFiscal", value: filtro.NotaFiscal, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND (E.NotaFiscal = :NotaFiscal OR F.NotaFiscal = :NotaFiscal OR G.NotaFiscal = :NotaFiscal OR H.NotaFiscal = :NotaFiscal) ");
                    }
                }

                var sql = $@"
                     SELECT * FROM (
                        SELECT Solicitacoes.*, ROWNUM row_num
                            FROM ( 
                                SELECT 
                                    Id,
                                    TipoSolicitacao,
                                    StatusSolicitacao,
                                    UnidadeSolicitacao,
                                    UnidadeSolicitacaoDescricao,
                                    TipoOperacao,
                                    TipoOperacaoDescricao,
                                    AreaOcorrenciaSolicitacao,
                                    Ocorrencia,
                                    Motivo,
                                    Cliente,        
                                    CriadoPor,
                                    CriadoPorId,
                                    count(*) over() TotalLinhas
                                FROM
                                    (";

                if (usuarioExternoFiltro.AcessoCancelamento)
                {
                    sql += $@"SELECT
                                DISTINCT
                                    B.Id,
                                    B.TipoSolicitacao,
                                    B.StatusSolicitacao,
                                    B.UnidadeSolicitacao,
                                    H.Descricao As UnidadeSolicitacaoDescricao,
                                    B.TipoOperacao,
                                    I.Resumido As TipoOperacaoDescricao,
                                    B.AreaOcorrenciaSolicitacao,
                                    D.Descricao As Ocorrencia,
                                    E.Descricao As Motivo,
                                    (
                                        SELECT 
                                            DISTINCT 
                                                DECODE(NVL(Cancelamento.NotaFiscalId, 0), 0, Conta.Descricao, Nota.NOMCLI) Cliente
                                        FROM 
                                            CRM.TB_CRM_SOLICITACAO_CANCEL_NF Cancelamento 
                                        LEFT JOIN 
                                            CRM.TB_CRM_CONTAS Conta ON Cancelamento.ContaId = Conta.Id 
                                        LEFT JOIN
                                            FATURA.FATURANOTA Nota ON Cancelamento.NotaFiscalId = Nota.Id 
                                        WHERE 
                                            Cancelamento.SolicitacaoId = B.Id 
                                        AND 
                                            ROWNUM = 1
                                    ) Cliente,        
                                    C.Login As CriadoPor,
                                    A.Id As CriadoPorId
                                FROM
                                    CRM.TB_CRM_SOLICITACOES B
                                LEFT JOIN
                                    CRM.TB_CRM_SOLICITACAO_CANCEL_NF A ON A.SolicitacaoId = B.Id
                                LEFT JOIN
                                    CRM.TB_CRM_USUARIOS C ON B.CriadoPor = C.Id
                                LEFT JOIN
                                    CRM.TB_CRM_SOLICITACAO_OCORRENCIAS D ON B.OcorrenciaId = D.Id
                                LEFT JOIN   
                                    CRM.TB_CRM_SOLICITACAO_MOTIVOS E ON B.MotivoId = E.Id
                                LEFT JOIN
                                    CRM.TB_CRM_CONTAS F ON A.ContaId = F.Id
                                LEFT JOIN
                                    FATURA.FATURANOTA G ON A.NotaFiscalId = G.Id 
                                LEFT JOIN
                                    CRM.TB_CRM_SOLICITACAO_UNIDADES H ON B.UnidadeSolicitacao = H.Id
                                LEFT JOIN   
                                    CRM.TB_CRM_SOLICITACAO_TIPO_OPER I ON B.TipoOperacao = I.Id
                                WHERE
                                    (
                                        (
                                            (F.Documento IN (SELECT B.DOCUMENTO FROM CRM.TB_CRM_USUARIOS_CONTAS A INNER JOIN CRM.TB_CRM_CONTAS B ON A.CONTAID = B.ID WHERE A.USUARIOID = :UsuarioExternoId) AND NVL(A.NotaFiscalId, 0) = 0)
                                        OR
                                            (G.CGCCPF IN (SELECT B.DOCUMENTO FROM CRM.TB_CRM_USUARIOS_CONTAS A INNER JOIN CRM.TB_CRM_CONTAS B ON A.CONTAID = B.ID WHERE A.USUARIOID = :UsuarioExternoId) AND NVL(A.NotaFiscalId, 0) > 0)
                                        ) 
                                        OR B.CriadoPor = :UsuarioExternoId
                                    )
                                AND
                                    B.TipoSolicitacao = 1 {filtroSQL.ToString()}";
                }

                if (usuarioExternoFiltro.AcessoDesconto)
                {
                    if (usuarioExternoFiltro.AcessoCancelamento)
                    {
                        sql += $@" UNION ALL ";
                    }

                    sql += $@" 
    
                            SELECT
                                DISTINCT
                                    B.Id,
                                    B.TipoSolicitacao,
                                    B.StatusSolicitacao,
                                    B.UnidadeSolicitacao,
                                    I.Descricao As UnidadeSolicitacaoDescricao,
                                    B.TipoOperacao,
                                    J.Resumido As TipoOperacaoDescricao,
                                    B.AreaOcorrenciaSolicitacao,
                                    D.Descricao As Ocorrencia,
                                    E.Descricao As Motivo,
                                    CASE                                           
                                        WHEN B.TipoSolicitacao = 2 AND B.TipoOperacao = 1 THEN
                                            (
                                                SELECT 
                                                    DISTINCT 
                                                        CASE WHEN (B.UnidadeSolicitacao <> 1 AND B.UnidadeSolicitacao <> 2) THEN Cliente.Descricao ELSE Parceiro.Razao END As ClienteDescricao
                                                FROM 
                                                    CRM.TB_CRM_SOLICITACAO_DESCONTO Desconto 
                                                LEFT JOIN 
                                                    CRM.TB_CRM_CONTAS Cliente ON Desconto.ClienteId = Cliente.Id 
                                                LEFT JOIN
                                                    CRM.TB_CRM_CONTAS Indicador ON Desconto.IndicadorId = Indicador.Id 
                                                LEFT JOIN
                                                    SGIPA.TB_CAD_PARCEIROS Parceiro ON Desconto.ClienteId = Parceiro.AUTONUM
                                                WHERE 
                                                    Desconto.SolicitacaoId = B.Id AND ROWNUM = 1
                                            )
                                    END Cliente,    
                                    C.Login As CriadoPor,
                                    C.Id As CriadoPorId
                                FROM
                                    CRM.TB_CRM_SOLICITACOES B
                                LEFT JOIN
                                    CRM.TB_CRM_SOLICITACAO_DESCONTO A ON A.SolicitacaoId = B.Id                                    
                                LEFT JOIN
                                    CRM.TB_CRM_USUARIOS C ON B.CriadoPor = C.Id
                                LEFT JOIN
                                    CRM.TB_CRM_SOLICITACAO_OCORRENCIAS D ON B.OcorrenciaId = D.Id
                                LEFT JOIN   
                                    CRM.TB_CRM_SOLICITACAO_MOTIVOS E ON B.MotivoId = E.Id        
                                LEFT JOIN
                                    SGIPA.TB_CAD_PARCEIROS F ON A.ClienteId = F.AUTONUM
                                LEFT JOIN
                                    SGIPA.TB_CAD_PARCEIROS H ON A.IndicadorId = H.AUTONUM
                                LEFT JOIN
                                    CRM.TB_CRM_SOLICITACAO_UNIDADES I ON B.UnidadeSolicitacao = I.Id
                                LEFT JOIN   
                                    CRM.TB_CRM_SOLICITACAO_TIPO_OPER J ON B.TipoOperacao = J.Id
                                WHERE
                                    ( 
                                        (
                                            F.CGC IN (SELECT B.DOCUMENTO FROM CRM.TB_CRM_USUARIOS_CONTAS A INNER JOIN SGIPA.TB_CRM_CONTAS B ON A.CONTAID = B.ID WHERE A.USUARIOID = :UsuarioExternoId)
                                        OR
                                            H.CGC IN (SELECT B.DOCUMENTO FROM CRM.TB_CRM_USUARIOS_CONTAS A INNER JOIN CRM.TB_CRM_CONTAS B ON A.CONTAID = B.ID WHERE A.USUARIOID = :UsuarioExternoId)
                                        ) 
                                        OR B.CriadoPor = :UsuarioExternoId
                                    )   
                                AND
                                    B.TipoSolicitacao = 2 AND B.TipoOperacao = 1 {filtroSQL.ToString()} ";

                    sql += $@" UNION ALL
    
                            SELECT
                                DISTINCT
                                    B.Id,
                                    B.TipoSolicitacao,
                                    B.StatusSolicitacao,
                                    B.UnidadeSolicitacao,
                                    I.Descricao As UnidadeSolicitacaoDescricao,
                                    B.TipoOperacao,
                                    J.Resumido As TipoOperacaoDescricao,
                                    B.AreaOcorrenciaSolicitacao,
                                    D.Descricao As Ocorrencia,
                                    E.Descricao As Motivo,
                                    CASE                                           
                                        WHEN B.TipoSolicitacao = 2 AND B.TipoOperacao = 3 THEN
                                            (
                                                SELECT
                                                    Distinct Parceiro.RAZAO 
                                                FROM
                                                    REDEX.TB_BOOKING Booking
                                                LEFT JOIN
                                                    CRM.TB_CRM_SOLICITACAO_DESCONTO Desconto ON Booking.Reference = Desconto.Reserva                                        
                                                LEFT JOIN
                                                    REDEX.TB_SERVICOS_FATURADOS Servicos ON Booking.AUTONUM_BOO = Servicos.BOOKING                                           
                                                LEFT JOIN
                                                    REDEX.TB_CAD_PARCEIROS Parceiro ON Servicos.Cliente_Fatura = Parceiro.Autonum
                                                WHERE
                                                    Desconto.SolicitacaoId = A.Id AND ROWNUM = 1
                                            )
                                    END Cliente,    
                                    C.Login As CriadoPor,
                                    C.Id As CriadoPorId
                                FROM
                                    CRM.TB_CRM_SOLICITACOES B
                                LEFT JOIN                                
                                    CRM.TB_CRM_SOLICITACAO_DESCONTO A ON A.SolicitacaoId = B.Id
                                LEFT JOIN
                                    CRM.TB_CRM_USUARIOS C ON B.CriadoPor = C.Id
                                LEFT JOIN
                                    CRM.TB_CRM_SOLICITACAO_OCORRENCIAS D ON B.OcorrenciaId = D.Id
                                LEFT JOIN   
                                    CRM.TB_CRM_SOLICITACAO_MOTIVOS E ON B.MotivoId = E.Id        
                                LEFT JOIN
                                    REDEX.TB_BOOKING F ON A.Reserva = F.REFERENCE
                                LEFT JOIN
                                    REDEX.TB_SERVICOS_FATURADOS G ON F.AUTONUM_BOO = G.BOOKING                                           
                                LEFT JOIN
                                    REDEX.TB_CAD_PARCEIROS H ON G.Cliente_Fatura = H.Autonum
                                LEFT JOIN
                                    CRM.TB_CRM_SOLICITACAO_UNIDADES I ON B.UnidadeSolicitacao = I.Id
                                LEFT JOIN   
                                    CRM.TB_CRM_SOLICITACAO_TIPO_OPER J ON B.TipoOperacao = J.Id
                                WHERE
                                    (
                                        H.CGC IN (SELECT B.DOCUMENTO FROM CRM.TB_CRM_USUARIOS_CONTAS A INNER JOIN CRM.TB_CRM_CONTAS B ON A.CONTAID = B.ID WHERE A.USUARIOID = :UsuarioExternoId) OR B.CriadoPor = :UsuarioExternoId 
                                    )
                                AND
                                    B.TipoSolicitacao = 2 AND B.TipoOperacao = 3 {filtroSQL.ToString()} ";

                }

                if (usuarioExternoFiltro.AcessoRestituicao)
                {
                    if (usuarioExternoFiltro.AcessoDesconto)
                    {
                        sql += $@" UNION ALL ";
                    }

                    sql += $@"
    
                            SELECT
                                DISTINCT
                                    B.Id,
                                    B.TipoSolicitacao,
                                    B.StatusSolicitacao,
                                    B.UnidadeSolicitacao,
                                    G.Descricao As UnidadeSolicitacaoDescricao,
                                    B.TipoOperacao,
                                    H.Resumido As TipoOperacaoDescricao,
                                    B.AreaOcorrenciaSolicitacao,
                                    D.Descricao As Ocorrencia,
                                    E.Descricao As Motivo,
                                    (
                                        SELECT 
                                            DISTINCT Conta.Descricao 
                                        FROM 
                                            CRM.TB_CRM_SOLICITACAO_RESTITUICAO Restituicao 
                                        LEFT JOIN 
                                            CRM.TB_CRM_CONTAS Conta ON Restituicao.FavorecidoId = Conta.Id 
                                        LEFT JOIN
                                            FATURA.FATURANOTA Nota ON Restituicao.NotaFiscalId = Nota.Id 
                                        WHERE 
                                            Restituicao.SolicitacaoId = B.Id AND ROWNUM = 1
                                    ) As Cliente,    
                                    C.Login As CriadoPor,
                                    C.Id As CriadoPorId
                                FROM
                                    CRM.TB_CRM_SOLICITACOES B
                                LEFT JOIN
                                    CRM.TB_CRM_SOLICITACAO_RESTITUICAO A ON A.SolicitacaoId = B.Id
                                LEFT JOIN
                                    CRM.TB_CRM_USUARIOS C ON B.CriadoPor = C.Id
                                LEFT JOIN
                                    CRM.TB_CRM_SOLICITACAO_OCORRENCIAS D ON B.OcorrenciaId = D.Id
                                LEFT JOIN   
                                    CRM.TB_CRM_SOLICITACAO_MOTIVOS E ON B.MotivoId = E.Id
                                LEFT JOIN
                                    CRM.TB_CRM_CONTAS F ON A.FavorecidoId = F.Id
                                LEFT JOIN
                                    CRM.TB_CRM_SOLICITACAO_UNIDADES G ON B.UnidadeSolicitacao = G.Id
                                LEFT JOIN   
                                    CRM.TB_CRM_SOLICITACAO_TIPO_OPER H ON B.TipoOperacao = H.Id
                                WHERE
                                    (
                                        F.Documento IN (SELECT B.DOCUMENTO FROM CRM.TB_CRM_USUARIOS_CONTAS A INNER JOIN CRM.TB_CRM_CONTAS B ON A.CONTAID = B.ID WHERE A.USUARIOID = :UsuarioExternoId)  OR B.CriadoPor = :UsuarioExternoId 
                                    )
                                AND
                                    B.TipoSolicitacao = 4 {filtroSQL.ToString()} ";

                }

                if (usuarioExternoFiltro.AcessoProrrogacao)
                {
                    if (usuarioExternoFiltro.AcessoRestituicao)
                    {
                        sql += $@" UNION ALL ";
                    }

                    sql += $@"    
                                SELECT
                                    DISTINCT
                                        B.Id,
                                        B.TipoSolicitacao,
                                        B.StatusSolicitacao,
                                        B.UnidadeSolicitacao,
                                        H.Descricao As UnidadeSolicitacaoDescricao,
                                        B.TipoOperacao,
                                        I.Resumido As TipoOperacaoDescricao,
                                        B.AreaOcorrenciaSolicitacao,
                                        D.Descricao As Ocorrencia,
                                        E.Descricao As Motivo,
                                        (
                                            SELECT 
                                                DISTINCT DECODE(NVL(Prorrogacao.NotaFiscalId,0), 0, Conta.Descricao, Nota.NOMCLI) Razao
                                            FROM 
                                                CRM.TB_CRM_SOLICITACAO_PRORROGACAO Prorrogacao 
                                            LEFT JOIN 
                                                CRM.TB_CRM_CONTAS Conta ON Prorrogacao.ContaId = Conta.Id 
                                            LEFT JOIN
                                                FATURA.FATURANOTA Nota ON Prorrogacao.NotaFiscalId = Nota.Id 
                                            WHERE 
                                                Prorrogacao.SolicitacaoId = B.Id AND ROWNUM = 1
                                        ) Cliente,    
                                        C.Login As CriadoPor,
                                        C.Id As CriadoPorId
                                    FROM
                                        CRM.TB_CRM_SOLICITACOES B
                                    LEFT JOIN
                                        CRM.TB_CRM_SOLICITACAO_PRORROGACAO A ON A.SolicitacaoId = B.Id
                                    LEFT JOIN
                                        CRM.TB_CRM_USUARIOS C ON B.CriadoPor = C.Id
                                    LEFT JOIN
                                        CRM.TB_CRM_SOLICITACAO_OCORRENCIAS D ON B.OcorrenciaId = D.Id
                                    LEFT JOIN   
                                        CRM.TB_CRM_SOLICITACAO_MOTIVOS E ON B.MotivoId = E.Id
                                    LEFT JOIN
                                        CRM.TB_CRM_CONTAS F ON A.ContaId = F.Id
                                    LEFT JOIN
                                        FATURA.FATURANOTA G ON A.NotaFiscalId = G.Id    
                                    LEFT JOIN
                                        CRM.TB_CRM_SOLICITACAO_UNIDADES H ON B.UnidadeSolicitacao = H.Id
                                    LEFT JOIN   
                                        CRM.TB_CRM_SOLICITACAO_TIPO_OPER I ON B.TipoOperacao = I.Id
                                     WHERE
                                        (
                                            (
                                                (F.Documento IN (SELECT B.DOCUMENTO FROM CRM.TB_CRM_USUARIOS_CONTAS A INNER JOIN CRM.TB_CRM_CONTAS B ON A.CONTAID = B.ID WHERE A.USUARIOID = :UsuarioExternoId) AND NVL(A.NotaFiscalId, 0) = 0)
                                            OR
                                                (G.CGCCPF IN (SELECT B.DOCUMENTO FROM CRM.TB_CRM_USUARIOS_CONTAS A INNER JOIN CRM.TB_CRM_CONTAS B ON A.CONTAID = B.ID WHERE A.USUARIOID = :UsuarioExternoId) AND NVL(A.NotaFiscalId, 0) > 0)
                                            )  
                                            OR B.CriadoPor = :UsuarioExternoId
                                        )
                                    AND
                                        B.TipoSolicitacao = 3 {filtroSQL.ToString()} ";

                }

                sql += $@" ) {orderBy} 
                        ) Solicitacoes
                            WHERE ROWNUM < (({pagina} * {registrosPorPagina}) + 1)) 
                        WHERE row_num >= ((({pagina} - 1) * {registrosPorPagina}) + 1) ";

                var query = con.Query<SolicitacaoDTO>(sql, parametros);

                totalFiltro = query.Select(c => c.TotalLinhas).FirstOrDefault();

                return query;
            }
        }

        public int ObterTotalSolicitacoesAcessoExterno()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<int>($@"SELECT COUNT(1) FROM TB_CRM_SOLICITACOES").Single();
            }
        }

        public int Cadastrar(SolicitacaoComercial solicitacao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TipoSolicitacao", value: solicitacao.TipoSolicitacao, direction: ParameterDirection.Input);
                parametros.Add(name: "StatusSolicitacao", value: solicitacao.StatusSolicitacao, direction: ParameterDirection.Input);
                parametros.Add(name: "UnidadeSolicitacao", value: solicitacao.UnidadeSolicitacao, direction: ParameterDirection.Input);
                parametros.Add(name: "AreaOcorrenciaSolicitacao", value: solicitacao.AreaOcorrenciaSolicitacao, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoOperacao", value: solicitacao.TipoOperacao, direction: ParameterDirection.Input);
                parametros.Add(name: "OcorrenciaId", value: solicitacao.OcorrenciaId, direction: ParameterDirection.Input);
                parametros.Add(name: "MotivoId", value: solicitacao.MotivoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Justificativa", value: solicitacao.Justificativa, direction: ParameterDirection.Input);
                parametros.Add(name: "CriadoPor", value: solicitacao.CriadoPor, direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_SOLICITACOES 
                            ( 
                                Id, 
                                TipoSolicitacao, 
                                StatusSolicitacao,                 
                                UnidadeSolicitacao, 
                                AreaOcorrenciaSolicitacao, 
                                TipoOperacao,
                                OcorrenciaId, 
                                MotivoId, 
                                Justificativa,
                                CriadoPor
                            ) VALUES ( 
                                CRM.SEQ_CRM_SOLICITACOES.NEXTVAL, 
                                :TipoSolicitacao,  
                                :StatusSolicitacao,                 
                                :UnidadeSolicitacao, 
                                :AreaOcorrenciaSolicitacao, 
                                :TipoOperacao,
                                :OcorrenciaId, 
                                :MotivoId, 
                                :Justificativa,
                                :CriadoPor
                            ) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void Atualizar(SolicitacaoComercial solicitacao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "UnidadeSolicitacao", value: solicitacao.UnidadeSolicitacao, direction: ParameterDirection.Input);
                parametros.Add(name: "AreaOcorrenciaSolicitacao", value: solicitacao.AreaOcorrenciaSolicitacao, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoOperacao", value: solicitacao.TipoOperacao, direction: ParameterDirection.Input);
                parametros.Add(name: "OcorrenciaId", value: solicitacao.OcorrenciaId, direction: ParameterDirection.Input);
                parametros.Add(name: "MotivoId", value: solicitacao.MotivoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Justificativa", value: solicitacao.Justificativa, direction: ParameterDirection.Input);
                parametros.Add(name: "CriadoPor", value: solicitacao.CriadoPor, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: solicitacao.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_SOLICITACOES 
                            SET                                                            
                                UnidadeSolicitacao = :UnidadeSolicitacao, 
                                AreaOcorrenciaSolicitacao = :AreaOcorrenciaSolicitacao, 
                                TipoOperacao = :TipoOperacao,
                                OcorrenciaId = :OcorrenciaId, 
                                MotivoId = :MotivoId, 
                                Justificativa = :Justificativa,
                                CriadoPor = :CriadoPor
                            WHERE
                                Id = :Id", parametros);
            }
        }

        public void AtualizarResumoRestituicao(SolicitacaoComercial solicitacao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ValorDevido", value: solicitacao.ValorDevido, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorCobrado", value: solicitacao.ValorCobrado, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorCredito", value: solicitacao.ValorCredito, direction: ParameterDirection.Input);
                parametros.Add(name: "HabilitaValorDevido", value: solicitacao.HabilitaValorDevido.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: solicitacao.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_SOLICITACOES SET ValorDevido = :ValorDevido, ValorCobrado = :ValorCobrado, ValorCredito = :ValorCredito, HabilitaValorDevido = :HabilitaValorDevido WHERE Id = :Id", parametros);
            }
        }

        public void AtualizarStatusSolicitacao(StatusSolicitacao status, int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "StatusSolicitacao", value: status, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_SOLICITACOES SET StatusSolicitacao = :StatusSolicitacao WHERE Id = :Id", parametros);
            }
        }

        public SolicitacaoComercial ObterSolicitacaoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<SolicitacaoComercial>(@"SELECT * FROM CRM.TB_CRM_SOLICITACOES WHERE Id = :sId", new { sId = id }).FirstOrDefault();
            }
        }

        public void Excluir(int id, TipoSolicitacao tipoSolicitacao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                var parametros = new DynamicParameters();

                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                using (var transaction = con.BeginTransaction())
                {
                    switch (tipoSolicitacao)
                    {
                        case TipoSolicitacao.CANCELAMENTO_NF:
                            con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_CANCEL_NF WHERE SolicitacaoId = :Id", parametros, transaction);
                            break;
                        case TipoSolicitacao.DESCONTO:
                            con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_DESCONTO WHERE SolicitacaoId = :Id", parametros, transaction);
                            break;
                        case TipoSolicitacao.PRORROGACAO_BOLETO:
                            con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_PRORROGACAO WHERE SolicitacaoId = :Id", parametros, transaction);
                            break;
                        case TipoSolicitacao.RESTITUICAO:
                            con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_RESTITUICAO WHERE SolicitacaoId = :Id", parametros, transaction);
                            break;
                        case TipoSolicitacao.OUTROS:
                            con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_FORMA_PGTO WHERE SolicitacaoId = :Id", parametros, transaction);
                            break;
                    }

                    con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACOES WHERE Id = :Id", parametros, transaction);

                    transaction.Commit();
                }
            }
        }

        public SolicitacaoDTO ObterDetalhesSolicitacao(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoDTO>($@"
                        SELECT 
                            A.Id,
                            A.TipoSolicitacao,
                            A.StatusSolicitacao,
                            A.UnidadeSolicitacao,
                            E.Descricao As UnidadeSolicitacaoDescricao,
                            A.TipoOperacao,
                            F.Descricao As TipoOperacaoDescricao,
                            F.Resumido As TipoOperacaoResumida,
                            A.AreaOcorrenciaSolicitacao,
                            C.Descricao As Ocorrencia,
                            D.Descricao As Motivo,
                            A.OcorrenciaId,
                            A.Justificativa,
                            CASE 
                                WHEN A.TipoSolicitacao = 1 THEN 
                                    (SELECT DISTINCT DECODE(NVL(Cancelamento.NotaFiscalId,0), 0, Conta.Descricao, Nota.NOMCLI) Cliente 
                                        FROM CRM.TB_CRM_SOLICITACAO_CANCEL_NF Cancelamento LEFT JOIN CRM.TB_CRM_CONTAS Conta ON Cancelamento.ContaId = Conta.Id LEFT JOIN 
                                        FATURA.FATURANOTA Nota ON Cancelamento.NotaFiscalId = Nota.Id WHERE Cancelamento.SolicitacaoId = A.Id) 
                                WHEN A.TipoSolicitacao = 2 THEN
                                    (SELECT 
                                        DISTINCT 
                                            DECODE(NVL(Desconto.Minuta, 0), 0, Gr.Cliente ,Minuta.Cliente) Cliente 
                                        FROM 
                                            CRM.TB_CRM_SOLICITACAO_DESCONTO Desconto 
                                        LEFT JOIN 
                                            (SELECT
                                                c.Razao as Cliente,
                                                gr.Seq_GR,
                                                bl.autonum as Lote
                                            FROM 
                                                SGIPA.TB_BL bl
                                            LEFT JOIN 
                                                 SGIPA.TB_GR_BL gr ON gr.BL = bl.AUTONUM
                                            LEFT JOIN
                                                SGIPA.TB_CAD_PARCEIROS C ON bl.Importador = c.Autonum ) Gr ON Desconto.Lote = Gr.Lote
                                        LEFT JOIN
                                            (
                                                SELECT
                                                    A.AUTONUM,
                                                    B.RAzao As Cliente
                                                FROM
                                                    OPERADOR.TB_MINUTA A
                                                INNER JOIN
                                                    OPERADOR.TB_CAD_CLIENTES B ON A.Cliente = B.AUTONUM) Minuta ON Desconto.Minuta = Minuta.AUTONUM
                                    WHERE Desconto.SolicitacaoId = A.Id) 
                                WHEN A.TipoSolicitacao = 3 THEN            
                                   (SELECT DISTINCT DECODE(NVL(Prorrogacao.NotaFiscalId,0), 0, Conta.Descricao, Nota.NOMCLI) Cliente 
                                        FROM CRM.TB_CRM_SOLICITACAO_PRORROGACAO Prorrogacao LEFT JOIN CRM.TB_CRM_CONTAS Conta ON Prorrogacao.ContaId = Conta.Id LEFT JOIN 
                                        FATURA.FATURANOTA Nota ON Prorrogacao.NotaFiscalId = Nota.Id WHERE Prorrogacao.SolicitacaoId = A.Id)
                                WHEN A.TipoSolicitacao = 4 THEN
                                    (SELECT DISTINCT Conta.Descricao Cliente 
                                        FROM CRM.TB_CRM_SOLICITACAO_RESTITUICAO Restituicao LEFT JOIN CRM.TB_CRM_CONTAS Conta ON Restituicao.FavorecidoId = Conta.Id LEFT JOIN 
                                        FATURA.FATURANOTA Nota ON Restituicao.NotaFiscalId = Nota.Id WHERE Restituicao.SolicitacaoId = A.Id)
                            END Cliente,
                            A.ValorDevido,
                            A.ValorCobrado,
                            A.ValorCredito,
                            A.HabilitaValorDevido,
                            B.Login As CriadoPor,
                     CASE 
                                 
                                WHEN A.TipoSolicitacao = 3 THEN            
                                   (SELECT max(isentarjuros) isentarjuros  
                                        FROM CRM.TB_CRM_SOLICITACAO_PRORROGACAO Prorrogacao  WHERE Prorrogacao.SolicitacaoId = A.Id)
                                  else 0    
                            END isentarjuros
                        FROM
                            CRM.TB_CRM_SOLICITACOES A 
                        LEFT JOIN
                            CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id
                        LEFT JOIN
                            CRM.TB_CRM_SOLICITACAO_OCORRENCIAS C ON A.OcorrenciaId = C.Id
                        LEFT JOIN    
                            CRM.TB_CRM_SOLICITACAO_MOTIVOS D ON A.MotivoId = D.Id
                        LEFT JOIN    
                            CRM.TB_CRM_SOLICITACAO_UNIDADES E ON A.UnidadeSolicitacao = E.Id
                        LEFT JOIN    
                            CRM.TB_CRM_SOLICITACAO_TIPO_OPER F ON A.TipoOperacao = F.Id
                        WHERE
                            A.Id = :Id", parametros).FirstOrDefault();
            }
        }

        public void CadastrarMotivo(SolicitacaoComercialMotivo solicitacaoMotivo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: solicitacaoMotivo.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: solicitacaoMotivo.Status, direction: ParameterDirection.Input);
                parametros.Add(name: "CancelamentoNF", value: solicitacaoMotivo.CancelamentoNF.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Desconto", value: solicitacaoMotivo.Desconto.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Restituicao", value: solicitacaoMotivo.Restituicao.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ProrrogacaoBoleto", value: solicitacaoMotivo.ProrrogacaoBoleto.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_SOLICITACAO_MOTIVOS (Id, Descricao, Status, CancelamentoNF, Desconto, Restituicao, ProrrogacaoBoleto) VALUES (CRM.SEQ_CRM_SOLICITACAO_MOTIVOS.NEXTVAL, :Descricao, :Status, :CancelamentoNF, :Desconto, :Restituicao, :ProrrogacaoBoleto)", parametros);
            }
        }

        public void AtualizarMotivo(SolicitacaoComercialMotivo solicitacaoMotivo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Id", value: solicitacaoMotivo.Id, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: solicitacaoMotivo.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: solicitacaoMotivo.Status, direction: ParameterDirection.Input);
                parametros.Add(name: "CancelamentoNF", value: solicitacaoMotivo.CancelamentoNF.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Desconto", value: solicitacaoMotivo.Desconto.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Restituicao", value: solicitacaoMotivo.Restituicao.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ProrrogacaoBoleto", value: solicitacaoMotivo.ProrrogacaoBoleto.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"UPDATE 
                                CRM.TB_CRM_SOLICITACAO_MOTIVOS SET 
                                    Descricao = :Descricao,
                                    Status = :Status,
                                    CancelamentoNF = :CancelamentoNF, 
                                    Desconto = :Desconto, 
                                    Restituicao = :Restituicao, 
                                    ProrrogacaoBoleto = :ProrrogacaoBoleto
                              WHERE Id = :Id", parametros);
            }
        }

        public void ExcluirMotivo(int motivoSolicitacaoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: motivoSolicitacaoId, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_MOTIVOS WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<SolicitacaoComercialMotivo> ObterSolicitacoesMotivo()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<SolicitacaoComercialMotivo>(@"SELECT * FROM CRM.TB_CRM_SOLICITACAO_MOTIVOS WHERE Status = 1 ORDER BY Descricao");
            }
        }

        public SolicitacaoComercialMotivo ObterSolicitacaoMotivoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<SolicitacaoComercialMotivo>(@"SELECT * FROM CRM.TB_CRM_SOLICITACAO_MOTIVOS WHERE Id = :smId", new { smId = id }).FirstOrDefault();
            }
        }

        public void CadastrarOcorrencia(SolicitacaoComercialOcorrencia solicitacaoOcorrencia)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: solicitacaoOcorrencia.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: solicitacaoOcorrencia.Status, direction: ParameterDirection.Input);
                parametros.Add(name: "CancelamentoNF", value: solicitacaoOcorrencia.CancelamentoNF.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Desconto", value: solicitacaoOcorrencia.Desconto.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Restituicao", value: solicitacaoOcorrencia.Restituicao.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ProrrogacaoBoleto", value: solicitacaoOcorrencia.ProrrogacaoBoleto.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_SOLICITACAO_OCORRENCIAS (Id, Descricao, Status, CancelamentoNF, Desconto, Restituicao, ProrrogacaoBoleto) VALUES (CRM.SEQ_CRM_SOLICITACAO_OCORRENCIA.NEXTVAL, :Descricao, :Status, :CancelamentoNF, :Desconto, :Restituicao, :ProrrogacaoBoleto)", parametros);
            }
        }

        public void AtualizarOcorrencia(SolicitacaoComercialOcorrencia solicitacaoOcorrencia)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Id", value: solicitacaoOcorrencia.Id, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: solicitacaoOcorrencia.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: solicitacaoOcorrencia.Status, direction: ParameterDirection.Input);
                parametros.Add(name: "CancelamentoNF", value: solicitacaoOcorrencia.CancelamentoNF.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Desconto", value: solicitacaoOcorrencia.Desconto.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Restituicao", value: solicitacaoOcorrencia.Restituicao.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ProrrogacaoBoleto", value: solicitacaoOcorrencia.ProrrogacaoBoleto.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"UPDATE 
                                CRM.TB_CRM_SOLICITACAO_OCORRENCIAS SET 
                                    Descricao = :Descricao,
                                    Status = :Status,
                                    CancelamentoNF = :CancelamentoNF, 
                                    Desconto = :Desconto, 
                                    Restituicao = :Restituicao, 
                                    ProrrogacaoBoleto = :ProrrogacaoBoleto
                              WHERE Id = :Id", parametros);
            }
        }

        public void ExcluirOcorrencia(int ocorrenciaSolicitacaoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: ocorrenciaSolicitacaoId, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_OCORRENCIAS WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<SolicitacaoComercialOcorrencia> ObterSolicitacoesOcorrencia()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<SolicitacaoComercialOcorrencia>(@"SELECT * FROM CRM.TB_CRM_SOLICITACAO_OCORRENCIAS WHERE Status = 1 Order By Descricao");
            }
        }

        public SolicitacaoComercialOcorrencia ObterSolicitacaoOcorrenciaPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<SolicitacaoComercialOcorrencia>(@"SELECT * FROM CRM.TB_CRM_SOLICITACAO_OCORRENCIAS WHERE Id = :smId", new { smId = id }).FirstOrDefault();
            }
        }

        public void CadastrarCancelamentoNF(SolicitacaoCancelamentoNF solicitacaoCancelamentoNF)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "SolicitacaoId", value: solicitacaoCancelamentoNF.SolicitacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisa", value: solicitacaoCancelamentoNF.TipoPesquisa, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisaNumero", value: solicitacaoCancelamentoNF.TipoPesquisaNumero, direction: ParameterDirection.Input);
                parametros.Add(name: "Lote", value: solicitacaoCancelamentoNF.Lote, direction: ParameterDirection.Input);
                parametros.Add(name: "NotaFiscalId", value: solicitacaoCancelamentoNF.NotaFiscalId, direction: ParameterDirection.Input);
                parametros.Add(name: "RazaoSocial", value: solicitacaoCancelamentoNF.RazaoSocial, direction: ParameterDirection.Input);
                parametros.Add(name: "NFE", value: solicitacaoCancelamentoNF.NFE, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorNF", value: solicitacaoCancelamentoNF.ValorNF, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaId", value: solicitacaoCancelamentoNF.ContaId, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamento", value: solicitacaoCancelamentoNF.FormaPagamento, direction: ParameterDirection.Input);
                parametros.Add(name: "DataEmissao", value: solicitacaoCancelamentoNF.DataEmissao, direction: ParameterDirection.Input);
                parametros.Add(name: "Desconto", value: solicitacaoCancelamentoNF.Desconto, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorNovaNF", value: solicitacaoCancelamentoNF.ValorNovaNF, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorAPagar", value: solicitacaoCancelamentoNF.ValorAPagar, direction: ParameterDirection.Input);
                parametros.Add(name: "DataProrrogacao", value: solicitacaoCancelamentoNF.DataProrrogacao, direction: ParameterDirection.Input);
                parametros.Add(name: "CriadoPor", value: solicitacaoCancelamentoNF.CriadoPor, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_SOLICITACAO_CANCEL_NF 
                            ( 
                                Id, 
                                SolicitacaoId,
                                TipoPesquisa, 
                                TipoPesquisaNumero, 
                                Lote, 
                                NotaFiscalId,
                                RazaoSocial,
                                NFE,
                                ValorNF, 
                                ContaId,
                                FormaPagamento, 
                                DataEmissao, 
                                Desconto, 
                                ValorNovaNF, 
                                ValorAPagar,
                                DataProrrogacao,
                                CriadoPor
                            ) VALUES ( 
                                CRM.SEQ_CRM_SOLICITACOES_DADOS_FIN.NEXTVAL, 
                                :SolicitacaoId,
                                :TipoPesquisa, 
                                :TipoPesquisaNumero, 
                                :Lote, 
                                :NotaFiscalId, 
                                :RazaoSocial,
                                :NFE,
                                :ValorNF, 
                                :ContaId,
                                :FormaPagamento, 
                                :DataEmissao, 
                                :Desconto, 
                                :ValorNovaNF, 
                                :ValorAPagar,
                                :DataProrrogacao,
                                :CriadoPor
                            )", parametros);
            }
        }

        public void AtualizarCancelamentoNF(SolicitacaoCancelamentoNF solicitacaoCancelamentoNF)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TipoPesquisa", value: solicitacaoCancelamentoNF.TipoPesquisa, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisaNumero", value: solicitacaoCancelamentoNF.TipoPesquisaNumero, direction: ParameterDirection.Input);
                parametros.Add(name: "Lote", value: solicitacaoCancelamentoNF.Lote, direction: ParameterDirection.Input);
                parametros.Add(name: "NotaFiscalId", value: solicitacaoCancelamentoNF.NotaFiscalId, direction: ParameterDirection.Input);
                parametros.Add(name: "RazaoSocial", value: solicitacaoCancelamentoNF.RazaoSocial, direction: ParameterDirection.Input);
                parametros.Add(name: "NFE", value: solicitacaoCancelamentoNF.NFE, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorNF", value: solicitacaoCancelamentoNF.ValorNF, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaId", value: solicitacaoCancelamentoNF.ContaId, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamento", value: solicitacaoCancelamentoNF.FormaPagamento, direction: ParameterDirection.Input);
                parametros.Add(name: "DataEmissao", value: solicitacaoCancelamentoNF.DataEmissao, direction: ParameterDirection.Input);
                parametros.Add(name: "Desconto", value: solicitacaoCancelamentoNF.Desconto, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorNovaNF", value: solicitacaoCancelamentoNF.ValorNovaNF, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorAPagar", value: solicitacaoCancelamentoNF.ValorAPagar, direction: ParameterDirection.Input);
                parametros.Add(name: "DataProrrogacao", value: solicitacaoCancelamentoNF.DataProrrogacao, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: solicitacaoCancelamentoNF.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_SOLICITACAO_CANCEL_NF 
                                SET 
                                    TipoPesquisa = :TipoPesquisa, 
                                    TipoPesquisaNumero = :TipoPesquisaNumero, 
                                    Lote = :Lote, 
                                    NotaFiscalId = :NotaFiscalId, 
                                    RazaoSocial = :RazaoSocial,
                                    NFE = :NFE, 
                                    ValorNF = :ValorNF, 
                                    ContaId = :ContaId,
                                    FormaPagamento = :FormaPagamento, 
                                    DataEmissao = :DataEmissao, 
                                    Desconto = :Desconto, 
                                    ValorNovaNF = :ValorNovaNF, 
                                    ValorAPagar = :ValorAPagar,
                                    DataProrrogacao = :DataProrrogacao
                            WHERE Id = :Id", parametros);
            }
        }

        public void ExcluirCancelamentoNF(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_CANCEL_NF WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<SolicitacaoCancelamentoNFDTO> ObterCancelamentosNF(int solicitacaoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "SolicitacaoId", value: solicitacaoId, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoCancelamentoNFDTO>(@"
                    SELECT 
                        A.Id,
                        A.SolicitacaoId,
                        B.StatusSolicitacao,
                        B.TipoOperacao,
                        H.Resumido As TipoOperacaoResumida,
                        A.TipoPesquisaNumero,
                        A.Lote,
                        DECODE(NVL(A.NFE,0), 0, C.NFE, A.NFE) NFE,
                        A.NotaFiscalId,
                        A.ValorNF,
                        A.ContaId,
                        CASE WHEN B.TipoOperacao <> 6 THEN
                            CASE WHEN A.ContaId IS NOT NULL THEN D.Descricao || ' ' || D.Documento ELSE C.NOMCLI || ' ' || C.CGCCPF END
                        ELSE
                            A.RazaoSocial    
                        END As RazaoSocial,
                        CASE WHEN A.ContaId IS NOT NULL THEN D.Documento ELSE C.CGCCPF END As Documento,
                        A.FormaPagamento,
                        A.Desconto,
                        A.ValorNovaNF,
                        A.ValorAPagar,
                        A.DataProrrogacao,
                        A.DataCadastro,
                        C.DT_EMISSAO As DataEmissao,
                        E.Login As CriadoPor,
                        E.Id As CriadoPorId,
                        NVL(F.TEMP_CIF,0) ValorCIF,
                        NVL(G.RAZAO, ' ') As Indicador,
                        NVL(G.CGC, ' ') As IndicadorDocumento
                    FROM 
                        CRM.TB_CRM_SOLICITACAO_CANCEL_NF A
                    INNER JOIN
                        CRM.TB_CRM_SOLICITACOES B ON A.SolicitacaoId = B.Id
                    LEFT JOIN
                        FATURA.FATURANOTA C ON A.NotaFiscalId = C.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS D ON A.ContaId = D.Id
                    LEFT JOIN
                        CRM.TB_CRM_USUARIOS E ON A.CriadoPor = E.Id
                    LEFT JOIN
                        SGIPA.TB_BL F ON A.Lote = F.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS G ON F.CAPTADOR = G.AUTONUM
                    LEFT JOIN
                        CRM.TB_CRM_SOLICITACAO_TIPO_OPER H ON B.TipoOperacao = H.Id
                    WHERE 
                        A.SolicitacaoId = :SolicitacaoId", parametros);
            }
        }

        public SolicitacaoCancelamentoNF ObterCancelamentoNFPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoCancelamentoNF>(@"
                    SELECT 
                        A.Id,
                        A.SolicitacaoId,
                        B.TipoOperacao,
                        A.TipoPesquisa,
                        A.TipoPesquisaNumero,
                        A.Lote,
                        A.NotaFiscalId,
                        DECODE(NVL(A.NFE,0), 0, C.NFE, A.NFE) NFE,
                        A.ValorNF,
                        CASE WHEN B.TipoOperacao <> 6 THEN
                            CASE WHEN A.ContaId IS NOT NULL THEN D.Descricao ELSE C.NOMCLI END
                        ELSE
                            A.RazaoSocial    
                        END As RazaoSocial,
                        A.ContaId,
                        D.Descricao As ContaDescricao,
                        A.FormaPagamento,
                        A.DataEmissao,
                        A.Desconto,
                        A.ValorNovaNF,
                        A.ValorAPagar,
                        A.DataProrrogacao,
                        A.CriadoPor
                    FROM 
                        CRM.TB_CRM_SOLICITACAO_CANCEL_NF A
                    INNER JOIN
                        CRM.TB_CRM_SOLICITACOES B ON A.Solicitacaoid = B.Id
                    LEFT JOIN
                        FATURA.FATURANOTA C ON A.NotaFiscalId = C.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS D ON A.ContaId = D.Id
                    WHERE 
                        A.Id = :Id", parametros).FirstOrDefault();
            }
        }

        public void CadastrarProrrogacao(SolicitacaoProrrogacao solicitacao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "SolicitacaoId", value: solicitacao.SolicitacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "NotaFiscalId", value: solicitacao.NotaFiscalId, direction: ParameterDirection.Input);
                parametros.Add(name: "NFE", value: solicitacao.NFE, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorNF", value: solicitacao.ValorNF, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaId", value: solicitacao.ContaId, direction: ParameterDirection.Input);
                parametros.Add(name: "RazaoSocial", value: solicitacao.RazaoSocial, direction: ParameterDirection.Input);
                parametros.Add(name: "VencimentoOriginal", value: solicitacao.VencimentoOriginal, direction: ParameterDirection.Input);
                parametros.Add(name: "DataProrrogacao", value: solicitacao.DataProrrogacao, direction: ParameterDirection.Input);
                parametros.Add(name: "NumeroProrrogacao", value: solicitacao.NumeroProrrogacao, direction: ParameterDirection.Input);
                parametros.Add(name: "IsentarJuros", value: solicitacao.IsentarJuros, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorJuros", value: solicitacao.ValorJuros, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorTotalComJuros", value: solicitacao.ValorTotalComJuros, direction: ParameterDirection.Input);
                parametros.Add(name: "Observacoes", value: solicitacao.Observacoes, direction: ParameterDirection.Input);
                parametros.Add(name: "CriadoPor", value: solicitacao.CriadoPor, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_SOLICITACAO_PRORROGACAO 
                            ( 
                                Id, 
                                SolicitacaoId,
                                NotaFiscalId,
                                NFE,
                                ValorNF,
                                ContaId,
                                RazaoSocial,
                                VencimentoOriginal,
                                DataProrrogacao,
                                NumeroProrrogacao,
                                IsentarJuros,
                                ValorJuros,
                                ValorTotalComJuros,
                                Observacoes,
                                CriadoPor
                            ) VALUES ( 
                                CRM.SEQ_CRM_SOLICITACAO_PRORROG.NEXTVAL, 
                                :SolicitacaoId,
                                :NotaFiscalId,
                                :NFE,
                                :ValorNF,
                                :ContaId,
                                :RazaoSocial,
                                :VencimentoOriginal,
                                :DataProrrogacao,
                                :NumeroProrrogacao,
                                :IsentarJuros,
                                :ValorJuros,
                                :ValorTotalComJuros,
                                :Observacoes,
                                :CriadoPor
                            )", parametros);
            }
        }

        public void AtualizarProrrogacao(SolicitacaoProrrogacao solicitacao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "SolicitacaoId", value: solicitacao.SolicitacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "NotaFiscalId", value: solicitacao.NotaFiscalId, direction: ParameterDirection.Input);
                parametros.Add(name: "NFE", value: solicitacao.NFE, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorNF", value: solicitacao.ValorNF, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaId", value: solicitacao.ContaId, direction: ParameterDirection.Input);
                parametros.Add(name: "RazaoSocial", value: solicitacao.RazaoSocial, direction: ParameterDirection.Input);
                parametros.Add(name: "VencimentoOriginal", value: solicitacao.VencimentoOriginal, direction: ParameterDirection.Input);
                parametros.Add(name: "DataProrrogacao", value: solicitacao.DataProrrogacao, direction: ParameterDirection.Input);
                parametros.Add(name: "NumeroProrrogacao", value: solicitacao.NumeroProrrogacao, direction: ParameterDirection.Input);
                parametros.Add(name: "IsentarJuros", value: solicitacao.IsentarJuros, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorJuros", value: solicitacao.ValorJuros, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorTotalComJuros", value: solicitacao.ValorTotalComJuros, direction: ParameterDirection.Input);
                parametros.Add(name: "Observacoes", value: solicitacao.Observacoes, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: solicitacao.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_SOLICITACAO_PRORROGACAO 
                                SET
                                    NotaFiscalId = :NotaFiscalId,
                                    NFE = :NFE,
                                    ValorNF = :ValorNF,
                                    ContaId = :ContaId,
                                    RazaoSocial = :RazaoSocial,
                                    VencimentoOriginal = :VencimentoOriginal,
                                    DataProrrogacao = :DataProrrogacao,
                                    NumeroProrrogacao = :NumeroProrrogacao,
                                    IsentarJuros = :IsentarJuros,
                                    ValorJuros = :ValorJuros,
                                    ValorTotalComJuros = :ValorTotalComJuros,
                                    Observacoes = :Observacoes
                                WHERE
                                    Id = :Id", parametros);
            }
        }

        public SolicitacaoProrrogacao ObterProrrogacaoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoProrrogacao>(@"
                    SELECT 
                        A.Id,
                        A.SolicitacaoId,
                        A.NotaFiscalId,
                        A.NFE,
                        A.ValorNF,
                        C.DT_EMISSAO As DataEmissao,
                        CASE WHEN B.TipoOperacao <> 6 THEN
                            CASE WHEN A.ContaId IS NOT NULL THEN D.Descricao || ' ' || D.Documento ELSE C.NOMCLI || ' ' || C.CGCCPF END
                        ELSE
                            A.RazaoSocial    
                        END As RazaoSocial,
                        CASE WHEN A.ContaId IS NOT NULL THEN D.Documento ELSE C.CGCCPF END As Documento,
                        A.VencimentoOriginal,
                        A.DataProrrogacao,
                        A.NumeroProrrogacao,
                        A.IsentarJuros,
                        A.ValorJuros,
                        A.ValorTotalComJuros,
                        A.ContaId,
                        D.Descricao As ContaDescricao,
                        A.Observacoes,
                        A.CriadoPor
                    FROM
                        CRM.TB_CRM_SOLICITACAO_PRORROGACAO A
                    INNER JOIN
                        CRM.TB_CRM_SOLICITACOES B ON A.SolicitacaoId = B.Id
                    LEFT JOIN
                        FATURA.FATURANOTA C ON A.NotaFiscalId = C.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS D ON A.ContaId = D.Id
                    WHERE 
                        A.Id = :Id", parametros).FirstOrDefault();
            }
        }

        public void ExcluirProrrogacao(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_PRORROGACAO WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<SolicitacaoProrrogacaoDTO> ObterProrrogacoes(int solicitacaoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "SolicitacaoId", value: solicitacaoId, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoProrrogacaoDTO>(@"
                    SELECT 
                        A.Id,
                        A.SolicitacaoId,
                        B.StatusSolicitacao,
                        A.NFE,
                        A.ValorNF,
                        CASE WHEN B.TipoOperacao <> 6 THEN
                            CASE WHEN A.ContaId IS NOT NULL THEN D.Descricao ELSE C.NOMCLI END
                        ELSE
                            A.RazaoSocial    
                        END As RazaoSocial,
                        CASE WHEN A.ContaId IS NOT NULL THEN D.Documento ELSE C.CGCCPF END As Documento,
                        A.ContaId,
                        A.VencimentoOriginal,
                        A.NotaFiscalId,
                        A.DataProrrogacao,
                        A.NumeroProrrogacao,
                        A.IsentarJuros,
                        A.ValorJuros,
                        A.ValorTotalComJuros,
                        A.Observacoes,
                        A.DataCadastro,
                        E.Login As CriadoPor,
                        E.Id As CriadoPorId
                    FROM
                        CRM.TB_CRM_SOLICITACAO_PRORROGACAO A
                    INNER JOIN
                        CRM.TB_CRM_SOLICITACOES B ON A.SolicitacaoId = B.Id
                    LEFT JOIN
                        FATURA.FATURANOTA C ON A.NotaFiscalId = C.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS D ON A.ContaId = D.Id
                    LEFT JOIN
                        CRM.TB_CRM_USUARIOS E ON A.CriadoPor = E.Id
                    WHERE 
                        A.SolicitacaoId = :SolicitacaoId", parametros);
            }
        }

        public void CadastrarRestituicao(SolicitacaoRestituicao solicitacao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "SolicitacaoId", value: solicitacao.SolicitacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisa", value: solicitacao.TipoPesquisa, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisaNumero", value: solicitacao.TipoPesquisaNumero, direction: ParameterDirection.Input);
                parametros.Add(name: "NFE", value: solicitacao.NFE, direction: ParameterDirection.Input);
                parametros.Add(name: "NotaFiscalId", value: solicitacao.NotaFiscalId, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorNF", value: solicitacao.ValorNF, direction: ParameterDirection.Input);
                parametros.Add(name: "RPS", value: solicitacao.RPS, direction: ParameterDirection.Input);
                parametros.Add(name: "Lote", value: solicitacao.Lote, direction: ParameterDirection.Input);
                parametros.Add(name: "Documento", value: solicitacao.Documento, direction: ParameterDirection.Input);
                parametros.Add(name: "FavorecidoId", value: solicitacao.FavorecidoId, direction: ParameterDirection.Input);
                parametros.Add(name: "RazaoSocial", value: solicitacao.RazaoSocial, direction: ParameterDirection.Input);
                parametros.Add(name: "BancoId", value: solicitacao.BancoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Agencia", value: solicitacao.Agencia, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaCorrente", value: solicitacao.ContaCorrente, direction: ParameterDirection.Input);
                parametros.Add(name: "FornecedorSAP", value: solicitacao.FornecedorSAP, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorPagar", value: solicitacao.ValorAPagar, direction: ParameterDirection.Input);
                parametros.Add(name: "DataVencimento", value: solicitacao.DataVencimento, direction: ParameterDirection.Input);
                parametros.Add(name: "Observacoes", value: solicitacao.Observacoes, direction: ParameterDirection.Input);
                parametros.Add(name: "CriadoPor", value: solicitacao.CriadoPor, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_SOLICITACAO_RESTITUICAO
                            ( 
                                Id, 
                                SolicitacaoId,
                                TipoPesquisa,
                                TipoPesquisaNumero,
                                NFE,
                                NotaFiscalId,
                                ValorNF,
                                RPS,
                                Lote,
                                Documento,
                                FavorecidoId,
                                RazaoSocial,
                                BancoId,
                                Agencia,
                                ContaCorrente,
                                FornecedorSAP,
                                ValorPagar,
                                DataVencimento,
                                Observacoes,
                                CriadoPor
                            ) VALUES ( 
                                CRM.SEQ_CRM_SOLICIT_RESTITUICAO.NEXTVAL, 
                                :SolicitacaoId,
                                :TipoPesquisa,
                                :TipoPesquisaNumero,
                                :NFE,
                                :NotaFiscalId,
                                :ValorNF,
                                :RPS,
                                :Lote,
                                :Documento,
                                :FavorecidoId,
                                :RazaoSocial,
                                :BancoId,
                                :Agencia,
                                :ContaCorrente,
                                :FornecedorSAP,
                                :ValorPagar,
                                :DataVencimento,
                                :Observacoes,
                                :CriadoPor
                            )", parametros);
            }
        }

        public void AtualizarRestituicao(SolicitacaoRestituicao solicitacao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "SolicitacaoId", value: solicitacao.SolicitacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisa", value: solicitacao.TipoPesquisa, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisaNumero", value: solicitacao.TipoPesquisaNumero, direction: ParameterDirection.Input);
                parametros.Add(name: "NFE", value: solicitacao.NFE, direction: ParameterDirection.Input);
                parametros.Add(name: "NotaFiscalId", value: solicitacao.NotaFiscalId, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorNF", value: solicitacao.ValorNF, direction: ParameterDirection.Input);
                parametros.Add(name: "RPS", value: solicitacao.RPS, direction: ParameterDirection.Input);
                parametros.Add(name: "Lote", value: solicitacao.Lote, direction: ParameterDirection.Input);
                parametros.Add(name: "Documento", value: solicitacao.Documento, direction: ParameterDirection.Input);
                parametros.Add(name: "FavorecidoId", value: solicitacao.FavorecidoId, direction: ParameterDirection.Input);
                parametros.Add(name: "RazaoSocial", value: solicitacao.RazaoSocial, direction: ParameterDirection.Input);
                parametros.Add(name: "BancoId", value: solicitacao.BancoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Agencia", value: solicitacao.Agencia, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaCorrente", value: solicitacao.ContaCorrente, direction: ParameterDirection.Input);
                parametros.Add(name: "FornecedorSAP", value: solicitacao.FornecedorSAP, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorPagar", value: solicitacao.ValorAPagar, direction: ParameterDirection.Input);
                parametros.Add(name: "DataVencimento", value: solicitacao.DataVencimento, direction: ParameterDirection.Input);
                parametros.Add(name: "Observacoes", value: solicitacao.Observacoes, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: solicitacao.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_SOLICITACAO_RESTITUICAO
                                SET                                
                                    SolicitacaoId = :SolicitacaoId,
                                    TipoPesquisa = :TipoPesquisa,
                                    TipoPesquisaNumero = :TipoPesquisaNumero,
                                    NFE = :NFE,
                                    NotaFiscalId = :NotaFiscalId,
                                    ValorNF = :ValorNF,
                                    RPS = :RPS,
                                    Lote = :Lote,
                                    Documento = :Documento,
                                    FavorecidoId = :FavorecidoId,
                                    RazaoSocial = :RazaoSocial,
                                    BancoId = :BancoId,
                                    Agencia = :Agencia,
                                    ContaCorrente = :ContaCorrente,
                                    FornecedorSAP = :FornecedorSAP,
                                    ValorPagar = :ValorPagar,
                                    DataVencimento = :DataVencimento,
                                    Observacoes = :Observacoes
                                WHERE
                                    Id = :Id", parametros);
            }
        }

        public SolicitacaoRestituicao ObterRestituicaoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoRestituicao>(@"SELECT * FROM CRM.TB_CRM_SOLICITACAO_RESTITUICAO WHERE Id = :Id", parametros).FirstOrDefault();
            }
        }

        public SolicitacaoRestituicao ObterRestituicaoPorNotaFiscal(int notaFiscalId, int solicitacaoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "NotaFiscalId", value: notaFiscalId, direction: ParameterDirection.Input);
                parametros.Add(name: "SolicitacaoId", value: solicitacaoId, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoRestituicao>(@"SELECT * FROM CRM.TB_CRM_SOLICITACAO_RESTITUICAO WHERE NotaFiscalId = :NotaFiscalId AND SolicitacaoId = :SolicitacaoId", parametros).FirstOrDefault();
            }
        }

        public SolicitacaoRestituicaoDTO ObterDetalhesRestituicao(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoRestituicaoDTO>(@"
                    SELECT 
                        A.Id, 
                        A.SolicitacaoId,                        
                        A.TipoPesquisa,
                        A.TipoPesquisaNumero,
                        A.NotaFiscalId,
                        DECODE(NVL(A.NFE,0), 0, E.NFE, A.NFE) NFE,
                        A.ValorNF,
                        A.RPS,
                        E.Dt_Emissao As DataEmissao,
                        A.Lote,
                        A.Documento,
                        A.FavorecidoId,
                        CASE WHEN B.TipoOperacao <> 6 THEN C.Descricao ELSE A.RazaoSocial END FavorecidoDescricao,
                        A.BancoId,
                        D.Descricao As BancoDescricao,
                        A.Agencia,
                        A.ContaCorrente,
                        A.FornecedorSAP,
                        A.ValorPagar As ValorAPagar,
                        A.DataVencimento,
                        A.Observacoes
                    FROM 
                        CRM.TB_CRM_SOLICITACAO_RESTITUICAO A
                    INNER JOIN
                        CRM.TB_CRM_SOLICITACOES B ON A.SolicitacaoId = B.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS C ON A.FavorecidoId = C.Id
                    LEFT JOIN
                        CRM.TB_CRM_BANCOS D ON A.BancoId = D.Id
                    LEFT JOIN
                        FATURA.FATURANOTA E ON A.NotaFiscalId = E.Id
                    WHERE 
                        A.Id = :Id", parametros).FirstOrDefault();
            }
        }

        public void ExcluirRestituicao(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_RESTITUICAO WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<SolicitacaoRestituicaoDTO> ObterRestituicoes(int solicitacaoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "SolicitacaoId", value: solicitacaoId, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoRestituicaoDTO>(@"
                    SELECT 
                        A.Id, 
                        A.SolicitacaoId,
                        B.StatusSolicitacao,
                        B.TipoOperacao,
                        A.TipoPesquisa,
                        A.TipoPesquisaNumero,
                        A.NotaFiscalId,
                        DECODE(NVL(A.NFE,0), 0, E.NFE, A.NFE) NFE,
                        A.ValorNF,
                        A.RPS,
                        A.Lote,
                        A.Documento,
                        A.FavorecidoId,
                        CASE WHEN B.TipoOperacao <> 6 THEN C.Descricao ELSE A.RazaoSocial END As FavorecidoDescricao,
                        C.Documento As FavorecidoDocumento,
                        A.BancoId,
                        D.Descricao As BancoDescricao,
                        A.Agencia,
                        A.ContaCorrente,
                        A.FornecedorSAP,
                        A.ValorPagar As ValorAPagar,
                        A.DataVencimento,
                        A.DataCadastro,
                        F.Login As CriadoPor,
                        F.Id As CriadoPorId
                    FROM 
                        CRM.TB_CRM_SOLICITACAO_RESTITUICAO A
                    INNER JOIN
                        CRM.TB_CRM_SOLICITACOES B ON A.SolicitacaoId = B.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS C ON A.FavorecidoId = C.Id
                    LEFT JOIN
                        CRM.TB_CRM_BANCOS D ON A.BancoId = D.Id
                    LEFT JOIN
                        FATURA.FATURANOTA E ON A.NotaFiscalId = E.Id
                    LEFT JOIN
                        CRM.TB_CRM_USUARIOS F ON A.CriadoPor = F.Id
                    WHERE 
                        A.SolicitacaoId = :SolicitacaoId", parametros);
            }
        }

        public void CadastrarDesconto(SolicitacaoDesconto solicitacao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "SolicitacaoId", value: solicitacao.SolicitacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisa", value: solicitacao.TipoPesquisa, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisaNumero", value: solicitacao.TipoPesquisaNumero, direction: ParameterDirection.Input);
                parametros.Add(name: "MinutaGRId", value: solicitacao.SeqGR, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorGR", value: solicitacao.ValorGR, direction: ParameterDirection.Input);
                parametros.Add(name: "ClienteId", value: solicitacao.ClienteId, direction: ParameterDirection.Input);
                parametros.Add(name: "RazaoSocial", value: solicitacao.RazaoSocial, direction: ParameterDirection.Input);
                parametros.Add(name: "IndicadorId", value: solicitacao.IndicadorId, direction: ParameterDirection.Input);
                parametros.Add(name: "ClienteFaturamentoId", value: solicitacao.ClienteFaturamentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Proposta", value: solicitacao.Proposta, direction: ParameterDirection.Input);
                parametros.Add(name: "VencimentoGR", value: solicitacao.VencimentoGR, direction: ParameterDirection.Input);
                parametros.Add(name: "FreeTimeGR", value: solicitacao.FreeTimeGR, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: solicitacao.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "Lote", value: solicitacao.Lote, direction: ParameterDirection.Input);
                parametros.Add(name: "Reserva", value: solicitacao.Reserva, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamento", value: solicitacao.FormaPagamento, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDesconto", value: solicitacao.TipoDesconto, direction: ParameterDirection.Input);
                parametros.Add(name: "Desconto", value: solicitacao.ValorDesconto, direction: ParameterDirection.Input);
                parametros.Add(name: "DescontoNoServico", value: solicitacao.ValorDescontoNoServico, direction: ParameterDirection.Input);
                parametros.Add(name: "DescontoFinal", value: solicitacao.ValorDescontoFinal, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoFaturadoId", value: solicitacao.ServicoFaturadoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: solicitacao.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoValor", value: solicitacao.ServicoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "DescontoComImposto", value: solicitacao.DescontoComImposto, direction: ParameterDirection.Input);
                parametros.Add(name: "Minuta", value: solicitacao.Minuta, direction: ParameterDirection.Input);
                parametros.Add(name: "Vencimento", value: solicitacao.Vencimento, direction: ParameterDirection.Input);
                parametros.Add(name: "FreeTime", value: solicitacao.FreeTime, direction: ParameterDirection.Input);
                parametros.Add(name: "PorServico", value: solicitacao.TipoDescontoPorServico.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "CriadoPor", value: solicitacao.CriadoPor, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_SOLICITACAO_DESCONTO
                            ( 
                                Id, 
                                SolicitacaoId,
                                TipoPesquisa,
                                TipoPesquisaNumero,
                                MinutaGRId,
                                ValorGR,
                                ClienteId,
                                RazaoSocial,
                                IndicadorId,
                                ClienteFaturamentoId,
                                Proposta,
                                VencimentoGR,
                                FreeTimeGR,
                                Periodo,
                                Lote,
                                Reserva,
                                FormaPagamento,
                                TipoDesconto,
                                Desconto,
                                DescontoNoServico,
                                DescontoFinal,
                                DescontoComImposto,
                                ServicoFaturadoId,
                                ServicoId,
                                ServicoValor,
                                Minuta,
                                Vencimento,
                                FreeTime,
                                PorServico,
                                CriadoPor
                            ) VALUES ( 
                                CRM.SEQ_CRM_SOLICITACAO_DESCONTO.NEXTVAL, 
                                :SolicitacaoId,
                                :TipoPesquisa,
                                :TipoPesquisaNumero,
                                :MinutaGRId,
                                :ValorGR,
                                :ClienteId,
                                :RazaoSocial,
                                :IndicadorId,
                                :ClienteFaturamentoId,
                                :Proposta,
                                :VencimentoGR,
                                :FreeTimeGR,
                                :Periodo,
                                :Lote,
                                :Reserva,
                                :FormaPagamento,
                                :TipoDesconto,
                                :Desconto,
                                :DescontoNoServico,
                                :DescontoFinal,
                                :DescontoComImposto,
                                :ServicoFaturadoId,
                                :ServicoId,
                                :ServicoValor,
                                :Minuta,
                                :Vencimento,
                                :FreeTime,
                                :PorServico,
                                :CriadoPor
                            )", parametros);
            }
        }

        public void AtualizarDesconto(SolicitacaoDesconto solicitacao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "SolicitacaoId", value: solicitacao.SolicitacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisa", value: solicitacao.TipoPesquisa, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisaNumero", value: solicitacao.TipoPesquisaNumero, direction: ParameterDirection.Input);
                parametros.Add(name: "MinutaGRId", value: solicitacao.SeqGR, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorGR", value: solicitacao.ValorGR, direction: ParameterDirection.Input);
                parametros.Add(name: "ClienteId", value: solicitacao.ClienteId, direction: ParameterDirection.Input);
                parametros.Add(name: "RazaoSocial", value: solicitacao.RazaoSocial, direction: ParameterDirection.Input);
                parametros.Add(name: "IndicadorId", value: solicitacao.IndicadorId, direction: ParameterDirection.Input);
                parametros.Add(name: "ClienteFaturamentoId", value: solicitacao.ClienteFaturamentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Proposta", value: solicitacao.Proposta, direction: ParameterDirection.Input);
                parametros.Add(name: "VencimentoGR", value: solicitacao.VencimentoGR, direction: ParameterDirection.Input);
                parametros.Add(name: "FreeTimeGR", value: solicitacao.FreeTimeGR, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: solicitacao.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "Lote", value: solicitacao.Lote, direction: ParameterDirection.Input);
                parametros.Add(name: "Reserva", value: solicitacao.Reserva, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamento", value: solicitacao.FormaPagamento, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDesconto", value: solicitacao.TipoDesconto, direction: ParameterDirection.Input);
                parametros.Add(name: "Desconto", value: solicitacao.ValorDesconto, direction: ParameterDirection.Input);
                parametros.Add(name: "DescontoNoServico", value: solicitacao.ValorDescontoNoServico, direction: ParameterDirection.Input);
                parametros.Add(name: "DescontoFinal", value: solicitacao.ValorDescontoFinal, direction: ParameterDirection.Input);
                parametros.Add(name: "DescontoComImposto", value: solicitacao.DescontoComImposto, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoFaturadoId", value: solicitacao.ServicoFaturadoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: solicitacao.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoValor", value: solicitacao.ServicoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "Minuta", value: solicitacao.Minuta, direction: ParameterDirection.Input);
                parametros.Add(name: "Vencimento", value: solicitacao.Vencimento, direction: ParameterDirection.Input);
                parametros.Add(name: "FreeTime", value: solicitacao.FreeTime, direction: ParameterDirection.Input);
                parametros.Add(name: "PorServico", value: solicitacao.TipoDescontoPorServico.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: solicitacao.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_SOLICITACAO_DESCONTO
                                SET 
                                    TipoPesquisa = :TipoPesquisa,
                                    TipoPesquisaNumero = :TipoPesquisaNumero,
                                    MinutaGRId = :MinutaGRId,
                                    ValorGR = :ValorGR,
                                    ClienteId = :ClienteId,
                                    RazaoSocial = :RazaoSocial,
                                    IndicadorId = :IndicadorId,
                                    ClienteFaturamentoId = :ClienteFaturamentoId,
                                    Proposta = :Proposta,
                                    VencimentoGR = :VencimentoGR,
                                    FreeTimeGR = :FreeTimeGR,
                                    Periodo = :Periodo,
                                    Lote = :Lote,
                                    Reserva = :Reserva,
                                    FormaPagamento = :FormaPagamento,
                                    TipoDesconto = :TipoDesconto,
                                    Desconto = :Desconto,
                                    DescontoNoServico = :DescontoNoServico,
                                    DescontoFinal = :DescontoFinal,
                                    DescontoComImposto = :DescontoComImposto,
                                    ServicoFaturadoId = :ServicoFaturadoId,
                                    ServicoId = :ServicoId,
                                    ServicoValor = :ServicoValor,
                                    Minuta = :Minuta,
                                    Vencimento = :Vencimento,
                                    FreeTime = :FreeTime,
                                    PorServico = :PorServico
                                WHERE
                                    Id = :Id", parametros);
            }
        }

        public SolicitacaoDesconto ObterDescontoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoDesconto>(@"SELECT * FROM CRM.TB_CRM_SOLICITACAO_DESCONTO WHERE Id = :Id", parametros).FirstOrDefault();
            }
        }

        public SolicitacaoDescontoDTO ObterDetalhesGR(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoDescontoDTO>(@"
                    SELECT
                        A.Id,
                        A.ClienteId,
                        B.RAZAO As ClienteDescricao,
                        A.IndicadorId,
                        C.RAZAO As IndicadorDescricao,        
                        C.CGC As IndicadorDocumento,
                        A.SolicitacaoId,
                        A.MinutaGRId,                        
                        A.Minuta,
                        A.ValorGR,                        
                        A.Proposta,
                        A.VencimentoGR,
                        A.FreeTimeGR,
                        A.Periodo,
                        A.Lote,
                        A.Reserva,
                        A.FormaPagamento,
                        A.TipoDesconto,
                        A.Desconto,
                        A.DescontoNoServico,
                        A.DescontoFinal,
                        A.ServicoFaturadoId,
                        D.ServicoDescricao,
                        A.Vencimento,
                        A.FreeTime,
                        A.PorServico,
                        A.DescontoComImposto
                    FROM
                        CRM.TB_CRM_SOLICITACAO_DESCONTO A 
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS B ON A.ClienteId = B.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS C ON A.IndicadorId = C.AUTONUM
                    LEFT JOIN
                        (
                            SELECT SF.AUTONUM, SI.DESCR As ServicoDescricao FROM SGIPA.TB_SERVICOS_FATURADOS SF INNER JOIN SGIPA.TB_SERVICOS_IPA SI ON SF.SERVICO = SI.AUTONUM
                        ) D ON A.ServicoFaturadoId = D.AUTONUM
                    WHERE
                        A.Id = :Id", parametros).FirstOrDefault();
            }
        }

        public void ExcluirDesconto(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_DESCONTO WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<SolicitacaoDescontoDTO> ObterDescontos(int solicitacaoid)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "SolicitacaoId", value: solicitacaoid, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoDescontoDTO>(@"
                    SELECT
                        DISTINCT
                            A.Id, 
                            A.SolicitacaoId,
                            B.StatusSolicitacao,
                            B.TipoOperacao,
                            A.TipoPesquisaNumero,
                            A.MinutaGRId As GR,
                            A.ValorGR,
                            A.ClienteId,
                            A.IndicadorId,
                            A.Proposta,
                            A.VencimentoGR,
                            A.FreeTimeGR,
                            A.Periodo,
                            A.FormaPagamento,
                            A.TipoDesconto,
                            A.Desconto,
                            A.DescontoNoServico,
                            A.DescontoFinal,
                            A.ServicoFaturadoId,
                            CASE WHEN NVL(A.Minuta,0) = 0 THEN D.DESCR ELSE E.DESCR END As ServicoDescricao,
                            A.ServicoValor,
                            A.Vencimento,
                            A.FreeTime,
                            A.PorServico,                       
                            A.Minuta,
                            A.Lote,
                            A.MinutaGRId,
                            A.Reserva,
                            A.DescontoComImposto,
                            F.Razao As IndicadorDescricao,
                            F.CGC As IndicadorDocumento,
                            K.Razao As ClienteDescricao,
                            K.CGC As ClienteDocumento,
                            H.TEMP_CIF ValorCIF,
                            A.DataCadastro,
                            CASE WHEN G.EXTERNO > 0 THEN G.LoginExterno ELSE G.Login END as CriadoPor,
                            G.Id as CriadoPorId,
                            CASE 
                                WHEN B.UnidadeSolicitacao <> 1 AND B.UnidadeSolicitacao <> 2 AND B.TipoOperacao = 6 THEN
                                    (
                                        CASE                         
                                            WHEN B.TipoSolicitacao = 2 THEN 
                                                (SELECT Distinct RazaoSocial Cliente FROM CRM.TB_CRM_SOLICITACAO_DESCONTO Desconto WHERE Desconto.SolicitacaoId = A.Id)                         
                                         END
                                    )
                                ELSE 
                                    (                            
                                    CASE                     
                                        WHEN B.TipoSolicitacao = 2 AND I.Id = 4 AND B.TipoOperacao <> 3 THEN 
                                            (SELECT DISTINCT DECODE(NVL(Cancelamento.NotaFiscalId, 0), 0, Conta.Descricao, Nota.NOMCLI) Cliente 
                                                FROM CRM.TB_CRM_SOLICITACAO_CANCEL_NF Cancelamento LEFT JOIN CRM.TB_CRM_CONTAS Conta ON Cancelamento.ContaId = Conta.Id LEFT JOIN 
                                                FATURA.FATURANOTA Nota ON Cancelamento.NotaFiscalId = Nota.Id WHERE Cancelamento.SolicitacaoId = A.Id) 
                                          WHEN B.TipoSolicitacao = 2 AND B.TipoOperacao = 3 THEN 
                                            (
                                                SELECT 
                                                    Distinct Parceiro.Razao As Cliente 
                                                FROM 
                                                    REDEX.TB_BOOKING Booking 
                                                LEFT JOIN 
                                                    CRM.TB_CRM_SOLICITACAO_DESCONTO Desconto ON Booking.Reference = Desconto.Reserva                                         
                                                LEFT JOIN 
                                                    REDEX.TB_SERVICOS_FATURADOS Servicos ON Booking.AUTONUM_BOO = Servicos.BOOKING                                            
                                                LEFT JOIN 
                                                    REDEX.TB_CAD_PARCEIROS Parceiro ON Servicos.Cliente_Fatura = Parceiro.Autonum 
                                                WHERE 
                                                    Desconto.SolicitacaoId = B.Id AND ROWNUM < 2
                                            )
                                        WHEN B.TipoSolicitacao = 2 AND I.Id <> 4 AND B.TipoOperacao <> 3 THEN 
                                            (SELECT 
                                                DISTINCT 
                                                    DECODE(NVL(Desconto.Minuta, 0), 0, Gr.Cliente ,Minuta.Cliente) Cliente 
                                                FROM 
                                                    CRM.TB_CRM_SOLICITACAO_DESCONTO Desconto 
                                                LEFT JOIN 
                                                    (SELECT
                                                        c.Razao as Cliente,
                                                        gr.Seq_GR,
                                                        bl.autonum as Lote
                                                    FROM 
                                                        SGIPA.TB_BL bl
                                                    LEFT JOIN 
                                                         SGIPA.TB_GR_BL gr ON gr.BL = bl.AUTONUM
                                                    LEFT JOIN
                                                        SGIPA.TB_CAD_PARCEIROS C ON bl.Importador = c.Autonum ) Gr ON Desconto.Lote = Gr.Lote
                                                LEFT JOIN
                                                    (
                                                        SELECT
                                                            A.AUTONUM,
                                                            B.RAzao As Cliente
                                                        FROM
                                                            OPERADOR.TB_MINUTA A
                                                        INNER JOIN
                                                            OPERADOR.TB_CAD_CLIENTES B ON A.Cliente = B.AUTONUM) Minuta ON Desconto.Minuta = Minuta.AUTONUM
                                            WHERE Desconto.SolicitacaoId = B.Id)                                         
                                        END                                                                                                        
                                    ) END RazaoSocial
                        FROM
                            CRM.TB_CRM_SOLICITACAO_DESCONTO A
                        INNER JOIN
                            CRM.TB_CRM_SOLICITACOES B ON A.SolicitacaoId = B.Id
                        LEFT JOIN
                            SGIPA.TB_GR_BL C ON A.MinutaGRId = C.Seq_GR                    
                        LEFT JOIN
                            SGIPA.TB_SERVICOS_IPA D ON A.ServicoId = D.AUTONUM
                        LEFT JOIN
                            OPERADOR.TB_SERVICOS E ON A.ServicoId = E.AUTONUM
                        LEFT JOIN
                            SGIPA.TB_CAD_PARCEIROS F ON A.IndicadorId = F.AUTONUM
                        LEFT JOIN
                            SGIPA.TB_CAD_PARCEIROS K ON A.ClienteId = K.AUTONUM
                        LEFT JOIN
                            CRM.TB_CRM_USUARIOS G ON A.CriadoPor = G.Id
                        LEFT JOIN
                            SGIPA.TB_BL H ON A.LOTE = H.AUTONUM
                        LEFT JOIN
                            CRM.TB_CRM_SOLICITACAO_OCORRENCIAS I ON B.OcorrenciaId = I.Id
                        LEFT JOIN    
                            CRM.TB_CRM_SOLICITACAO_MOTIVOS J ON B.MotivoId = J.Id    
                        WHERE 
                            A.SolicitacaoId = :SolicitacaoId
                        ORDER BY 
                            A.Id", parametros);
            }
        }

        public IEnumerable<SolicitacaoDescontoDTO> ObterDescontosRedex(int solicitacaoid)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "SolicitacaoId", value: solicitacaoid, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoDescontoDTO>(@"
                    SELECT 
                        A.Id, 
                        A.SolicitacaoId,
                        B.StatusSolicitacao,
                        B.TipoOperacao,
                        A.TipoPesquisaNumero,
                        A.MinutaGRId As GR,
                        A.ValorGR,
                        A.ClienteId,
                        A.IndicadorId,
                        A.Proposta,
                        A.VencimentoGR,
                        A.FreeTimeGR,
                        A.Periodo,
                        A.FormaPagamento,
                        A.TipoDesconto,
                        A.Desconto,
                        A.DescontoNoServico,
                        A.DescontoFinal,
                        A.ServicoFaturadoId,
                        C.DESCR As ServicoDescricao,
                        A.ServicoId,
                        A.ServicoValor,
                        A.Vencimento,
                        A.FreeTime,
                        A.PorServico,                       
                        A.Minuta,
                        A.DescontoComImposto,
                        A.Lote,
                        A.Reserva,
                        A.DescontoComImposto,
                        D.Razao As IndicadorDescricao,
                        D.CGC As IndicadorDocumento,
                        D.CGC As IndicadorDocumento,
                        0 ValorCIF,
                        A.DataCadastro,
                        E.Login as CriadoPor
                    FROM 
                        CRM.TB_CRM_SOLICITACAO_DESCONTO A
                    INNER JOIN
                        CRM.TB_CRM_SOLICITACOES B ON A.SolicitacaoId = B.Id
                    LEFT JOIN
                        REDEX.TB_SERVICOS_REDEX C ON A.ServicoId = C.AUTONUM
                    LEFT JOIN
                        REDEX.TB_CAD_PARCEIROS D ON A.IndicadorId = D.AUTONUM
                    LEFT JOIN
                        CRM.TB_CRM_USUARIOS E ON A.CriadoPor = E.Id
                    WHERE 
                        A.SolicitacaoId = :SolicitacaoId
                    ORDER BY 
                        A.Id", parametros);
            }
        }

        public SolicitacaoDescontoDTO ObterDetalhesDesconto(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoDescontoDTO>(@"
                   SELECT
                        A.Id,
                        A.SolicitacaoId,
                        B.UnidadeSolicitacao,
                        A.TipoPesquisa,
                        A.TipoPesquisaNumero,
                        A.ClienteId,
                        A.RazaoSocial,
                        Case When (B.UnidadeSolicitacao <> 1 AND B.UnidadeSolicitacao <> 2) THEN E.Descricao ELSE C.Razao END As ClienteDescricao,
                        A.IndicadorId,
                        Case When (B.UnidadeSolicitacao <> 1 AND B.UnidadeSolicitacao <> 2) THEN F.Descricao ELSE D.Razao END As IndicadorDescricao,
                        Case When (B.UnidadeSolicitacao <> 1 AND B.UnidadeSolicitacao <> 2) THEN F.Documento ELSE D.CGC END As IndicadorDocumento,
                        A.SolicitacaoId,
                        A.MinutaGRId,
                        A.ValorGR,                  
                        A.Proposta,
                        A.VencimentoGR,
                        A.FreeTimeGR,
                        A.Periodo,
                        A.Lote,
                        A.Reserva,
                        A.FormaPagamento,
                        A.TipoDesconto,
                        A.Desconto,
                        A.DescontoNoServico,
                        A.DescontoFinal,
                        A.ServicoFaturadoId,   
                        CASE WHEN NVL(A.Minuta,0) = 0 THEN G.DESCR ELSE H.DESCR END As ServicoDescricao,
                        A.Minuta,
                        A.Vencimento,
                        A.FreeTime,
                        A.PorServico,
                        A.ServicoId,
                        A.ServicoValor,
                        A.DescontoComImposto
                    FROM
                        CRM.TB_CRM_SOLICITACAO_DESCONTO A 
                    INNER JOIN
                        CRM.TB_CRM_SOLICITACOES B ON A.SolicitacaoId = B.Id
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS C ON A.ClienteId = C.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS D ON A.IndicadorId = D.AUTONUM
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS E ON A.ClienteId = E.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS F ON A.IndicadorId = F.Id                                        
                    LEFT JOIN
                        SGIPA.TB_SERVICOS_IPA G ON A.ServicoId = G.AUTONUM
                    LEFT JOIN
                         OPERADOR.TB_SERVICOS H ON A.ServicoId = H.AUTONUM
                    WHERE 
                        A.Id = :Id", parametros).FirstOrDefault();
            }
        }

        public SolicitacaoDescontoDTO ObterDetalhesDescontoRedex(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoDescontoDTO>(@"
                   SELECT
                        A.Id,
                        A.SolicitacaoId,
                        B.UnidadeSolicitacao,
                        A.TipoPesquisa,
                        A.TipoPesquisaNumero,
                        A.ClienteId,
                        DECODE(B.UnidadeSolicitacao, 4, E.Descricao, C.Razao) As ClienteDescricao,
                        A.IndicadorId,
                        DECODE(B.UnidadeSolicitacao, 4, F.Descricao, D.Razao) As IndicadorDescricao,
                        DECODE(B.UnidadeSolicitacao, 4, F.Documento, D.CGC) As IndicadorDocumento,
                        A.SolicitacaoId,
                        A.ClienteFaturamentoId,
                        D.Fantasia As ClienteFaturamentoDescricao,
                        A.MinutaGRId,
                        A.ValorGR,                  
                        A.Proposta,
                        A.VencimentoGR,
                        A.FreeTimeGR,
                        A.Periodo,
                        A.Lote,
                        A.Reserva,
                        A.FormaPagamento,
                        A.TipoDesconto,
                        A.Desconto,
                        A.DescontoNoServico,
                        A.DescontoFinal,
                        A.ServicoFaturadoId,   
                        F.ServicoDescricao,
                        A.Vencimento,
                        A.PorServico,
                        A.ServicoId,
                        A.ServicoValor,
                        A.DescontoComImposto
                    FROM
                        CRM.TB_CRM_SOLICITACAO_DESCONTO A 
                    INNER JOIN
                        CRM.TB_CRM_SOLICITACOES B ON A.SolicitacaoId = B.Id
                    LEFT JOIN
                        REDEX.TB_CAD_PARCEIROS C ON A.ClienteId = C.AUTONUM
                    LEFT JOIN
                        REDEX.TB_CAD_PARCEIROS D ON A.IndicadorId = D.AUTONUM
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS E ON A.ClienteId = E.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS F ON A.IndicadorId = F.Id    
                    LEFT JOIN
                        (
                            SELECT 
                                SF.AUTONUM, 
                                SI.DESCR || ' R$ ' || (VALOR + DESCONTO + ADICIONAL) As ServicoDescricao 
                            FROM 
                                REDEX.TB_SERVICOS_FATURADOS SF 
                            INNER JOIN REDEX.TB_SERVICOS_REDEX SI ON SF.SERVICO = SI.AUTONUM
                        ) F ON A.ServicoFaturadoId = F.AUTONUM
                    WHERE 
                        A.Id = :Id", parametros).FirstOrDefault();
            }
        }

        public IEnumerable<NotaFiscal> ObterNotasFiscaisPorTipoPesquisa(TipoPesquisa tipoPesquisa, TipoOperacao tipoOperacao, string termoPesquisa, SolicitacoesUsuarioExternoFiltro usuarioExternoFiltro)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TermoPesquisa", value: termoPesquisa, direction: ParameterDirection.Input);

                IEnumerable<NotaFiscal> listaNotasFiscais = new List<NotaFiscal>();

                if (tipoPesquisa == TipoPesquisa.NF)
                {
                    if (tipoOperacao == TipoOperacao.RE)
                    {
                        listaNotasFiscais = con.Query<NotaFiscal>($@"
                            SELECT 
                                A.Id, 
                                A.NFE, 
                                A.DT_EMISSAO As DataEmissao,
                                E.CGC As CnpjExportador,
                                '' As CnpjCaptador
                            FROM 
                                FATURA.FATURANOTA A
                            LEFT JOIN
                                FATURA.RPSFAT B ON B.FATSEQ = A.ID
                            LEFT JOIN 
                                FATURA.FAT_GR_RED C ON C.FATID = A.ID 
                            LEFT JOIN 
                                REDEX.TB_GR_BOOKING D ON D.SEQ_GR = C.SEQ_GR 
                            LEFT JOIN 
                                REDEX.TB_BOOKING E ON D.Booking = E.AUTONUM_BOO 
                            LEFT JOIN
                                REDEX.TB_CAD_PARCEIROS E ON E.AUTONUM_PARCEIRO = E.AUTONUM
                            WHERE 
                                A.NFE = :TermoPesquisa", parametros);
                    }
                    else
                    {
                        listaNotasFiscais = con.Query<NotaFiscal>($@"
                            SELECT 
                                A.Id, 
                                A.NFE, 
                                A.DT_EMISSAO As DataEmissao,
                                E.CGC As CnpjImportador,
                                F.CGC As CnpjCaptador
                            FROM 
                                FATURA.FATURANOTA A
                            INNER JOIN
                                FATURA.FAT_GR B ON A.ID = B.FATID
                            LEFT JOIN
                                SGIPA.TB_GR_BL C ON B.SEQ_GR = C.SEQ_GR
                            LEFT JOIN
                                SGIPA.TB_BL D ON C.BL = D.AUTONUM
                            LEFT JOIN
                                SGIPA.TB_CAD_PARCEIROS E ON D.IMPORTADOR = E.AUTONUM
                            LEFT JOIN
                                SGIPA.TB_CAD_PARCEIROS F ON D.CAPTADOR = F.AUTONUM
                            WHERE 
                                A.NFE = :TermoPesquisa 
                            AND 
                                (A.STATUSNFE = 3 OR A.STATUSNFE = 5)", parametros);
                    }
                }

                if (tipoPesquisa == TipoPesquisa.LOTE)
                {
                    listaNotasFiscais = con.Query<NotaFiscal>($@"
                        SELECT 
                            FN.Id, 
                            FN.NFE, 
                            FN.DT_EMISSAO As DataEmissao,
                            E.CGC As CnpjImportador,
                            F.CGC As CnpjCaptador
                        FROM 
                            SGIPA.TB_GR_BL GR 
                        INNER JOIN 
                            FATURA.FAT_GR FAT ON FAT.SEQ_GR = GR.SEQ_GR 
                        INNER JOIN 
                            FATURA.FATURANOTA FN ON FN.ID = FAT.FATID
                        LEFT JOIN
                            SGIPA.TB_BL BL ON GR.BL = BL.AUTONUM
                        LEFT JOIN
                            SGIPA.TB_CAD_PARCEIROS E ON BL.IMPORTADOR = E.AUTONUM
                        LEFT JOIN
                            SGIPA.TB_CAD_PARCEIROS F ON BL.CAPTADOR = F.AUTONUM
                        WHERE 
                            GR.BL = :TermoPesquisa 
                        AND 
                            FN.STATUSNFE = 3", parametros);
                }

                if (tipoPesquisa == TipoPesquisa.BL)
                {
                    listaNotasFiscais = con.Query<NotaFiscal>($@"
                        SELECT 
                            FN.Id, 
                            FN.NFE, 
                            FN.DT_EMISSAO As DataEmissao,
                            E.CGC As CnpjImportador,
                            F.CGC As CnpjCaptador
                        FROM 
                            SGIPA.TB_GR_BL GR 
                        INNER JOIN 
                            FATURA.FAT_GR FAT ON FAT.SEQ_GR = GR.SEQ_GR 
                        INNER JOIN 
                            FATURA.FATURANOTA FN ON FN.ID = FAT.FATID 
                        INNER JOIN
                            SGIPA.TB_BL BL ON GR.BL = BL.AUTONUM
                        LEFT JOIN
                            SGIPA.TB_CAD_PARCEIROS E ON BL.IMPORTADOR = E.AUTONUM
                        LEFT JOIN
                            SGIPA.TB_CAD_PARCEIROS F ON BL.CAPTADOR = F.AUTONUM
                        WHERE 
                            BL.NUMERO = :TermoPesquisa 
                        AND 
                            FN.STATUSNFE = 3", parametros);
                }

                if (tipoPesquisa == TipoPesquisa.GR)
                {
                    listaNotasFiscais = con.Query<NotaFiscal>($@"
                        SELECT 
                            FN.Id, 
                            FN.NFE, 
                            FN.DT_EMISSAO As DataEmissao,
                            E.CGC As CnpjImportador,
                            F.CGC As CnpjCaptador
                        FROM 
                            SGIPA.TB_GR_BL GR 
                        INNER JOIN 
                            FATURA.FAT_GR FATG ON FATG.SEQ_GR = GR.SEQ_GR 
                        INNER JOIN 
                            FATURA.FATURANOTA FN ON FN.ID = FATG.FATID 
                        LEFT JOIN
                            SGIPA.TB_BL D ON GR.BL = D.AUTONUM
                        LEFT JOIN 
                            SGIPA.TB_CAD_PARCEIROS E ON D.IMPORTADOR = E.AUTONUM
                        LEFT JOIN
                            SGIPA.TB_CAD_PARCEIROS F ON D.CAPTADOR = F.AUTONUM
                        WHERE 
                            GR.SEQ_GR = :TermoPesquisa 
                        AND 
                            FN.STATUSNFE = 3", parametros);
                }

                if (tipoPesquisa == TipoPesquisa.MINUTA)
                {
                    listaNotasFiscais = con.Query<NotaFiscal>($@"
                        SELECT 
                            FN.Id, 
                            FN.NFE, 
                            FN.DT_EMISSAO As DataEmissao 
                        FROM 
                            OPERADOR.TB_MINUTA M 
                        INNER JOIN 
                            FATURA.FAT_MINUTA FATM ON FATM.MINUTA = M.AUTONUM 
                        INNER JOIN 
                            FATURA.FATURANOTA FN ON FN.ID = FATM.FATID 
                        WHERE 
                            M.AUTONUM = :TermoPesquisa 
                        AND 
                            FN.STATUSNFE = 3", parametros);
                }

                return listaNotasFiscais;
            }
        }

        public IEnumerable<AnexosDTO> ObterAnexosDaSolicitacao(int solicitacaoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<AnexosDTO>(@"
                    SELECT
                        A.Id,
                        A.IdProcesso,
                        B.Login As CriadoPor,
                        A.Anexo,
                        A.DataCadastro,
                        A.TipoAnexo,
                        A.Versao,
                        RAWTOHEX(A.IdFile) IdFile
                    FROM
                        CRM.TB_CRM_ANEXOS A
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id
                    WHERE
                        A.IdProcesso = :solicitacaoId
                    AND
                        A.TipoDocto = 2
                    ORDER BY
                        A.DataCadastro DESC", new { solicitacaoId });
            }
        }

        public IEnumerable<UsuarioDTO> ObterUsuariosSolicitacoes()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<UsuarioDTO>(@"
                    SELECT
                        DISTINCT
                            B.Id,
                            B.Login                       
                    FROM
                        CRM.TB_CRM_SOLICITACOES A
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id                    
                    ORDER BY
                        B.Login");
            }
        }

        public void CadastrarAlteracaoFormaPgto(SolicitacaoAlteraFormaPagamento solicitacao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "SolicitacaoId", value: solicitacao.SolicitacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisa", value: solicitacao.TipoPesquisa, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisaNumero", value: solicitacao.TipoPesquisaNumero, direction: ParameterDirection.Input);
                parametros.Add(name: "Gr", value: solicitacao.Gr, direction: ParameterDirection.Input);
                parametros.Add(name: "Lote", value: solicitacao.Lote, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: solicitacao.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicaoPagamentoId", value: solicitacao.CondicaoPagamentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "NotaClienteId", value: solicitacao.FaturadoContraId, direction: ParameterDirection.Input);
                parametros.Add(name: "EmailNota", value: solicitacao.EmailNota, direction: ParameterDirection.Input);
                parametros.Add(name: "CriadoPor", value: solicitacao.CriadoPor, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_SOLICITACAO_FORMA_PGTO
                            ( 
                                Id, 
                                SolicitacaoId,
                                TipoPesquisa,
                                TipoPesquisaNumero,
                                Gr,
                                Lote,
                                Valor,
                                CondicaoPagamentoId,
                                NotaClienteId,
                                EmailNota,
                                CriadoPor
                            ) VALUES ( 
                                CRM.SEQ_CRM_SOLICITACAO_DESCONTO.NEXTVAL, 
                                :SolicitacaoId,
                                :TipoPesquisa,
                                :TipoPesquisaNumero,
                                :Gr,
                                :Lote,
                                :Valor,
                                :CondicaoPagamentoId,
                                :NotaClienteId,
                                :EmailNota,
                                :CriadoPor
                            )", parametros);
            }
        }

        public void AtualizarAlteracaoFormaPgto(SolicitacaoAlteraFormaPagamento solicitacao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "SolicitacaoId", value: solicitacao.SolicitacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisa", value: solicitacao.TipoPesquisa, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoPesquisaNumero", value: solicitacao.TipoPesquisaNumero, direction: ParameterDirection.Input);
                parametros.Add(name: "Gr", value: solicitacao.Gr, direction: ParameterDirection.Input);
                parametros.Add(name: "Lote", value: solicitacao.Lote, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: solicitacao.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicaoPagamentoId", value: solicitacao.CondicaoPagamentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "NotaClienteId", value: solicitacao.FaturadoContraId, direction: ParameterDirection.Input);
                parametros.Add(name: "EmailNota", value: solicitacao.EmailNota, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: solicitacao.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_SOLICITACAO_FORMA_PGTO
                                SET  
                                    SolicitacaoId = :SolicitacaoId,
                                    TipoPesquisa = :TipoPesquisa,
                                    TipoPesquisaNumero = :TipoPesquisaNumero,
                                    Gr = :Gr,
                                    Lote = :Lote,
                                    Valor = :Valor,
                                    CondicaoPagamentoId = :CondicaoPagamentoId,
                                    NotaClienteId = :NotaClienteId,
                                    EmailNota = :EmailNota
                                WHERE
                                    Id = :Id", parametros);
            }
        }

        public SolicitacaoAlteraFormaPagamento ObterAlteracaoFormaPgtoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoAlteraFormaPagamento>(@"
                    SELECT 
                        A.Id, 
                        A.TipoPesquisa,
                        A.TipoPesquisaNumero,
                        A.SolicitacaoId, 
                        A.Lote, 
                        A.Gr, 
                        A.Valor, 
                        A.EmailNota,
                        A.DataCadastro,
                        C.StatusSolicitacao,   
                        D.TABELA_GR As Proposta,
                        D.DT_FIM_PERIODO As Vencimento,
                        D.VALIDADE_GR AS FreeTime,
                        D.PERIODOS As Periodo,
                        A.CondicaoPagamentoId,
                        A.NotaClienteId As FaturadoContraId,
                        E.Descricao As FaturadoContraDescricao,
                        G.RAZAO As Indicador,
                        H.RAZAO As Cliente
                    FROM 
                        CRM.TB_CRM_SOLICITACAO_FORMA_PGTO A
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id
                    INNER JOIN
                        CRM.TB_CRM_SOLICITACOES C ON A.SolicitacaoId = C.Id
                    INNER JOIN
                        SGIPA.TB_GR_BL D ON A.Lote = D.BL    
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS E ON A.NotaClienteId = E.Id
                    LEFT JOIN
                        SGIPA.TB_BL F ON D.BL = F.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS G ON F.CAPTADOR = G.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS H ON F.IMPORTADOR = H.AUTONUM
                    WHERE
                        A.Id = :Id
    
                    UNION ALL

                    SELECT
                        E.Id, 
                        E.TipoPesquisa,
                        E.TipoPesquisaNumero,
                        E.SolicitacaoId, 
                        E.Lote, 
                        E.Gr, 
                        (
                            SELECT
                                SUM(VALOR + ADICIONAL + DESCONTO + NVL(
                                    B.VALORIMP,
                                    0
                                )) VALOR
                            FROM
                                SGIPA.TB_SERVICOS_FATURADOS A
                                LEFT JOIN (
                                    SELECT
                                        A.AUTONUM_SERVICO_FATURADO AUTONUM,
                                        SUM(VALOR_IMPOSTO) VALORIMP
                                    FROM
                                        SGIPA.TB_SERVICOS_FATURADOS_IMPOSTOS A
                                    GROUP BY
                                        A.AUTONUM_SERVICO_FATURADO
                                ) B ON A.AUTONUM = B.AUTONUM
                            WHERE
                                    SEQ_GR IS NULL
                                AND
                                    BL = E.Lote AND SEQ_GR IS NULL                           
                        ) AS Valor,
                        E.EmailNota,
                        E.DataCadastro,
                        G.StatusSolicitacao,   
                        A.LISTA AS Proposta,
                        A.DATA_FINAL AS Vencimento,
                        A.VALIDADE_GR AS FreeTime,
                        A.PERIODOS AS Periodo,
                        E.CondicaoPagamentoId,
                        E.NotaClienteId As FaturadoContraId,
                        I.Descricao As FaturadoContraDescricao,
                        D.RAZAO AS Indicador,
                        C.RAZAO AS Cliente
                    FROM
                        SGIPA.TB_GR_PRE_CALCULO A
                    LEFT JOIN 
                        SGIPA.TB_BL B ON A.BL = B.AUTONUM 
                    LEFT JOIN 
                        SGIPA.TB_CAD_PARCEIROS C ON B.IMPORTADOR = C.AUTONUM 
                    LEFT JOIN 
                        SGIPA.TB_CAD_PARCEIROS D ON B.CAPTADOR = D.AUTONUM
                    INNER JOIN 
                        (
                            SELECT BL FROM SGIPA.TB_SERVICOS_FATURADOS WHERE  SEQ_GR IS NULL GROUP BY BL
                        ) SVF ON A.BL = SVF.BL  
                    INNER JOIN
                         CRM.TB_CRM_SOLICITACAO_FORMA_PGTO E ON A.BL = E.LOTE
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS F ON E.CriadoPor = F.Id
                    INNER JOIN
                        CRM.TB_CRM_SOLICITACOES G ON E.SolicitacaoId = G.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS I ON E.NotaClienteId = I.Id
                    WHERE
                        E.Id = :Id", parametros).FirstOrDefault();
            }
        }

        public IEnumerable<SolicitacaoFormaPagamentoDTO> ObterAlteracoesFormaPagamento(int solicitacaoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "SolicitacaoId", value: solicitacaoId, direction: ParameterDirection.Input);

                return con.Query<SolicitacaoFormaPagamentoDTO>(@"
                    SELECT 
                        A.Id, 
                        A.SolicitacaoId, 
                        A.Lote, 
                        A.Gr, 
                        A.Valor, 
                        A.DataCadastro,
                        B.Login As CriadoPor,
                        C.StatusSolicitacao,
                        D.Descricao As FaturadoContra,
                        D.Documento As FaturadoContraDocumento,
                        A.EmailNota,
                        A.CondicaoPagamentoId,
                        E.DESCPG As CondicaoPagamentoDescricao,
                        G.RAZAO As Indicador,
                        H.RAZAO As Cliente
                    FROM 
                        CRM.TB_CRM_SOLICITACAO_FORMA_PGTO A
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id
                    INNER JOIN
                        CRM.TB_CRM_SOLICITACOES C ON A.SolicitacaoId = C.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS D ON A.NotaClienteId = D.Id
                    LEFT JOIN
                        FATURA.TB_COND_PGTO E ON A.CondicaoPagamentoId = E.CODCPG
                    LEFT JOIN
                        SGIPA.TB_BL F ON A.LOTE = F.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS G ON F.CAPTADOR = G.AUTONUM
                    LEFT JOIN
                        SGIPA.TB_CAD_PARCEIROS H ON F.IMPORTADOR = H.AUTONUM
                    WHERE 
                        A.SolicitacaoId = :SolicitacaoId", parametros);
            }
        }

        public void ExcluirAlteracoesFormaPagamento(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_FORMA_PGTO WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<SolicitacaoUnidade> ObterUnidadesSolicitacao()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<SolicitacaoUnidade>(@"SELECT Id, Descricao FROM CRM.TB_CRM_SOLICITACAO_UNIDADES ORDER BY Id");
            }
        }

        public IEnumerable<SolicitacaoTipoOperacao> ObterTiposOperacaoSolicitacao()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<SolicitacaoTipoOperacao>(@"SELECT Id, Descricao, Resumido FROM CRM.TB_CRM_SOLICITACAO_TIPO_OPER ORDER BY Id");
            }
        }
    }
}