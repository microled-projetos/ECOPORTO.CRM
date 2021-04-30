using Dapper;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class ControleAcessoRepositorio : IControleAcessoRepositorio
    {
        public IEnumerable<PermissaoAcessoMenu> ObterMenus()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var lookup = new Dictionary<int, PermissaoAcessoMenu>();

                var menus = con.Query<PermissaoAcessoMenu, PermissaoAcessoMenuCampos, PermissaoAcessoMenu>(@"
                        SELECT 
                            mn.*, cm.* 
                        FROM 
                            CRM.TB_CRM_MENUS mn 
                        LEFT JOIN 
                            CRM.TB_CRM_MENUS_CAMPOS cm ON mn.Id = cm.MenuId
                        ORDER BY mn.Id ASC",
                     (mn, cm) =>
                        {
                            PermissaoAcessoMenu menu;

                            if (!lookup.TryGetValue(mn.Id, out menu))
                                lookup.Add(mn.Id, menu = mn);

                            if (menu.Campos == null)
                                menu.Campos = new List<PermissaoAcessoMenuCampos>();

                            menu.Campos.Add(cm);

                            return menu;

                        }).AsQueryable();

                return lookup.Values;
            }
        }

        public void AplicarPermissoes(int cargoId, IReadOnlyCollection<PermissaoAcesso> permissoes)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "CargoId", value: cargoId, direction: ParameterDirection.Input);

                    con.Execute(@"DELETE FROM CRM.TB_CRM_USUARIOS_PERMISSAO_TELA WHERE PermissaoId IN (SELECT Id FROM CRM.TB_CRM_USUARIOS_PERMISSOES WHERE CargoId = :CargoId)", parametros);
                    con.Execute(@"DELETE FROM CRM.TB_CRM_USUARIOS_PERMISSOES WHERE CargoId = :CargoId", parametros);

                    foreach (var permissao in permissoes)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "MenuId", value: permissao.MenuId, direction: ParameterDirection.Input);
                        parametros.Add(name: "CargoId", value: permissao.CargoId, direction: ParameterDirection.Input);
                        parametros.Add(name: "Acessar", value: permissao.Acessar.ToInt(), direction: ParameterDirection.Input);
                        parametros.Add(name: "Cadastrar", value: permissao.Cadastrar.ToInt(), direction: ParameterDirection.Input);
                        parametros.Add(name: "Atualizar", value: permissao.Atualizar.ToInt(), direction: ParameterDirection.Input);
                        parametros.Add(name: "Excluir", value: permissao.Excluir.ToInt(), direction: ParameterDirection.Input);
                        parametros.Add(name: "Logs", value: permissao.Logs.ToInt(), direction: ParameterDirection.Input);

                        parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        con.Execute(@"INSERT INTO CRM.TB_CRM_USUARIOS_PERMISSOES (Id, MenuId, CargoId, Acessar, Cadastrar, Atualizar, Excluir, Logs) VALUES (CRM.SEQ_CRM_USUARIOS_PERMISSOES.NEXTVAL, :MenuId, :CargoId, :Acessar, :Cadastrar, :Atualizar, :Excluir, :Logs) RETURNING Id INTO :Id", parametros);

                        var permissaoId = parametros.Get<int>("Id");

                        foreach (var campo in permissao.Campos)
                        {
                            parametros = new DynamicParameters();

                            parametros.Add(name: "PermissaoId", value: permissaoId, direction: ParameterDirection.Input);
                            parametros.Add(name: "CampoId", value: campo.Id, direction: ParameterDirection.Input);
                            parametros.Add(name: "SomenteLeitura", value: campo.SomenteLeitura.ToInt(), direction: ParameterDirection.Input);
                            parametros.Add(name: "TipoPermissao", value: campo.TipoPermissao, direction: ParameterDirection.Input);

                            con.Execute(@"INSERT INTO CRM.TB_CRM_USUARIOS_PERMISSAO_TELA (Id, PermissaoId, CampoId, SomenteLeitura, TipoPermissao) VALUES (CRM.SEQ_CRM_USUARIOS_PERM_TELA.NEXTVAL, :PermissaoId, :CampoId, :SomenteLeitura, :TipoPermissao)", parametros);
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        public IEnumerable<PermissaoAcessoMenu> ObterPermissoes(int cargoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "CargoId", value: cargoId, direction: ParameterDirection.Input);

                var lookup = new Dictionary<int, PermissaoAcessoMenu>();

                var menus = con.Query<PermissaoAcessoMenu, PermissaoAcessoMenuCampos, PermissaoAcessoMenu>(@"
                        SELECT 
                            B.Id,
                            B.Descricao,
                            B.DescricaoCompleta,
                            A.Acessar,
                            A.Cadastrar,
                            A.Atualizar,
                            A.Excluir,
                            A.Logs,
                            C.Id,
                            C.MenuId,
                            C.ObjetoId,
                            C.ObjetoDescricao,
                            C.Requerido,
                            D.SomenteLeitura,
                            D.TipoPermissao
                        FROM
                            CRM.TB_CRM_USUARIOS_PERMISSOES A
                        LEFT JOIN
                            CRM.TB_CRM_MENUS B ON A.MenuId = B.Id
                        LEFT JOIN
                            CRM.TB_CRM_MENUS_CAMPOS C ON B.Id = C.MenuId
                        LEFT JOIN
                            CRM.TB_CRM_USUARIOS_PERMISSAO_TELA D ON C.Id = D.CampoId And A.Id = D.PermissaoId
                        WHERE
                            A.CargoId = :CargoId
                        ORDER BY 
                            B.Id, C.ObjetoDescricao",
                     (mn, cm) =>
                     {
                         PermissaoAcessoMenu menu;

                         if (!lookup.TryGetValue(mn.Id, out menu))
                             lookup.Add(mn.Id, menu = mn);

                         if (menu.Campos == null)
                             menu.Campos = new List<PermissaoAcessoMenuCampos>();

                         menu.Campos.Add(cm);

                         return menu;

                     }, parametros, splitOn: "Id").AsQueryable();

                return lookup.Values;
            }
        }

        public IEnumerable<Menu> ObterMenusDinamicos()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var lookup = new Dictionary<int, Menu>();

                var menus = con.Query<Menu, SubMenu, Menu>(@"
                        SELECT 
                            A.Id, A.Descricao, A.Url, B.Id, B.Descricao, B.Url, B.UrlExterna, B.ValidaUser, B.ObjetoId
                        FROM 
                            CRM.TB_CRM_MENUS A 
                        LEFT JOIN 
                            CRM.TB_CRM_SUB_MENUS B ON A.Id = B.MenuId
                        WHERE
                            A.Dinamico = 1 AND Inativo = 0",
                     (mn, smn) =>
                     {
                         Menu menu;

                         if (!lookup.TryGetValue(mn.Id, out menu))
                             lookup.Add(mn.Id, menu = mn);

                         if (menu.SubMenus == null)
                             menu.SubMenus = new List<SubMenu>();

                         menu.SubMenus.Add(smn);

                         return menu;

                     }).AsQueryable();

                return lookup.Values;
            }
        }

        public bool ExistePermissaoNoCargo(int cargoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "CargoId", value: cargoId, direction: ParameterDirection.Input);

                return con.Query<bool>(@"
                        SELECT * FROM CRM.TB_CRM_USUARIOS_PERMISSOES WHERE CargoId = :CargoId", parametros).Any();
            }
        }
    }
}
