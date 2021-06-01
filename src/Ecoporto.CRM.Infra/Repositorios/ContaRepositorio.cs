using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class ContaRepositorio : IContaRepositorio
    {
        public IEnumerable<ContaDTO> ObterContas(int pagina, int registrosPorPagina, string filtro, string orderBy, out int totalFiltro, int? usuarioId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                var filtroSQL = string.Empty;

                if (!string.IsNullOrEmpty(filtro))
                {
                    parametros.Add(name: "Filtro", value: "%" + filtro + "%", direction: ParameterDirection.Input);
                    filtroSQL = " AND (A.Descricao LIKE :Filtro OR A.Documento LIKE :Filtro) ";
                }

                if (usuarioId.HasValue)
                {
                    parametros.Add(name: "UsuarioId", value: usuarioId.Value, direction: ParameterDirection.Input);
                    filtroSQL = " AND A.Id IN (SELECT ContaId FROM TB_CRM_USUARIOS_CONTAS WHERE UsuarioId = :UsuarioId) ";
                }

                if (string.IsNullOrEmpty(orderBy))
                {
                    orderBy = " ORDER BY A.Descricao, A.NomeFantasia, A.Documento, A.Segmento, C.Nome ASC ";
                }

                var sql = $@"
                    SELECT * FROM (
                        SELECT Contas.*, ROWNUM row_num
                            FROM (
                                SELECT 
                                    A.Id,
                                    A.Descricao,
                                    A.NomeFantasia,
                                    A.Documento,
                                    A.SituacaoCadastral,
                                    A.Segmento,
                                    A.ClassificacaoFiscal,
                                    B.Login As CriadoPor,
                                    C.Nome As Vendedor,
                                    count(*) over() TotalLinhas
                                FROM
                                    CRM.TB_CRM_CONTAS A 
                                LEFT JOIN
                                    CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id 
                                LEFT JOIN
                                    CRM.TB_CRM_USUARIOS C ON A.VendedorId = C.Id 
                                WHERE
                                    A.Id > 0 {filtroSQL} {orderBy}) Contas
                        WHERE ROWNUM < (({pagina} * {registrosPorPagina}) + 1)) 
                    WHERE row_num >= ((({pagina} - 1) * {registrosPorPagina}) + 1)";


                var query = con.Query<ContaDTO>(sql, parametros);

                totalFiltro = query.Select(c => c.TotalLinhas).FirstOrDefault();
                return query;
            }
        }

        public IEnumerable<Conta> ObterContas()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Conta>("SELECT * FROM CRM.TB_CRM_CONTAS ORDER BY Id");
            }
        }

        public int ObterTotalContas()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<int>($@"SELECT COUNT(1) FROM TB_CRM_CONTAS").Single();
            }
        }

        public int Cadastrar(Conta conta)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "CriadoPor", value: conta.CriadoPor, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: conta.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Documento", value: conta.Documento, direction: ParameterDirection.Input);
                parametros.Add(name: "NomeFantasia", value: conta.NomeFantasia, direction: ParameterDirection.Input);
                parametros.Add(name: "InscricaoEstadual", value: conta.InscricaoEstadual, direction: ParameterDirection.Input);
                parametros.Add(name: "Telefone", value: conta.Telefone, direction: ParameterDirection.Input);
                parametros.Add(name: "VendedorId", value: conta.VendedorId, direction: ParameterDirection.Input);
                parametros.Add(name: "SituacaoCadastral", value: conta.SituacaoCadastral, direction: ParameterDirection.Input);
                parametros.Add(name: "Segmento", value: conta.Segmento, direction: ParameterDirection.Input);
                parametros.Add(name: "ClassificacaoFiscal", value: conta.ClassificacaoFiscal, direction: ParameterDirection.Input);
                parametros.Add(name: "Logradouro", value: conta.Logradouro, direction: ParameterDirection.Input);
                parametros.Add(name: "Bairro", value: conta.Bairro, direction: ParameterDirection.Input);
                parametros.Add(name: "Estado", value: conta.Estado, direction: ParameterDirection.Input);
                parametros.Add(name: "Numero", value: conta.Numero, direction: ParameterDirection.Input);
                parametros.Add(name: "Complemento", value: conta.Complemento, direction: ParameterDirection.Input);
                parametros.Add(name: "CEP", value: conta.CEP, direction: ParameterDirection.Input);
                parametros.Add(name: "CidadeId", value: conta.CidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "PaisId", value: conta.PaisId, direction: ParameterDirection.Input);
                parametros.Add(name: "Blacklist", value: conta.Blacklist.ToInt(), direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"
                        INSERT INTO CRM.TB_CRM_CONTAS 
                            ( 
                                Id,
                                CriadoPor,
                                Descricao,
                                Documento,
                                NomeFantasia,
                                InscricaoEstadual,
                                Telefone,
                                VendedorId,
                                SituacaoCadastral,
                                Segmento,
                                ClassificacaoFiscal,
                                Logradouro,
                                Bairro,
                                Estado,
                                Numero,
                                Complemento,
                                CEP,
                                CidadeId,
                                PaisId,
                                Blacklist
                            ) VALUES ( 
                                CRM.SEQ_CRM_CONTAS.NEXTVAL, 
                                :CriadoPor,
                                :Descricao,
                                :Documento,
                                :NomeFantasia,
                                :InscricaoEstadual,
                                :Telefone,
                                :VendedorId,
                                :SituacaoCadastral,
                                :Segmento,
                                :ClassificacaoFiscal,
                                :Logradouro,
                                :Bairro,
                                :Estado,
                                :Numero,
                                :Complemento,
                                :CEP,
                                :CidadeId,
                                :PaisId,
                                :Blacklist
                            ) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void Atualizar(Conta conta)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: conta.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Documento", value: conta.Documento, direction: ParameterDirection.Input);
                parametros.Add(name: "NomeFantasia", value: conta.NomeFantasia, direction: ParameterDirection.Input);
                parametros.Add(name: "InscricaoEstadual", value: conta.InscricaoEstadual, direction: ParameterDirection.Input);
                parametros.Add(name: "Telefone", value: conta.Telefone, direction: ParameterDirection.Input);
                parametros.Add(name: "VendedorId", value: conta.VendedorId, direction: ParameterDirection.Input);
                parametros.Add(name: "SituacaoCadastral", value: conta.SituacaoCadastral, direction: ParameterDirection.Input);
                parametros.Add(name: "Segmento", value: conta.Segmento, direction: ParameterDirection.Input);
                parametros.Add(name: "ClassificacaoFiscal", value: conta.ClassificacaoFiscal, direction: ParameterDirection.Input);
                parametros.Add(name: "Logradouro", value: conta.Logradouro, direction: ParameterDirection.Input);
                parametros.Add(name: "Bairro", value: conta.Bairro, direction: ParameterDirection.Input);
                parametros.Add(name: "Estado", value: conta.Estado, direction: ParameterDirection.Input);
                parametros.Add(name: "Numero", value: conta.Numero, direction: ParameterDirection.Input);
                parametros.Add(name: "Complemento", value: conta.Complemento, direction: ParameterDirection.Input);
                parametros.Add(name: "CEP", value: conta.CEP, direction: ParameterDirection.Input);
                parametros.Add(name: "CidadeId", value: conta.CidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "PaisId", value: conta.PaisId, direction: ParameterDirection.Input);
                parametros.Add(name: "Blacklist", value: conta.Blacklist.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: conta.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE 
                                CRM.TB_CRM_CONTAS 
                                    SET
                                        Descricao = :Descricao,
                                        Documento = :Documento,
                                        NomeFantasia = :NomeFantasia,
                                        InscricaoEstadual = :InscricaoEstadual,
                                        Telefone = :Telefone,
                                        VendedorId = :VendedorId,
                                        SituacaoCadastral = :SituacaoCadastral,
                                        Segmento = :Segmento,
                                        ClassificacaoFiscal = :ClassificacaoFiscal,
                                        Logradouro = :Logradouro,
                                        Bairro = :Bairro,
                                        Estado = :Estado,
                                        Numero = :Numero,
                                        Complemento = :Complemento,
                                        CEP = :CEP,
                                        CidadeId = :CidadeId,
                                        PaisId = :PaisId,
                                        Blacklist = :Blacklist
                                    WHERE Id = :Id", parametros);
                con.Execute(@"update CRM.TB_CRM_contas set  blacklist=1  where SUBSTR(REPLACE(REPLACE(REPLACE(Documento, '.',''), '/', ''), '-', ''), 0, 8)  in(
							select SUBSTR(REPLACE(REPLACE(REPLACE(Documento, '.',''), '/', ''), '-', ''), 0, 8)  from  CRM.TB_CRM_contas where blacklist=1)
								and nvl(blacklist,0)=0");
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_CONTAS WHERE Id = :Id", new { id });
            }
        }

        public IEnumerable<Conta> ObterContasPorDescricao(string descricao, int? usuarioId)
        {
            var criterioDescricao = "%" + descricao.ToUpper() + "%";
            var criterioDocumento = "" + descricao.ToUpper() + "%";

            var filtroSQL = string.Empty;

            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "CriterioDescricao", value: criterioDescricao, direction: ParameterDirection.Input);
                parametros.Add(name: "CriterioDocumento", value: criterioDocumento, direction: ParameterDirection.Input);

                if (usuarioId.HasValue)
                {
                    parametros.Add(name: "UsuarioId", value: usuarioId, direction: ParameterDirection.Input);
                    filtroSQL = " AND A.Id IN (SELECT ContaId FROM TB_CRM_USUARIOS_CONTAS WHERE UsuarioId = :UsuarioId) ";
                }

                return con.Query<Conta>($@"
                    SELECT 
                        A.Id, 
                        A.Descricao, 
                        A.Documento, 
                        A.NomeFantasia, 
                        A.SituacaoCadastral, 
                        A.VendedorId, 
                        A.Segmento, 
                        A.ClassificacaoFiscal,
                        B.Nome As Vendedor
                    FROM 
                        CRM.TB_CRM_CONTAS A
                    LEFT JOIN
                        CRM.TB_CRM_USUARIOS B ON A.VendedorId = B.Id
                    WHERE 
                        (UPPER(DESCRICAO) LIKE :CriterioDescricao OR Documento LIKE :CriterioDocumento) {filtroSQL}
                    AND
                        A.SituacaoCadastral = 1
                    AND 
                        ROWNUM < 25", parametros);
            }
        }

        public IEnumerable<Conta> ObterContasPorRaizDocumento(string documento)
        {           
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Documento", value: documento.Substring(0, 8), direction: ParameterDirection.Input);

                return con.Query<Conta>($@"
                    SELECT 
                        Id, 
                        Descricao, 
                        Documento, 
                        NomeFantasia, 
                        SituacaoCadastral, 
                        VendedorId, 
                        Segmento, 
                        ClassificacaoFiscal                       
                    FROM 
                        CRM.TB_CRM_CONTAS
                    WHERE 
                        SUBSTR(REPLACE(REPLACE(REPLACE(Documento, '.',''), '/', ''), '-', ''), 0, 8) = :Documento", parametros);
            }
        }


        public IEnumerable<Conta> ObterContasImportadoresPorDescricao(string descricao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Conta>(@"
                    SELECT ID, DESCRICAO, DOCUMENTO, VENDEDORID FROM CRM.TB_CRM_CONTAS WHERE SEGMENTO = 1 AND DESCRICAO LIKE :criterio AND ROWNUM < 15", new { criterio = "%" + descricao + "%" });
            }
        }

        public Conta ObterContaPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Conta>(@"SELECT * FROM CRM.TB_CRM_CONTAS WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }
        public Conta ObterContaPorIdAnalise(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Conta>(@"
                         SELECT * FROM CRM.TB_CRM_CONTAS WHERE Id in(
                            select contaid   from crm.tb_crm_spc_consultas where  substr(cnpj,1,10) in(
                            SELECT substr(documento,1,10) FROM CRM.TB_CRM_CONTAS WHERE Id=:id))

                            union all
                            SELECT * FROM CRM.TB_CRM_CONTAS WHERE Id=:id  and substr(documento,1,10)  not in(
                            select nvl(max(substr(cnpj,1,10)),'z') from crm.tb_crm_spc_consultas  )", new { id }).FirstOrDefault();
            }
        }
        public Conta ObterContaPorDocumento(string documento)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Documento", value: documento, direction: ParameterDirection.Input);

                return con.Query<Conta>(@"SELECT * FROM CRM.TB_CRM_CONTAS WHERE Documento = :Documento", parametros).FirstOrDefault();
            }
        }

        public Conta ContaExistente(string descricao, string documento)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Conta>(@"SELECT Id, Descricao, Documento FROM CRM.TB_CRM_CONTAS WHERE Documento = :documento", new { descricao, documento }).FirstOrDefault();
            }
        }

        public void CadastrarRangeIPS(ControleAcessoConta controle)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: controle.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaId", value: controle.ContaId, direction: ParameterDirection.Input);
                parametros.Add(name: "IPInicial", value: controle.IPInicial, direction: ParameterDirection.Input);
                parametros.Add(name: "IPFinal", value: controle.IPFinal, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_CONTAS_IPS (Id, Descricao, ContaId, IPInicial, IPFinal) VALUES (CRM.SEQ_CRM_CONTAS_IPS.NEXTVAL, :Descricao, :ContaId, :IPInicial, :IPFinal)", parametros);
            }
        }

        public void AtualizarRangeIPS(ControleAcessoConta controle)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: controle.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "IPInicial", value: controle.IPInicial, direction: ParameterDirection.Input);
                parametros.Add(name: "IPFinal", value: controle.IPFinal, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: controle.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_CONTAS_IPS SET Descricao = :Descricao, IPInicial = :IPInicial, IPFinal = :IPFinal WHERE Id = :Id", parametros);
            }
        }

        public void ExcluirRangeIP(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_CONTAS_IPS WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<ControleAcessoConta> ObterVinculosIPs(int contaId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ContaId", value: contaId, direction: ParameterDirection.Input);

                return con.Query<ControleAcessoConta>(@"SELECT Id, Descricao, IPInicial, IPFinal FROM CRM.TB_CRM_CONTAS_IPS WHERE ContaId = :ContaId", parametros);
            }
        }

        public ControleAcessoConta ObterVinculoIPPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<ControleAcessoConta>(@"SELECT Id, Descricao, IPInicial, IPFinal FROM CRM.TB_CRM_CONTAS_IPS WHERE Id = :Id", parametros).FirstOrDefault();
            }
        }
    }
}
