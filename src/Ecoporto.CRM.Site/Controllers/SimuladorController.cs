using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Helpers;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Sharepoint.Models;
using Ecoporto.CRM.Site.Extensions;
using Ecoporto.CRM.Site.Filtros;
using Ecoporto.CRM.Site.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class SimuladorController : BaseController
    {
        private readonly ILogger _logger;
        private readonly ISimuladorRepositorio _simuladorRepositorio;
        private readonly IDocumentoRepositorio _documentoRepositorio;
        private readonly ILocalAtracacaoRepositorio _localAtracacaoRepositorio;
        private readonly IGrupoAtracacaoRepositorio _grupoAtracacaoRepositorio;
        private readonly IParceiroRepositorio _parceiroRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IAnexoRepositorio _anexoRepositorio;
        private readonly IMargemRepositorio _margemRepositorio;

        public SimuladorController(
            ISimuladorRepositorio simuladorRepositorio,
            IDocumentoRepositorio documentoRepositorio,
            ILocalAtracacaoRepositorio localAtracacaoRepositorio,
            IGrupoAtracacaoRepositorio grupoAtracacaoRepositorio,
            IParceiroRepositorio parceiroRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IAnexoRepositorio anexoRepositorio,
            IMargemRepositorio margemRepositorio,
            ILogger logger) : base(logger)
        {
            _simuladorRepositorio = simuladorRepositorio;
            _documentoRepositorio = documentoRepositorio;
            _localAtracacaoRepositorio = localAtracacaoRepositorio;
            _grupoAtracacaoRepositorio = grupoAtracacaoRepositorio;
            _parceiroRepositorio = parceiroRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _anexoRepositorio = anexoRepositorio;
            _margemRepositorio = margemRepositorio;
            _logger = logger;
        }

        public void PopularTiposDocumentos(SimuladorViewModel viewModel)
        {
            var tiposDocumentos = _documentoRepositorio
               .ObterTiposDocumentos().ToList();

            viewModel.TiposDocumentos = tiposDocumentos;
        }

        public void PopularLocaisAtracacao(SimuladorViewModel viewModel)
        {
            var locaisAtracacao = _localAtracacaoRepositorio
               .ObterLocaisAtracacao().ToList();

            viewModel.LocaisAtracacao = locaisAtracacao;
        }

        public void PopularGruposAtracacao(SimuladorViewModel viewModel)
        {
            var gruposAtracacao = _grupoAtracacaoRepositorio
                .ObterGruposAtracacao().ToList();

            viewModel.GruposAtracacao = gruposAtracacao;
        }

        public void PopularMargens(SimuladorViewModel viewModel)
        {
            var margens = _margemRepositorio
               .ObterMargens().ToList();

            viewModel.Margens = margens;
        }

        public void PopularCargaConteiner(SimuladorViewModel viewModel)
        {
            if (viewModel.Id == 0)
                return;

            var cargaConteiner = _simuladorRepositorio
               .ObterCargaConteiner(viewModel.Id).ToList();

            viewModel.CargasConteiner = cargaConteiner;
        }

        public void PopularCargaSolta(SimuladorViewModel viewModel)
        {
            if (viewModel.Id == 0)
                return;

            var cargaSolta = _simuladorRepositorio
               .ObterCargaSolta(viewModel.Id).ToList();

            viewModel.CargasSolta = cargaSolta;
        }

        public void PopularSimuladores(SimuladorViewModel viewModel)
        {
            viewModel.Simuladores = _simuladorRepositorio
                     .ObterSimuladoresPorUsuario(User.ObterId()).ToList();
        }

        [HttpGet]
        public PartialViewResult ConsultarArmadoresPorDescricao(string descricao)
        {
            var resultado = _parceiroRepositorio
                .ObterArmadoresPorDescricao(descricao).ToList();

            return PartialView("_PesquisarArmadoresConsulta", resultado);
        }

        [HttpGet]
        [CanActivate(Roles = "Simulador:Acessar")]
        public ActionResult Index(int? id)
        {
            var viewModel = new SimuladorViewModel();

            PopularSimuladores(viewModel);

            if (id.HasValue)
            {
                var simulador = _simuladorRepositorio.ObterDetalhesSimuladorPorId(id.Value);

                if (simulador == null)
                    RegistroNaoEncontrado();

                viewModel.Id = simulador.Id;
                viewModel.ArmadorId = simulador.ArmadorId;
                viewModel.ArmadorDescricao = simulador.ArmadorDescricao;
                viewModel.ArmadorDocumento = simulador.ArmadorDocumento;
                viewModel.Descricao = simulador.Descricao;
                viewModel.Regime = simulador.Regime;
                viewModel.TipoDocumentoId = simulador.TipoDocumentoId;
                viewModel.NumeroLotes = simulador.NumeroLotes;
                viewModel.CifConteiner = simulador.CifConteiner;
                viewModel.CifCargaSolta = simulador.CifCargaSolta;
                viewModel.Periodos = simulador.Periodos;
                viewModel.LocalAtracacaoId = simulador.LocalAtracacaoId;
                viewModel.GrupoAtracacaoId = simulador.GrupoAtracacaoId;
                viewModel.Margem = simulador.Margem;
                viewModel.VolumeM3 = simulador.VolumeM3;

                if (viewModel.ArmadorId.HasValue)
                {
                    viewModel.Armadores.Add(new Parceiro(viewModel.ArmadorId.Value, viewModel.ArmadorDescricao));
                }

                viewModel.Tabelas = _simuladorRepositorio
                    .ObterTabelasSimulador("60.857.349/0001-76").ToList();
            }

            PopularTiposDocumentos(viewModel);
            PopularLocaisAtracacao(viewModel);
            PopularGruposAtracacao(viewModel);
            PopularMargens(viewModel);
            PopularCargaConteiner(viewModel);
            PopularCargaSolta(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [CanActivate(Roles = "Simulador:Cadastrar")]
        public ActionResult Index([Bind(Include = "Id, Descricao, Regime, NumeroLotes, ArmadorId, CifConteiner, CifCargaSolta, Margem, LocalAtracacaoId, GrupoAtracacaoId, VolumeM3, Periodos, TipoDocumentoId")] SimuladorViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var simulador = new Simulador(
                    viewModel.Descricao,
                    viewModel.Regime,
                    viewModel.NumeroLotes,
                    viewModel.ArmadorId,
                    viewModel.CifConteiner,
                    viewModel.CifCargaSolta,
                    viewModel.Margem,
                    viewModel.LocalAtracacaoId,
                    viewModel.GrupoAtracacaoId,
                    viewModel.VolumeM3,
                    viewModel.Periodos,
                    viewModel.TipoDocumentoId,
                    User.ObterId());

                if (Validar(simulador))
                {
                    if (viewModel.Id == 0)
                    {
                        simulador.Id = _simuladorRepositorio.CadastrarSimulador(simulador);

                        TempData["Sucesso"] = true;
                        TempData["Atualizacao"] = false;

                        GravarLogAuditoria(TipoLogAuditoria.INSERT, simulador);
                    }
                    else
                    {
                        simulador.Id = viewModel.Id;

                        _simuladorRepositorio.AtualizarSimulador(simulador);

                        TempData["Sucesso"] = true;
                        TempData["Atualizacao"] = true;

                        GravarLogAuditoria(TipoLogAuditoria.UPDATE, simulador);
                    }

                    return RedirectToAction("Index", "Simulador", new { id = simulador.Id });
                }
            }

            PopularTiposDocumentos(viewModel);
            PopularLocaisAtracacao(viewModel);
            PopularGruposAtracacao(viewModel);
            PopularMargens(viewModel);
            PopularCargaConteiner(viewModel);
            PopularCargaSolta(viewModel);
            PopularSimuladores(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var simulador = _simuladorRepositorio.ObterDetalhesSimuladorPorId(id);

                if (simulador == null)
                    RegistroNaoEncontrado();

                _simuladorRepositorio.ExcluirSimulador(simulador.Id);

                GravarLogAuditoria(TipoLogAuditoria.DELETE, simulador);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult VincularCargaConteiner(int id, string tamanho, string peso, string quantidade)
        {
            try
            {
                var simulador = _simuladorRepositorio.ObterDetalhesSimuladorPorId(id);

                if (simulador == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Simulador não encontrado");

                if (StringHelpers.IsInteger(tamanho))
                {
                    if (tamanho.ToInt() == 0)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Tamanho Inválido");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Informe o Tamanho corretamente");
                }

                if (StringHelpers.IsNumero(peso))
                {
                    if (peso.ToDecimal() == 0)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Peso Inválido");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Informe o Peso corretamente");
                }

                if (StringHelpers.IsInteger(quantidade))
                {
                    if (quantidade.ToInt() == 0)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Quantidade Inválida");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Informe a Quantidade corretamente");
                }

                _simuladorRepositorio.IncluirCargaConteiner(new SimuladorCargaConteiner
                {
                    SimuladorId = simulador.Id,
                    Tamanho = tamanho.ToInt(),
                    Peso = peso.ToDecimal(),
                    Quantidade = quantidade.ToInt(),
                    UsuarioId = User.ObterId()
                });

                var cargaConteiner = _simuladorRepositorio
                    .ObterCargaConteiner(simulador.Id).ToList();

                return PartialView("_ConsultarCargaConteiner", cargaConteiner);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult VincularCargaSolta(int id, string quantidade, string peso)
        {
            try
            {
                var simulador = _simuladorRepositorio.ObterDetalhesSimuladorPorId(id);

                if (simulador == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Simulador não encontrado");

                if (StringHelpers.IsInteger(quantidade))
                {
                    if (quantidade.ToInt() == 0)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Quantidade Inválida");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Informe a Quantidade corretamente");
                }

                if (StringHelpers.IsNumero(peso))
                {
                    if (peso.ToDecimal() == 0)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Peso Inválido");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Informe o Peso corretamente");
                }

                _simuladorRepositorio.IncluirCargaSolta(new SimuladorCargaSolta
                {
                    SimuladorId = simulador.Id,
                    Quantidade = quantidade.ToInt(),
                    Peso = peso.ToDecimal(),
                    UsuarioId = User.ObterId()
                });

                var cargaSolta = _simuladorRepositorio
                    .ObterCargaSolta(simulador.Id).ToList();

                return PartialView("_ConsultarCargaSolta", cargaSolta);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult ExcluirCargaConteiner(int id)
        {
            try
            {
                var cargaConteiner = _simuladorRepositorio.ObterCargaConteinerPorId(id);

                if (cargaConteiner == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Carga não encontrada ou já excluída");

                if (cargaConteiner.UsuarioId != User.ObterId())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível excluir esta carga");

                _simuladorRepositorio.ExcluirCargaConteiner(cargaConteiner.Id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult ExcluirCargaSolta(int id)
        {
            try
            {
                var cargaSolta = _simuladorRepositorio.ObterCargaSoltaPorId(id);

                if (cargaSolta == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Carga não encontrada ou já excluída");

                if (cargaSolta.UsuarioId != User.ObterId())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível excluir esta carga");

                _simuladorRepositorio.ExcluirCargaSolta(cargaSolta.Id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public ActionResult ConsultarTabelas(int id)
        {
            try
            {
                var simulador = _simuladorRepositorio.ObterDetalhesSimuladorPorId(id);

                if (simulador == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Simulador não encontrado");

                var tabelas = ConsultarTabelasSimulador(simulador, (int?)ViewBag.UsuarioExternoId);

                return PartialView("_ConsultarTabelas", tabelas);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        private List<SimuladorTabelasDTO> ConsultarTabelasSimulador(SimuladorDTO simulador, int? usuarioExternoId)
        {
            var tabelas = new List<SimuladorTabelasDTO>();

            using (var wsSimulador = new WsSimulador.SimuladorCalculo())
            {
                int cliente = -9999;

                if (simulador.Classe <= 0)
                {
                    cliente = wsSimulador.ObterClienteSimulador(simulador.Id);
                }

                WsSimulador.Response response = new WsSimulador.Response();

                if (usuarioExternoId.HasValue)
                {
                    var clientesVinculados = _usuarioRepositorio
                        .ObterVinculosContas(usuarioExternoId.Value)
                        .Select(c => c.ContaDocumento).ToList();

                    var cnpjsVinculados = string.Join(",", clientesVinculados);

                    response = wsSimulador.ObterTabelas(cliente, cnpjsVinculados, simulador.Classe, simulador.Id, true, false, 0);
                }
                else
                {
                    response = wsSimulador.ObterTabelas(cliente, string.Empty, simulador.Classe, simulador.Id, false, false, 0);
                }


                if (response != null)
                {
                    foreach (var tabela in response.Lista)
                    {
                        tabelas.Add(new SimuladorTabelasDTO
                        {
                            Coloader = tabela.Coloader,
                            Descricao = tabela.Descricao,
                            Despachante = tabela.Despachante,
                            Importador = tabela.Importador,
                            NVOCC = tabela.NVOCC,
                            Proposta = tabela.Proposta,
                            Id = tabela.TabelaId,
                            SimuladorId = simulador.Id
                        });
                    }
                }
            }

            return tabelas;
        }

        [HttpPost]
        [UsuarioExternoFilter]
        public ActionResult SimularTabelas(int simuladorId, string tabelas, bool completo, bool vertical)
        {
            try
            {
                var simulador = _simuladorRepositorio.ObterDetalhesSimuladorPorId(simuladorId);

                if (simulador == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Simulador não encontrado");

                using (var wsSimulador = new WsSimulador.SimuladorCalculo())
                {
                    wsSimulador.Timeout = 900000;

                    if (completo)
                        tabelas = string.Join(",", ConsultarTabelasSimulador(simulador, (int?)ViewBag.UsuarioExternoId).Select(c => c.Id));

                    var response = wsSimulador.CalcularTabelas(simulador.Id, tabelas, false);

                    if (response == null)
                        return PartialView("_Erro", new SimuladorDownloadRelatorioViewModel("Nenhum retorno do serviço do Simulador"));

                    if (response.Sucesso == false)
                        return PartialView("_Erro", new SimuladorDownloadRelatorioViewModel(response.Mensagem));
                    else
                    {
                        var responseRelatorio = wsSimulador.GerarRelatorioExcel(simulador.Id, false, false, true, false, DateTime.Now.AddMonths(-6).ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), false, vertical);

                        if (responseRelatorio.Sucesso == false)
                            return PartialView("_Erro", new SimuladorDownloadRelatorioViewModel(responseRelatorio.Mensagem));

                        var anexoBusca = _anexoRepositorio.ObterDetalhesAnexo(responseRelatorio.ArquivoId);

                        if (anexoBusca == null)
                            return PartialView("_Erro", new SimuladorDownloadRelatorioViewModel("Anexo não encontrado"));

                        return PartialView("_Sucesso", new SimuladorDownloadRelatorioViewModel
                        {
                            SimuladorId = simulador.Id,
                            NomeArquivo = anexoBusca.Anexo,
                            ArquivoId = anexoBusca.IdFile,
                            TamanhoArquivo = string.Format("{0} kb", responseRelatorio.TamanhoArquivo / 1024)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("_Erro", new SimuladorDownloadRelatorioViewModel(ex.Message));
            }
        }

        [HttpGet]
        public ActionResult BarraParametros()
        {
            return PartialView("_BarraParametrosExcel");
        }

        private int IncluirAnexo(string caminho, int simuladorId, FileInfo arquivo)
        {
            var bytes = System.IO.File.ReadAllBytes(caminho);

            if (bytes != null && bytes.Length > 0)
            {
                var token = Sharepoint.Services.Autenticador.Autenticar();

                if (token == null)
                    throw new HttpException(404, "Não foi possível se autenticar no serviço de Anexos");

                var dados = new DadosArquivoUpload
                {
                    Name = arquivo.Name,
                    Extension = arquivo.Extension,
                    System = 3,
                    DataArray = Convert.ToBase64String(bytes)
                };

                var retornoUpload = new Sharepoint.Services.AnexosService(token)
                    .EnviarArquivo(dados);

                if (!retornoUpload.success)
                    throw new HttpException(500, "Retorno API anexos: " + retornoUpload.message);

                var anexoInclusaoId = _anexoRepositorio.IncluirAnexo(
                    new Anexo
                    {
                        IdProcesso = simuladorId,
                        Arquivo = dados.Name,
                        CriadoPor = User.ObterId(),
                        TipoAnexo = TipoAnexo.RELATORIO_SIMULADOR,
                        TipoDoc = 3,
                        IdArquivo = Converters.GuidToRaw(retornoUpload.Arquivo.id)
                    });

                return anexoInclusaoId;
            }

            return 0;
        }
    }
}