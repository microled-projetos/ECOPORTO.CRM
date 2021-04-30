using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Datatables;
using Ecoporto.CRM.Site.Extensions;
using Ecoporto.CRM.Site.Filtros;
using Ecoporto.CRM.Site.Models;
using NLog;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class ContasController : BaseController
    {
        private readonly IContaRepositorio _contaRepositorio;
        private readonly ICidadeRepositorio _cidadeRepositorio;
        private readonly IPaisRepositorio _paisRepositorio;
        private readonly IVendedorRepositorio _vendedorRepositorio;
        private readonly IContatoRepositorio _contatoRepositorio;
        private readonly IParceiroRepositorio _parceiroRepositorio;
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IEquipeContaRepositorio _equipeContaRepositorio;

        public ContasController(
            IContaRepositorio contaRepositorio,
            ICidadeRepositorio cidadeRepositorio,
            IPaisRepositorio paisRepositorio,
            IVendedorRepositorio vendedorRepositorio,
            IContatoRepositorio contatoRepositorio,
            IParceiroRepositorio parceiroRepositorio,
            IOportunidadeRepositorio oportunidadeRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IEquipeContaRepositorio equipeContaRepositorio,
            ILogger logger) : base(logger)
        {
            _contaRepositorio = contaRepositorio;
            _cidadeRepositorio = cidadeRepositorio;
            _paisRepositorio = paisRepositorio;
            _vendedorRepositorio = vendedorRepositorio;
            _contatoRepositorio = contatoRepositorio;
            _parceiroRepositorio = parceiroRepositorio;
            _oportunidadeRepositorio = oportunidadeRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _equipeContaRepositorio = equipeContaRepositorio;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public ActionResult Consultar(JQueryDataTablesParamViewModel Params)
        {
            var contas = _contaRepositorio
                .ObterContas(Params.Pagina, Params.iDisplayLength, Params.sSearch?.ToUpper(), Params.OrderBy, out int totalFiltro, (int?)ViewBag.UsuarioExternoId)
                .Select(c => new
                {
                    c.Id,
                    c.CriadoPor,
                    c.Descricao,
                    c.NomeFantasia,
                    c.Documento,
                    c.Vendedor,
                    SituacaoCadastral = c.SituacaoCadastral.ToName(),
                    Segmento = c.Segmento.ToName()
                });

            var totalRegistros = _contaRepositorio.ObterTotalContas();

            var resultado = new
            {
                Params.sEcho,
                iTotalRecords = totalRegistros,
                iTotalDisplayRecords = totalFiltro,
                aaData = contas
            };

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        private void PopularVendedores(ContaViewModel viewModel)
        {
            viewModel.Vendedores = _vendedorRepositorio
                .ObterVendedores()
                .ToList();
        }

        private void PopularPaises(ContaViewModel viewModel)
        {
            viewModel.Paises = _paisRepositorio
                .ObterPaises()
                .ToList();
        }

        private void PopularCidades(ContaViewModel viewModel)
        {
            viewModel.Cidades = _cidadeRepositorio
                .ObterCidadesPorEstado(viewModel.Estado)
                .ToList();
        }

        private void PopularContatos(ContaViewModel viewModel)
        {
            viewModel.Contatos = _contatoRepositorio
                .ObterContatosPorConta(viewModel.Id)
                .ToList();
        }

        private void PopularVinculosIPs(ContaViewModel viewModel)
        {
            viewModel.VinculoIPS = _contaRepositorio
                .ObterVinculosIPs(viewModel.Id)
                .ToList();
        }

        private void PopularOportunidades(ContaViewModel viewModel)
        {
            var oportunidades = _oportunidadeRepositorio
                .ObterOportunidadesPorConta(viewModel.Id)
                .ToList();

            viewModel.Oportunidades = oportunidades
                .Where(c => c.StatusOportunidade == StatusOportunidade.ATIVA || !Enum.IsDefined(typeof(StatusOportunidade), c.StatusOportunidade)).ToList();

            viewModel.OportunidadesInativas = oportunidades
                .Where(c => c.StatusOportunidade == StatusOportunidade.CANCELADA ||
                            c.StatusOportunidade == StatusOportunidade.REVISADA ||
                            c.StatusOportunidade == StatusOportunidade.VENCIDO).ToList();
        }

        [HttpGet]
        [CanActivate(Roles = "Contas:Cadastrar")]
        public ActionResult Cadastrar()
        {
            var viewModel = new ContaViewModel
            {
                ClassificacaoFiscal = ClassificacaoFiscal.PJ,
                SituacaoCadastral = SituacaoCadastral.ATIVO,
                Estado = Estado.SP
            };

            PopularVendedores(viewModel);
            PopularPaises(viewModel);
            PopularCidades(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Cadastrar([Bind(Include = "Descricao, Documento, NomeFantasia, InscricaoEstadual, Telefone, VendedorId, SituacaoCadastral, Segmento, ClassificacaoFiscal, Logradouro, Bairro, Numero, Complemento, CEP, Estado, CidadeId, PaisId, Blacklist")] ContaViewModel viewModel)
        {
            var contaExistente = _contaRepositorio.ContaExistente(viewModel.Descricao, viewModel.Documento);

            if (contaExistente != null)
                ModelState.AddModelError(string.Empty, $"Já existe uma Conta cadastrada com o mesmo CNPJ. (Cód: {contaExistente.Id} - {contaExistente.Descricao} - CNPJ: {contaExistente.Documento})");

            if (ModelState.IsValid)
            {                
                var conta = new Conta(
                    User.ObterId(),
                    viewModel.Descricao,
                    viewModel.Documento,
                    viewModel.NomeFantasia,
                    viewModel.InscricaoEstadual,
                    viewModel.Telefone,
                    viewModel.VendedorId,
                    viewModel.SituacaoCadastral,
                    viewModel.Segmento,
                    viewModel.ClassificacaoFiscal,
                    viewModel.Logradouro,
                    viewModel.Bairro,
                    viewModel.Numero,
                    viewModel.Complemento,
                    viewModel.CEP,
                    viewModel.Estado,
                    viewModel.CidadeId,
                    viewModel.PaisId,
                    viewModel.Blacklist);

                if (Validar(conta))
                {
                    conta.Id = _contaRepositorio.Cadastrar(conta);
                    TempData["Sucesso"] = true;

                    GravarLogAuditoria(TipoLogAuditoria.INSERT, conta);

                    return RedirectToAction("Atualizar", "Contas", new { id = conta.Id });
                }
            }

            PopularVendedores(viewModel);
            PopularPaises(viewModel);
            PopularCidades(viewModel);

            return View(viewModel);
        }

        [HttpGet]
        [CanActivate(Roles = "Contas:Atualizar")]
        [CanActivateVisualizarContas(Roles = "Contas:Atualizar")]
        [CanActivateContaUsuarioExterno]
        public ActionResult Atualizar(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var conta = _contaRepositorio.ObterContaPorId(id.Value);

            if (conta == null)
                RegistroNaoEncontrado();

            var viewModel = new ContaViewModel
            {
                Id = conta.Id,
                Descricao = conta.Descricao,
                SituacaoCadastral = conta.SituacaoCadastral,
                Segmento = conta.Segmento,
                ClassificacaoFiscal = conta.ClassificacaoFiscal,
                Documento = conta.Documento,
                InscricaoEstadual = conta.InscricaoEstadual,
                NomeFantasia = conta.NomeFantasia,
                Telefone = conta.Telefone,
                VendedorId = conta.VendedorId,
                CEP = conta.CEP,
                Logradouro = conta.Logradouro,
                Numero = conta.Numero,
                Bairro = conta.Bairro,
                Estado = conta.Estado,
                CidadeId = conta.CidadeId,
                PaisId = conta.PaisId,
                Complemento = conta.Complemento,
                CriadoPor = conta.CriadoPor,
                Blacklist = conta.Blacklist
            };

            var usuario = _usuarioRepositorio.ObterUsuarioPorId(conta.CriadoPor);

            if (usuario != null)
            {
                ViewBag.UsuarioCriacao = usuario.Login;
                ViewBag.DataCriacao = conta.DataCriacao.DataHoraFormatada();
            }

            PopularCidades(viewModel);
            PopularPaises(viewModel);
            PopularVendedores(viewModel);
            PopularContatos(viewModel);
            PopularOportunidades(viewModel);
            PopularVinculosIPs(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [CanActivateAtualizarContas(Roles = "Contas:Atualizar")]
        public ActionResult Atualizar([Bind(Include = "Id, Descricao, Documento, NomeFantasia, InscricaoEstadual, Telefone, VendedorId, SituacaoCadastral, Segmento, ClassificacaoFiscal, Logradouro, Bairro, Numero, Complemento, CEP, Estado, CidadeId, PaisId, Blacklist")] ContaViewModel viewModel, int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var conta = _contaRepositorio.ObterContaPorId(id.Value);

            if (conta == null)
                RegistroNaoEncontrado();

            var contaExistente = _contaRepositorio.ContaExistente(viewModel.Descricao, viewModel.Documento);

            if (contaExistente != null && contaExistente.Id != conta.Id)
                ModelState.AddModelError(string.Empty, $"Já existe uma Conta cadastrada com o mesmo CNPJ. (Cód: {contaExistente.Id} - {contaExistente.Descricao} - CNPJ: {contaExistente.Documento})");

            if (ModelState.IsValid)
            {
                conta.Alterar(
                    new Conta(
                        viewModel.CriadoPor,
                        viewModel.Descricao,
                        viewModel.Documento,
                        viewModel.NomeFantasia,
                        viewModel.InscricaoEstadual,
                        viewModel.Telefone,
                        viewModel.VendedorId,
                        viewModel.SituacaoCadastral,
                        viewModel.Segmento,
                        viewModel.ClassificacaoFiscal,
                        viewModel.Logradouro,
                        viewModel.Bairro,
                        viewModel.Numero,
                        viewModel.Complemento,
                        viewModel.CEP,
                        viewModel.Estado,
                        viewModel.CidadeId,
                        viewModel.PaisId,
                        viewModel.Blacklist));

                if (Validar(conta))
                {
                    _contaRepositorio.Atualizar(conta);
                    TempData["Sucesso"] = true;

                    GravarLogAuditoria(TipoLogAuditoria.UPDATE, conta);
                }
            }

            PopularCidades(viewModel);
            PopularPaises(viewModel);
            PopularVendedores(viewModel);
            PopularContatos(viewModel);
            PopularOportunidades(viewModel);
            PopularVinculosIPs(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var conta = _contaRepositorio.ObterContaPorId(id);

                if (conta == null)
                    RegistroNaoEncontrado();

                var oportunidade = _oportunidadeRepositorio.ObterOportunidadesPorConta(conta.Id);

                if (oportunidade.Any())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Esta conta já está sendo utilizada em uma oportunidade");

                var contatos = _contatoRepositorio.ObterContatosPorConta(conta.Id);

                if (contatos.Any())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Existem contatos vinculados a esta conta");

                _contaRepositorio.Excluir(conta.Id);

                GravarLogAuditoria(TipoLogAuditoria.DELETE, conta);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public ActionResult ObterCidadesPorEstado(Estado estado)
        {
            var cidades = _cidadeRepositorio
                .ObterCidadesPorEstado(estado)
                .Select(c => new
                {
                    c.Id,
                    c.Descricao
                });

            return Json(cidades, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObterClientePorDocumento(string documento)
        {
            var parceiro = _parceiroRepositorio.ObterParceiroPorDocumento(documento);

            if (parceiro == null)
                return HttpNotFound();

            return Json(
                new
                {
                    parceiro.NomeFantasia,
                    parceiro.InscricaoEstadual,
                    parceiro.Logradouro,
                    parceiro.Numero,
                    parceiro.Bairro,
                    parceiro.CEP,
                    parceiro.Complemento,
                    parceiro.Estado,
                    parceiro.Cidade
                }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObterContatosPorConta(int contaId)
        {
            var permiteEditar = (User.IsInRole("Administrador") || User.IsInRole("Contatos:Atualizar"));
            var permiteExcluir = (User.IsInRole("Administrador") || User.IsInRole("Contatos:Excluir"));

            var resultado = _contatoRepositorio
                .ObterContatosPorConta(contaId)
                .Select(c => new
                {
                    c.Id,
                    c.Nome,
                    c.Sobrenome,
                    c.DataNascimento,
                    c.Telefone,
                    c.Celular,
                    c.Email,
                    c.Cargo,
                    c.Departamento,
                    c.Status,
                    c.ContaId,
                    permiteEditar,
                    permiteExcluir
                });

            return Json(new
            {
                dados = resultado
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public JsonResult ObterContasPorDescricao(string descricao)
        {
            var resultado = _contaRepositorio
                .ObterContasPorDescricao(descricao, (int?)ViewBag.UsuarioExternoId)
                .Select(c => new
                {
                    c.Id,
                    c.Descricao,
                    c.Documento,
                    Display = string.Concat(c.Descricao, " (", c.Documento, ")")
                }).ToList();

            return Json(new
            {
                dados = resultado
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ValidarAcessoAtualizacaoExclusaoContato(int contaId)
        {
            if (User.IsInRole("Administrador"))
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);

            if (User.IsInRole("UsuarioExterno"))
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);

            var login = User.Identity.Name;

            var permissoesPorVendedor = _equipeContaRepositorio
                    .ObterPermissoesContaPorVendedor(contaId, login);

            var contaBusca = _contaRepositorio.ObterContaPorId(contaId);
          
            if (permissoesPorVendedor != null)
            {
                if (permissoesPorVendedor.AcessoConta == 1)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NoContent);
                }
            }
            else
            {
                if (contaBusca.VendedorId == User.ObterId())
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NoContent);
                }
                else
                {
                    var permissoesPorConta = _equipeContaRepositorio
                    .ObterPermissoesContaPorConta(contaId, login);

                    if (permissoesPorConta != null)
                    {
                        if (permissoesPorConta.AcessoConta == 1)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
                        }
                    }
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui acesso a cadastro de Contatos para esta Conta.");
        }

        [HttpGet]
        public ActionResult ValidarAcessoAtualizacaoExclusaoOportunidade(int contaId)
        {
            if (User.IsInRole("Administrador"))
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);

            if (User.IsInRole("UsuarioExterno"))
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            
            var permissoesPorVendedor = _equipeContaRepositorio
                    .ObterPermissoesContaPorVendedor(contaId, User.Identity.Name);

            if (permissoesPorVendedor != null)
            {
                if (permissoesPorVendedor.AcessoOportunidade == 1)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NoContent);
                }
            }
            else
            {
                var permissoesPorConta = _equipeContaRepositorio
                    .ObterPermissoesContaPorConta(contaId, User.Identity.Name);

                if (permissoesPorConta != null)
                {
                    if (permissoesPorConta.AcessoOportunidade == 1)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NoContent);
                    }
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui acesso a cadastro de Oportunidade para esta Conta.");
        }

        [HttpPost]
        public ActionResult CadastrarRangeIP(string contaIdVinculoIP, string descricaoVinculoIP, string ipInicial, string ipFinal)
        {
            var contaBusca = _contaRepositorio.ObterContaPorId(contaIdVinculoIP.ToInt());

            if (contaBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Conta não encontrada ou já excluída");

            var existeVinculo = _contaRepositorio.ObterVinculosIPs(contaBusca.Id)
                .Where(c => c.IPInicial == ipInicial && c.IPFinal == ipFinal).Any();

            if (existeVinculo)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Range de IPs já cadastrado");

            _contaRepositorio.CadastrarRangeIPS(new ControleAcessoConta(contaIdVinculoIP.ToInt(), descricaoVinculoIP, ipInicial, ipFinal));

            var vinculos = _contaRepositorio.ObterVinculosIPs(contaBusca.Id);

            return PartialView("_ConsultarVinculosIPs", vinculos);
        }

        [HttpPost]
        public ActionResult ExcluirRangeIP(int id)
        {
            try
            {
                var vinculoBusca = _contaRepositorio.ObterVinculoIPPorId(id);

                if (vinculoBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Vínculo IP não encontrado ou já excluído");

                _contaRepositorio.ExcluirRangeIP(id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}