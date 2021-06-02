using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Helpers;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class LayoutRepositorio : ILayoutRepositorio, ILayoutPropostaRepositorio
    {
        private readonly bool _layoutProposta;
        private readonly string _tabela;
        private readonly string _view;
        private readonly string _criterioBusca;

        public LayoutRepositorio(bool layoutProposta)
        {
            _layoutProposta = layoutProposta;

            if (layoutProposta)
            {
                _tabela = "CRM.TB_CRM_OPORTUNIDADE_LAYOUT";
                _view = "CRM.VW_CRM_OPORTUNIDADE_LAYOUTS";
            }
            else
            {
                _tabela = "CRM.TB_CRM_LAYOUT";
                _view = "CRM.VW_CRM_LAYOUTS";
            }

            if (layoutProposta)
                _criterioBusca = "OportunidadeId";
            else
                _criterioBusca = "ModeloId";
        }

        public bool LayoutProposta() => _layoutProposta;

        public void CadastrarLayoutTitulo(LayoutTitulo layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, Linha, Descricao, Ocultar) VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :Linha, :Descricao, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutTitulo(LayoutTitulo layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE {_tabela} SET ModeloId = :ModeloId, TipoRegistro = :TipoRegistro, Linha = :Linha, Descricao = :Descricao, Ocultar = :Ocultar WHERE Id = :Id", parametros);
            }
        }

        public void CadastrarLayoutTituloMaster(LayoutTituloMaster layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, Linha, Descricao, Ocultar) VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :Linha, :Descricao, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutTituloMaster(LayoutTituloMaster layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE {_tabela} SET ModeloId = :ModeloId, TipoRegistro = :TipoRegistro, Linha = :Linha, Descricao = :Descricao, Ocultar = :Ocultar WHERE Id = :Id", parametros);
            }
        }

        public void CadastrarLayoutSubTitulo(LayoutSubTitulo layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, Linha, Descricao, Ocultar) VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :Linha, :Descricao, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutSubTitulo(LayoutSubTitulo layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE {_tabela} SET ModeloId = :ModeloId, TipoRegistro = :TipoRegistro, Linha = :Linha, Descricao = :Descricao, Ocultar = :Ocultar WHERE Id = :Id", parametros);
            }
        }

        public void CadastrarLayoutSubTituloMargem(LayoutSubTituloMargem layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, Linha, Descricao, Ocultar) VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :Linha, :Descricao, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutSubTituloMargem(LayoutSubTituloMargem layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE {_tabela} SET ModeloId = :ModeloId, TipoRegistro = :TipoRegistro, Linha = :Linha, Descricao = :Descricao, Ocultar = :Ocultar WHERE Id = :Id", parametros);
            }
        }

        public void CadastrarLayoutSubTituloAllIn(LayoutSubTituloAllIn layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, Linha, Descricao, Ocultar) VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :Linha, :Descricao, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutSubTituloAllIn(LayoutSubTituloAllIn layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE {_tabela} SET ModeloId = :ModeloId, TipoRegistro = :TipoRegistro, Linha = :Linha, Descricao = :Descricao, Ocultar = :Ocultar WHERE Id = :Id", parametros);
            }
        }

        public void CadastrarLayoutArmazenagem(LayoutArmazenagem layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: layout.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalArmazenagem", value: layout.AdicionalArmazenagem, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalGRC", value: layout.AdicionalGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "MinimoGRC", value: layout.MinimoGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMOGRC", value: layout.AdicionalIMOGRC, direction: ParameterDirection.Input);                
                parametros.Add(name: "ValorANVISA", value: layout.ValorANVISA, direction: ParameterDirection.Input);
                parametros.Add(name: "AnvisaGRC", value: layout.AnvisaGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: layout.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: layout.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseExcesso", value: layout.BaseExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: layout.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorExcesso", value: layout.ValorExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalPeso", value: layout.AdicionalPeso, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoLimite", value: layout.PesoLimite, direction: ParameterDirection.Input);
                parametros.Add(name: "ProRata", value: layout.ProRata.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "GrupoAtracacaoId", value: layout.GrupoAtracacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, TipoCarga, Linha, Descricao, ServicoId, BaseCalculo, QtdeDias, Valor, Valor20, Valor40, AdicionalArmazenagem, AdicionalGRC, MinimoGRC, AdicionalIMO, Exercito, AdicionalIMOGRC, ValorANVISA, AnvisaGRC, Periodo, Moeda, DescricaoValor, TipoDocumentoId, BaseExcesso, Margem, ValorExcesso, AdicionalPeso, PesoLimite, GrupoAtracacaoId, ProRata, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :TipoCarga, :Linha, :Descricao, :ServicoId, :BaseCalculo, :QtdeDias, :Valor, :Valor20, :Valor40, :AdicionalArmazenagem, :AdicionalGRC, :MinimoGRC, :AdicionalIMO, :Exercito, :AdicionalIMOGRC, :ValorANVISA, :AnvisaGRC, :Periodo, :Moeda, :DescricaoValor, :TipoDocumentoId, :BaseExcesso, :Margem, :ValorExcesso, :AdicionalPeso, :PesoLimite, :GrupoAtracacaoId, :ProRata, :Ocultar)", parametros);
            }
        }

        public void CadastrarLayoutArmazenagemCIF(LayoutArmazenagemCIF layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: layout.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorCIF", value: layout.ValorCif, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalArmazenagem", value: layout.AdicionalArmazenagem, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalGRC", value: layout.AdicionalGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "MinimoGRC", value: layout.MinimoGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMOGRC", value: layout.AdicionalIMOGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorANVISA", value: layout.ValorANVISA, direction: ParameterDirection.Input);
                parametros.Add(name: "AnvisaGRC", value: layout.AnvisaGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: layout.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: layout.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseExcesso", value: layout.BaseExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: layout.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorExcesso", value: layout.ValorExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalPeso", value: layout.AdicionalPeso, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoLimite", value: layout.PesoLimite, direction: ParameterDirection.Input);
                parametros.Add(name: "ProRata", value: layout.ProRata.ToInt(), direction: ParameterDirection.Input);

                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, TipoCarga, Linha, Descricao, ServicoId, BaseCalculo, QtdeDias, ValorCIF, Valor, Valor20, Valor40, AdicionalArmazenagem, AdicionalGRC, MinimoGRC, AdicionalIMO, AdicionalIMOGRC, ValorANVISA, AnvisaGRC, Periodo, Moeda, DescricaoValor, TipoDocumentoId, BaseExcesso, Margem, ValorExcesso, AdicionalPeso, PesoLimite, ProRata, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :TipoCarga, :Linha, :Descricao, :ServicoId, :BaseCalculo, :QtdeDias, :ValorCIF, :Valor, :Valor20, :Valor40, :AdicionalArmazenagem, :AdicionalGRC, :MinimoGRC, :AdicionalIMO, :AdicionalIMOGRC, :ValorANVISA, :AnvisaGRC, :Periodo, :Moeda, :DescricaoValor, :TipoDocumentoId, :BaseExcesso, :Margem, :ValorExcesso, :AdicionalPeso, :PesoLimite, :ProRata, :Ocultar)", parametros);
            }
        }

        public void CadastrarLayoutPeriodoPadrao(LayoutPeriodoPadrao layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: layout.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: layout.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);

                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, TipoCarga, Linha, Descricao, ServicoId, BaseCalculo, QtdeDias, Valor, Valor20, Valor40, Periodo, DescricaoValor, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :TipoCarga, :Linha, :Descricao, :ServicoId, :BaseCalculo, :QtdeDias, :Valor, :Valor20, :Valor40, :Periodo, :DescricaoValor, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutPeriodoPadrao(LayoutPeriodoPadrao layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: layout.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: layout.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET 
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    TipoCarga = :TipoCarga, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ServicoId = :ServicoId, 
                                    BaseCalculo = :BaseCalculo, 
                                    QtdeDias = :QtdeDias, 
                                    Valor = :Valor, 
                                    Valor20 = :Valor20, 
                                    Valor40 = :Valor40,                                     
                                    Periodo = :Periodo,                                     
                                    DescricaoValor = :DescricaoValor,                                    
                                    Ocultar = :Ocultar
                            WHERE Id = :Id", parametros);
            }
        }

        public void AtualizarLayoutArmazenagem(LayoutArmazenagem layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: layout.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalArmazenagem", value: layout.AdicionalArmazenagem, direction: ParameterDirection.Input);                
                parametros.Add(name: "AdicionalGRC", value: layout.AdicionalGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "MinimoGRC", value: layout.MinimoGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMOGRC", value: layout.AdicionalIMOGRC, direction: ParameterDirection.Input);                
                parametros.Add(name: "ValorANVISA", value: layout.ValorANVISA, direction: ParameterDirection.Input);
                parametros.Add(name: "AnvisaGRC", value: layout.AnvisaGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: layout.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: layout.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseExcesso", value: layout.BaseExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: layout.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorExcesso", value: layout.ValorExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalPeso", value: layout.AdicionalPeso, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoLimite", value: layout.PesoLimite, direction: ParameterDirection.Input);
                parametros.Add(name: "GrupoAtracacaoId", value: layout.GrupoAtracacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ProRata", value: layout.ProRata.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET 
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    TipoCarga = :TipoCarga, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ServicoId = :ServicoId, 
                                    BaseCalculo = :BaseCalculo, 
                                    QtdeDias = :QtdeDias, 
                                    Valor = :Valor, 
                                    Valor20 = :Valor20, 
                                    Valor40 = :Valor40, 
                                    AdicionalArmazenagem = :AdicionalArmazenagem, 
                                    AdicionalGRC = :AdicionalGRC, 
                                    MinimoGRC = :MinimoGRC, 
                                    AdicionalIMO = :AdicionalIMO, 
                                    Exercito = :Exercito,
                                    AdicionalIMOGRC = :AdicionalIMOGRC,
                                    ValorANVISA = :ValorANVISA, 
                                    AnvisaGRC = :AnvisaGRC,
                                    Periodo = :Periodo, 
                                    Moeda = :Moeda, 
                                    DescricaoValor = :DescricaoValor,
                                    TipoDocumentoId = :TipoDocumentoId, 
                                    BaseExcesso = :BaseExcesso, 
                                    Margem = :Margem,
                                    ValorExcesso = :ValorExcesso, 
                                    AdicionalPeso = :AdicionalPeso, 
                                    PesoLimite = :PesoLimite, 
                                    GrupoAtracacaoId = :GrupoAtracacaoId,
                                    ProRata = :ProRata,
                                    Ocultar = :Ocultar
                            WHERE Id = :Id", parametros);
            }
        }

        public void AtualizarLayoutArmazenagemCIF(LayoutArmazenagemCIF layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: layout.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorCIF", value: layout.ValorCif, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalArmazenagem", value: layout.AdicionalArmazenagem, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalGRC", value: layout.AdicionalGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "MinimoGRC", value: layout.MinimoGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMOGRC", value: layout.AdicionalIMOGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorANVISA", value: layout.ValorANVISA, direction: ParameterDirection.Input);
                parametros.Add(name: "AnvisaGRC", value: layout.AnvisaGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: layout.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: layout.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseExcesso", value: layout.BaseExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: layout.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorExcesso", value: layout.ValorExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalPeso", value: layout.AdicionalPeso, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoLimite", value: layout.PesoLimite, direction: ParameterDirection.Input);
                parametros.Add(name: "ProRata", value: layout.ProRata.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET 
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    TipoCarga = :TipoCarga, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ServicoId = :ServicoId, 
                                    BaseCalculo = :BaseCalculo, 
                                    QtdeDias = :QtdeDias, 
                                    ValorCIF = :ValorCIF,
                                    Valor = :Valor, 
                                    Valor20 = :Valor20, 
                                    Valor40 = :Valor40, 
                                    AdicionalArmazenagem = :AdicionalArmazenagem, 
                                    AdicionalGRC = :AdicionalGRC, 
                                    MinimoGRC = :MinimoGRC, 
                                    AdicionalIMO = :AdicionalIMO, 
                                    AdicionalIMOGRC = :AdicionalIMOGRC,
                                    ValorANVISA = :ValorANVISA, 
                                    AnvisaGRC = :AnvisaGRC,
                                    Periodo = :Periodo, 
                                    Moeda = :Moeda, 
                                    DescricaoValor = :DescricaoValor,
                                    TipoDocumentoId = :TipoDocumentoId, 
                                    BaseExcesso = :BaseExcesso, 
                                    Margem = :Margem,
                                    ValorExcesso = :ValorExcesso, 
                                    AdicionalPeso = :AdicionalPeso, 
                                    PesoLimite = :PesoLimite, 
                                    ProRata = :ProRata,
                                    Exercito = :Exercito,
                                    Ocultar = :Ocultar
                            WHERE Id = :Id", parametros);
            }
        }

        public int CadastrarLayoutArmazenagemMinimo(LayoutArmazenagemMinimo layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: layout.ValorCarga.ValorMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo20", value: layout.ValorCarga.ValorMinimo20, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo40", value: layout.ValorCarga.ValorMinimo40, direction: ParameterDirection.Input);
                parametros.Add(name: "LinhaReferencia", value: layout.LinhaReferencia, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "LimiteBls", value: layout.LimiteBls, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: layout.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, TipoCarga, Linha, Descricao, ServicoId, ValorMinimo, ValorMinimo20, ValorMinimo40, LinhaReferencia, DescricaoValor, LimiteBls, Margem, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :TipoCarga, :Linha, :Descricao, :ServicoId, :ValorMinimo, :ValorMinimo20, :ValorMinimo40, :LinhaReferencia, :DescricaoValor, :LimiteBls, :Margem, :Ocultar) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public int CadastrarLayoutArmazenagemMinimoCIF(LayoutArmazenagemMinimoCIF layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorCIF", value: layout.ValorCIF, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: layout.ValorCarga.ValorMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo20", value: layout.ValorCarga.ValorMinimo20, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo40", value: layout.ValorCarga.ValorMinimo40, direction: ParameterDirection.Input);
                parametros.Add(name: "LinhaReferencia", value: layout.LinhaReferencia, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "LimiteBls", value: layout.LimiteBls, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, TipoCarga, Linha, Descricao, ServicoId, ValorCIF, ValorMinimo, ValorMinimo20, ValorMinimo40, LinhaReferencia, DescricaoValor, LimiteBls, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :TipoCarga, :Linha, :Descricao, :ServicoId, :ValorCIF, :ValorMinimo, :ValorMinimo20, :ValorMinimo40, :LinhaReferencia, :DescricaoValor, :LimiteBls, :Ocultar) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void AtualizarLayoutArmazenagemMinimo(LayoutArmazenagemMinimo layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: layout.ValorCarga.ValorMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo20", value: layout.ValorCarga.ValorMinimo20, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo40", value: layout.ValorCarga.ValorMinimo40, direction: ParameterDirection.Input);
                parametros.Add(name: "LinhaReferencia", value: layout.LinhaReferencia, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "LimiteBls", value: layout.LimiteBls, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: layout.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    TipoCarga = :TipoCarga, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ServicoId = :ServicoId, 
                                    ValorMinimo = :ValorMinimo, 
                                    ValorMinimo20 = :ValorMinimo20, 
                                    ValorMinimo40 = :ValorMinimo40, 
                                    LinhaReferencia = :LinhaReferencia, 
                                    DescricaoValor = :DescricaoValor,
                                    LimiteBls = :LimiteBls,
                                    Ocultar = :Ocultar,
                                    Margem = :Margem
                                WHERE
                                    Id = :Id", parametros);
            }
        }

        public void AtualizarLayoutArmazenagemMinimoCIF(LayoutArmazenagemMinimoCIF layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorCIF", value: layout.ValorCIF, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: layout.ValorCarga.ValorMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo20", value: layout.ValorCarga.ValorMinimo20, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo40", value: layout.ValorCarga.ValorMinimo40, direction: ParameterDirection.Input);
                parametros.Add(name: "LinhaReferencia", value: layout.LinhaReferencia, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "LimiteBls", value: layout.LimiteBls, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    TipoCarga = :TipoCarga, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ServicoId = :ServicoId, 
                                    ValorCIF = :ValorCIF,
                                    ValorMinimo = :ValorMinimo, 
                                    ValorMinimo20 = :ValorMinimo20, 
                                    ValorMinimo40 = :ValorMinimo40, 
                                    LinhaReferencia = :LinhaReferencia, 
                                    DescricaoValor = :DescricaoValor,
                                    LimiteBls = :LimiteBls,
                                    Ocultar = :Ocultar
                                WHERE
                                    Id = :Id", parametros);
            }
        }

        public int CadastrarLayoutArmazenagemAllIn(LayoutArmazenagemAllIn layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: layout.ValorMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: layout.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: layout.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoPeriodo", value: layout.DescricaoPeriodo, direction: ParameterDirection.Input);
                parametros.Add(name: "CifMinimo", value: layout.CifMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "CifMaximo", value: layout.CifMaximo, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoCif", value: layout.DescricaoCif, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: layout.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseExcesso", value: layout.BaseExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorExcesso", value: layout.ValorExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalPeso", value: layout.AdicionalPeso, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoLimite", value: layout.PesoLimite, direction: ParameterDirection.Input);
                parametros.Add(name: "ProRata", value: layout.ProRata.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, Linha, Descricao, ServicoId, BaseCalculo, Moeda, ValorMinimo, Valor20, Valor40, Margem, Periodo, DescricaoPeriodo, CifMinimo, CifMaximo, DescricaoCif, DescricaoValor, TipoDocumentoId, BaseExcesso, ValorExcesso, AdicionalPeso, PesoLimite, ProRata, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :Linha, :Descricao, :ServicoId, :BaseCalculo, :Moeda, :ValorMinimo, :Valor20, :Valor40, :Margem, :Periodo, :DescricaoPeriodo, :CifMinimo, :CifMaximo, :DescricaoCif, :DescricaoValor, :TipoDocumentoId, :BaseExcesso, :ValorExcesso, :AdicionalPeso, :PesoLimite, :ProRata, :Ocultar) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void AtualizarLayoutArmazenagemAllIn(LayoutArmazenagemAllIn layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: layout.ValorMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: layout.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: layout.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoPeriodo", value: layout.DescricaoPeriodo, direction: ParameterDirection.Input);
                parametros.Add(name: "CifMinimo", value: layout.CifMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "CifMaximo", value: layout.CifMaximo, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoCif", value: layout.DescricaoCif, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: layout.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseExcesso", value: layout.BaseExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorExcesso", value: layout.ValorExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalPeso", value: layout.AdicionalPeso, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoLimite", value: layout.PesoLimite, direction: ParameterDirection.Input);
                parametros.Add(name: "ProRata", value: layout.ProRata.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ServicoId = :ServicoId, 
                                    BaseCalculo = :BaseCalculo, 
                                    Moeda = :Moeda, 
                                    ValorMinimo = :ValorMinimo, 
                                    Valor20 = :Valor20, 
                                    Valor40 = :Valor40, 
                                    Margem = :Margem, 
                                    Periodo = :Periodo,
                                    DescricaoPeriodo = :DescricaoPeriodo,
                                    CifMinimo = :CifMinimo,
                                    CifMaximo = :CifMaximo,
                                    DescricaoCif = :DescricaoCif,
                                    DescricaoValor = :DescricaoValor,
                                    TipoDocumentoId = :TipoDocumentoId, 
                                    BaseExcesso = :BaseExcesso, 
                                    ValorExcesso = :ValorExcesso, 
                                    AdicionalPeso = :AdicionalPeso, 
                                    PesoLimite = :PesoLimite, 
                                    ProRata = :ProRata,
                                    Ocultar = :Ocultar
                                WHERE Id = :Id", parametros);
            }
        }

        public int CadastrarLayoutServicoParaMargem(LayoutServicoParaMargem layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMargemDireita", value: layout.ValorMargemDireita, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMargemEsquerda", value: layout.ValorMargemEsquerda, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorEntreMargens", value: layout.ValorEntreMargens, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoMaximo", value: layout.PesoMaximo, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalPeso", value: layout.AdicionalPeso, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: layout.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseExcesso", value: layout.BaseExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorExcesso", value: layout.ValorExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoLimite", value: layout.PesoLimite, direction: ParameterDirection.Input);
                parametros.Add(name: "ProRata", value: layout.ProRata.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, Linha, Descricao, ServicoId, BaseCalculo, TipoCarga, ValorMargemDireita, ValorMargemEsquerda, ValorEntreMargens, AdicionalIMO, Exercito, Moeda, PesoMaximo, AdicionalPeso, DescricaoValor, TipoDocumentoId, BaseExcesso, ValorExcesso, PesoLimite, ProRata, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :Linha, :Descricao, :ServicoId, :BaseCalculo, :TipoCarga, :ValorMargemDireita, :ValorMargemEsquerda, :ValorEntreMargens, :AdicionalIMO, :Exercito, :Moeda, :PesoMaximo, :AdicionalPeso, :DescricaoValor, :TipoDocumentoId, :BaseExcesso, :ValorExcesso, :PesoLimite, :ProRata, :Ocultar) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void AtualizarLayoutServicoParaMargem(LayoutServicoParaMargem layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMargemDireita", value: layout.ValorMargemDireita, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMargemEsquerda", value: layout.ValorMargemEsquerda, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorEntreMargens", value: layout.ValorEntreMargens, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoMaximo", value: layout.PesoMaximo, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalPeso", value: layout.AdicionalPeso, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: layout.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseExcesso", value: layout.BaseExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorExcesso", value: layout.ValorExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalPeso", value: layout.AdicionalPeso, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoLimite", value: layout.PesoLimite, direction: ParameterDirection.Input);
                parametros.Add(name: "ProRata", value: layout.ProRata.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ServicoId = :ServicoId, 
                                    BaseCalculo = :BaseCalculo, 
                                    TipoCarga = :TipoCarga, 
                                    ValorMargemDireita = :ValorMargemDireita, 
                                    ValorMargemEsquerda = :ValorMargemEsquerda, 
                                    ValorEntreMargens = :ValorEntreMargens, 
                                    AdicionalIMO = :AdicionalIMO, 
                                    Exercito = :Exercito,
                                    Moeda = :Moeda, 
                                    PesoMaximo = :PesoMaximo, 
                                    DescricaoValor = :DescricaoValor,
                                    TipoDocumentoId = :TipoDocumentoId, 
                                    BaseExcesso = :BaseExcesso, 
                                    ValorExcesso = :ValorExcesso, 
                                    AdicionalPeso = :AdicionalPeso, 
                                    PesoLimite = :PesoLimite, 
                                    ProRata = :ProRata,
                                    Ocultar = :Ocultar
                                WHERE 
                                    Id = :Id", parametros);

            }
        }

        public void CadastrarLayoutMinimoParaMargem(LayoutMinimoParaMargem layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimoMargemDireita", value: layout.ValorMinimoMargemDireita, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimoMargemEsquerda", value: layout.ValorMinimoMargemEsquerda, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimoEntreMargens", value: layout.ValorMinimoEntreMargens, direction: ParameterDirection.Input);
                parametros.Add(name: "LinhaReferencia", value: layout.LinhaReferencia, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.LinhaReferencia, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, Linha, Descricao, ServicoId, ValorMinimoMargemDireita, ValorMinimoMargemEsquerda, ValorMinimoEntreMargens, LinhaReferencia, DescricaoValor, Ocultar) 
                            VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :Linha, :Descricao, :ServicoId, :ValorMinimoMargemDireita, :ValorMinimoMargemEsquerda, :ValorMinimoEntreMargens, :LinhaReferencia, :DescricaoValor, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutMinimoParaMargem(LayoutMinimoParaMargem layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimoMargemDireita", value: layout.ValorMinimoMargemDireita, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimoMargemEsquerda", value: layout.ValorMinimoMargemEsquerda, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimoEntreMargens", value: layout.ValorMinimoEntreMargens, direction: ParameterDirection.Input);
                parametros.Add(name: "LinhaReferencia", value: layout.LinhaReferencia, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ServicoId = :ServicoId, 
                                    ValorMinimoMargemDireita = :ValorMinimoMargemDireita, 
                                    ValorMinimoMargemEsquerda = :ValorMinimoMargemEsquerda, 
                                    ValorMinimoEntreMargens = :ValorMinimoEntreMargens, 
                                    LinhaReferencia = :LinhaReferencia, 
                                    DescricaoValor = :DescricaoValor,
                                    Ocultar = :Ocultar
                                WHERE Id = :Id", parametros);
            }
        }

        public void CadastrarLayoutServicoMecanicaManual(LayoutServicoMecanicaManual layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoMaximo", value: layout.PesoMaximo, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalPeso", value: layout.AdicionalPeso, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoTrabalho", value: layout.TipoTrabalho, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, Linha, Descricao, ServicoId, BaseCalculo, TipoCarga, Valor, Valor20, Valor40, AdicionalIMO, Exercito, Moeda, PesoMaximo, AdicionalPeso, TipoTrabalho, DescricaoValor, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :Linha, :Descricao, :ServicoId, :BaseCalculo, :TipoCarga, :Valor, :Valor20, :Valor40, :AdicionalIMO, :Exercito, :Moeda, :PesoMaximo, :AdicionalPeso, :TipoTrabalho, :DescricaoValor, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutServicoMecanicaManual(LayoutServicoMecanicaManual layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoMaximo", value: layout.PesoMaximo, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalPeso", value: layout.AdicionalPeso, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoTrabalho", value: layout.TipoTrabalho, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET 
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ServicoId = :ServicoId, 
                                    BaseCalculo = :BaseCalculo, 
                                    TipoCarga = :TipoCarga,
                                    Valor = :Valor,
                                    Valor20 = :Valor20, 
                                    Valor40 = :Valor40, 
                                    AdicionalIMO = :AdicionalIMO, 
                                    Exercito = :Exercito,
                                    Moeda = :Moeda, 
                                    PesoMaximo = :AdicionalPeso, 
                                    AdicionalPeso = :AdicionalPeso, 
                                    TipoTrabalho = :TipoTrabalho, 
                                    DescricaoValor = :DescricaoValor,
                                    Ocultar = :Ocultar
                                WHERE Id = :Id", parametros);
            }
        }

        public void CadastrarLayoutMinimoMecanicaManual(LayoutMinimoMecanicaManual layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo20", value: layout.ValorCarga.ValorMinimo20, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo40", value: layout.ValorCarga.ValorMinimo40, direction: ParameterDirection.Input);
                parametros.Add(name: "LinhaReferencia", value: layout.LinhaReferencia, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, Linha, Descricao, ValorMinimo20, ValorMinimo40, LinhaReferencia, DescricaoValor, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :Linha, :Descricao, :ValorMinimo20, :ValorMinimo40, :LinhaReferencia, :DescricaoValor, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutMinimoMecanicaManual(LayoutMinimoMecanicaManual layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo20", value: layout.ValorCarga.ValorMinimo20, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo40", value: layout.ValorCarga.ValorMinimo40, direction: ParameterDirection.Input);
                parametros.Add(name: "LinhaReferencia", value: layout.LinhaReferencia, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET 
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ValorMinimo20 = :ValorMinimo20, 
                                    ValorMinimo40 = :ValorMinimo40, 
                                    LinhaReferencia = :LinhaReferencia, 
                                    DescricaoValor = :DescricaoValor,
                                    Ocultar = :Ocultar
                                WHERE Id = :Id", parametros);
            }
        }

        public void CadastrarLayoutServicoLiberacao(LayoutServicoLiberacao layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: layout.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "Reembolso", value: layout.Reembolso, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: layout.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "GrupoAtracacaoId", value: layout.GrupoAtracacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, TipoCarga, Linha, Descricao, ServicoId, BaseCalculo, Margem, Reembolso, Valor20, Valor40, Valor, Moeda, DescricaoValor, TipoDocumentoId, GrupoAtracacaoId, AdicionalIMO, Exercito, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :TipoCarga, :Linha, :Descricao, :ServicoId, :BaseCalculo, :Margem, :Reembolso, :Valor20, :Valor40, :Valor, :Moeda, :DescricaoValor, :TipoDocumentoId, :GrupoAtracacaoId, :AdicionalIMO, :Exercito, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutServicoLiberacao(LayoutServicoLiberacao layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: layout.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "Reembolso", value: layout.Reembolso, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: layout.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "GrupoAtracacaoId", value: layout.GrupoAtracacaoId, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET 
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    TipoCarga = :TipoCarga, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ServicoId = :ServicoId, 
                                    BaseCalculo = :BaseCalculo, 
                                    Margem = :Margem,
                                    Reembolso = :Reembolso, 
                                    Valor20 = :Valor20, 
                                    Valor40 = :Valor40, 
                                    Valor = :Valor, 
                                    Moeda = :Moeda, 
                                    DescricaoValor = :DescricaoValor,
                                    TipoDocumentoId = :TipoDocumentoId,
                                    GrupoAtracacaoId = :GrupoAtracacaoId,
                                    AdicionalIMO = :AdicionalIMO,
                                    Exercito = :Exercito,
                                    Ocultar = :Ocultar
                                WHERE Id = :Id", parametros);
            }
        }

        public void CadastrarLayoutServicoHubPort(LayoutServicoHubPort layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Origem", value: layout.Origem, direction: ParameterDirection.Input);
                parametros.Add(name: "Destino", value: layout.Destino, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamentoNVOCC", value: layout.FormaPagamentoNVOCC, direction: ParameterDirection.Input);                
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, Linha, Descricao, ServicoId, BaseCalculo, Valor, Origem, Destino, Moeda, FormaPagamentoNVOCC, DescricaoValor, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :Linha, :Descricao, :ServicoId, :BaseCalculo, :Valor, :Origem, :Destino, :Moeda, :FormaPagamentoNVOCC, :DescricaoValor, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutServicoHubPort(LayoutServicoHubPort layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Origem", value: layout.Origem, direction: ParameterDirection.Input);
                parametros.Add(name: "Destino", value: layout.Destino, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamentoNVOCC", value: layout.FormaPagamentoNVOCC, direction: ParameterDirection.Input);                
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET 
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ServicoId = :ServicoId, 
                                    BaseCalculo = :BaseCalculo, 
                                    Valor = :Valor, 
                                    Origem = :Origem, 
                                    Destino = :Destino, 
                                    Moeda = :Moeda, 
                                    FormaPagamentoNVOCC = :FormaPagamentoNVOCC,
                                    DescricaoValor = :DescricaoValor,
                                    Ocultar = :Ocultar
                                WHERE Id = :Id", parametros);
            }
        }

        public void CadastrarLayoutServicosGerais(LayoutServicosGerais layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: layout.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseExcesso", value: layout.BaseExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorExcesso", value: layout.ValorExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamentoNVOCC", value: layout.FormaPagamentoNVOCC, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, TipoCarga, Linha, Descricao, ServicoId, BaseCalculo, Valor, Valor20, Valor40, Moeda, AdicionalIMO, Exercito, DescricaoValor, TipoDocumentoId, BaseExcesso, ValorExcesso, FormaPagamentoNVOCC, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :TipoCarga, :Linha, :Descricao, :ServicoId, :BaseCalculo, :Valor, :Valor20, :Valor40, :Moeda, :AdicionalIMO, :Exercito, :DescricaoValor, :TipoDocumentoId, :BaseExcesso, :ValorExcesso, :FormaPagamentoNVOCC, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutServicosGerais(LayoutServicosGerais layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ServicoId", value: layout.ServicoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocumentoId", value: layout.TipoDocumentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseExcesso", value: layout.BaseExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorExcesso", value: layout.ValorExcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamentoNVOCC", value: layout.FormaPagamentoNVOCC, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET 
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    TipoCarga = :TipoCarga, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ServicoId = :ServicoId,
                                    BaseCalculo = :BaseCalculo, 
                                    Valor = :Valor, 
                                    Valor20 = :Valor20, 
                                    Valor40 = :Valor40, 
                                    AdicionalIMO = :AdicionalIMO,
                                    Moeda = :Moeda, 
                                    DescricaoValor = :DescricaoValor,
                                    TipoDocumentoId = :TipoDocumentoId, 
                                    BaseExcesso = :BaseExcesso, 
                                    ValorExcesso = :ValorExcesso, 
                                    FormaPagamentoNVOCC = :FormaPagamentoNVOCC,
                                    Ocultar = :Ocultar
                                WHERE Id = :Id", parametros);
            }
        }

        public void CadastrarLayoutMinimoGeral(LayoutMinimoGeral layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: layout.ValorCarga.ValorMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo20", value: layout.ValorCarga.ValorMinimo20, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo40", value: layout.ValorCarga.ValorMinimo40, direction: ParameterDirection.Input);
                parametros.Add(name: "LinhaReferencia", value: layout.LinhaReferencia, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, TipoCarga, Linha, Descricao, ValorMinimo, ValorMinimo20, ValorMinimo40, LinhaReferencia, DescricaoValor, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :TipoCarga, :Linha, :Descricao, :ValorMinimo, :ValorMinimo20, :ValorMinimo40, :LinhaReferencia, :DescricaoValor, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutMinimoGeral(LayoutMinimoGeral layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: layout.ValorCarga.ValorMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo20", value: layout.ValorCarga.ValorMinimo20, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo40", value: layout.ValorCarga.ValorMinimo40, direction: ParameterDirection.Input);
                parametros.Add(name: "LinhaReferencia", value: layout.LinhaReferencia, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET 
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    TipoCarga = :TipoCarga, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    ValorMinimo = :ValorMinimo, 
                                    ValorMinimo20 = :ValorMinimo20, 
                                    ValorMinimo40 = :ValorMinimo40, 
                                    LinhaReferencia = :LinhaReferencia, 
                                    DescricaoValor = :DescricaoValor,
                                    Ocultar = :Ocultar
                                WHERE Id = :Id", parametros);
            }
        }

        public void CadastrarLayoutCondicoesIniciais(LayoutCondicoesIniciais layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicoesIniciais", value: layout.CondicoesIniciais, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, Linha, Descricao, CondicoesIniciais, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :Linha, :Descricao, :CondicoesIniciais, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutCondicoesIniciais(LayoutCondicoesIniciais layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicoesIniciais", value: layout.CondicoesIniciais, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                {_tabela} SET 
                                    ModeloId = :ModeloId, 
                                    TipoRegistro = :TipoRegistro, 
                                    Linha = :Linha, 
                                    Descricao = :Descricao, 
                                    CondicoesIniciais = :CondicoesIniciais,
                                    Ocultar = Ocultar
                                WHERE Id = :Id", parametros);
            }
        }

        public void CadastrarLayoutCondicoesGerais(LayoutCondicoesGerais layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicoesGerais", value: layout.CondicoesGerais, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT (Id, ModeloId, TipoRegistro, Linha, Descricao, CondicoesGerais, Ocultar) 
                                VALUES (CRM.SEQ_CRM_LAYOUT.NEXTVAL, :ModeloId, :TipoRegistro, :Linha, :Descricao, :CondicoesGerais, :Ocultar)", parametros);
            }
        }

        public void AtualizarLayoutCondicoesGerais(LayoutCondicoesGerais layout)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: layout.Cabecalho.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: layout.Cabecalho.TipoRegistro, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: layout.Cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicoesGerais", value: layout.CondicoesGerais, direction: ParameterDirection.Input);
                parametros.Add(name: "Ocultar", value: layout.Cabecalho.Ocultar.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: layout.Id, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE {_tabela} SET ModeloId = :ModeloId, TipoRegistro = :TipoRegistro, Linha = :Linha, 
                                Descricao = :Descricao, CondicoesGerais = :CondicoesGerais, Ocultar = :Ocultar WHERE Id = :Id", parametros);
            }
        }

        public bool LinhaJaCadastrada(Cabecalho cabecalho)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "ModeloId", value: cabecalho.ModeloId, direction: ParameterDirection.Input);

                return con.Query($"SELECT Linha FROM CRM.TB_CRM_LAYOUT WHERE Linha = :Linha AND ModeloId = :ModeloId", parametros).Any();
            }
        }

        public bool ExistemLinhasPosteriores(Cabecalho cabecalho)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "ModeloId", value: cabecalho.ModeloId, direction: ParameterDirection.Input);

                return con.Query("SELECT Linha FROM CRM.TB_CRM_LAYOUT WHERE Linha >= :Linha AND ModeloId = :ModeloId", parametros).Any();
            }
        }

        public void AtualizarLinhasPosteriores(Cabecalho cabecalho)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: cabecalho.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "ModeloId", value: cabecalho.ModeloId, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_LAYOUT SET Linha = Linha + 1, LinhaReferencia = LinhaReferencia + 1 WHERE Linha >= :Linha AND ModeloId = :ModeloId", parametros);
            }
        }

        public IEnumerable<LayoutDTO> ObterLayouts(int id, bool ocultar)
        {
            var parametros = new DynamicParameters();

            var filtroSQL = string.Empty;
            var campoOportunidade = string.Empty;

            parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

            if (ocultar)
                filtroSQL = " AND A.Ocultar = 0 ";
            else
                filtroSQL = " AND A.Ocultar >= 0 ";

            if (_layoutProposta)
                campoOportunidade = " A.OportunidadeId, ";
            else
                campoOportunidade = " 0 As OportunidadeId, ";

            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var result = con.Query<LayoutDTO, Servico, LayoutDTO>($@"
                    SELECT 
                        A.Id,
                        {campoOportunidade}
                        A.Linha, 
                        A.Descricao, 
                        A.ModeloId,
                        A.TipoRegistro, 
                        A.Valor,
                        A.Valor20, 
                        A.Valor40, 
                        A.DescricaoValor, 
                        A.ValorMinimo, 
                        A.ValorMinimo20, 
                        A.ValorMinimo40,
                        A.ValorMargemDireita,
                        A.ValorMargemEsquerda,
                        A.ValorMinimoMargemDireita,
                        A.ValorMinimoMargemEsquerda,
                        A.ValorEntreMargens,
                        A.ValorMinimoEntreMargens,
                        A.TipoCarga,
                        A.BaseCalculo,
                        A.CondicoesIniciais,
                        A.CondicoesGerais,
                        A.Moeda,
                        A.Periodo,
                        A.DescricaoPeriodo,
                        A.Margem,
                        A.CifMinimo,
                        A.CifMaximo,
                        A.DescricaoCif,
                        B.Id,
                        B.Descricao                        
                    FROM 
                        {_tabela} A
                    LEFT JOIN
                        CRM.TB_CRM_SERVICOS B ON A.ServicoId = B.Id
                    WHERE
                        {_criterioBusca} = :Id {filtroSQL}
                    ORDER BY Linha
                ", (l, s) =>
                {
                    if (s != null)
                        l.Servico = s;
                    else
                        l.Servico = new Servico();

                    return l;
                }, parametros, splitOn: "Id").ToList();

                return result;
            }
        }

        public LayoutDTO ObterLayoutPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<LayoutDTO>($@"SELECT * FROM {_tabela} WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }

        public IEnumerable<EdicaoValoresPropostaDTO> ObterLayoutEdicaoProposta(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<EdicaoValoresPropostaDTO>($@"SELECT * FROM {_tabela} WHERE OportunidadeId = :OportunidadeId ORDER BY Linha", parametros);
            }
        }

        public LayoutDTO ObterLayoutEdicaoPorLinha(int oportunidadeId, int linha)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);

                return con.Query<LayoutDTO>($@"SELECT * FROM {_tabela} WHERE OportunidadeId = :OportunidadeId AND Linha = :Linha", parametros).FirstOrDefault();
            }
        }

        public LayoutDTO ObterLayoutPorServico(int servicoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<LayoutDTO>(@"SELECT * FROM CRM.TB_CRM_LAYOUT WHERE ServicoId = :sId", new { sId = servicoId }).FirstOrDefault();
            }
        }

        public LayoutDTO ObterLayoutPorModelo(int modeloId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<LayoutDTO>(@"SELECT * FROM CRM.TB_CRM_LAYOUT WHERE ModeloId = :mId", new { mId = modeloId }).FirstOrDefault();
            }
        }

        public int ObterUltimaLinha(int modeloId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<int>(@"SELECT NVL(MAX(Linha),0) + 1 FROM CRM.TB_CRM_LAYOUT WHERE ModeloId = :mId", new { mId = modeloId }).FirstOrDefault();
            }
        }

        public LayoutDTO ObterLinhaAnterior(int linha)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);

                return con.Query<LayoutDTO>(@"SELECT * FROM CRM.TB_CRM_LAYOUT WHERE Linha < :Linha ORDER BY LINHA DESC", parametros).FirstOrDefault();
            }
        }

        public void ExcluirLinha(int linha, int modeloId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_LAYOUT WHERE Linha = :linha AND ModeloId = :modeloId", new { linha, modeloId });
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute($@"DELETE FROM {_tabela} WHERE {_criterioBusca} = :id", new { id });
            }
        }

        public void ExcluirLinhaOportunidadeProposta(int oportunidadeId, int linha)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);

                con.Execute($@"DELETE FROM TB_CRM_OPORTUNIDADE_LAYOUT WHERE OportunidadeId = :OportunidadeId AND Linha = :Linha", parametros);
            }
        }

        public string ObterValorPorLinha(int linha, int modeloId, string campo, int oportunidadeId = 0)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                if (_layoutProposta)
                    return con.Query<string>($@"SELECT {campo} FROM {_view} WHERE Linha = :lLinha AND OportunidadeId = :oId", new { lLinha = linha, oId = oportunidadeId }).FirstOrDefault();
                else
                    return con.Query<string>($@"SELECT {campo} FROM {_view} WHERE Linha = :lLinha AND ModeloId = :mId", new { lLinha = linha, mId = modeloId }).FirstOrDefault();
            }
        }

        public string ObterValorSemLinha(int modeloId, string campo, int oportunidadeId = 0)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                if (_layoutProposta)
                    return con.Query<string>($@"SELECT {campo} FROM {_view} WHERE OportunidadeId = :oId", new { oId = oportunidadeId }).FirstOrDefault();
                else
                    return con.Query<string>($@"SELECT {campo} FROM {_view} WHERE ModeloId = :mId", new { mId = modeloId }).FirstOrDefault();
            }
        }

        public IEnumerable<LayoutDTO> ObterProximasLinhas(int linha, int modeloId, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "ModeloId", value: modeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<LayoutDTO>($@"SELECT Id, Linha, OportunidadeId, TipoRegistro, QtdeDias, AdicionalArmazenagem, AdicionalGRC, MinimoGRC, AdicionalIMO, ValorANVISA  FROM CRM.TB_CRM_OPORTUNIDADE_LAYOUT WHERE Linha > :Linha AND ModeloId = :ModeloId AND OportunidadeId = :OportunidadeId ORDER BY Linha", parametros);
            }
        }

        public string ObterTipoRegistroPorLinha(int linha, int modeloId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<string>($@"SELECT TipoRegistro FROM CRM.TB_CRM_LAYOUT WHERE Linha = :lLinha AND ModeloId = :mId", new { lLinha = linha, mId = modeloId }).FirstOrDefault();
            }
        }

        public void ImportarLayout(int modeloNovo, int modeloAntigo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ModeloNovo", value: modeloNovo, direction: ParameterDirection.Input);
                parametros.Add(name: "ModeloAntigo", value: modeloAntigo, direction: ParameterDirection.Input);

                con.Execute(@"
                            INSERT INTO CRM.TB_CRM_LAYOUT (
                                Id,
                                ModeloId,
                                ServicoId,
                                Linha,
                                Ocultar,
                                Descricao,
                                TipoRegistro,
                                BaseCalculo,
                                QtdeDias,
                                Periodo,
                                Valor20,
                                Valor40,
                                AdicionalArmazenagem,
                                AdicionalGRC,
                                MinimoGRC,
                                ValorANVISA,
                                Moeda,
                                ValorMinimo20,
                                ValorMinimo40,
                                ValorMargemDireita,
                                ValorMargemEsquerda,
                                ValorEntreMargens,
                                AdicionalIMO,
                                PesoMaximo,
                                AdicionalPeso,
                                ValorMinimoMargemDireita,
                                ValorMinimoMargemEsquerda,
                                ValorMinimoEntreMargens,
                                Reembolso,
                                ValorMinimo,
                                Origem,
                                Destino,
                                Valor,
                                CondicoesGerais,
                                CondicoesIniciais,
                                TipoTrabalho,
                                LinhaReferencia,
                                DescricaoValor,
                                TipoCarga,
                                ValorCif,
                                TipoDocumentoId, 
                                BaseExcesso, 
                                ValorExcesso, 
                                PesoLimite, 
                                GrupoAtracacaoId, 
                                FormaPagamentoNVOCC, 
                                ProRata,
                                LimiteBls,
                                AnvisaGRC,
                                AdicionalIMOGRC,
                                Margem
                            )
                            SELECT
                                CRM.SEQ_CRM_LAYOUT.NEXTVAL,
                                :ModeloNovo,
                                ServicoId,
                                Linha,
                                Ocultar,
                                Descricao,
                                TipoRegistro,
                                BaseCalculo,
                                QtdeDias,
                                Periodo,
                                Valor20,
                                Valor40,
                                AdicionalArmazenagem,
                                AdicionalGRC,
                                MinimoGRC,
                                ValorANVISA,
                                Moeda,
                                ValorMinimo20,
                                ValorMinimo40,
                                ValorMargemDireita,
                                ValorMargemEsquerda,
                                ValorEntreMargens,
                                AdicionalIMO,
                                PesoMaximo,
                                AdicionalPeso,
                                ValorMinimoMargemDireita,
                                ValorMinimoMargemEsquerda,
                                ValorMinimoEntreMargens,
                                Reembolso,
                                ValorMinimo,
                                Origem,
                                Destino,
                                Valor,                                
                                CondicoesGerais,
                                CondicoesIniciais,
                                TipoTrabalho,
                                LinhaReferencia,
                                DescricaoValor,
                                TipoCarga,
                                ValorCif,
                                TipoDocumentoId, 
                                BaseExcesso, 
                                ValorExcesso, 
                                PesoLimite, 
                                GrupoAtracacaoId, 
                                FormaPagamentoNVOCC, 
                                ProRata,
                                LimiteBls,
                                AnvisaGRC,
                                AdicionalIMOGRC,
                                Margem
                            FROM 
                                CRM.TB_CRM_LAYOUT
                            WHERE 
                                ModeloId = :ModeloAntigo", parametros);
            }
        }

        public void AtualizarTitulos(int linha, string descricao, int oportunidadeId, int tipoRegistro)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoRegistro", value: tipoRegistro, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET Descricao = :Descricao 
                        WHERE Linha = :Linha AND TipoRegistro = :TipoRegistro AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarCondicoesIniciais(LayoutCondicoesIniciais layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicoesIniciais", value: layout.CondicoesIniciais, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET
                                    CondicoesIniciais = :CondicoesIniciais
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 1 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarCondicoesGerais(LayoutCondicoesGerais layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicoesGerais", value: layout.CondicoesGerais, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET
                                    CondicoesGerais = :CondicoesGerais
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 18 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarArmazenagem(LayoutArmazenagem layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: layout.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalArmazenagem", value: layout.AdicionalArmazenagem, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalGRC", value: layout.AdicionalGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "MinimoGRC", value: layout.MinimoGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMOGRC", value: layout.AdicionalIMOGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorANVISA", value: layout.ValorANVISA, direction: ParameterDirection.Input);
                parametros.Add(name: "AnvisaGRC", value: layout.AnvisaGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: layout.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET                                    
                                    TipoCarga = :TipoCarga, 
                                    Descricao = :Descricao, 
                                    BaseCalculo = :BaseCalculo, 
                                    QtdeDias = :QtdeDias, 
                                    Valor = :Valor, 
                                    Valor20 = :Valor20, 
                                    Valor40 = :Valor40, 
                                    AdicionalArmazenagem = :AdicionalArmazenagem, 
                                    AdicionalGRC = :AdicionalGRC, 
                                    MinimoGRC = :MinimoGRC, 
                                    AdicionalIMO = :AdicionalIMO, 
                                    Exercito = :Exercito,
                                    AdicionalIMOGRC = :AdicionalIMOGRC,
                                    ValorANVISA = :ValorANVISA, 
                                    AnvisaGRC = :AnvisaGRC,
                                    Periodo = :Periodo, 
                                    Moeda = :Moeda
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 7 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarArmazenagemCIF(LayoutArmazenagemCIF layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: layout.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorCIF", value: layout.ValorCif, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalArmazenagem", value: layout.AdicionalArmazenagem, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalGRC", value: layout.AdicionalGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "MinimoGRC", value: layout.MinimoGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMOGRC", value: layout.AdicionalIMOGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorANVISA", value: layout.ValorANVISA, direction: ParameterDirection.Input);
                parametros.Add(name: "AnvisaGRC", value: layout.AnvisaGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: layout.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET                                    
                                    TipoCarga = :TipoCarga, 
                                    Descricao = :Descricao, 
                                    BaseCalculo = :BaseCalculo, 
                                    QtdeDias = :QtdeDias, 
                                    ValorCIF = :ValorCIF,
                                    Valor = :Valor, 
                                    Valor20 = :Valor20, 
                                    Valor40 = :Valor40, 
                                    AdicionalArmazenagem = :AdicionalArmazenagem, 
                                    AdicionalGRC = :AdicionalGRC, 
                                    MinimoGRC = :MinimoGRC, 
                                    AdicionalIMO = :AdicionalIMO, 
                                    AdicionalIMOGRC = :AdicionalIMOGRC,
                                    ValorANVISA = :ValorANVISA, 
                                    AnvisaGRC = :AnvisaGRC,
                                    Periodo = :Periodo, 
                                    Moeda = :Moeda
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 21 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarCamposEmComumArmazenagem(LayoutArmazenagem layout, string campo,  int oportunidadeId, int linha, int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: layout.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalArmazenagem", value: layout.AdicionalArmazenagem, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalGRC", value: layout.AdicionalGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "MinimoGRC", value: layout.MinimoGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorANVISA", value: layout.ValorANVISA, direction: ParameterDirection.Input);
                parametros.Add(name: "AnvisaGRC", value: layout.AnvisaGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                if (string.IsNullOrEmpty(campo))
                {
                    con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET                                                                        
                                    QtdeDias = :QtdeDias,
                                    AdicionalArmazenagem = :AdicionalArmazenagem, 
                                    AdicionalGRC = :AdicionalGRC, 
                                    MinimoGRC = :MinimoGRC, 
                                    AdicionalIMO = :AdicionalIMO, 
                                    ValorANVISA = :ValorANVISA,
                                    AnvisaGRC = :AnvisaGRC
                                WHERE
                                    Id = :Id", parametros);
                }
                else
                {                    
                    var obj = con.Query<LayoutDTO>($"SELECT {campo} FROM TB_CRM_OPORTUNIDADE_LAYOUT WHERE OportunidadeId = :OportunidadeId AND Linha = :Linha", parametros).FirstOrDefault();

                    if (obj != null)
                    {
                        ObjetoHelpers.GetValue(obj, campo, out object valor);

                        parametros.Add(name: campo, value: valor, direction: ParameterDirection.Input);

                        con.Execute($@"UPDATE CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET {campo} = :{campo} WHERE Id = :Id", parametros);
                    }                    
                }
            }
        }

        public void AtualizarCamposEmComumArmazenagemCIF(LayoutArmazenagemCIF layout, string campo, int oportunidadeId, int linha, int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: layout.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalArmazenagem", value: layout.AdicionalArmazenagem, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalGRC", value: layout.AdicionalGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "MinimoGRC", value: layout.MinimoGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorANVISA", value: layout.ValorANVISA, direction: ParameterDirection.Input);
                parametros.Add(name: "AnvisaGRC", value: layout.AnvisaGRC, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                if (string.IsNullOrEmpty(campo))
                {
                    con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET                                                                        
                                    QtdeDias = :QtdeDias,
                                    AdicionalArmazenagem = :AdicionalArmazenagem, 
                                    AdicionalGRC = :AdicionalGRC, 
                                    MinimoGRC = :MinimoGRC, 
                                    AdicionalIMO = :AdicionalIMO, 
                                    ValorANVISA = :ValorANVISA,
                                    AnvisaGRC = :AnvisaGRC
                                WHERE
                                    Id = :Id", parametros);
                }
                else
                {
                    var obj = con.Query<LayoutDTO>($"SELECT {campo} FROM TB_CRM_OPORTUNIDADE_LAYOUT WHERE OportunidadeId = :OportunidadeId AND Linha = :Linha", parametros).FirstOrDefault();

                    if (obj != null)
                    {
                        ObjetoHelpers.GetValue(obj, campo, out object valor);

                        parametros.Add(name: campo, value: valor, direction: ParameterDirection.Input);

                        con.Execute($@"UPDATE CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET {campo} = :{campo} WHERE Id = :Id", parametros);
                    }
                }
            }
        }

        public void AtualizarArmazenagemMinimo(LayoutArmazenagemMinimo layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: layout.ValorCarga.ValorMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo20", value: layout.ValorCarga.ValorMinimo20, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo40", value: layout.ValorCarga.ValorMinimo40, direction: ParameterDirection.Input);
                parametros.Add(name: "LimiteBls", value: layout.LimiteBls, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET                                    
                                    TipoCarga = :TipoCarga, 
                                    Descricao = :Descricao,                                     
                                    ValorMinimo = :ValorMinimo, 
                                    ValorMinimo20 = :ValorMinimo20, 
                                    ValorMinimo40 = :ValorMinimo40, 
                                    LimiteBls = :LimiteBls,
                                    DescricaoValor = :DescricaoValor
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 8 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarArmazenagemMinimoCIF(LayoutArmazenagemMinimoCIF layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorCIF", value: layout.ValorCIF, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: layout.ValorCarga.ValorMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo20", value: layout.ValorCarga.ValorMinimo20, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo40", value: layout.ValorCarga.ValorMinimo40, direction: ParameterDirection.Input);
                parametros.Add(name: "LimiteBls", value: layout.LimiteBls, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET                                    
                                    TipoCarga = :TipoCarga, 
                                    Descricao = :Descricao,                                     
                                    ValorCif = :ValorCif, 
                                    ValorMinimo = :ValorMinimo, 
                                    ValorMinimo20 = :ValorMinimo20, 
                                    ValorMinimo40 = :ValorMinimo40, 
                                    LimiteBls = :LimiteBls,
                                    DescricaoValor = :DescricaoValor
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 22 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarArmazenagemAllIn(LayoutArmazenagemAllIn layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: layout.ValorMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "CifMinimo", value: layout.CifMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "CifMaximo", value: layout.CifMaximo, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoCif", value: layout.DescricaoCif, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "Periodo", value: layout.Periodo, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoPeriodo", value: layout.DescricaoPeriodo, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: layout.Margem, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET
                                    Descricao = :Descricao,
                                    ValorMinimo = :ValorMinimo, 
                                    Valor20 = :Valor20, 
                                    Valor40 = :Valor40,
                                    CifMinimo = :CifMinimo,
                                    CifMaximo = :CifMaximo,
                                    DescricaoCif = :DescricaoCif,
                                    BaseCalculo = :BaseCalculo,
                                    Periodo = :Periodo,
                                    DescricaoPeriodo = :DescricaoPeriodo,
                                    Margem = :Margem
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 9 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarServicoMargem(LayoutServicoParaMargem layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMargemDireita", value: layout.ValorMargemDireita, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMargemEsquerda", value: layout.ValorMargemEsquerda, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorEntreMargens", value: layout.ValorEntreMargens, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoMaximo", value: layout.PesoMaximo, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalPeso", value: layout.AdicionalPeso, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET
                                    Descricao = :Descricao,
                                    BaseCalculo = :BaseCalculo, 
                                    TipoCarga = :TipoCarga, 
                                    ValorMargemDireita = :ValorMargemDireita,
                                    ValorMargemEsquerda = :ValorMargemEsquerda,
                                    ValorEntreMargens = :ValorEntreMargens,
                                    AdicionalIMO = :AdicionalIMO,
                                    Exercito = :Exercito,
                                    PesoMaximo = :PesoMaximo,
                                    AdicionalPeso = :AdicionalPeso,
                                    Moeda = :Moeda
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 10 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarMinimoMargem(LayoutMinimoParaMargem layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimoMargemDireita", value: layout.ValorMinimoMargemDireita, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimoMargemEsquerda", value: layout.ValorMinimoMargemEsquerda, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimoEntreMargens", value: layout.ValorMinimoEntreMargens, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET
                                    Descricao = :Descricao,
                                    ValorMinimoMargemDireita = :ValorMinimoMargemDireita,
                                    ValorMinimoMargemEsquerda = :ValorMinimoMargemEsquerda,
                                    ValorMinimoEntreMargens = :ValorMinimoEntreMargens,
                                    DescricaoValor = :DescricaoValor
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 11 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarServicoMecanicaManual(LayoutServicoMecanicaManual layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "PesoMaximo", value: layout.PesoMaximo, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalPeso", value: layout.AdicionalPeso, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoTrabalho", value: layout.TipoTrabalho, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET
                                    Descricao = :Descricao,
                                    BaseCalculo = :BaseCalculo,
                                    Valor = :Valor,
                                    Valor20 = :Valor20,
                                    Valor40 = :Valor40,
                                    AdicionalIMO = :AdicionalIMO,
                                    Exercito = :Exercito,
                                    PesoMaximo = :PesoMaximo,
                                    AdicionalPeso = :AdicionalPeso,
                                    Moeda = :Moeda,
                                    TipoTrabalho = :TipoTrabalho,
                                    DescricaoValor = :DescricaoValor
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 12 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarMinimoMecanicaManual(LayoutMinimoMecanicaManual layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo20", value: layout.ValorCarga.ValorMinimo20, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo40", value: layout.ValorCarga.ValorMinimo40, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET
                                    Descricao = :Descricao,
                                    ValorMinimo20 = :ValorMinimo20,
                                    ValorMinimo40 = :ValorMinimo40,
                                    DescricaoValor = :DescricaoValor
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 13 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarServicoLiberacao(LayoutServicoLiberacao layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET
                                    Descricao = :Descricao,
                                    TipoCarga = :TipoCarga,
                                    Valor = :Valor,
                                    Valor20 = :Valor20,
                                    Valor40 = :Valor40,
                                    AdicionalIMO = :AdicionalIMO,
                                    Exercito = :Exercito,
                                    DescricaoValor = :DescricaoValor
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 14 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarHubPort(LayoutServicoHubPort layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Origem", value: layout.Origem, direction: ParameterDirection.Input);
                parametros.Add(name: "Destino", value: layout.Destino, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET
                                    Descricao = :Descricao,
                                    Origem = :Origem,
                                    Destino = :Destino,
                                    Valor = :Valor,
                                    DescricaoValor = :DescricaoValor
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 15 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarServicosGerais(LayoutServicosGerais layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "AdicionalIMO", value: layout.AdicionalIMO, direction: ParameterDirection.Input);
                parametros.Add(name: "Exercito", value: layout.Exercito, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "Moeda", value: layout.Moeda, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET
                                    Descricao = :Descricao,
                                    TipoCarga = :TipoCarga,
                                    Valor = :Valor,
                                    Valor20 = :Valor20,
                                    Valor40 = :Valor40,
                                    AdicionalIMO = :AdicionalIMO,
                                    Exercito = :Exercito,
                                    BaseCalculo = :BaseCalculo,
                                    Moeda = :Moeda,
                                    DescricaoValor = :DescricaoValor
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 16 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarMinimoGeral(LayoutMinimoGeral layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: layout.ValorCarga.ValorMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo20", value: layout.ValorCarga.ValorMinimo20, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo40", value: layout.ValorCarga.ValorMinimo40, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET
                                    Descricao = :Descricao,
                                    TipoCarga = :TipoCarga,
                                    ValorMinimo = :ValorMinimo,
                                    ValorMinimo20 = :ValorMinimo20,
                                    ValorMinimo40 = :ValorMinimo40,                                    
                                    DescricaoValor = :DescricaoValor
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 17 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void AtualizarPeriodoPadrao(LayoutPeriodoPadrao layout, int linha, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Linha", value: linha, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: layout.Cabecalho.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoCarga", value: layout.ValorCarga.TipoCarga, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor", value: layout.ValorCarga.Valor, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: layout.ValorCarga.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: layout.ValorCarga.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "BaseCalculo", value: layout.BaseCalculo, direction: ParameterDirection.Input);
                parametros.Add(name: "DescricaoValor", value: layout.DescricaoValor, direction: ParameterDirection.Input);

                con.Execute($@"UPDATE 
                                CRM.TB_CRM_OPORTUNIDADE_LAYOUT SET
                                    Descricao = :Descricao,
                                    TipoCarga = :TipoCarga,
                                    Valor = :Valor,
                                    Valor20 = :Valor20,
                                    Valor40 = :Valor40,
                                    BaseCalculo = :BaseCalculo,
                                    DescricaoValor = :DescricaoValor
                                WHERE
                                    Linha = :Linha AND TipoRegistro = 19 AND OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public void GravarCampoPropostaAlterado(OportunidadeAlteracaoLinhaProposta alteracao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: alteracao.OportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Linha", value: alteracao.Linha, direction: ParameterDirection.Input);
                parametros.Add(name: "Propriedade", value: alteracao.Propriedade, direction: ParameterDirection.Input);
                parametros.Add(name: "UsuarioId", value: alteracao.UsuarioId, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_OPORTUNIDADE_LAYOUT_LOG (Id, OportunidadeId, Linha, Propriedade, UsuarioId) 
                    VALUES (CRM.SEQ_CRM_OPORTUN_LAYOUT_LOG.NEXTVAL, :OportunidadeId, :Linha, :Propriedade, :UsuarioId)", parametros);
            }
        }

        public void LimparCamposAlterados(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_LAYOUT_LOG WHERE OportunidadeId = :OportunidadeId", parametros);
            }
        }

        public IEnumerable<OportunidadeAlteracaoLinhaProposta> ObterAlteracoesProposta(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<OportunidadeAlteracaoLinhaProposta>(@"SELECT DISTINCT OportunidadeId, UsuarioId, Linha, Propriedade FROM CRM.TB_CRM_OPORTUNIDADE_LAYOUT_LOG WHERE OportunidadeId = :OportunidadeId ORDER BY Linha", parametros);
            }
        }

        public void CadastrarFaixasBL(FaixaBL faixa)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeLayoutId", value: faixa.LayoutId, direction: ParameterDirection.Input);
                parametros.Add(name: "BLMinimo", value: faixa.BLMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "BLMaximo", value: faixa.BLMaximo, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: faixa.ValorMinimo, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_OPORTUNIDADE_L_MIN_BL (Id, OportunidadeLayoutId, BLMinimo, BLMaximo, ValorMinimo) VALUES (CRM.SEQ_CRM_LAYOUT_VL_MINIMO_BL.NEXTVAL, :OportunidadeLayoutId, :BLMinimo, :BLMaximo, :ValorMinimo)", parametros);
            }
        }

        public void ExcluirFaixaBL(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_L_MIN_BL WHERE Id = :id", new { id });
            }
        }

        public IEnumerable<FaixaBL> ObterFaixasBL(int layoutId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaBL>(@"SELECT Id, OportunidadeLayoutId, BLMinimo, BLMaximo, ValorMinimo FROM CRM.TB_CRM_OPORTUNIDADE_L_MIN_BL WHERE OportunidadeLayoutId = :lId ORDER BY Id", new { lId = layoutId });
            }
        }

        public FaixaBL ObterFaixaBLPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaBL>(@"SELECT * FROM CRM.TB_CRM_OPORTUNIDADE_L_MIN_BL WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }

        public void CadastrarFaixasCIF(FaixaCIF faixa)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeLayoutId", value: faixa.LayoutId, direction: ParameterDirection.Input);
                parametros.Add(name: "Minimo", value: faixa.Minimo, direction: ParameterDirection.Input);
                parametros.Add(name: "Maximo", value: faixa.Maximo, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: faixa.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: faixa.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: faixa.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: faixa.Descricao, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_OPORTUNIDADE_L_CIF_BL (Id, OportunidadeLayoutId, Minimo, Maximo, Margem, Valor20, Valor40, Descricao) VALUES (CRM.SEQ_CRM_LAYOUT_VL_CIF_BL.NEXTVAL, :OportunidadeLayoutId, :Minimo, :Maximo, :Margem, :Valor20, :Valor40, :Descricao)", parametros);
            }
        }

        public void ExcluirFaixaCIF(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_L_CIF_BL WHERE Id = :id", new { id });
            }
        }

        public IEnumerable<FaixaCIF> ObterFaixasCIF(int layoutId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaCIF>(@"SELECT Id, OportunidadeLayoutId, Minimo, Maximo, Margem, Valor20, Valor40, Descricao FROM CRM.TB_CRM_OPORTUNIDADE_L_CIF_BL WHERE OportunidadeLayoutId = :lId ORDER BY Id", new { lId = layoutId });
            }
        }

        public FaixaCIF ObterFaixaCIFPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaCIF>(@"SELECT * FROM CRM.TB_CRM_OPORTUNIDADE_L_CIF_BL WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }

        public void CadastrarFaixasVolume(FaixaVolume faixa)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeLayoutId", value: faixa.LayoutId, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorInicial", value: faixa.ValorInicial, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorFinal", value: faixa.ValorFinal, direction: ParameterDirection.Input);
                parametros.Add(name: "Preco", value: faixa.Preco, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_OPORTUNIDADE_VL_VOLUME (Id, OportunidadeLayoutId, ValorInicial, ValorFinal, Preco) VALUES (CRM.SEQ_CRM_LAYOUT_VL_VOLUME.NEXTVAL, :OportunidadeLayoutId, :ValorInicial, :ValorFinal, :Preco)", parametros);
            }
        }

        public void ExcluirFaixaVolume(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_VL_VOLUME WHERE Id = :id", new { id });
            }
        }

        public IEnumerable<FaixaVolume> ObterFaixasVolume(int layoutId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaVolume>(@"SELECT Id, OportunidadeLayoutId, ValorInicial, ValorFinal, Preco FROM CRM.TB_CRM_OPORTUNIDADE_VL_VOLUME WHERE OportunidadeLayoutId = :lId ORDER BY Id", new { lId = layoutId });
            }
        }

        public FaixaVolume ObterFaixasVolumePorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaVolume>(@"SELECT * FROM CRM.TB_CRM_OPORTUNIDADE_VL_VOLUME WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }

        public void CadastrarFaixasPeso(FaixaPeso faixa)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeLayoutId", value: faixa.LayoutId, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorInicial", value: faixa.ValorInicial, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorFinal", value: faixa.ValorFinal, direction: ParameterDirection.Input);
                parametros.Add(name: "Preco", value: faixa.Preco, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_OPORTUNIDADE_VL_PESO (Id, OportunidadeLayoutId, ValorInicial, ValorFinal, Preco) VALUES (CRM.SEQ_CRM_LAYOUT_VL_PESO.NEXTVAL, :OportunidadeLayoutId, :ValorInicial, :ValorFinal, :Preco)", parametros);
            }
        }

        public void ExcluirFaixaPeso(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADE_VL_PESO WHERE Id = :id", new { id });
            }
        }

        public IEnumerable<FaixaPeso> ObterFaixasPeso(int layoutId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaPeso>(@"SELECT Id, OportunidadeLayoutId, ValorInicial, ValorFinal, Preco FROM CRM.TB_CRM_OPORTUNIDADE_VL_PESO WHERE OportunidadeLayoutId = :lId ORDER BY Id", new { lId = layoutId });
            }
        }

        public FaixaPeso ObterFaixaPesoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaPeso>(@"SELECT * FROM CRM.TB_CRM_OPORTUNIDADE_VL_PESO WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }
    }
}