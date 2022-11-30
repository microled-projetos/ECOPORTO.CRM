using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Filtros;
using Ecoporto.CRM.Business.Helpers;
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

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class OportunidadeRepositorio : IOportunidadeRepositorio
    {
        public IEnumerable<OportunidadeDTO> ObterOportunidades(int pagina, int registrosPorPagina, OportunidadesFiltro filtro, string orderBy, out int totalFiltro, int? usuarioId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                var filtroSQL = new StringBuilder();

                if (filtro != null)
                {
                    if (filtro.Identificacao.HasValue)
                    {
                        parametros.Add(name: "Identificacao", value: filtro.Identificacao.Value, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.Identificacao = :Identificacao");
                    }

                    if (!string.IsNullOrEmpty(filtro.Descricao))
                    {
                        parametros.Add(name: "Descricao", value: "%" + filtro.Descricao.ToLower() + "%", direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND LOWER(A.Descricao) LIKE :Descricao");
                    }

                    if (filtro.ContaId.HasValue)
                    {
                        parametros.Add(name: "ContaId", value: filtro.ContaId.Value, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.ContaId = :ContaId");
                    }

                    if (filtro.TabelaId.HasValue)
                    {
                        parametros.Add(name: "TabelaId", value: filtro.TabelaId.Value, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.TabelaId = :TabelaId");
                    }

                    if (filtro.ModeloId.HasValue)
                    {
                        parametros.Add(name: "ModeloId", value: filtro.ModeloId.Value, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.ModeloId = :ModeloId");
                    }

                    if (filtro.StatusOportunidade.HasValue)
                    {
                        parametros.Add(name: "StatusOportunidade", value: filtro.StatusOportunidade.Value, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.StatusOportunidade = :StatusOportunidade");
                    }

                    if (filtro.SucessoNegociacao.HasValue)
                    {
                        parametros.Add(name: "SucessoNegociacao", value: filtro.SucessoNegociacao.Value, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.SucessoNegociacao = :SucessoNegociacao");
                    }

                    if (filtro.TipoServico.HasValue)
                    {
                        parametros.Add(name: "TipoServico", value: filtro.TipoServico.Value, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.TipoServico = :TipoServico");
                    }

                    if (filtro.CriadoPor.HasValue)
                    {
                        parametros.Add(name: "CriadoPor", value: filtro.CriadoPor.Value, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.CriadoPor = :CriadoPor");
                    }

                    if (DateTimeHelpers.IsDate(filtro.DataInicio))
                    {
                        var dataInicio = Convert.ToDateTime(filtro.DataInicio);
                        parametros.Add(name: "DataInicio", value: new DateTime(dataInicio.Year, dataInicio.Month, dataInicio.Day, 0, 0, 0), direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.DataInicio >= :DataInicio");
                    }

                    if (DateTimeHelpers.IsDate(filtro.DataTermino))
                    {
                        var dataTermino = Convert.ToDateTime(filtro.DataTermino);
                        parametros.Add(name: "DataTermino", value: new DateTime(dataTermino.Year, dataTermino.Month, dataTermino.Day, 23, 59, 59), direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.DataTermino <= :DataTermino");
                    }

                    if (!string.IsNullOrEmpty(filtro.AdendoId) && StringHelpers.IsInteger(filtro.AdendoId.Replace("A-", string.Empty)))
                    {
                        parametros.Add(name: "AdendoId", value: filtro.AdendoId.Replace("A-", string.Empty), direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND C.Id = :AdendoId");
                    }

                    if (!string.IsNullOrEmpty(filtro.PremioId) && StringHelpers.IsInteger(filtro.PremioId.Replace("P-", string.Empty)))
                    {
                        parametros.Add(name: "PremioId", value: filtro.PremioId.Replace("P-", string.Empty), direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND D.Id = :PremioId");
                    }

                    if (!string.IsNullOrEmpty(filtro.FichaId) && StringHelpers.IsInteger(filtro.FichaId.Replace("F-", string.Empty)))
                    {
                        parametros.Add(name: "FichaId", value: filtro.FichaId.Replace("F-", string.Empty), direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND E.Id = :FichaId");
                    }
                }

                if (usuarioId.HasValue)
                {
                    parametros.Add(name: "UsuarioId", value: usuarioId.Value, direction: ParameterDirection.Input);
                    filtroSQL.Append(" AND A.ContaId IN (SELECT ContaId FROM TB_CRM_USUARIOS_CONTAS WHERE UsuarioId = :UsuarioId) ");
                }

                var sql = $@"
                    SELECT * FROM (
                        SELECT Oportunidades.*, ROWNUM row_num
                            FROM (
                                SELECT 
                                    DISTINCT
                                        A.Id,
                                        A.Descricao,
                                        A.Identificacao, 
                                        A.StatusOportunidade, 
                                        A.SucessoNegociacao, 
                                        A.TipoServico, 
                                        A.TabelaId, 
                                        A.DataInicio, 
                                        A.DataTermino,
                                        A.DataCriacao,
                                        B.Nome As Vendedor,
                                        count(*) over() TotalLinhas
                                    FROM 
                                        TB_CRM_OPORTUNIDADES A
                                    LEFT JOIN
                                        TB_CRM_USUARIOS B ON A.VendedorId = B.Id
                                    LEFT JOIN
                                        TB_CRM_OPORTUNIDADE_ADENDOS C ON A.Id = C.OportunidadeId
                                    LEFT JOIN
                                        TB_CRM_OPORTUNIDADE_PREMIOS D ON A.Id = D.OportunidadeId
                                    LEFT JOIN
                                        TB_CRM_OPORTUNIDADE_FICHA_FAT E ON A.Id = E.OportunidadeId
                                    WHERE
                                        A.Id > 0 {filtroSQL.ToString()} {orderBy}) Oportunidades
                            WHERE ROWNUM < (({pagina} * {registrosPorPagina}) + 1)) 
                        WHERE row_num >= ((({pagina} - 1) * {registrosPorPagina}) + 1)";


                var query = con.Query<OportunidadeDTO>(sql, parametros);

                totalFiltro = query.Select(c => c.TotalLinhas).FirstOrDefault();

                return query;
            }
        }

        public int ObterTotalOportunidades()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<int>($@"SELECT COUNT(1) FROM CRM.TB_CRM_OPORTUNIDADES").Single();
            }
        }

        public IEnumerable<OportunidadeDTO> ObterOportunidadesPorStatus(StatusOportunidade statusOportunidade)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "StatusOportunidade", value: statusOportunidade, direction: ParameterDirection.Input);

                return con.Query<OportunidadeDTO>($@"SELECT * FROM CRM.TB_CRM_OPORTUNIDADES WHERE StatusOportunidade = :StatusOportunidade", parametros);
            }
        }

        public IEnumerable<Oportunidade> ObterOportunidades()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Oportunidade>($@"SELECT * FROM CRM.TB_CRM_OPORTUNIDADES ORDER BY Id");
            }
        }


        public int AnaliseDeCreditoPendente(int contaId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ContaId", value: contaId, direction: ParameterDirection.Input);

                return con.Query<int>(@"SELECT COUNT(1)  conaer from CRM.TB_CRM_SPC_CONSULTAS WHERE contaid = :ContaId and  STATUSANALISEDECREDITO <> 3", parametros).Single();
              
            }
        }
        
         public int LimiteDeCreditoPendente(int contaId)
         {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ContaId", value: contaId, direction: ParameterDirection.Input);

                return con.Query<int>(@"SELECT COUNT(1) contar  from  CRM.TB_CRM_SPC_LIMITE_CREDITO  where contaid = :ContaId and  STATUSLIMITECREDITO <> 3", parametros).Single();

            }
        }


        public int Cadastrar(Oportunidade oportunidade)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Identificacao", value: oportunidade.Identificacao, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaId", value: oportunidade.ContaId, direction: ParameterDirection.Input);
                parametros.Add(name: "EmpresaId", value: oportunidade.EmpresaId, direction: ParameterDirection.Input);
                parametros.Add(name: "Aprovada", value: oportunidade.Aprovada.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ConsultaTabela", value: oportunidade.ConsultaTabela.ToInt(), direction: ParameterDirection.Input);                
                parametros.Add(name: "Descricao", value: oportunidade.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "SucessoNegociacao", value: oportunidade.SucessoNegociacao, direction: ParameterDirection.Input);
                parametros.Add(name: "ContatoId", value: oportunidade.ContatoId, direction: ParameterDirection.Input);
                parametros.Add(name: "EstagioNegociacao", value: oportunidade.EstagioNegociacao, direction: ParameterDirection.Input);
                parametros.Add(name: "DataFechamento", value: oportunidade.DataFechamento, direction: ParameterDirection.Input);
                parametros.Add(name: "ClassificacaoCliente", value: oportunidade.ClassificacaoCliente, direction: ParameterDirection.Input);
                parametros.Add(name: "Probabilidade", value: oportunidade.Probabilidade, direction: ParameterDirection.Input);
                parametros.Add(name: "Segmento", value: oportunidade.Segmento, direction: ParameterDirection.Input);
                parametros.Add(name: "RevisaoId", value: oportunidade.RevisaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoOperacaoOportunidade", value: oportunidade.TipoOperacaoOportunidade, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoNegocio", value: oportunidade.TipoNegocio, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDeProposta", value: oportunidade.TipoDeProposta, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoServico", value: oportunidade.TipoServico, direction: ParameterDirection.Input);
                parametros.Add(name: "MotivoPerda", value: oportunidade.MotivoPerda, direction: ParameterDirection.Input);
                parametros.Add(name: "StatusOportunidade", value: oportunidade.StatusOportunidade, direction: ParameterDirection.Input);
                parametros.Add(name: "PremioParceria", value: oportunidade.PremioParceria, direction: ParameterDirection.Input);
                parametros.Add(name: "FaturamentoMensalLCL", value: oportunidade.FaturamentoMensalLCL, direction: ParameterDirection.Input);
                parametros.Add(name: "FaturamentoMensalFCL", value: oportunidade.FaturamentoMensalFCL, direction: ParameterDirection.Input);
                parametros.Add(name: "VolumeMensal", value: oportunidade.VolumeMensal, direction: ParameterDirection.Input);
                parametros.Add(name: "CIFMedio", value: oportunidade.CIFMedio, direction: ParameterDirection.Input);
                parametros.Add(name: "MercadoriaId", value: oportunidade.MercadoriaId, direction: ParameterDirection.Input);
                parametros.Add(name: "Observacao", value: oportunidade.Observacao, direction: ParameterDirection.Input);
                parametros.Add(name: "CriadoPor", value: oportunidade.CriadoPor, direction: ParameterDirection.Input);
                parametros.Add(name: "DataCriacao", value: DateTime.Now, direction: ParameterDirection.Input);
                parametros.Add(name: "VendedorId", value: oportunidade.Conta.VendedorId, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"
                            INSERT INTO
                                CRM.TB_CRM_OPORTUNIDADES
                                    (
                                        Id,
                                        Identificacao, 
                                        ContaId, 
                                        EmpresaId,
                                        Aprovada, 
                                        Consulta_Tabela,
                                        Descricao, 
                                        SucessoNegociacao, 
                                        ContatoId, 
                                        EstagioNegociacao, 
                                        DataFechamento,                                             
                                        ClassificacaoCliente, 
                                        Probabilidade, 
                                        Segmento, 
                                        RevisaoId, 
                                        TipoOperacaoOportunidade, 
                                        TipoNegocio, 
                                        TipoDeProposta, 
                                        TipoServico, 
                                        MotivoPerda, 
                                        StatusOportunidade, 
                                        PremioParceria, 
                                        FaturamentoMensalLCL, 
                                        FaturamentoMensalFCL, 
                                        VolumeMensal, 
                                        CIFMedio, 
                                        MercadoriaId, 
                                        Observacao,
                                        CriadoPor,
                                        DataCriacao,
                                        VendedorId
                                    ) VALUES (
                                        CRM.SEQ_CRM_OPORTUNIDADES.NEXTVAL,
                                        CRM.SEQ_CRM_OPORTUNIDADE_IDENT.NEXTVAL, 
                                        :ContaId, 
                                        :EmpresaId,
                                        :Aprovada, 
                                        :ConsultaTabela,
                                        :Descricao, 
                                        :SucessoNegociacao, 
                                        :ContatoId, 
                                        :EstagioNegociacao, 
                                        :DataFechamento,                                              
                                        :ClassificacaoCliente, 
                                        :Probabilidade, 
                                        :Segmento, 
                                        :RevisaoId, 
                                        :TipoOperacaoOportunidade, 
                                        :TipoNegocio, 
                                        :TipoDeProposta, 
                                        :TipoServico, 
                                        :MotivoPerda, 
                                        :StatusOportunidade, 
                                        :PremioParceria, 
                                        :FaturamentoMensalLCL, 
                                        :FaturamentoMensalFCL, 
                                        :VolumeMensal, 
                                        :CIFMedio, 
                                        :MercadoriaId, 
                                        :Observacao,
                                        :CriadoPor,
                                        :DataCriacao,
                                        :VendedorId
                                    ) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void Atualizar(Oportunidade oportunidade)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Identificacao", value: oportunidade.Identificacao, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaId", value: oportunidade.ContaId, direction: ParameterDirection.Input);
                parametros.Add(name: "EmpresaId", value: oportunidade.EmpresaId, direction: ParameterDirection.Input);
                parametros.Add(name: "TabelaId", value: oportunidade.TabelaId, direction: ParameterDirection.Input);
                parametros.Add(name: "Aprovada", value: oportunidade.Aprovada.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ConsultaTabela", value: oportunidade.ConsultaTabela.ToInt(), direction: ParameterDirection.Input);                
                parametros.Add(name: "Descricao", value: oportunidade.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "SucessoNegociacao", value: oportunidade.SucessoNegociacao, direction: ParameterDirection.Input);
                parametros.Add(name: "Cancelado", value: oportunidade.Cancelado.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "DataCancelamento", value: oportunidade.DataCancelamento, direction: ParameterDirection.Input);
                parametros.Add(name: "DataFechamento", value: oportunidade.DataFechamento, direction: ParameterDirection.Input);
                parametros.Add(name: "ContatoId", value: oportunidade.ContatoId, direction: ParameterDirection.Input);
                parametros.Add(name: "EstagioNegociacao", value: oportunidade.EstagioNegociacao, direction: ParameterDirection.Input);
                parametros.Add(name: "StatusOportunidade", value: oportunidade.StatusOportunidade, direction: ParameterDirection.Input);
                parametros.Add(name: "ClassificacaoCliente", value: oportunidade.ClassificacaoCliente, direction: ParameterDirection.Input);
                parametros.Add(name: "Probabilidade", value: oportunidade.Probabilidade, direction: ParameterDirection.Input);
                parametros.Add(name: "Segmento", value: oportunidade.Segmento, direction: ParameterDirection.Input);
                parametros.Add(name: "RevisaoId", value: oportunidade.RevisaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoOperacaoOportunidade", value: oportunidade.TipoOperacaoOportunidade, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoNegocio", value: oportunidade.TipoNegocio, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDeProposta", value: oportunidade.TipoDeProposta, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoServico", value: oportunidade.TipoServico, direction: ParameterDirection.Input);
                parametros.Add(name: "MotivoPerda", value: oportunidade.MotivoPerda, direction: ParameterDirection.Input);
                parametros.Add(name: "PremioParceria", value: oportunidade.PremioParceria, direction: ParameterDirection.Input);
                parametros.Add(name: "FaturamentoMensalLCL", value: oportunidade.FaturamentoMensalLCL, direction: ParameterDirection.Input);
                parametros.Add(name: "FaturamentoMensalFCL", value: oportunidade.FaturamentoMensalFCL, direction: ParameterDirection.Input);
                parametros.Add(name: "VolumeMensal", value: oportunidade.VolumeMensal, direction: ParameterDirection.Input);
                parametros.Add(name: "CIFMedio", value: oportunidade.CIFMedio, direction: ParameterDirection.Input);
                parametros.Add(name: "MercadoriaId", value: oportunidade.MercadoriaId, direction: ParameterDirection.Input);
                parametros.Add(name: "Observacao", value: oportunidade.Observacao, direction: ParameterDirection.Input);
                parametros.Add(name: "AlteradoPor", value: oportunidade.AlteradoPor, direction: ParameterDirection.Input);
                parametros.Add(name: "UltimaAlteracao", value: oportunidade.UltimaAlteracao, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: oportunidade.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE
                                CRM.TB_CRM_OPORTUNIDADES
                                    SET
                                        Identificacao = :Identificacao, 
                                        ContaId = :ContaId, 
                                        EmpresaId = :EmpresaId,
                                        TabelaId = :TabelaId,
                                        Aprovada = :Aprovada, 
                                        Consulta_Tabela = :ConsultaTabela,
                                        Descricao = :Descricao, 
                                        SucessoNegociacao = :SucessoNegociacao, 
                                        Cancelado = :Cancelado,
                                        DataCancelamento = :DataCancelamento,
                                        DataFechamento = :DataFechamento, 
                                        ContatoId = :ContatoId, 
                                        EstagioNegociacao = :EstagioNegociacao,    
                                        StatusOportunidade = :StatusOportunidade,
                                        ClassificacaoCliente = :ClassificacaoCliente, 
                                        Probabilidade = :Probabilidade, 
                                        Segmento = :Segmento, 
                                        RevisaoId = :RevisaoId, 
                                        TipoOperacaoOportunidade = :TipoOperacaoOportunidade, 
                                        TipoNegocio = :TipoNegocio, 
                                        TipoDeProposta = :TipoDeProposta, 
                                        TipoServico = :TipoServico, 
                                        MotivoPerda = :MotivoPerda, 
                                        PremioParceria = :PremioParceria, 
                                        FaturamentoMensalLCL = :FaturamentoMensalLCL, 
                                        FaturamentoMensalFCL = :FaturamentoMensalFCL, 
                                        VolumeMensal = :VolumeMensal, 
                                        CIFMedio = :CIFMedio, 
                                        MercadoriaId = :MercadoriaId, 
                                        Observacao = :Observacao,                                                
                                        AlteradoPor = :AlteradoPor,
                                        UltimaAlteracao = :UltimaAlteracao                                        
                                    WHERE Id = :Id", parametros);
            }
        }

        public void AtualizarDataInicial(OportunidadeProposta oportunidadeProposta)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "DataInicio", value: DateTime.Now, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeProposta.OportunidadeId, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET DataInicio = :DataInicio WHERE Id = :OportunidadeId", parametros);
            }
        }

        public void AtualizarDataTermino(OportunidadeProposta oportunidadeProposta)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "DataTermino", value: oportunidadeProposta.DataTermino, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeProposta.OportunidadeId, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET DataTermino = :DataTermino WHERE Id = :OportunidadeId", parametros);
            }
        }

        public void AtualizarNumeroTabela(string tabela, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TabelaId", value: tabela, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: oportunidadeId, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET TabelaId = :TabelaId WHERE Id = :Id", parametros);
            }
        }

        public void AtualizarStatusOportunidade(StatusOportunidade? statusOportunidade, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "StatusOportunidade", value: statusOportunidade, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: oportunidadeId, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET StatusOportunidade = :StatusOportunidade WHERE Id = :Id", parametros);
            }
        }

        public void AtualizarOportunidadeCancelada(Oportunidade oportunidade)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "StatusOportunidade", value: StatusOportunidade.CANCELADA, direction: ParameterDirection.Input);
                parametros.Add(name: "DataCancelamento", value: oportunidade.DataCancelamento, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: oportunidade.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET StatusOportunidade = :StatusOportunidade, Aprovada = 0, Cancelado = 1, DataCancelamento = :DataCancelamento WHERE Id = :Id", parametros);
            }
        }

        public void AtualizarDataCancelamento(Oportunidade oportunidade)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                
                parametros.Add(name: "DataCancelamento", value: oportunidade.DataCancelamento, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: oportunidade.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET DataCancelamento = :DataCancelamento WHERE Id = :Id", parametros);
            }
        }

        public void AtualizarDataCancelamentoOportunidade(DateTime? data, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "DataCancelamento", value: data, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: oportunidadeId, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET DataCancelamento = :DataCancelamento WHERE Id = :Id", parametros);
            }
        }

        public void AtualizarProposta(OportunidadeProposta oportunidadeProposta)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TipoOperacao", value: oportunidadeProposta.TipoOperacao, direction: ParameterDirection.Input);
                parametros.Add(name: "ModeloId", value: oportunidadeProposta.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamento", value: oportunidadeProposta.FormaPagamento, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: oportunidadeProposta.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "Validade", value: oportunidadeProposta.Validade, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoValidade", value: oportunidadeProposta.TipoValidade, direction: ParameterDirection.Input);
                parametros.Add(name: "DiasFreeTime", value: oportunidadeProposta.DiasFreeTime, direction: ParameterDirection.Input);
                parametros.Add(name: "VendedorId", value: oportunidadeProposta.VendedorId, direction: ParameterDirection.Input);
                parametros.Add(name: "ImpostoId", value: oportunidadeProposta.ImpostoId, direction: ParameterDirection.Input);
                parametros.Add(name: "DataInicio", value: oportunidadeProposta.DataInicio, direction: ParameterDirection.Input);
                parametros.Add(name: "DataTermino", value: oportunidadeProposta.DataTermino, direction: ParameterDirection.Input);
                parametros.Add(name: "Acordo", value: oportunidadeProposta.Acordo.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ParametroBL", value: oportunidadeProposta.ParametroBL.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ParametroLote", value: oportunidadeProposta.ParametroLote.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ParametroConteiner", value: oportunidadeProposta.ParametroConteiner.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ParametroIdTabela", value: oportunidadeProposta.ParametroIdTabela.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "BL", value: oportunidadeProposta.BL, direction: ParameterDirection.Input);
                parametros.Add(name: "Lote", value: oportunidadeProposta.Lote, direction: ParameterDirection.Input);
                parametros.Add(name: "Conteiner", value: oportunidadeProposta.Conteiner, direction: ParameterDirection.Input);
                parametros.Add(name: "HubPort", value: oportunidadeProposta.HubPort.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "CobrancaEspecial", value: oportunidadeProposta.CobrancaEspecial.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "DesovaParcial", value: oportunidadeProposta.DesovaParcial, direction: ParameterDirection.Input);
                parametros.Add(name: "FatorCP", value: oportunidadeProposta.FatorCP, direction: ParameterDirection.Input);
                parametros.Add(name: "PosicIsento", value: oportunidadeProposta.PosicIsento, direction: ParameterDirection.Input);
                parametros.Add(name: "TabelaReferencia", value: oportunidadeProposta.TabelaReferencia, direction: ParameterDirection.Input);
                
                parametros.Add(name: "Id", value: oportunidadeProposta.OportunidadeId, direction: ParameterDirection.Input);                

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES 
                                SET
                                    ModeloId = :ModeloId, 
                                    TipoOperacao = :TipoOperacao, 
                                    FormaPagamento = :FormaPagamento,
                                    QtdeDias = :QtdeDias,
                                    Validade = :Validade,
                                    TipoValidade = :TipoValidade,
                                    DiasFreeTime = :DiasFreeTime,
                                    VendedorId = :VendedorId,
                                    ImpostoId = :ImpostoId,
                                    DataInicio = :DataInicio,
                                    DataTermino = :DataTermino,
                                    Acordo = :Acordo,
                                    ParametroBL = :ParametroBL,
                                    ParametroLote = :ParametroLote,
                                    ParametroConteiner = :ParametroConteiner,
                                    ParametroIdTabela = :ParametroIdTabela,
                                    BL = :BL,
                                    Lote = :Lote,
                                    Conteiner = :Conteiner,
                                    DesovaParcial = :DesovaParcial, 
                                    FatorCP = :FatorCP, 
                                    PosicIsento = :PosicIsento, 
                                    HubPort = :HubPort, 
                                    CobrancaEspecial = :CobrancaEspecial,
                                    TabelaReferencia = :TabelaReferencia
                                WHERE Id = :Id", parametros);
            }
        }

        public void AtualizarReferencia(string referencia, int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Referencia", value: referencia, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET Referencia = :Referencia WHERE Id = :Id", parametros);
            }
        }

        public OportunidadePropostaDTO ObterDetalhesProposta(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<OportunidadePropostaDTO>(@"
                        SELECT 
                            A.Id,
                            DECODE(A.TipoOperacao, 1, 'RA', 2, 'OP', 3, 'RE') DescricaoTipoOperacao,
                            TO_CHAR(A.DataInicio, 'DD/MM/YYYY') As DataInicio,
                            TO_CHAR(A.DataTermino, 'DD/MM/YYYY') As DataTermino, 
                            B.Descricao As ModeloDescricao,
                            DECODE(A.FormaPagamento, 1, 'À Vista', 2, 'Faturado') As FormaPagamento,
                            A.DiasFreeTime,
                            A.QtdeDias,
                            A.Validade,
                            DECODE(A.TipoValidade, 1, 'Dias', 2, 'Meses', 3, 'Anos') As TipoValidade,
                            C.Nome As VendedorDescricao,
                            D.Descricao As ImpostoDescricao,
                            DECODE(A.Acordo, 0, 'Não', 'Sim') As Acordo
                        FROM 
                            CRM.TB_CRM_OPORTUNIDADES A
                        LEFT JOIN
                            CRM.TB_CRM_MODELO B ON A.ModeloId = B.Id
                        LEFT JOIN
                            CRM.TB_CRM_USUARIOS C ON A.VendedorId = C.Id    
                        LEFT JOIN
                            CRM.TB_CRM_IMPOSTOS D ON A.ImpostoId = D.Id
                        WHERE 
                            A.Id = :Id", parametros).FirstOrDefault();                
            }
        }
        public Oportunidade ObterOportunidadePorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Oportunidade, OportunidadeProposta, Conta, Oportunidade>(@"
                    SELECT 
                        A.Id, 
                        A.Identificacao,
                        A.ContaId,
                        A.EmpresaId,
                        A.Aprovada,
                        A.Consulta_Tabela As ConsultaTabela,
                        A.Descricao,
                        A.OrigemClone,
                        A.DataFechamento,                       
                        A.TabelaId,
                        B.TabelaId As TabelaRevisadaId,
                        A.ContatoId,
                        A.Probabilidade,
                        A.SucessoNegociacao,
                        A.ClassificacaoCliente,
                        A.Segmento,
                        A.PermiteAlterarDataCancelamento,
                        A.EstagioNegociacao,
                        A.StatusOportunidade,
                        A.MotivoPerda,
                        A.TipoServico,
                        A.TipoDeProposta,
                        A.TipoNegocio,
                        A.TipoOperacaoOportunidade,
                        A.RevisaoId,
                        A.MercadoriaId,
                        A.Observacao,
                        A.Referencia,
                        A.DataCancelamento,
                        A.FaturamentoMensalLCL,
                        A.FaturamentoMensalFCL,
                        A.Cancelado,
                        A.VolumeMensal,
                        A.CIFMedio,
                        A.PremioParceria, 
                        A.SallesId,
                        A.CriadoPor,
                        A.DataCriacao,
                        A.AlteradoPor,
                        A.UltimaAlteracao,
                        A.VendedorId,
                        A.TipoOperacao,
                        A.DataInicio,
                        A.DataTermino,
                        A.ModeloId,
                        A.FormaPagamento,
                        A.DiasFreeTime,
                        A.QtdeDias,
                        A.Validade,
                        A.TipoValidade,
                        A.VendedorId,
                        A.ImpostoId, 
                        A.Acordo,
                        A.ParametroBL,
                        A.ParametroLote,
                        A.ParametroConteiner,
                        A.ParametroIdTabela,
                        A.TabelaReferencia,
                        A.BL,
                        A.Lote,
                        A.Conteiner,
                        A.FatorCP,
                        A.PosicIsento,
                        A.DesovaParcial,
                        A.Acordo,
                        A.HubPort,
                        A.CobrancaEspecial,
                        C.Id,
                        C.Descricao
                    FROM 
                        CRM.TB_CRM_OPORTUNIDADES A
                    LEFT JOIN
                        CRM.TB_CRM_OPORTUNIDADES B ON A.RevisaoId = B.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS C ON A.ContaId = C.Id
                    WHERE A.Id = :id", (o, op, c) =>
                {
                    if (op != null)
                        o.OportunidadeProposta = op;
                    else
                        o.OportunidadeProposta = new OportunidadeProposta();

                    if (c != null)
                        o.Conta = c;
                    else
                        o.Conta = new Conta();

                    return o;
                }, new { id }, splitOn: "TipoOperacao, Id").FirstOrDefault();
            }
        }

        public IEnumerable<Oportunidade> ObterOportunidadePorRevisaoId(int revisaoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "RevisaoId", value: revisaoId, direction: ParameterDirection.Input);

                return con.Query<Oportunidade>(@"
                    SELECT 
                        Id, 
                        Identificacao,
                        SucessoNegociacao,
                        ContaId,
                        EmpresaId,
                        Aprovada,
                        Descricao,
                        StatusOportunidade
                    FROM 
                        CRM.TB_CRM_OPORTUNIDADES
                    WHERE 
                        RevisaoId = :RevisaoId", parametros);
            }
        }

        public Oportunidade ObterOportunidadePorTabela(int tabelaId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Oportunidade, OportunidadeProposta, Conta, Oportunidade>(@"
                    SELECT 
                        A.Id, 
                        A.Identificacao,
                        A.ContaId,
                        A.EmpresaId,
                        A.Aprovada,
                        A.Consulta_Tabela As ConsultaTabela,
                        A.Descricao,
                        A.OrigemClone,
                        A.DataFechamento,                       
                        A.TabelaId,
                        A.ContatoId,
                        A.Probabilidade,
                        A.SucessoNegociacao,
                        A.ClassificacaoCliente,
                        A.Segmento,
                        A.EstagioNegociacao,
                        A.StatusOportunidade,
                        A.MotivoPerda,
                        A.TipoServico,
                        A.TipoDeProposta,
                        A.TipoNegocio,
                        A.TipoOperacaoOportunidade,
                        A.RevisaoId,
                        A.MercadoriaId,
                        A.Observacao,
                        A.Referencia,
                        A.DataCancelamento,
                        A.FaturamentoMensalLCL,
                        A.FaturamentoMensalFCL,
                        A.Cancelado,
                        A.VolumeMensal,
                        A.CIFMedio,
                        A.PremioParceria, 
                        A.SallesId,
                        A.CriadoPor,
                        A.DataCriacao,
                        A.AlteradoPor,
                        A.UltimaAlteracao,
                        A.VendedorId,
                        A.TipoOperacao,
                        A.DataInicio,
                        A.DataTermino,
                        A.ModeloId,
                        A.FormaPagamento,
                        A.DiasFreeTime,
                        A.QtdeDias,
                        A.Validade,
                        A.TipoValidade,
                        A.VendedorId,
                        A.ImpostoId, 
                        A.Acordo,
                        A.ParametroBL,
                        A.ParametroLote,
                        A.ParametroConteiner,
                        A.ParametroIdTabela,
                        A.TabelaReferencia,
                        A.BL,
                        A.Lote,
                        A.Conteiner,
                        A.FatorCP,
                        A.PosicIsento,
                        A.DesovaParcial,
                        A.Acordo,
                        A.HubPort,
                        A.CobrancaEspecial,
                        B.Id,
                        B.Descricao
                    FROM 
                        CRM.TB_CRM_OPORTUNIDADES A
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS B ON A.ContaId = B.Id
                    WHERE A.TabelaId = :tabelaId", (o, op, c) =>
                {
                    if (op != null)
                        o.OportunidadeProposta = op;
                    else
                        o.OportunidadeProposta = new OportunidadeProposta();

                    if (c != null)
                        o.Conta = c;
                    else
                        o.Conta = new Conta();

                    return o;
                }, new { tabelaId }, splitOn: "TipoOperacao, Id").FirstOrDefault();
            }
        }

        public DetalhesOportunidadeDTO ObterDetalhesOportunidade(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<DetalhesOportunidadeDTO>(@"SELECT * FROM VW_CRM_OPORTUNIDADES WHERE Id = :Id", parametros).FirstOrDefault();
            }
        }

        public IEnumerable<OportunidadeDTO> ObterOportunidadesPorDescricao(string descricao, int? usuarioId)
        {
            var criterioDescricao = "%" + descricao.ToUpper() + "%";
            var criterioIdentificacao = "%" + descricao.ToUpper() + "%";
            var criterioDocumento = descricao.ToUpper();

            var filtroSQL = string.Empty;

            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "CriterioDescricao", value: criterioDescricao, direction: ParameterDirection.Input);
                parametros.Add(name: "CriterioIdentificacao", value: criterioIdentificacao, direction: ParameterDirection.Input);
                parametros.Add(name: "CriterioDocumento", value: criterioDocumento, direction: ParameterDirection.Input);

                if (usuarioId.HasValue)
                {
                    parametros.Add(name: "UsuarioId", value: usuarioId, direction: ParameterDirection.Input);
                    filtroSQL = " AND A.ContaId IN (SELECT ContaId FROM TB_CRM_USUARIOS_CONTAS WHERE UsuarioId = :UsuarioId) ";
                }

                return con.Query<OportunidadeDTO>($@"
                    SELECT 
                        A.Id, 
                        A.ContaId,
                        A.Identificacao,
                        A.Descricao, 
                        A.TabelaId,
                        A.EstagioNegociacao, 
                        A.StatusOportunidade, 
                        A.SucessoNegociacao,
                        A.TipoDeProposta,
                        B.Descricao As ContaDescricao
                    FROM 
                        CRM.TB_CRM_OPORTUNIDADES A
                    INNER JOIN
                        CRM.TB_CRM_CONTAS B ON A.ContaId = B.Id
                    WHERE 
                        (UPPER(A.Descricao) LIKE :CriterioDescricao OR A.Identificacao LIKE :CriterioIdentificacao OR B.Documento = :CriterioDocumento) {filtroSQL}
                    AND 
                        ROWNUM < 30", parametros).ToList();
            }
        }

        public IEnumerable<OportunidadeDTO> ObterOportunidadesPorConta(int contaId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ContaId", value: contaId, direction: ParameterDirection.Input);

                return con.Query<OportunidadeDTO>(@"
                    SELECT
	                    A.Id,
                        A.Identificacao,
                        A.ContaId,
                        A.SucessoNegociacao,
	                    A.CriadoPor,
	                    A.Descricao,
	                    A.EstagioNegociacao,
	                    A.StatusOportunidade,
	                    A.TipoDeProposta,
	                    A.UltimaAlteracao,
                        A.TipoOperacao,
                        A.DataInicio,
                        A.DataTermino,
                        A.ModeloId,
                        A.FormaPagamento,
                        A.DiasFreeTime,
                        A.QtdeDias,
                        A.Validade,
                        A.TipoValidade,
                        A.VendedorId,
                        A.ImpostoId,
                        C.Login As CriadoPor,
                        B.Descricao As ContaDescricao
                    FROM
	                    CRM.TB_CRM_OPORTUNIDADES A
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS B ON A.ContaId = B.Id
                    LEFT JOIN
                        CRM.TB_CRM_USUARIOS C ON A.CriadoPor = C.Id
                    WHERE
                        A.ContaId = :ContaId
                    ORDER BY
	                    A.Descricao", parametros).ToList();
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: id, direction: ParameterDirection.Input);

                using (var transaction = con.BeginTransaction())
                {
                    con.Execute(@"DELETE FROM CRM.TB_CRM_ANEXOS WHERE IdProcesso = :OportunidadeId", parametros);
                    con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_CLIENTES WHERE OportunidadeId = :OportunidadeId", parametros);
                    con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT WHERE OportunidadeId = :OportunidadeId", parametros);
                    con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_GRUPO_CNPJ WHERE OportunidadeId = :OportunidadeId", parametros);
                    con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_PREMIOS WHERE OportunidadeId = :OportunidadeId", parametros);
                    con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_LAYOUT WHERE OportunidadeId = :OportunidadeId", parametros);
                    con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADES WHERE Id = :OportunidadeId", parametros);

                    transaction.Commit();
                }
            }
        }

        public void IncluirSubCliente(int segmento, int contaId, int oportunidadeId, int criadoPor)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Segmento", value: segmento, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaId", value: contaId, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "CriadoPor", value: criadoPor, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_OPORTUNIDADE_CLIENTES (Id, Segmento, ContaId, OportunidadeId, CriadoPor) VALUES (CRM.SEQ_CRM_OPORTUNIDADE_CLIENTES.NEXTVAL, :segmento, :contaId, :oportunidadeId, :criadoPor)", parametros);
            }
        }

        public void ExcluirSubCliente(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_CLIENTES WHERE Id = :id", new { id });
            }
        }

        public ClientePropostaDTO ObterSubClientePorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<ClientePropostaDTO>(@"SELECT Id, ContaId, Segmento, OportunidadeId FROM CRM.TB_CRM_OPORTUNIDADE_CLIENTES WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }

        public void IncluirClienteGrupoCNPJ(int contaId, int oportunidadeId, int criadoPor)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"INSERT INTO CRM.TB_CRM_OPORTUNIDADE_GRUPO_CNPJ (Id, ContaId, OportunidadeId, CriadoPor) VALUES (CRM.SEQ_CRM_OPORTUNIDADE_GRUPOCNPJ.NEXTVAL, :contaId, :oportunidadeId, :criadoPor)", new { contaId, oportunidadeId, criadoPor });
            }
        }

        public void ExcluirClienteGrupoCNPJ(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_GRUPO_CNPJ WHERE Id = :id", new { id });
            }
        }

        public IEnumerable<ClientePropostaDTO> ObterClientesGrupoCNPJ(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<ClientePropostaDTO>(@"
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
                        B.OportunidadeId = :oportunidadeId", new { oportunidadeId });
            }
        }

        public IEnumerable<ClientePropostaDTO> ObterClientesGrupoCNPJPorConta(int contaId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<ClientePropostaDTO>(@"
                    SELECT
                        C.Identificacao As OportunidadeIdentificacao,
                        B.Id,
                        B.ContaId,
                        D.Login As CriadoPor,
                        A.Descricao,
                        A.Documento,
                        A.NomeFantasia,
                        B.DataCriacao
                    FROM
                        CRM.TB_CRM_CONTAS A 
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADE_GRUPO_CNPJ B ON A.Id = B.ContaId
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADES C ON B.OportunidadeId = C.Id
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS D ON A.CriadoPor = D.Id
                    WHERE
                        B.ContaId = :contaId", new { contaId });
            }
        }

        public ClientePropostaDTO ObterClienteGrupoCNPJPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<ClientePropostaDTO>(@"SELECT Id, ContaId, OportunidadeId FROM CRM.TB_CRM_OPORTUNIDADE_GRUPO_CNPJ WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }

        public IEnumerable<ClientePropostaDTO> ObterSubClientes(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<ClientePropostaDTO>(@"
                    SELECT
                        B.Id,
                        DECODE(C.Login, NULL, C.LoginExterno, C.Login) As CriadoPor,
                        b.ContaId,
                        A.Descricao,
                        A.Documento,
                        A.NomeFantasia,
                        B.Segmento,
                        B.DataCriacao
                    FROM
                        CRM.TB_CRM_CONTAS A 
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADE_CLIENTES B ON A.Id = B.ContaId
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS C ON B.CriadoPor = C.Id
                    WHERE
                        B.OportunidadeId = :oportunidadeId", new { oportunidadeId });
            }
        }

        public IEnumerable<ClientePropostaDTO> ObterSubClientesEdicaoAdendo(int oportunidadeId, AdendoAcao adendoAcao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "AdendoAcao", value: adendoAcao, direction: ParameterDirection.Input);

                return con.Query<ClientePropostaDTO>(@"
                    SELECT
                        DISTINCT
                            B.Id,
                            C.Login As CriadoPor,
                            b.ContaId,
                            A.Descricao,
                            A.Documento,
                            A.NomeFantasia,
                            B.Segmento,
                            B.DataCriacao,
                            (SELECT COUNT(1) FROM CRM.TB_CRM_ADENDO_SUB_CLIENTE WHERE AdendoId = D.Id AND Acao = :adendoAcao AND SubClienteId = A.Id) Checado
                    FROM
                        CRM.TB_CRM_CONTAS A 
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADE_CLIENTES B ON A.Id = B.ContaId
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS C ON B.CriadoPor = C.Id
                    LEFT JOIN
                        CRM.TB_CRM_OPORTUNIDADE_ADENDOS D ON B.OportunidadeId = D.OportunidadeId
                    WHERE
                        B.OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public IEnumerable<ClientePropostaDTO> ObterSubClientesPorConta(int contaId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<ClientePropostaDTO>(@"
                    SELECT
                        B.Id,
                        C.Login As CriadoPor,
                        b.ContaId,
                        A.Descricao,
                        A.Documento,
                        A.NomeFantasia,
                        B.Segmento,
                        D.Id As OportunidadeId,
                        D.Identificacao As OportunidadeIdentificacao,
                        D.Descricao As OportunidadeDescricao,
                        D.StatusOportunidade,
                        B.DataCriacao
                    FROM
                        CRM.TB_CRM_CONTAS A 
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADE_CLIENTES B ON A.Id = B.ContaId
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS C ON A.CriadoPor = C.Id
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADES D ON B.OportunidadeId = D.Id
                    WHERE
                        B.ContaId = :contaId", new { contaId });
            }
        }

        public ClientePropostaDTO ObterSubClientePorContaEOportunidade(int contaId, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ContaId", value: contaId, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<ClientePropostaDTO>(@"
                    SELECT
                        B.Id,
                        C.Login As CriadoPor,
                        b.ContaId,
                        A.Descricao,
                        A.Documento,
                        A.NomeFantasia,
                        B.Segmento,
                        D.Id As OportunidadeId,
                        D.Descricao As OportunidadeDescricao,
                        D.StatusOportunidade,
                        B.DataCriacao
                    FROM
                        CRM.TB_CRM_CONTAS A 
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADE_CLIENTES B ON A.Id = B.ContaId
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS C ON A.CriadoPor = C.Id
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADES D ON B.OportunidadeId = D.Id
                    WHERE
                        B.ContaId = :ContaId
                    AND
                        D.Id = :OportunidadeId", parametros).FirstOrDefault();
            }
        }

        public IEnumerable<Conta> ObterContasDaOportunidade(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Conta>(@"
                    SELECT
                        A.Id,
                        A.Descricao,
                        A.Documento,
                        A.DataCriacao
                    FROM
                        CRM.TB_CRM_CONTAS A 
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADES B ON A.Id = B.ContaId
                    WHERE
                        B.Id = :oportunidadeId", new { oportunidadeId });
            }
        }

        public IEnumerable<ClientePropostaDTO> ObterSubClientesPorDescricao(string descricao, int? usuarioId)
        {
            var criterio = "%" + descricao.ToUpper() + "%";

            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Criterio", value: criterio, direction: ParameterDirection.Input);

                var filtroSQL = string.Empty;

                if (usuarioId.HasValue)
                {
                    parametros.Add(name: "UsuarioId", value: usuarioId, direction: ParameterDirection.Input);
                    filtroSQL = " AND A.Id IN (SELECT ContaId FROM TB_CRM_USUARIOS_CONTAS WHERE UsuarioId = :UsuarioId) ";
                }

                return con.Query<ClientePropostaDTO>($@"
                    SELECT
                        C.Identificacao As OportunidadeIdentificacao,
                        B.OportunidadeId,
                        A.Descricao,
                        A.Documento,
                        A.NomeFantasia,                      
                        B.Segmento,
                        C.Descricao As OportunidadeDescricao,
                        C.StatusOportunidade,
                        B.DataCriacao
                    FROM
                        CRM.TB_CRM_CONTAS A 
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADE_CLIENTES B ON A.Id = B.ContaId
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADES C ON B.OportunidadeId = C.Id
                    WHERE
                        (A.Descricao LIKE :Criterio OR A.Documento LIKE :Criterio) {filtroSQL}
                    AND 
                        ROWNUM < 300", parametros);
            }
        }

        public IEnumerable<ClientePropostaDTO> ObterClientesGrupoCNPJPorDescricao(string descricao, int? usuarioId)
        {
            var criterio = "%" + descricao.ToUpper() + "%";

            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Criterio", value: criterio, direction: ParameterDirection.Input);

                var filtroSQL = string.Empty;

                if (usuarioId.HasValue)
                {
                    parametros.Add(name: "UsuarioId", value: usuarioId, direction: ParameterDirection.Input);
                    filtroSQL = " AND A.Id IN (SELECT ContaId FROM TB_CRM_USUARIOS_CONTAS WHERE UsuarioId = :UsuarioId) ";
                }

                return con.Query<ClientePropostaDTO>($@"
                    SELECT
                        C.Identificacao As OportunidadeIdentificacao,
                        B.OportunidadeId,
                        A.Descricao,
                        A.Documento,
                        A.NomeFantasia,
                        B.DataCriacao
                    FROM
                        CRM.TB_CRM_CONTAS A 
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADE_GRUPO_CNPJ B ON A.Id = B.ContaId
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADES C ON B.OportunidadeId = C.Id
                    WHERE
                        (A.Descricao LIKE :Criterio OR A.Documento LIKE :Criterio) {filtroSQL}
                    AND 
                        ROWNUM < 300", parametros);
            }
        }

        public IEnumerable<ClientePropostaDTO> ObterSubClientesDaProposta(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<ClientePropostaDTO>(@"
                    SELECT
                        A.ID,
                        B.ContaId,
                        C.Login As CriadoPor,
                        A.Descricao,
                        A.Documento,
                        A.NomeFantasia,                      
                        B.Segmento,
                        B.DataCriacao
                    FROM
                        CRM.TB_CRM_CONTAS A 
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADE_CLIENTES B ON A.Id = B.ContaId
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS C ON B.CriadoPor = C.Id
                    WHERE
                        B.OportunidadeId = :oportunidadeId

                    UNION ALL

                    SELECT
                        A.ID,
                        B.ContaId,
                        C.Login As CriadoPor,
                        A.Descricao,
                        A.Documento,
                        A.NomeFantasia,                      
                        1 As Segmento,
                        B.DataCriacao
                    FROM
                        CRM.TB_CRM_CONTAS A 
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADE_GRUPO_CNPJ B ON A.Id = B.ContaId
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS C ON B.CriadoPor = C.Id
                    WHERE
                        B.OportunidadeId = :oportunidadeId", new { oportunidadeId });
            }
        }

        public IEnumerable<ClientePropostaDTO> ObterSubClientesDaPropostaPorSegmento(int oportunidadeId, SegmentoSubCliente segmento)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Segmento", value: segmento, direction: ParameterDirection.Input);

                return con.Query<ClientePropostaDTO>(@"
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
                    WHERE
                        B.OportunidadeId = :OportunidadeId
                    AND 
                        B.Segmento = :Segmento", parametros);
            }
        }

        public IEnumerable<AnexosDTO> ObterAnexosDaOportunidade(int oportunidadeId, bool usuarioExterno)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                var filtroSQL = string.Empty;

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "UsuarioExterno", value: usuarioExterno, direction: ParameterDirection.Input);

                if (usuarioExterno)
                {
                    filtroSQL = " AND UPPER(SUBSTR(A.Anexo, LENGTH(A.Anexo)-3, 4)) <> '.MSG' ";
                }

                return con.Query<AnexosDTO>($@"
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
                        A.IdProcesso = :OportunidadeId {filtroSQL}
                    AND
                        (A.TipoDocto = 1 OR A.TipoDocto = 3)
                    ORDER BY
                        A.DataCadastro DESC", parametros);
            }
        }

        public IEnumerable<AnexosNotasDTO> ObterNotasDaOportunidade(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<AnexosNotasDTO>(@"
                    SELECT
                        A.Id,
                        A.OportunidadeId,
                        B.Login As CriadoPor,
                        A.Nota,
                        A.Descricao,
                        A.DataCriacao
                    FROM
                        CRM.TB_CRM_OPORTUNIDADE_NOTAS A
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id
                    WHERE
                        A.OportunidadeId = :oportunidadeId
                    ORDER BY
                        A.DataCriacao DESC", new { oportunidadeId });
            }
        }
        public int ObterStatusCondPgto(int oportunidadeId )
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "oportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
            
                return con.Query<int>(@"
SELECT NVL (
           MAX (
               CASE
                   WHEN NVL (D.PRZMED, 0) < 31 THEN 3
                   ELSE NVL (C.StatusLimiteCredito, 0)
               END),
           0)
  FROM crm.tb_crm_oportunidades  a
       INNER JOIN crm.tb_crm_oportunidade_ficha_fat b
           ON a.id = b.oportunidadeid
       LEFT JOIN crm.tb_crm_spc_limite_credito c
           ON      b.fontepagadoraid  = c.contaid
              AND b.condicaopagamentofaturamentoid = c.condicaopagamentoid
       LEFT JOIN FATURA.TB_COND_PGTO D
           ON b.condicaopagamentofaturamentoid = D.CODCPG where      a . id  = :oportunidadeId", parametros).Single();


            }
        }

        public int ObterStatusCondPgtoNew(int contaid, string condpgtoid)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "contaId", value: contaid, direction: ParameterDirection.Input);
                parametros.Add(name: "condpgtoId", value: condpgtoid, direction: ParameterDirection.Input);

                return con.Query<int>(@"
SELECT NVL (
           MAX (
               CASE
                   WHEN NVL (D.PRZMED, 0) < 31 THEN 3
                   ELSE NVL (C.StatusLimiteCredito, 0)
               END),
           0)
  FROM  ( SELECT * FROM FATURA.TB_COND_PGTO WHERE CODCPG= :condpgtoId) D  
       LEFT  JOIN crm.tb_crm_spc_limite_credito c
           ON     c.contaid = :contaId
              AND  c.condicaopagamentoid=:condpgtoid
      ", parametros).Single();


            }
        }



        public IEnumerable<FichaFaturamentoDTO> ObterFichasFaturamento(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FichaFaturamentoDTO>(@"
                    SELECT
                        A.Id,
                        A.ContaId,
                        A.FaturadoContraId,
                        C.Descricao AS Cliente,
                        C.Documento AS ClienteDocumento,
                        CASE WHEN A.ContaId = B.ContaId THEN 1 ELSE 0 END MesmaContaOportunidade,
                        D.Descricao AS FaturadoContra,
                        A.EmailFaturamento,
                        A.DiasSemana,
                        A.DiasFaturamento,
                        A.DataCorte,
                        CASE WHEN A.RevisaoId > 0 THEN A.RevisaoId ELSE NULL END RevisaoId,
                        E.DESCPG As CondicaoPagamento,
                        A.StatusFichaFaturamento,
                        B.StatusOportunidade,                        
                        RAWTOHEX(F.IdFile) IdFile
                    FROM
                        CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT A
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADES B ON A.OportunidadeId = B.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS C ON A.ContaId = C.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS D ON A.FaturadoContraId = D.Id
                    LEFT JOIN
                        FATURA.TB_COND_PGTO E ON A.CondicaoPagamentoFaturamentoId = E.CODCPG
                    LEFT JOIN
                        CRM.TB_CRM_ANEXOS F ON A.AnexoFaturamentoId = F.Id
                    WHERE
                        A.OportunidadeId = :oportunidadeId
                    ORDER BY
                        A.Id", new { oportunidadeId });
            }
        }

        public FichaFaturamentoDTO ObterDetalhesFichaFaturamento(int fichaFaturamentoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FichaFaturamentoDTO>(@"
                    SELECT
                        A.Id,
                        A.OportunidadeId,
                        A.ContaId,
                        A.FaturadoContraId,
                        B.Descricao AS FaturadoContra,
                        B.Documento AS FaturadoContraDocumento,
                        C.Descricao As Cliente,
                        C.Documento As ClienteDocumento,
                        A.EmailFaturamento,
                        (
                            SELECT
                                rtrim(
                                    xmlagg(xmlelement(
                                        e,
                                            Descricao || ','
                                        )).extract('//text()'),
                                    ','
                                ) dias
                            FROM
                                CRM.TB_CRM_DIAS
                            WHERE
                                Id IN (SELECT COLUMN_VALUE FROM TABLE (SPLIT (A.DiasSemana)))        
                        ) DiasSemana,
                        A.DiasSemana As DiasSemanaLiterais,
                        A.DiasFaturamento,
                        A.DataCorte,
                        A.RevisaoId,
                        D.CODCPG As CondicaoPagamentoId,
                        D.DESCPG As CondicaoPagamento,
                        A.StatusFichaFaturamento,
                        A.ObservacoesFaturamento,
                        A.AnexoFaturamentoId,
                        A.OportunidadeId,
                        A.FontePagadoraId,
                        E.Descricao As FontePagadora,
                        E.Documento As FontePagadoraDocumento,
                        A.CondicaoPgtoDia As CondicaoPagamentoPorDia,
                        A.CondicaoPgtoDiaSemana As CondicaoPagamentoPorDiaSemana,
                        A.EntregaManual,
                        A.CorreiosSedex As EntregaManualSedex,
                        A.DiaUtil,
                        A.UltimoDiaDoMes,
                        A.EntregaEletronica
                    FROM
                        CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT A
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS B ON A.FaturadoContraId = B.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS C ON A.ContaId = C.Id
                    LEFT JOIN
                        FATURA.TB_COND_PGTO D ON A.CondicaoPagamentoFaturamentoId = D.CODCPG
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS E ON A.FontePagadoraId = E.Id
                    WHERE
                        A.Id = :fichaFaturamentoId", new { fichaFaturamentoId }).FirstOrDefault();
            }
        }

        public OportunidadeFichaFaturamento ObterFichaFaturamentoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<OportunidadeFichaFaturamento>(@"
                    SELECT
                        A.Id,
                        A.OportunidadeId,
                        A.ContaId,
                        A.FaturadoContraId,
                        A.DiasSemana,
                        A.DiasFaturamento,
                        A.DataCorte,
                        A.CondicaoPagamentoFaturamentoId,
                        A.EmailFaturamento,
                        A.ObservacoesFaturamento,
                        A.StatusFichaFaturamento,
                        A.AnexoFaturamentoId,
                        B.Descricao As FaturadoContraDescricao,
                        C.Descricao As FontePagadoraDescricao,
                        A.CondicaoPgtoDia As CondicaoPagamentoPorDia,
                        A.CondicaoPgtoDiaSemana As CondicaoPagamentoPorDiaSemana,
                        A.EntregaManual,
                        A.CorreiosSedex,
                        A.DiaUtil,
                        A.UltimoDiaDoMes,
                        A.EntregaEletronica,
                        A.FontePagadoraId
                    FROM
                        CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT A
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS B ON A.FaturadoContraId = B.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS C ON A.FontePagadoraId = C.Id
                    WHERE
                        A.Id = :id", new { id }).FirstOrDefault();
            }
        }

        public OportunidadeFichaFaturamento ObterFichaFaturamentoPorClienteCnpj(string clienteCnpj, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ClienteCnpj", value: clienteCnpj, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<OportunidadeFichaFaturamento>(@"
                    SELECT
                        A.Id,
                        A.OportunidadeId,
                        A.ContaId,
                        A.FaturadoContraId,
                        A.DiasSemana,
                        A.DiasFaturamento,
                        A.DataCorte,
                        A.CondicaoPagamentoFaturamentoId,
                        A.EmailFaturamento,
                        A.ObservacoesFaturamento,
                        A.StatusFichaFaturamento,
                        A.AnexoFaturamentoId,
                        B.Descricao As FaturadoContraDescricao,
                        C.Descricao As FontePagadoraDescricao,
                        A.CondicaoPgtoDia As CondicaoPagamentoPorDia,
                        A.CondicaoPgtoDiaSemana As CondicaoPagamentoPorDiaSemana,
                        A.EntregaManual,
                        A.CorreiosSedex,
                        A.DiaUtil,
                        A.UltimoDiaDoMes,
                        A.EntregaEletronica
                    FROM
                        CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT A
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS B ON A.FaturadoContraId = B.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS C ON A.FontePagadoraId = C.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS D ON A.ContaId = D.Id
                    WHERE
                        D.Documento = :ClienteCnpj
                    AND
                        A.OportunidadeId = :OportunidadeId", parametros).FirstOrDefault();
            }
        }

        public int IncluirFichaFaturamento(OportunidadeFichaFaturamento ficha)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "StatusFichaFaturamento", value: ficha.StatusFichaFaturamento, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: ficha.OportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "StatusFichaFaturamento", value: ficha.StatusFichaFaturamento, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaId", value: ficha.ContaId, direction: ParameterDirection.Input);
                parametros.Add(name: "FaturadoContraId", value: ficha.FaturadoContraId, direction: ParameterDirection.Input);
                parametros.Add(name: "DiasSemana", value: ficha.DiasSemana, direction: ParameterDirection.Input);
                parametros.Add(name: "DiasFaturamento", value: ficha.DiasFaturamento, direction: ParameterDirection.Input);
                parametros.Add(name: "DataCorte", value: ficha.DataCorte, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicaoPagamentoFaturamentoId", value: ficha.CondicaoPagamentoFaturamentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "EmailFaturamento", value: ficha.EmailFaturamento, direction: ParameterDirection.Input);
                parametros.Add(name: "ObservacoesFaturamento", value: ficha.ObservacoesFaturamento, direction: ParameterDirection.Input);
                parametros.Add(name: "AnexoFaturamentoId", value: ficha.AnexoFaturamentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "FontePagadoraId", value: ficha.FontePagadoraId, direction: ParameterDirection.Input);                
                parametros.Add(name: "CondicaoPgtoDia", value: ficha.CondicaoPagamentoPorDia, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicaoPgtoDiaSemana", value: ficha.CondicaoPagamentoPorDiaSemana, direction: ParameterDirection.Input);
                parametros.Add(name: "EntregaManual", value: ficha.EntregaManual.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "CorreiosSedex", value: ficha.EntregaManualSedex.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "DiaUtil", value: ficha.DiaUtil.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "UltimoDiaDoMes", value: ficha.UltimoDiaDoMes.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "EntregaEletronica", value: ficha.EntregaEletronica.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "RevisaoId", value: ficha.RevisaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"
                            INSERT 
                                INTO CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT 
                                    (
                                        Id, 
                                        OportunidadeId, 
                                        StatusFichaFaturamento, 
                                        ContaId, 
                                        FaturadoContraId, 
                                        DiasSemana, 
                                        DiasFaturamento,
                                        DataCorte,
                                        CondicaoPagamentoFaturamentoId,
                                        EmailFaturamento,
                                        ObservacoesFaturamento,
                                        AnexoFaturamentoId,
                                        FontePagadoraId,
                                        CondicaoPgtoDia,
                                        CondicaoPgtoDiaSemana,
                                        EntregaManual,
                                        CorreiosSedex,
                                        DiaUtil,
                                        UltimoDiaDoMes,
                                        EntregaEletronica,
                                        RevisaoId
                                    ) VALUES (
                                        CRM.SEQ_CRM_OPORTUNIDADE_FICHA_FAT.NEXTVAL, 
                                        :OportunidadeId, 
                                        :StatusFichaFaturamento,
                                        :ContaId, 
                                        :FaturadoContraId, 
                                        :DiasSemana, 
                                        :DiasFaturamento,
                                        :DataCorte,
                                        :CondicaoPagamentoFaturamentoId, 
                                        :EmailFaturamento, 
                                        :ObservacoesFaturamento,
                                        :AnexoFaturamentoId,
                                        :FontePagadoraId,
                                        :CondicaoPgtoDia,
                                        :CondicaoPgtoDiaSemana,
                                        :EntregaManual,
                                        :CorreiosSedex,
                                        :DiaUtil,
                                        :UltimoDiaDoMes,
                                        :EntregaEletronica,
                                        :RevisaoId) RETURNING Id INTO :Id", parametros);

                var id = parametros.Get<int>("Id");

                return id;
            }
        }

        public IEnumerable<OportunidadeFichaFaturamento> ObterFichasFaturamentoPorStatus(StatusFichaFaturamento statusFichaFaturamento)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "StatusFichaFaturamento", value: statusFichaFaturamento, direction: ParameterDirection.Input);

                return con.Query<OportunidadeFichaFaturamento>($@"SELECT * FROM CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT WHERE StatusFichaFaturamento = :StatusFichaFaturamento", parametros);
            }
        }

        public bool ExisteFichaFaturamento(OportunidadeFichaFaturamento ficha)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: ficha.OportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaId", value: ficha.ContaId, direction: ParameterDirection.Input);

                return con.Query<bool>(@"SELECT * FROM CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT WHERE OportunidadeId = :OportunidadeId AND ContaId = :ContaId", parametros).Any();
            }
        }

        public void AtualizarFichaFaturamento(OportunidadeFichaFaturamento ficha)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "FaturadoContraId", value: ficha.FaturadoContraId, direction: ParameterDirection.Input);
                parametros.Add(name: "FontePagadoraId", value: ficha.FontePagadoraId, direction: ParameterDirection.Input);
                parametros.Add(name: "DiasSemana", value: ficha.DiasSemana, direction: ParameterDirection.Input);
                parametros.Add(name: "DiasFaturamento", value: ficha.DiasFaturamento, direction: ParameterDirection.Input);
                parametros.Add(name: "DataCorte", value: ficha.DataCorte, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicaoPagamentoFaturamentoId", value: ficha.CondicaoPagamentoFaturamentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "EmailFaturamento", value: ficha.EmailFaturamento, direction: ParameterDirection.Input);
                parametros.Add(name: "ObservacoesFaturamento", value: ficha.ObservacoesFaturamento, direction: ParameterDirection.Input);
                parametros.Add(name: "AnexoFaturamentoId", value: ficha.AnexoFaturamentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicaoPgtoDia", value: ficha.CondicaoPagamentoPorDia, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicaoPgtoDiaSemana", value: ficha.CondicaoPagamentoPorDiaSemana, direction: ParameterDirection.Input);
                parametros.Add(name: "EntregaManual", value: ficha.EntregaManual.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "CorreiosSedex", value: ficha.EntregaManualSedex.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "DiaUtil", value: ficha.DiaUtil.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "UltimoDiaDoMes", value: ficha.UltimoDiaDoMes.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "EntregaEletronica", value: ficha.EntregaEletronica.ToInt(), direction: ParameterDirection.Input);

                parametros.Add(name: "Id", value: ficha.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT 
                                SET                                       
                                    FaturadoContraId = :FaturadoContraId, 
                                    FontePagadoraId = :FontePagadoraId,
                                    DiasSemana = :DiasSemana, 
                                    DiasFaturamento = :DiasFaturamento,
                                    DataCorte = :DataCorte,
                                    CondicaoPagamentoFaturamentoId = :CondicaoPagamentoFaturamentoId,
                                    EmailFaturamento = :EmailFaturamento,
                                    ObservacoesFaturamento = :ObservacoesFaturamento,
                                    AnexoFaturamentoId = :AnexoFaturamentoId,
                                    CondicaoPgtoDia = :CondicaoPgtoDia,
                                    CondicaoPgtoDiaSemana = :CondicaoPgtoDiaSemana,
                                    EntregaManual = :EntregaManual,
                                    CorreiosSedex = :CorreiosSedex,
                                    DiaUtil = :DiaUtil,
                                    UltimoDiaDoMes = :UltimoDiaDoMes,
                                    EntregaEletronica = :EntregaEletronica
                                WHERE Id = :Id", parametros);
            }
        }

        public void ExcluirFichasFaturamentoDaOportunidade(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT WHERE OportunidadeId = :oportunidadeId", new { oportunidadeId });
            }
        }

        public void ExcluirFichaFaturamento(int fichaFaturamentoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Id", value: fichaFaturamentoId, direction: ParameterDirection.Input);

                    con.Execute(@"DELETE FROM CRM.TB_CRM_ANEXOS WHERE Id IN (SELECT ANEXOFATURAMENTOID FROM TB_CRM_OPORTUNIDADE_FICHA_FAT WHERE Id = :Id)", parametros);
                    con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT WHERE Id = :Id", parametros);

                    transaction.Commit();
                }
            }
        }

        public void AtualizarStatusFichaFaturamento(StatusFichaFaturamento status, int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "StatusFichaFaturamento", value: status, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT SET StatusFichaFaturamento = :StatusFichaFaturamento WHERE Id = :Id", parametros);
            }
        }

        public bool ExisteLayoutNaProposta(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<bool>($@"SELECT Id FROM CRM.TB_CRM_OPORTUNIDADE_LAYOUT WHERE OportunidadeId = :oportunidadeId", new { oportunidadeId }).Any();
            }
        }

        public int IncluirAdendo(OportunidadeAdendo adendo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: adendo.OportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoAdendo", value: adendo.TipoAdendo, direction: ParameterDirection.Input);
                parametros.Add(name: "StatusAdendo", value: adendo.StatusAdendo, direction: ParameterDirection.Input);
                parametros.Add(name: "CriadoPor", value: adendo.CriadoPor, direction: ParameterDirection.Input);
                parametros.Add(name: "DataCadastro", value: DateTime.Now, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"
                        INSERT INTO 
                            CRM.TB_CRM_OPORTUNIDADE_ADENDOS 
                                (
                                    Id, 
                                    OportunidadeId, 
                                    TipoAdendo, 
                                    StatusAdendo, 
                                    CriadoPor, 
                                    DataCadastro
                                ) VALUES (
                                    CRM.SEQ_CRM_OPORTUNIDADE_ADENDOS.NEXTVAL, 
                                    :OportunidadeId, 
                                    :TipoAdendo, 
                                    :StatusAdendo, 
                                    :CriadoPor, 
                                    :DataCadastro) 
                        RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public IEnumerable<OportunidadeAdendosDTO> ObterAdendos(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<OportunidadeAdendosDTO>(@"
                        SELECT
                            A.Id,
                            A.TipoAdendo,
                            B.Login As CriadoPor,
                            A.StatusAdendo,
                            A.DataCadastro,
                            CASE WHEN A.TipoAdendo = 2 THEN 
                                (SELECT DISTINCT RAWTOHEX(Anexo.IdFile) IdFile FROM TB_CRM_ADENDO_FORMA_PAGAMENTO Adendo LEFT JOIN TB_CRM_ANEXOS Anexo ON Adendo.AnexoId = Anexo.Id WHERE Adendo.AdendoId = A.Id)
                            WHEN TipoAdendo = 6 THEN                                
                                (SELECT DISTINCT RAWTOHEX(Anexo.IdFile) IdFile FROM TB_CRM_ADENDO_GRUPO_CNPJ Adendo LEFT JOIN TB_CRM_ANEXOS Anexo ON Adendo.AnexoId = Anexo.Id WHERE Adendo.AdendoId = A.Id)
                            WHEN TipoAdendo = 4 THEN
                                (SELECT DISTINCT RAWTOHEX(Anexo.IdFile) IdFile FROM TB_CRM_ADENDO_SUB_CLIENTE Adendo LEFT JOIN TB_CRM_ANEXOS Anexo ON Adendo.AnexoId = Anexo.Id WHERE Adendo.AdendoId = A.Id)
                            END IdFile
                        FROM
                            CRM.TB_CRM_OPORTUNIDADE_ADENDOS A
                        INNER JOIN
                            CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id
                        WHERE
                            A.OportunidadeId = :oportunidadeId
                        ORDER BY
                            A.DataCadastro DESC", new { oportunidadeId });
            }
        }

        public IEnumerable<OportunidadeAdendosDTO> ObterAdendos(AdendosFiltro filtro)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                var filtroSQL = new StringBuilder();

                if (filtro != null)
                {
                    if (filtro.Id.HasValue)
                    {
                        parametros.Add(name: "Id", value: filtro.Id.Value, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.Id = :Id");
                    }

                    if (filtro.OportunidadeId.HasValue)
                    {
                        parametros.Add(name: "OportunidadeId", value: filtro.OportunidadeId.Value, direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND A.OportunidadeId = :OportunidadeId");
                    }

                    if (!string.IsNullOrEmpty(filtro.DescricaoCliente))
                    {
                        parametros.Add(name: "DescricaoCliente", value: "%" + filtro.DescricaoCliente.ToLower() + "%", direction: ParameterDirection.Input);
                        filtroSQL.Append(" AND (LOWER(E.Descricao) LIKE :DescricaoCliente OR LOWER(F.Descricao) LIKE :DescricaoCliente) ");
                    }
                }

                return con.Query<OportunidadeAdendosDTO>($@"
                        SELECT
                            A.Id,
                            A.TipoAdendo,
                            B.Login As CriadoPor,
                            A.StatusAdendo,
                            A.DataCadastro,
                            CASE WHEN A.TipoAdendo = 2 THEN 
                                (SELECT DISTINCT RAWTOHEX(Anexo.IdFile) IdFile FROM TB_CRM_ADENDO_FORMA_PAGAMENTO Adendo LEFT JOIN TB_CRM_ANEXOS Anexo ON Adendo.AnexoId = Anexo.Id WHERE Adendo.AdendoId = A.Id)
                            WHEN TipoAdendo = 6 THEN                                
                                (SELECT DISTINCT RAWTOHEX(Anexo.IdFile) IdFile FROM TB_CRM_ADENDO_GRUPO_CNPJ Adendo LEFT JOIN TB_CRM_ANEXOS Anexo ON Adendo.AnexoId = Anexo.Id WHERE Adendo.AdendoId = A.Id)
                            WHEN TipoAdendo = 4 THEN
                                (SELECT DISTINCT RAWTOHEX(Anexo.IdFile) IdFile FROM TB_CRM_ADENDO_SUB_CLIENTE Adendo LEFT JOIN TB_CRM_ANEXOS Anexo ON Adendo.AnexoId = Anexo.Id WHERE Adendo.AdendoId = A.Id)
                            END IdFile
                        FROM
                            CRM.TB_CRM_OPORTUNIDADE_ADENDOS A
                        INNER JOIN
                            CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id
                        LEFT JOIN
                            CRM.TB_CRM_ADENDO_SUB_CLIENTE C ON C.AdendoId = A.Id
                        LEFT JOIN
                            CRM.TB_CRM_ADENDO_GRUPO_CNPJ D ON D.AdendoId = A.Id
                        LEFT JOIN
                            CRM.TB_CRM_CONTAS E ON C.SubClienteId = E.Id
                        LEFT JOIN
                            CRM.TB_CRM_CONTAS F ON D.GrupoCnpjId = F.Id
                        WHERE
                            A.Id > 0 {filtroSQL.ToString()}
                        AND
                            A.OportunidadeId = :OportunidadeId
                        ORDER BY
                            A.DataCadastro DESC", parametros);
            }
        }

        public IEnumerable<OportunidadeAdendo> ObterAdendosOportunidadePorStatus(StatusAdendo statusAdendo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "StatusAdendo", value: statusAdendo, direction: ParameterDirection.Input);

                return con.Query<OportunidadeAdendo>($@"SELECT * FROM CRM.TB_CRM_OPORTUNIDADE_ADENDOS WHERE StatusAdendo = :StatusAdendo", parametros);
            }
        }

        public OportunidadeAdendo ObterAdendoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<OportunidadeAdendo>(@"
                        SELECT
                            Id,
                            OportunidadeId,
                            TipoAdendo,
                            CriadoPor,
                            StatusAdendo,
                            DataCadastro                       
                        FROM
                            CRM.TB_CRM_OPORTUNIDADE_ADENDOS
                        WHERE
                            Id = :id", new { id }).FirstOrDefault();
            }
        }

        public void ExcluirAdendo(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                    con.Execute(@"DELETE FROM CRM.TB_CRM_ADENDO_VENDEDOR WHERE AdendoId = :Id", parametros);

                    con.Execute(@"DELETE FROM CRM.TB_CRM_ANEXOS WHERE Id IN (SELECT AnexoId FROM TB_CRM_ADENDO_FORMA_PAGAMENTO WHERE AdendoId = :Id)", parametros);
                    con.Execute(@"DELETE FROM CRM.TB_CRM_ADENDO_FORMA_PAGAMENTO WHERE AdendoId = :Id", parametros);

                    con.Execute(@"DELETE FROM CRM.TB_CRM_ANEXOS WHERE Id IN (SELECT AnexoId FROM TB_CRM_ADENDO_GRUPO_CNPJ WHERE AdendoId = :Id)", parametros);
                    con.Execute(@"DELETE FROM CRM.TB_CRM_ADENDO_GRUPO_CNPJ WHERE AdendoId = :Id", parametros);

                    con.Execute(@"DELETE FROM CRM.TB_CRM_ANEXOS WHERE Id IN (SELECT AnexoId FROM TB_CRM_ADENDO_SUB_CLIENTE WHERE AdendoId = :Id)", parametros);
                    con.Execute(@"DELETE FROM CRM.TB_CRM_ADENDO_SUB_CLIENTE WHERE AdendoId = :Id", parametros);

                    con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_ADENDOS WHERE Id = :Id", parametros);

                    transaction.Commit();
                }
            }
        }

        public int IncluirAdendoVendedor(OportunidadeAdendoVendedor adendoVendedor)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "AdendoId", value: adendoVendedor.AdendoId, direction: ParameterDirection.Input);
                parametros.Add(name: "VendedorId", value: adendoVendedor.VendedorId, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_ADENDO_VENDEDOR (Id, AdendoId, VendedorId) VALUES (CRM.SEQ_CRM_ADENDO_VENDEDOR.NEXTVAL, :AdendoId, :VendedorId) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public bool ExisteAdendo(OportunidadeAdendo adendo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: adendo.OportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoAdendo", value: adendo.TipoAdendo, direction: ParameterDirection.Input);

                return con.Query<bool>(@"SELECT Id FROM CRM.TB_CRM_OPORTUNIDADE_ADENDOS WHERE OportunidadeId = :OportunidadeId AND TipoAdendo = :TipoAdendo AND StatusAdendo = 2", parametros).Any();
            }
        }

        public void AtualizarAdendoVendedor(OportunidadeAdendoVendedor adendoVendedor)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Id", value: adendoVendedor.Id, direction: ParameterDirection.Input);
                parametros.Add(name: "VendedorId", value: adendoVendedor.VendedorId, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_ADENDO_VENDEDOR SET VendedorId = :VendedorId WHERE Id = :Id", parametros);
            }
        }

        public OportunidadeAdendoVendedor ObterAdendoVendedor(int adendoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<OportunidadeAdendoVendedor>(@"SELECT * FROM CRM.TB_CRM_ADENDO_VENDEDOR WHERE AdendoId = :adendoId", new { adendoId }).FirstOrDefault();
            }
        }

        public int IncluirAdendoFormaPagamento(OportunidadeAdendoFormaPagamento adendoFormaPagamento)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "AdendoId", value: adendoFormaPagamento.AdendoId, direction: ParameterDirection.Input);
                parametros.Add(name: "AnexoId", value: adendoFormaPagamento.AnexoId, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamento", value: adendoFormaPagamento.FormaPagamento, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_ADENDO_FORMA_PAGAMENTO (Id, AdendoId, AnexoId, FormaPagamento) VALUES (CRM.SEQ_CRM_ADENDO_VENDEDOR.NEXTVAL, :AdendoId, :AnexoId, :FormaPagamento) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void AtualizarAdendoFormaPagamento(OportunidadeAdendoFormaPagamento adendoFormaPagamento)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Id", value: adendoFormaPagamento.Id, direction: ParameterDirection.Input);
                parametros.Add(name: "AnexoId", value: adendoFormaPagamento.AnexoId, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamento", value: adendoFormaPagamento.FormaPagamento, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_ADENDO_FORMA_PAGAMENTO SET FormaPagamento = :FormaPagamento, AnexoId = :AnexoId WHERE Id = :Id", parametros);
            }
        }

        public OportunidadeAdendoFormaPagamento ObterAdendoFormaPagamento(int adendoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<OportunidadeAdendoFormaPagamento>(@"SELECT * FROM CRM.TB_CRM_ADENDO_FORMA_PAGAMENTO WHERE AdendoId = :adendoId", new { adendoId }).FirstOrDefault();
            }
        }    

        public int IncluirAdendoGruposCNPJ(OportunidadeAdendoGrupoCNPJ adendoGrupoCNPJ)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "AdendoId", value: adendoGrupoCNPJ.AdendoId, direction: ParameterDirection.Input);
                parametros.Add(name: "GrupoCNPJId", value: adendoGrupoCNPJ.ClienteId, direction: ParameterDirection.Input);
                parametros.Add(name: "AnexoId", value: adendoGrupoCNPJ.AnexoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Acao", value: adendoGrupoCNPJ.Acao, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_ADENDO_GRUPO_CNPJ (Id, AdendoId, GrupoCNPJId, AnexoId, Acao) VALUES (CRM.SEQ_CRM_ADENDO_GRUPO_CNPJ.NEXTVAL, :AdendoId, :GrupoCNPJId, :AnexoId, :Acao) RETURNING Id INTO :Id", parametros);

                var id = parametros.Get<int>("Id");

                return id;
            }
        }

        public void ExcluirGruposCNPJDoAdendo(int adendoId, AdendoAcao acao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "AdendoId", value: adendoId, direction: ParameterDirection.Input);
                parametros.Add(name: "AdendoAcao", value: acao, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_ADENDO_GRUPO_CNPJ WHERE AdendoId = :AdendoId AND Acao = :AdendoAcao", parametros);
            }
        }

        public bool ExisteAdendoClienteGrupoCNPJ(int oportunidadeId, int grupoCnpjId, AdendoAcao adendoAcao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "GrupoCnpjId", value: grupoCnpjId, direction: ParameterDirection.Input);
                parametros.Add(name: "AdendoAcao", value: adendoAcao, direction: ParameterDirection.Input);

                return con.Query<bool>(@"
                    SELECT 
                        A.Id
                    FROM 
                        TB_CRM_ADENDO_GRUPO_CNPJ A 
                    INNER JOIN 
                        TB_CRM_OPORTUNIDADE_ADENDOS B ON A.AdendoId = B.Id 
                    WHERE 
                        A.GrupoCnpjId = :GrupoCnpjId 
                    AND 
                        A.Acao = :AdendoAcao
                    And 
                        B.OportunidadeId = :OportunidadeId
                    AND 
                        (StatusAdendo = 1 OR StatusAdendo = 2 OR StatusAdendo = 3)", parametros).Any();
            }
        }

        public IEnumerable<ClientePropostaDTO> ObterAdendoGruposCNPJ(int adendoId, AdendoAcao acao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "AdendoId", value: adendoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Acao", value: acao, direction: ParameterDirection.Input);

                return con.Query<ClientePropostaDTO>(@"
                        SELECT 
                            A.Id, 
                            A.AdendoId, 
                            A.GrupoCNPJId, 
                            C.Descricao, 
                            C.Documento,
                            D.Segmento As SegmentoOportunidade
                        FROM 
                            CRM.TB_CRM_ADENDO_GRUPO_CNPJ A 
                        INNER JOIN
                            CRM.TB_CRM_OPORTUNIDADE_ADENDOS B ON A.AdendoId = B.Id
                        INNER JOIN 
                            CRM.TB_CRM_CONTAS C ON A.GrupoCNPJId = C.Id 
                        INNER JOIN 
                            CRM.TB_CRM_OPORTUNIDADES D ON B.OportunidadeId = D.Id 
                        WHERE 
                            A.AdendoId = :AdendoId 
                        AND 
                            A.ACAO = :Acao", parametros);
            }
        }

        public int IncluirAdendoSubCliente(OportunidadeAdendoSubCliente adendoSubCliente)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "AdendoId", value: adendoSubCliente.AdendoId, direction: ParameterDirection.Input);
                parametros.Add(name: "SubClienteId", value: adendoSubCliente.ClienteId, direction: ParameterDirection.Input);
                parametros.Add(name: "Segmento", value: adendoSubCliente.Segmento, direction: ParameterDirection.Input);
                parametros.Add(name: "Acao", value: adendoSubCliente.Acao, direction: ParameterDirection.Input);
                parametros.Add(name: "AnexoId", value: adendoSubCliente.AnexoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_ADENDO_SUB_CLIENTE (Id, AdendoId, SubClienteId, Segmento, Acao, AnexoId) VALUES (CRM.SEQ_CRM_ADENDO_SUB_CLIENTE.NEXTVAL, :AdendoId, :SubClienteId, :Segmento, :Acao, :AnexoId) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void ExcluirSubClientesDoAdendo(int adendoId, AdendoAcao adendoAcao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "AdendoId", value: adendoId, direction: ParameterDirection.Input);
                parametros.Add(name: "AdendoAcao", value: adendoAcao, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_ADENDO_SUB_CLIENTE WHERE AdendoId = :AdendoId AND Acao = :AdendoAcao", parametros);
            }
        }

        public bool ExisteAdendoSubCliente(int oportunidadeId, int subClienteId, AdendoAcao adendoAcao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "SubClienteId", value: subClienteId, direction: ParameterDirection.Input);
                parametros.Add(name: "AdendoAcao", value: adendoAcao, direction: ParameterDirection.Input);

                return con.Query<bool>(@"
                    SELECT 
                        A.Id
                    FROM 
                        TB_CRM_ADENDO_SUB_CLIENTE A 
                    INNER JOIN 
                        TB_CRM_OPORTUNIDADE_ADENDOS B ON A.AdendoId = B.Id                     
                    WHERE 
                        A.SubClienteId = :SubClienteId 
                    AND 
                        A.Acao = :AdendoAcao
                    AND 
                        B.OportunidadeId = :OportunidadeId
                    AND
                       (B.StatusAdendo = 1 OR B.StatusAdendo = 2)", parametros).Any();
            }
        }

        public IEnumerable<ClientePropostaDTO> ObterAdendoSubClientesInclusao(int adendoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "AdendoId", value: adendoId, direction: ParameterDirection.Input);

                return con.Query<ClientePropostaDTO>(@"SELECT A.Id, A.AdendoId, A.SubClienteId, B.Descricao, A.Segmento, B.Documento FROM CRM.TB_CRM_ADENDO_SUB_CLIENTE A INNER JOIN CRM.TB_CRM_CONTAS B ON A.SubClienteId = B.Id WHERE A.AdendoId = :AdendoId AND A.ACAO = 1", parametros);
            }
        }

        public IEnumerable<ClientePropostaDTO> ObterAdendoSubClientesExclusao(int adendoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "AdendoId", value: adendoId, direction: ParameterDirection.Input);

                return con.Query<ClientePropostaDTO>(@"
                    SELECT 
                        DISTINCT
                            A.Id, 
                            A.AdendoId, 
                            A.SubClienteId, 
                            C.Descricao, 
                            D.Segmento, 
                            C.Documento 
                    FROM 
                        CRM.TB_CRM_ADENDO_SUB_CLIENTE A 
                    INNER JOIN
                        CRM.TB_CRM_OPORTUNIDADE_ADENDOS B ON A.AdendoId = B.Id
                    INNER JOIN 
                        CRM.TB_CRM_CONTAS C ON A.SubClienteId = C.Id 
                    LEFT JOIN 
                        CRM.TB_CRM_OPORTUNIDADE_CLIENTES D ON D.ContaId = A.SubClienteId 
                        AND  d.oportunidadeid=b.oportunidadeid                    
                        WHERE 
                        A.AdendoId = :AdendoId
                    AND 
                        A.ACAO = 2", parametros);
            }
        }

        public IEnumerable<OportunidadeAdendoClientesDTO> ObterAdendosPorSubClientes(string descricao, int? usuarioId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var criterio = "%" + descricao.ToUpper() + "%";

                var parametros = new DynamicParameters();
                parametros.Add(name: "Criterio", value: criterio, direction: ParameterDirection.Input);

                var filtroSQL = string.Empty;

                if (usuarioId.HasValue)
                {
                    parametros.Add(name: "UsuarioId", value: usuarioId, direction: ParameterDirection.Input);
                    filtroSQL = " AND (E.Id IN (SELECT ContaId FROM TB_CRM_USUARIOS_CONTAS WHERE UsuarioId = :UsuarioId) OR F.Id IN (SELECT ContaId FROM TB_CRM_USUARIOS_CONTAS WHERE UsuarioId = :UsuarioId)) ";
                }

                return con.Query<OportunidadeAdendoClientesDTO>($@"
                    SELECT 
                        A.Id,
                        B.Identificacao As OportunidadeIdentificacao,
                        A.OportunidadeId,
                        A.TipoAdendo,
                        A.StatusAdendo,
                        DECODE(E.Id, NULL, F.Descricao, E.Descricao) As DescricaoCliente,
                        DECODE(E.Id, NULL, F.Documento, E.Documento) As Documento,
                        A.DataCadastro,
                        B.Descricao As OportunidadeDescricao,
                        B.StatusOportunidade
                    FROM
                        CRM.TB_CRM_OPORTUNIDADE_ADENDOS A
                    LEFT JOIN
                        CRM.TB_CRM_OPORTUNIDADES B ON A.OportunidadeId = B.Id
                    LEFT JOIN
                        CRM.TB_CRM_ADENDO_SUB_CLIENTE C ON C.AdendoId = A.Id
                    LEFT JOIN
                        CRM.TB_CRM_ADENDO_GRUPO_CNPJ D ON D.AdendoId = A.Id    
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS E ON C.SubClienteId = E.Id
                    LEFT JOIN
                        CRM.TB_CRM_CONTAS F ON D.GrupoCnpjId = F.Id    
                    WHERE
                        ((E.Descricao LIKE :Criterio OR E.Documento LIKE :Criterio) OR (F.Descricao LIKE :Criterio OR F.Documento LIKE :Criterio)) {filtroSQL}
                    AND 
                        ROWNUM < 300", parametros);
            }
        }

        public void AtualizarStatusAdendo(StatusAdendo status, int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "StatusAdendo", value: status, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADE_ADENDOS SET StatusAdendo = :StatusAdendo WHERE Id = :Id", parametros);
            }
        }

        public DetalhesAdendoDTO ObterDetalhesAdendo(int adendoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "AdendoId", value: adendoId, direction: ParameterDirection.Input);

                return con.Query<DetalhesAdendoDTO>(@"
                    SELECT
                        A.Id,
                        '' As OportunidadeDescricao,
                        DECODE(A.TipoAdendo, 1, 'Alteração Vendedor', 2, 'Forma Pagamento', 3, 'Inclusão de Sub Cliente', 4, 'Exclusão de Sub Cliente', 5, 'Inclusão de Grupo CNPJ', 6, 'Exclusão de Grupo CNPJ') TipoAdendo,
                        DECODE(A.StatusAdendo, 1, 'Aberto', 2, 'Enviado', 3, 'Rejeitado', 4, 'Aprovado') StatusAdendo,
                        C.Nome As CriadoPor,
                        D.VendedorId,
                        E.Nome As Vendedor,
                        DECODE(F.FormaPagamento, 1, 'À Vista', 2, 'Faturado') As FormaPagamento,
                        rtrim(
                            xmlagg(xmlelement(
                                e,
                                (
                                     SubCliI.Descricao
                                )
                                 || '#'
                            ) ).extract('//text()'),
                            '#'
                        ) SubClientesInclusao,
                        rtrim(
                            xmlagg(xmlelement(
                                e,
                                (
                                     SubCliE.Descricao
                                )
                                 || '#'
                            ) ).extract('//text()'),
                            '#'
                        ) SubClientesExclusao,
                        rtrim(
                            xmlagg(xmlelement(
                                e,
                                (
                                     GrupoCnpjI.Descricao
                                )
                                 || '#'
                            ) ).extract('//text()'),
                            '#'
                        ) GruposCnpjInclusao,
                        rtrim(
                            xmlagg(xmlelement(
                                e,
                                (
                                     GrupoCnpjE.Descricao
                                )
                                 || '#'
                            ) ).extract('//text()'),
                            '#'
                        ) GruposCnpjExclusao      
                    FROM
                        TB_CRM_OPORTUNIDADE_ADENDOS A
                    LEFT JOIN
                        TB_CRM_USUARIOS C ON A.CriadoPor = C.Id
                    LEFT JOIN
                        (
                            SELECT
                                A.AdendoId,
                                B.Descricao
                            FROM
                                TB_CRM_ADENDO_SUB_CLIENTE A
                            INNER JOIN
                                TB_CRM_CONTAS B ON A.SubClienteId = B.Id
                            WHERE
                                A.Acao = 1
                        ) SubCliI ON SubCliI.AdendoId = A.Id
                    LEFT JOIN
                        (
                            SELECT
                                A.AdendoId,
                                B.Descricao
                            FROM
                                TB_CRM_ADENDO_SUB_CLIENTE A
                            INNER JOIN
                                TB_CRM_CONTAS B ON A.SubClienteId = B.Id
                            WHERE
                                A.Acao = 2
                        ) SubCliE ON SubCliE.AdendoId = A.Id    
                    LEFT JOIN
                        (
                            SELECT
                                A.AdendoId,
                                B.Descricao
                            FROM
                                TB_CRM_ADENDO_GRUPO_CNPJ A
                            INNER JOIN
                                TB_CRM_CONTAS B ON A.GrupoCnpjId = B.Id
                            WHERE
                                A.Acao = 1
                        ) GrupoCnpjI ON GrupoCnpjI.AdendoId = A.Id
                    LEFT JOIN
                        (
                            SELECT
                                A.AdendoId,
                                B.Descricao
                            FROM
                                TB_CRM_ADENDO_GRUPO_CNPJ A
                            INNER JOIN
                                TB_CRM_CONTAS B ON A.GrupoCnpjId = B.Id
                            WHERE
                                A.Acao = 2
                        ) GrupoCnpjE ON GrupoCnpjE.AdendoId = A.Id      
                    LEFT  JOIN
                        TB_CRM_ADENDO_VENDEDOR D ON A.Id = D.AdendoId
                    LEFT  JOIN
                        TB_CRM_USUARIOS E ON D.VendedorId = E.Id
                    LEFT  JOIN
                        TB_CRM_ADENDO_FORMA_PAGAMENTO F ON A.Id = F.AdendoId
                    WHERE
                        A.Id = :AdendoId
                    GROUP BY
                        A.Id,
                        A.TipoAdendo,
                        A.StatusAdendo,
                        C.Nome,
                        D.VendedorId,
                        E.Nome,
                        F.FormaPagamento", parametros).FirstOrDefault();
            }
        }

        public string ObterValorPorCampo(string campo, int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                if (campo.ToUpper() == "DATAFECHAMENTO")
                {
                    return con.Query<string>($@"SELECT to_char({campo},'DD/MM/YYYY') FROM CRM.VW_CRM_OPORTUNIDADES WHERE Id = :id", new { id }).FirstOrDefault();
                }
                if (campo.ToUpper() == "DATAINICIO")
                {
                    return con.Query<string>($@"SELECT to_char({campo},'DD/MM/YYYY') FROM CRM.VW_CRM_OPORTUNIDADES WHERE Id = :id", new { id }).FirstOrDefault();
                }

                if (campo.ToUpper() == "DATATERMINO")
                {
                    return con.Query<string>($@"SELECT to_char({campo},'DD/MM/YYYY') FROM CRM.VW_CRM_OPORTUNIDADES WHERE Id = :id", new { id }).FirstOrDefault();
                }
                return con.Query<string>($@"SELECT {campo} FROM CRM.VW_CRM_OPORTUNIDADES WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }

        public IEnumerable<UsuarioDTO> ObterUsuariosOportunidade()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<UsuarioDTO>(@"
                    SELECT
                        DISTINCT
                            B.Id,
                            B.Login                       
                    FROM
                        CRM.TB_CRM_OPORTUNIDADES A
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id                    
                    ORDER BY
                        B.Login");
            }
        }

        public void IncluirNota(OportunidadeNotas nota)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Nota", value: nota.Nota, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: nota.OportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: nota.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "CriadoPor", value: nota.CriadoPor, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_OPORTUNIDADE_NOTAS (Id, OportunidadeId, Nota, Descricao, CriadoPor) VALUES (CRM.SEQ_CRM_OPORTUNIDADE_NOTAS.NEXTVAL, :OportunidadeId, :Nota, :Descricao, :CriadoPor)", parametros);
            }
        }

        public void AtualizarNota(OportunidadeNotas nota)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Nota", value: nota.Nota, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: nota.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: nota.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADE_NOTAS SET Nota = :Nota, Descricao = :Descricao WHERE Id = :Id", parametros);
            }
        }

        public void ExcluirNota(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_NOTAS WHERE Id = :Id", parametros);
            }
        }

        public OportunidadeNotas ObterNotaPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<OportunidadeNotas>(@"SELECT * FROM CRM.TB_CRM_OPORTUNIDADE_NOTAS WHERE Id = :Id", parametros).FirstOrDefault();
            }
        }

        public IEnumerable<AnexosNotasDTO> ObterNotaPorDescricao(string descricao, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: "%" + descricao.ToUpper() + "%", direction: ParameterDirection.Input);

                return con.Query<AnexosNotasDTO>(@"
                    SELECT
                        A.Id,
                        A.OportunidadeId,
                        B.Login As CriadoPor,
                        A.Nota,
                        A.Descricao,
                        A.DataCriacao
                    FROM
                        CRM.TB_CRM_OPORTUNIDADE_NOTAS A
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id
                    WHERE
                        (UPPER(A.Nota) LIKE :Descricao OR UPPER(A.Descricao) LIKE :Descricao)
                    AND 
                        A.OportunidadeId = :OportunidadeId
                    ORDER BY
                        A.DataCriacao DESC", parametros);
            }
        }

        public IEnumerable<SimuladorDTO> ObterParametrosSimulador()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<SimuladorDTO>(@"SELECT AUTONUM As Id, Descricao FROM SGIPA.TB_SIMULADOR_CALCULO WHERE ROWNUM < 15 ORDER BY Descricao");
            }
        }

        public bool PermiteAlterarDataCancelamento(int oportunidadeId)
        {            
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<bool>(@"SELECT PermiteAlterarDataCancelamento FROM TB_CRM_OPORTUNIDADES WHERE Id = :Id", parametros).Any();
            }
        }
        
        public bool PermiteAlterarDataCancelamento(int oportunidadeId, bool permite)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Id", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Permite", value: permite.ToInt(), direction: ParameterDirection.Input);

                return con.Query<bool>(@"UPDATE TB_CRM_OPORTUNIDADES SET PermiteAlterarDataCancelamento = :Permite WHERE Id = :Id", parametros).Any();
            }
        }

        public void AtualizarCancelamentoOportunidade(bool cancelado, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Cancelado", value: cancelado.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: oportunidadeId, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET Cancelado = :Cancelado WHERE Id = :Id", parametros);
            }
        }

        public bool ExistemImpostosExcecao(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                var resultado = con.Query<int>(@"SELECT COUNT(1) FROM TB_CRM_OPORTUNIDADES_IMPOSTOS WHERE OportunidadeId = :OportunidadeId", parametros).Single();

                return resultado > 0;
            }
        }

        public bool TabelaCanceladaChronos(int tabelaId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                var resultado = con.Query<int>(@"SELECT NVL(MAX(CANCELADO_CRM), 0) Cancelado FROM SGIPA.TB_LISTAS_PRECOS WHERE AUTONUM = :TabelaId", parametros).Single();

                return resultado > 0;
            }
        }
    }
}